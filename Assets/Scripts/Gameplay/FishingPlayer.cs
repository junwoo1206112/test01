using System;
using UnityEngine;
using Mirror;
using System.Collections;

namespace MultiplayFishing.Gameplay
{
    /// <summary>
    /// 플레이어의 상태를 관리하며, 소환 시 땅 박힘 현상을 방지하는 로직을 포함합니다.
    /// </summary>
    public class FishingPlayer : NetworkBehaviour
    {
        public event Action<string> OnPlayerNameChangedEvent;
        public event Action<int> OnScoreChangedEvent;
        public event Action<bool> OnReadyChangedEvent;
        public event Action<Color> OnPlayerColorChangedEvent;

        static readonly System.Collections.Generic.List<FishingPlayer> playersList = new System.Collections.Generic.List<FishingPlayer>();

        [Header("Player Info")]
        [SyncVar(hook = nameof(OnPlayerNameChanged))] public string playerName = "";
        [SyncVar(hook = nameof(OnScoreChanged))] public int score = 0;
        [SyncVar(hook = nameof(OnReadyChanged))] public bool isReady = false;
        [SyncVar(hook = nameof(OnPlayerColorChanged))] public Color playerColor = Color.white;

        [Header("References")]
        [SerializeField] private Renderer characterRenderer;

        #region Physics Escape Logic (땅 박힘 방지)

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Debug.Log($"[FishingPlayer] 로컬 플레이어 '{playerName}' 소환됨. 땅 박힘 방지 로직 가동.");
            
            // 소환 즉시 땅에서 탈출하도록 코루틴 실행
            StartCoroutine(SmartEscapeRoutine());
        }

        private IEnumerator SmartEscapeRoutine()
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                // 물리 충돌을 잠시 끄고 위치를 위로 보정한 뒤 다시 켭니다.
                cc.enabled = false;
                transform.position += Vector3.up * 0.2f; 
                yield return new WaitForFixedUpdate();
                cc.enabled = true;
            }
        }

        #endregion

        #region Server System

        public override void OnStartServer()
        {
            base.OnStartServer();
            playersList.Add(this);
            playerName = $"낚시꾼 {playersList.Count}";
            playerColor = Color.HSVToRGB(UnityEngine.Random.value, 0.7f, 0.9f);
        }

        [Command]
        void CmdSetPlayerName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || newName.Length > 20) return;
            playerName = newName;
        }

        public override void OnStopServer()
        {
            playersList.Remove(this);
            base.OnStopServer();
        }

        #endregion

        #region Client Sync

        public override void OnStartClient()
        {
            OnPlayerNameChangedEvent?.Invoke(playerName);
            OnScoreChangedEvent?.Invoke(score);
            OnReadyChangedEvent?.Invoke(isReady);
            UpdateCharacterColor(playerColor);
        }

        void OnPlayerNameChanged(string oldValue, string newValue) => OnPlayerNameChangedEvent?.Invoke(newValue);
        void OnScoreChanged(int oldValue, int newValue) => OnScoreChangedEvent?.Invoke(newValue);
        void OnReadyChanged(bool oldValue, bool newValue) => OnReadyChangedEvent?.Invoke(newValue);
        void OnPlayerColorChanged(Color oldColor, Color newColor) { UpdateCharacterColor(newColor); OnPlayerColorChangedEvent?.Invoke(newColor); }

        void UpdateCharacterColor(Color color)
        {
            if (characterRenderer != null) characterRenderer.material.color = color;
        }

        #endregion
    }
}
