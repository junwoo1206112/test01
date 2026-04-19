using UnityEngine;
using Mirror;
using System;

namespace MultiplayFishing.Network
{
    public class FishingRoomManager : NetworkRoomManager
    {
        public static new FishingRoomManager singleton => (FishingRoomManager)NetworkManager.singleton;

        public static event Action NetworkStateChanged;

        public string ModeText => mode switch
        {
            NetworkManagerMode.Host => "호스트",
            NetworkManagerMode.ClientOnly => "클라이언트",
            _ => "오프라인"
        };

        public int ConnectedClientCount => NetworkServer.active 
            ? NetworkServer.connections.Count 
            : FindObjectsByType<NetworkIdentity>(FindObjectsSortMode.None).Length;

        public override void Awake()
        {
            base.Awake();
            
            // 씬 이름 강제 설정 (인스펙터 설정을 덮어씁니다)
            // 'Not in Room scene' 에러를 방지하기 위해 RoomScene을 현재 씬인 Lobby로 강제합니다.
            offlineScene = "Lobby";
            RoomScene = "Lobby"; 
            GameplayScene = "Gameplay";

            showRoomGUI = false;
        }

        #region 핵심 로직 (즉시 게임 시작)

        public override void OnRoomServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnRoomServerAddPlayer(conn);
            
            // 플레이어가 추가되면 즉시 게임 씬으로 이동을 준비합니다.
            // ReadyStatusChanged를 호출하여 OnRoomServerPlayersReady가 실행되게 유도합니다.
            ReadyStatusChanged();
        }

        public override void OnRoomServerPlayersReady()
        {
            Debug.Log($"[FishingRoomManager] 모든 플레이어 감지 - 즉시 {GameplayScene}으로 전환합니다.");
            ServerChangeScene(GameplayScene);
        }

        #endregion

        #region 상태 알림

        public override void OnRoomStartServer() { base.OnRoomStartServer(); NetworkStateChanged?.Invoke(); }
        public override void OnRoomStopServer() { base.OnRoomStopServer(); NetworkStateChanged?.Invoke(); }
        public override void OnRoomClientConnect() { base.OnRoomClientConnect(); NetworkStateChanged?.Invoke(); }
        public override void OnRoomClientDisconnect() { base.OnRoomClientDisconnect(); NetworkStateChanged?.Invoke(); }

        #endregion

        public static string GetLocalIPAddress()
        {
            try
            {
                foreach (var ip in System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()))
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (!ip.ToString().StartsWith("127.")) return ip.ToString();
                    }
                }
            } catch { }
            return "127.0.0.1";
        }
    }
}
