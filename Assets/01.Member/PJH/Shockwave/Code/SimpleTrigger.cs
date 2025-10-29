using UnityEngine;
using UnityEngine.Events;

namespace Shockwave.Code
{
    public class SimpleTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onPlayerEntered;
        [SerializeField] private UnityEvent onPlayerExited;
        
        public UnityEvent OnPlayerEntered => onPlayerEntered;
        public UnityEvent OnPlayerExited => onPlayerExited;

        private const string PLAYER_TAG = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
                onPlayerEntered?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
                onPlayerExited?.Invoke();
        }
    }
}