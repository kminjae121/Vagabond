using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo
{
    public class WallSliding : MonoBehaviour, IEntityComponent
    {
        [Header("Wall Detection")]
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private float wallCheckDistance = 0.8f;
        [SerializeField] private float wallRunMinSpeed = 5f;
        
        [Header("Wall Run Settings")]
        [SerializeField] private float wallRunSpeed = 12f;
        [SerializeField] private float wallRunGravity = 5f;
        [SerializeField] private float wallRunDuration = 2f;
        [SerializeField] private float wallTilt = 15f;
        
        [Header("Wall Climb Settings")]
        [SerializeField] private float wallClimbSpeed = 8f;
        [SerializeField] private float wallClimbStamina = 3f;
        
        public bool _isWallSliding { get; private set; }
        public bool isOnLeftWall { get; private set; }
        public bool isOnRightWall { get; private set; }
        
        private float wallRunTimer;
        private float climbStamina;
        private Vector3 wallNormal;
        
        private Rigidbody _rbCompo;
        private CharacterMovement _movementCompo;
        private Player _player;

        public void Initialize(Entity entity)
        {
            _player = entity as Player;
            _movementCompo = entity.GetCompo<CharacterMovement>();
            _rbCompo = entity.GetComponent<Rigidbody>();
            climbStamina = wallClimbStamina;
        }

        private void Update()
        {
            if (_isWallSliding)
            {
                wallRunTimer += Time.deltaTime;
                
                if (wallRunTimer >= wallRunDuration || !CanWallRun())
                {
                    EndWallSlide();
                }
            }
        }

        public string CanSlidingWall()
        {
            Vector3 checkPos = transform.position + Vector3.up * 1f;
            
            isOnLeftWall = Physics.Raycast(checkPos, -transform.right, out RaycastHit leftHit, wallCheckDistance, wallLayer);
            isOnRightWall = Physics.Raycast(checkPos, transform.right, out RaycastHit rightHit, wallCheckDistance, wallLayer);
            
            if (isOnLeftWall)
            {
                wallNormal = leftHit.normal;
                return "Left";
            }
            else if (isOnRightWall)
            {
                wallNormal = rightHit.normal;
                return "Right";
            }
            
            return "None";
        }
        
        private bool CanWallRun()
        {
            return _movementCompo.GetHorizontalSpeed() >= wallRunMinSpeed && 
                   !_movementCompo.CheckGroundDetected() &&
                   (isOnLeftWall || isOnRightWall);
        }

        public void StartWallSlide()
        {
            _isWallSliding = true;
            wallRunTimer = 0f;
            climbStamina = wallClimbStamina;
            
            // 카메라 기울기 적용
            if (_player.camCompo != null)
            {
                _player.camCompo.SetTilt(isOnLeftWall ? -wallTilt : wallTilt);
            }
        }

        public void WallSlide()
        {
            Vector3 velocity = _rbCompo.linearVelocity;
            
            // 벽을 따라 이동하는 방향 계산
            Vector3 wallDirection = Vector3.Cross(wallNormal, Vector3.up);
            float inputDirection = _movementCompo._inputReader.MoveValue.y;
            
            if (isOnLeftWall)
                wallDirection = -wallDirection;
            
            // 벽타기 이동
            velocity = wallDirection * wallRunSpeed * inputDirection;
            
            // 벽타기 중력 (천천히 하강)
            velocity.y = Mathf.Max(velocity.y - wallRunGravity * Time.fixedDeltaTime, -3f);
            
            // 벽 오르기 (W키)
            if (inputDirection > 0.1f && climbStamina > 0)
            {
                velocity.y = wallClimbSpeed;
                climbStamina -= Time.fixedDeltaTime;
            }
            
            _rbCompo.linearVelocity = velocity;
        }

        public void EndWallSlide()
        {
            _isWallSliding = false;
            isOnLeftWall = false;
            isOnRightWall = false;
            
            // 카메라 복구
            if (_player.camCompo != null)
            {
                _player.camCompo.ReturnOwnTilt();
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 checkPos = transform.position + Vector3.up * 1f;
            
            Gizmos.color = isOnLeftWall ? Color.blue : Color.gray;
            Gizmos.DrawRay(checkPos, -transform.right * wallCheckDistance);
            
            Gizmos.color = isOnRightWall ? Color.blue : Color.gray;
            Gizmos.DrawRay(checkPos, transform.right * wallCheckDistance);
        }
    }
}
