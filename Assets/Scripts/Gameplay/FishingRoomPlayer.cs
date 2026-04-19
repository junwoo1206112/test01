using UnityEngine;
using Mirror;

namespace MultiplayFishing.Gameplay
{
    public class FishingRoomPlayer : NetworkRoomPlayer
    {
        [Header("Player Settings")]
        [SyncVar(hook = nameof(OnPlayerNameChanged))]
        public string playerName = "Player";

        #region SyncVar Hooks

        void OnPlayerNameChanged(string oldValue, string newValue)
        {
            Debug.Log($"[FishingRoomPlayer] Player Name Changed: {oldValue} -> {newValue}");
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            base.ReadyStateChanged(oldReadyState, newReadyState);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            // 클라이언트 본인이 서버에 준비 완료 신호를 보냅니다.
            // 이 방식은 Mirror의 표준 방식이며 Weaver 에러가 발생하지 않습니다.
            if (isOwned)
            {
                Debug.Log("[FishingRoomPlayer] 클라이언트 자동 준비 요청");
                CmdChangeReadyState(true);
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
        }

        #endregion
    }
}
