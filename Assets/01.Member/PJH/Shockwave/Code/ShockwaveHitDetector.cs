using UnityEngine;
using UnityEngine.Events;

namespace Shockwave.Code
{
    public class ShockwaveHitDetector : MonoBehaviour
    {
        [SerializeField] private SimpleTrigger innerTrigger;
        [SerializeField] private SimpleTrigger outerTrigger;

        [Header("Hit Zone Events")]
        [SerializeField] private UnityEvent onHitZoneEntered;
        [SerializeField] private UnityEvent onHitZoneExited;

        private bool isPlayerInInner;
        private bool isPlayerInOuter;
        private bool isPlayerInHitZone;

        private void Start()
        {
            if (innerTrigger != null)
            {
                innerTrigger.OnPlayerEntered.AddListener(HandleInnerEnter);
                innerTrigger.OnPlayerExited.AddListener(HandleInnerExit);
            }

            if (outerTrigger != null)
            {
                outerTrigger.OnPlayerEntered.AddListener(HandleOuterEnter);
                outerTrigger.OnPlayerExited.AddListener(HandleOuterExit);
            }
        }
    
        private void HandleInnerEnter()
        {
            isPlayerInInner = true;
            CheckHitZoneStatus();
        }

        private void HandleInnerExit()
        {
            isPlayerInInner = false;
            CheckHitZoneStatus();
        }

        private void HandleOuterEnter()
        {
            isPlayerInOuter = true;
            CheckHitZoneStatus();
        }

        private void HandleOuterExit()
        {
            isPlayerInOuter = false;
            CheckHitZoneStatus();
        }

        private void CheckHitZoneStatus()
        {
            bool wasInHitZone = isPlayerInHitZone;
            isPlayerInHitZone = isPlayerInInner && isPlayerInOuter;

            if (wasInHitZone != isPlayerInHitZone)
            {
                if (isPlayerInHitZone)
                {
                    Debug.Log("플레이어가 히트 존에 진입했습니다.");
                    onHitZoneEntered?.Invoke();
                }
                else
                {
                    Debug.Log("플레이어가 히트 존에서 이탈했습니다.");
                    onHitZoneExited?.Invoke();
                }
            }
        }
    }
}