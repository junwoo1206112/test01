using UnityEngine;
using Mirror;
using Unity.Cinemachine;

namespace MultiplayFishing.Gameplay
{
    /// <summary>
    /// 시네머신의 모든 자동 기능을 차단하고, 수학적으로 3인칭 시점을 강제 고정합니다.
    /// </summary>
    public class FishingCameraFollow : MonoBehaviour
    {
        [Header("Simple 3rd Person Settings")]
        [SerializeField] private float distance = 8.0f;    // 뒤로 떨어진 거리
        [SerializeField] private float height = 5.0f;      // 위로 떨어진 높이
        [SerializeField] private float lookAtHeight = 1.5f; // 캐릭터의 어디를 쳐다볼지 (머리 높이)

        private CinemachineCamera vcam;

        void Awake()
        {
            vcam = GetComponent<CinemachineCamera>();
            
            // 시네머신의 자동 기능을 모두 끕니다. (우리가 직접 제어하기 위함)
            if (vcam != null)
            {
                vcam.Follow = null;
                vcam.LookAt = null;
                
                // 기존 시네머신 추적 컴포넌트들을 모두 비활성화
                foreach (var comp in vcam.GetComponents<CinemachineComponentBase>())
                {
                    comp.enabled = false;
                }
            }
        }

        void LateUpdate()
        {
            // 1. 로컬 플레이어 확인
            if (NetworkClient.localPlayer == null) return;

            // 2. 가상 카메라 참조 확인 및 자동 할당
            if (vcam == null)
            {
                vcam = GetComponent<CinemachineCamera>();
                if (vcam == null)
                {
                    vcam = Object.FindFirstObjectByType<CinemachineCamera>();
                }
                
                if (vcam == null) return; // 여전히 없으면 다음 프레임에 재시도
            }

            Transform player = NetworkClient.localPlayer.transform;

            // 3. 3인칭 위치 계산 (캐릭터 뒤쪽 상단)
            Vector3 targetPos = player.position - (player.forward * distance) + (Vector3.up * height);
            
            // 4. 카메라 위치 및 회전 강제 적용
            vcam.transform.position = targetPos;
            
            Vector3 lookAtPos = player.position + (Vector3.up * lookAtHeight);
            vcam.transform.LookAt(lookAtPos);

            // 5. 시네머신 브레인에게 위치 갱신 알림
            vcam.OnTargetObjectWarped(player, Vector3.zero);
        }
    }
}
