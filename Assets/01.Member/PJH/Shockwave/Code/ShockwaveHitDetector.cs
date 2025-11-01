using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Combat
{
    public class ShockwaveHitDetector : MonoBehaviour
    {
        [SerializeField] private SimpleTrigger innerTrigger;
        [SerializeField] private SimpleTrigger outerTrigger;

        [Header("Hit Zone Events")]
        [SerializeField] private UnityEvent<Collider> onHitZoneEntered;
        [SerializeField] private UnityEvent<Collider> onHitZoneExited;

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
    
        private void HandleInnerEnter(Collider other)
        {
            isPlayerInInner = true;
            CheckHitZoneStatus(other);
        }

        private void HandleInnerExit(Collider other)
        {
            isPlayerInInner = false;
            CheckHitZoneStatus(other);
        }

        private void HandleOuterEnter(Collider other)
        {
            isPlayerInOuter = true;
            CheckHitZoneStatus(other);
        }

        private void HandleOuterExit(Collider other)
        {
            isPlayerInOuter = false;
            CheckHitZoneStatus(other);
        }

        private void CheckHitZoneStatus(Collider other)
        {
            bool wasInHitZone = isPlayerInHitZone;
            isPlayerInHitZone = isPlayerInInner && !isPlayerInOuter;

            if (wasInHitZone == isPlayerInHitZone)
                return;
            
            if (isPlayerInHitZone)
            {
                UnityLogger.Log("쇼크 웨이브 히트 존 진입");
                onHitZoneEntered?.Invoke(other);
            }
            else
            {
                UnityLogger.Log("쇼크 웨이브 히트 존 이탈");
                onHitZoneExited?.Invoke(other);
            }
        }
    }
}