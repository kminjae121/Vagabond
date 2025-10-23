using UnityEngine;

namespace Code.Enemies
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class RagDollPart : MonoBehaviour
    {
        private Rigidbody _rigid;
        private Collider _collider;

        public void InitializePart()
        {
            _rigid = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void SetRagDollActive(bool isActive)
            => _rigid.isKinematic = !isActive;

        public void SetCollider(bool isActive)
            => _collider.enabled = isActive;

        public async void KnockBack(Vector3 force, Vector3 position)
        {
            await Awaitable.FixedUpdateAsync();
            _rigid.AddForceAtPosition(force, position, ForceMode.Impulse);
        }
    }
}