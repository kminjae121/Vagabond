using UnityEngine;

namespace Map
{
    public class DoorDetectedObject : MonoBehaviour
    {
        [SerializeField] private LayerMask _whatIsPlayer;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _whatIsPlayer) != 0)
            {
                MapOpen.Instance.CanCollectEnemies();
            }
        }
    }
}