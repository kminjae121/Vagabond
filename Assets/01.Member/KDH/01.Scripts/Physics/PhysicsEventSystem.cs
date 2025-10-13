using UnityEngine;
using UnityEngine.Events;

namespace _01.Member.KDH._01.Scripts.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsEventSystem : MonoBehaviour
    {
        [System.Serializable]
        public class SpeedEvent : UnityEvent<float> { }
        
        [System.Serializable]
        public class ImpactEvent : UnityEvent<Vector3, float> { }
        
        [Header("Speed Thresholds")]
        [SerializeField] private float highSpeedThreshold = 15f;
        [SerializeField] private float landingDamageThreshold = 20f;
        
        [Header("Ground Check")]
        [SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] private LayerMask groundLayer = 1 << 0; // Default layer
        
        [Header("Events")]
        public UnityEvent OnLanded;
        public UnityEvent OnHighSpeed;
        public SpeedEvent OnSpeedChanged;
        public ImpactEvent OnHardLanding;
        public UnityEvent OnJump;
        public UnityEvent OnDoubleJump;
        
        private Rigidbody rb;
        private bool wasGrounded;
        private float lastYVelocity;
        private bool wasHighSpeed;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            if (rb == null)
            {
                Debug.LogError("[PhysicsEventSystem] Rigidbody를 찾을 수 없습니다! Player 오브젝트에 이 컴포넌트를 추가해주세요.");
                enabled = false;
                return;
            }
        }
        
        private void FixedUpdate()
        {
            if (rb == null) return;
            
            CheckGroundState();
            CheckSpeed();
        }
        
        private void CheckGroundState()
        {
            // 지면 체크
            bool isGrounded = UnityEngine.Physics.Raycast(
                transform.position, 
                Vector3.down, 
                groundCheckDistance,
                groundLayer);
            
            if (isGrounded && !wasGrounded)
            {
                // 착지
                float impactForce = Mathf.Abs(lastYVelocity);
                OnLanded?.Invoke();
                
                if (impactForce > landingDamageThreshold)
                {
                    OnHardLanding?.Invoke(transform.position, impactForce);
                }
            }
            
            wasGrounded = isGrounded;
            lastYVelocity = rb.linearVelocity.y;
        }
        
        private void CheckSpeed()
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            float speed = horizontalVelocity.magnitude;
            
            OnSpeedChanged?.Invoke(speed);
            
            bool isHighSpeed = speed >= highSpeedThreshold;
            
            if (isHighSpeed && !wasHighSpeed)
            {
                OnHighSpeed?.Invoke();
            }
            
            wasHighSpeed = isHighSpeed;
        }
        
        /// <summary>
        /// 외부에서 점프 이벤트 발생시킬 때 호출
        /// </summary>
        public void TriggerJump()
        {
            OnJump?.Invoke();
        }
        
        /// <summary>
        /// 외부에서 더블점프 이벤트 발생시킬 때 호출
        /// </summary>
        public void TriggerDoubleJump()
        {
            OnDoubleJump?.Invoke();
        }
        
        private void OnDrawGizmosSelected()
        {
            // Ground check 시각화
            Gizmos.color = wasGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        }
    }
}