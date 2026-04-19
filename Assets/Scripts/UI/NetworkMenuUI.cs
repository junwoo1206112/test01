using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MultiplayFishing.Network;

namespace MultiplayFishing.UI
{
    public class NetworkMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button disconnectButton;
        [SerializeField] private Button copyIPButton;

        [Header("Input")]
        [SerializeField] private TMP_InputField addressInput;

        [Header("Display")]
        [SerializeField] private GameObject offlineControlsRoot;
        [SerializeField] private GameObject onlineControlsRoot;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text connectionInfoText;
        [SerializeField] private string defaultAddress = "127.0.0.1";

        FishingRoomManager manager;

        void Awake()
        {
            manager = FishingRoomManager.singleton;
            ForceUIPosition();
        }

        void ForceUIPosition()
        {
            // 1. 온라인 패널(버튼들)을 우측 상단 끝으로 고정
            if (onlineControlsRoot != null)
            {
                RectTransform rect = onlineControlsRoot.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(1, 1);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(1, 1);
                    rect.anchoredPosition = new Vector2(-10, -10); // 끝에 가깝게 (10픽셀 여백)
                }
            }

            // 2. 커넥션 인포 텍스트를 화면 "최상단 중앙"으로 밀어올림
            if (connectionInfoText != null)
            {
                connectionInfoText.transform.SetParent(transform, true);
                RectTransform rect = connectionInfoText.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0.5f, 1f);
                    rect.anchorMax = new Vector2(0.5f, 1f);
                    rect.pivot = new Vector2(0.5f, 1f);
                    
                    // 요청하신 Y 위치 적용
                    rect.anchoredPosition = new Vector2(0, 430); 
                    
                    connectionInfoText.alignment = TextAlignmentOptions.Center;
                    rect.sizeDelta = new Vector2(800, 40);
                }
            }
        }

        void SetTopRight(GameObject go)
        {
            if (go == null) return;
            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
                rect.anchoredPosition = new Vector2(-20, -20);
            }
        }

        void OnEnable()
        {
            if (hostButton != null) hostButton.onClick.AddListener(OnHostClicked);
            if (joinButton != null) joinButton.onClick.AddListener(OnJoinClicked);
            if (disconnectButton != null) disconnectButton.onClick.AddListener(OnDisconnectClicked);
            if (copyIPButton != null) copyIPButton.onClick.AddListener(OnCopyIPClicked);

            FishingRoomManager.NetworkStateChanged += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (hostButton != null) hostButton.onClick.RemoveListener(OnHostClicked);
            if (joinButton != null) joinButton.onClick.RemoveListener(OnJoinClicked);
            if (disconnectButton != null) disconnectButton.onClick.RemoveListener(OnDisconnectClicked);
            if (copyIPButton != null) copyIPButton.onClick.RemoveListener(OnCopyIPClicked);

            FishingRoomManager.NetworkStateChanged -= Refresh;
        }

        void Update()
        {
            if (manager == null)
            {
                manager = FishingRoomManager.singleton;
                if (manager != null) Refresh();
                return;
            }

            Refresh();
        }

        #region Button Handlers

        void OnHostClicked()
        {
            if (!CanStartNetwork()) return;
            manager.StartHost();
            Debug.Log("<color=green>[NetworkMenuUI]</color> 호스트 시작.");
        }

        void OnJoinClicked()
        {
            if (!CanStartNetwork()) return;
            string addr = GetAddress();
            manager.networkAddress = addr;
            
            Debug.Log($"<color=yellow>[NetworkMenuUI]</color> {addr}로 접속 시도...");
            manager.StartClient();
        }

        void OnDisconnectClicked()
        {
            if (manager == null) return;
            Debug.Log("<color=red>[NetworkMenuUI]</color> 연결 끊기.");
            
            switch (manager.mode)
            {
                case NetworkManagerMode.Host: manager.StopHost(); break;
                case NetworkManagerMode.ClientOnly: manager.StopClient(); break;
                case NetworkManagerMode.ServerOnly: manager.StopServer(); break;
            }
        }

        void OnCopyIPClicked()
        {
            string ip = FishingRoomManager.GetLocalIPAddress();
            GUIUtility.systemCopyBuffer = ip;
            Debug.Log($"<color=cyan>[NetworkMenuUI]</color> IP 복사 완료: {ip}. 참가자에게 전달하세요.");
        }

        #endregion

        #region Helpers

        string GetAddress()
        {
            if (addressInput != null && !string.IsNullOrWhiteSpace(addressInput.text))
            {
                string input = addressInput.text.Trim();
                if (input.Contains(":")) input = input.Split(':')[0];
                return input;
            }
            return defaultAddress;
        }

        bool CanStartNetwork()
        {
            return manager != null && manager.mode == NetworkManagerMode.Offline;
        }

        #endregion

        #region UI Refresh

        void Refresh()
        {
            if (manager == null) return;

            string currentSceneName = SceneManager.GetActiveScene().name;
            bool isOfflineScene = currentSceneName == manager.offlineScene || currentSceneName == "Lobby";
            
            bool isOfflineMode = manager.mode == NetworkManagerMode.Offline;

            if (offlineControlsRoot != null)
                offlineControlsRoot.SetActive(isOfflineMode && isOfflineScene);

            if (onlineControlsRoot != null)
                onlineControlsRoot.SetActive(!isOfflineMode);

            if (hostButton != null) hostButton.interactable = isOfflineMode;
            if (joinButton != null) joinButton.interactable = isOfflineMode;
            if (addressInput != null) addressInput.interactable = isOfflineMode;
            if (disconnectButton != null) disconnectButton.interactable = !isOfflineMode;

            if (copyIPButton != null)
                copyIPButton.gameObject.SetActive(manager.mode == NetworkManagerMode.Host);

            UpdateStatusText();
            UpdateConnectionInfo();
        }

        void UpdateStatusText()
        {
            if (statusText == null) return;

            if (manager.mode == NetworkManagerMode.Offline)
            {
                statusText.text = "오프라인";
            }
            else if (NetworkClient.active && !NetworkClient.isConnected)
            {
                statusText.text = $"연결 시도 중: {manager.networkAddress}";
            }
            else
            {
                statusText.text = $"{manager.ModeText} 모드 활성화됨";
            }
        }

        void UpdateConnectionInfo()
        {
            if (connectionInfoText == null) return;

            if (manager.mode == NetworkManagerMode.Offline)
            {
                connectionInfoText.text = "";
                return;
            }

            if (manager.mode == NetworkManagerMode.Host)
            {
                string localIP = FishingRoomManager.GetLocalIPAddress();
                connectionInfoText.text = $"내 IP: {localIP}\n인원: {manager.ConnectedClientCount}/{manager.maxConnections}";
            }
            else if (manager.mode == NetworkManagerMode.ClientOnly)
            {
                if (NetworkClient.isConnected)
                    connectionInfoText.text = $"{manager.networkAddress} 연결됨\n인원: {manager.ConnectedClientCount}/{manager.maxConnections}";
                else
                    connectionInfoText.text = "서버 응답 대기 중...";
            }
        }

        #endregion
    }
}
