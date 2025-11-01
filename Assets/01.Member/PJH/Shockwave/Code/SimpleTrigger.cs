using UnityEngine;
using UnityEngine.Events;

namespace Code.Combat
{
    public class SimpleTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider> onPlayerEntered;
        [SerializeField] private UnityEvent<Collider> onPlayerExited;
        
        public UnityEvent<Collider> OnPlayerEntered => onPlayerEntered;
        public UnityEvent<Collider> OnPlayerExited => onPlayerExited;

        private const string PLAYER_TAG = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
                onPlayerEntered?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
                onPlayerExited?.Invoke(other);
        }
    }
}