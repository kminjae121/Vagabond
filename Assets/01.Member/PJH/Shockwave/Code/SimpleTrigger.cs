using UnityEngine;
using UnityEngine.Events;

namespace Shockwave.Code
{
    public class SimpleTrigger : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onPlayerEntered = new();
        
        [SerializeField]
        private UnityEvent onPlayerExited = new();
        
        public UnityEvent OnPlayerEntered => onPlayerEntered;
        public UnityEvent OnPlayerExited => onPlayerExited;

        private const string PLAYER_TAG = "Player";

        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
            {
                onPlayerEntered.Invoke();
            }
        }

        private void OnTriggerExit(UnityEngine.Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
            {
                onPlayerExited.Invoke();
            }
        }
    }
}