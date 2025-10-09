using UnityEngine;

namespace Code.Entities.Combat
{
    public class Arrow : MonoBehaviour
    {
        private Rigidbody _rigid;
        private float _speed;
        private Vector3 _dir;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody>();
        }

        public void Initialize(Vector3 dir, float speed)
        {
            _dir = dir;
            _speed = speed;
        }

        private void Update()
        {
            _rigid.AddForce(_dir, ForceMode.Impulse);
        }
    }
}