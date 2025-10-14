using _00.CORE._02.Scripts.Input;
using Code.Core.Debugs;
using Code.Core.Stats;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class CharacterMovement : MonoBehaviour, IEntityComponent
    {
        [Header("Stat Settings")]
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private StatSO jumpSpeedStat;
        [SerializeField] private StatSO maxMoveSpeedStat;
        
        [Header("Quake 3 Physics")]
        [SerializeField] private bool useQuake3Physics = true;
        [SerializeField] private float groundAccelerate = 14f;
        [SerializeField] private float airAccelerate = 2f;
        [SerializeField] private float friction = 6f;
        [SerializeField] private float stopSpeed = 1.5f;
        
        [Header("Strafe Jumping")]
        [SerializeField] private bool enableStrafeJumping = true;
        [SerializeField] private float strafeMultiplier = 1.2f;
        [SerializeField] private float maxStrafeSpeed = 20f;
        
        [Header("Bunny Hop")]
        [SerializeField] private bool enableAutoBhop = false;
        [SerializeField] private float bhopSpeedRetention = 0.9f;
        
        [Header("Ground Check")]
        [SerializeField] private float jumpRaySize = 0.3f;
        [SerializeField] private LayerMask whatIsGround;
        
        [field: SerializeField] public InputReader _inputReader { get; private set; }
        
        public Vector3 _move;
        public int _jumpCnt { get; set; }
        
        public float moveSpeed { get; set; } = 8f;
        public float baseSpeed { get; private set; } = 8f;
        public float maxmoveSpeed { get; set; } = 15f;
        public float jumpSpeed { get; private set; }
        public int maxJumpCnt { get; set; } = 2;
        
        private Entity _entity;
        private EntityStatCompo _statCompo;
        private Rigidbody _rbCompo;
        
        // Quake 3 specific variables
        private Vector3 velocity;
        private Vector3 wishDir;
        private bool wasGrounded;
        private float currentHorizontalSpeed;
        private bool jumpQueued;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStatCompo>();
            _rbCompo = entity.GetComponent<Rigidbody>();
            
            if (_rbCompo != null && useQuake3Physics)
            {
                _rbCompo.useGravity = true;
                _rbCompo.freezeRotation = true;
            }
            
            AfterInitialize();
        }
        
        public void SetMove(float XMove, float ZMove)
        {
            _move.x = XMove;
            _move.z = ZMove;
        }
        
        public bool CheckGroundDetected()
        {
            return Physics.Raycast(transform.position, Vector3.down, jumpRaySize, whatIsGround);
        }
        
        /// <summary>
        /// Quake 3 style jump with optional bunny hop
        /// </summary>
        public void Jump()
        {
            bool isGrounded = CheckGroundDetected();
            
            if (useQuake3Physics)
            {
                Quake3Jump(isGrounded);
            }
            else
            {
                StandardJump(isGrounded);
            }
        }
        
        private void StandardJump(bool isGrounded)
        {
            if (isGrounded)
            {
                _jumpCnt = 0;
                Vector3 velocity = _rbCompo.linearVelocity;
                velocity.y = 0;
                _rbCompo.linearVelocity = velocity;
                _rbCompo.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                _jumpCnt++;
            }
            else if (_jumpCnt < maxJumpCnt)
            {
                Vector3 velocity = _rbCompo.linearVelocity;
                velocity.y = 0;
                _rbCompo.linearVelocity = velocity;
                _rbCompo.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                _jumpCnt++;
            }
        }
        
        private void Quake3Jump(bool isGrounded)
        {
            if (isGrounded)
            {
                _jumpCnt = 0;
                
                // Preserve horizontal velocity for bunny hopping
                Vector3 currentVel = _rbCompo.linearVelocity;
                if (enableAutoBhop && wasGrounded && currentHorizontalSpeed > baseSpeed)
                {
                    // Retain some speed for bunny hop
                    float retainedSpeed = currentHorizontalSpeed * bhopSpeedRetention;
                    Vector3 horizontalDir = new Vector3(currentVel.x, 0, currentVel.z).normalized;
                    currentVel.x = horizontalDir.x * retainedSpeed;
                    currentVel.z = horizontalDir.z * retainedSpeed;
                }
                
                currentVel.y = jumpSpeed;
                _rbCompo.linearVelocity = currentVel;
                _jumpCnt++;
            }
            else if (_jumpCnt < maxJumpCnt)
            {
                Vector3 velocity = _rbCompo.linearVelocity;
                velocity.y = jumpSpeed;
                _rbCompo.linearVelocity = velocity;
                _jumpCnt++;
            }
        }
        
        public void AfterInitialize()
        {
            moveSpeed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 4f);
            jumpSpeed = _statCompo.SubscribeStat(jumpSpeedStat, HandleJumpPowerChange, 3f);
            maxmoveSpeed = _statCompo.SubscribeStat(maxMoveSpeedStat, HandleMaxMoveSpeedChange, 3f);
            baseSpeed = moveSpeed;
        }
        
        private void OnDestroy()
        {
            _statCompo.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
            _statCompo.UnSubscribeStat(jumpSpeedStat, HandleJumpPowerChange);
            _statCompo.UnSubscribeStat(maxMoveSpeedStat, HandleMaxMoveSpeedChange);
        }
        
        private void HandleMoveSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            moveSpeed = currentvalue;
        }
        
        private void HandleJumpPowerChange(StatSO stat, float currentvalue, float previousvalue)
        {
            jumpSpeed = currentvalue;
        }
        
        private void HandleMaxMoveSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            maxmoveSpeed = currentvalue;
        }
        
        public void StopMoving()
        {
            _rbCompo.linearVelocity = Vector3.zero;
        }
        
        public void SetSpeed(float targetSpeedValue)
        {
            moveSpeed = targetSpeedValue;
        }
        
        public void SetReturnOriginMoveSpeed()
        {
            moveSpeed = baseSpeed;
        }
        
        private void Update()
        {
            currentHorizontalSpeed = GetHorizontalSpeed();
            
            if (useQuake3Physics)
            {
                UnityLogger.Log($"Speed: {currentHorizontalSpeed:F2} | Max: {maxmoveSpeed:F2}");
            }
        }
        
        private void FixedUpdate()
        {
            if (useQuake3Physics)
            {
                Quake3Movement();
            }
            
            wasGrounded = CheckGroundDetected();
        }
        
        /// <summary>
        /// Quake 3 Arena movement physics implementation
        /// </summary>
        private void Quake3Movement()
        {
            bool isGrounded = CheckGroundDetected();
            velocity = _rbCompo.linearVelocity;
            
            // Calculate wish direction from input
            CalculateWishDirection();
            
            if (isGrounded)
            {
                GroundMove();
            }
            else
            {
                AirMove();
            }
            
            _rbCompo.linearVelocity = velocity;
        }
        
        private void CalculateWishDirection()
        {
            // Transform movement input to world space
            wishDir = transform.TransformDirection(_move);
            wishDir.y = 0;
            wishDir.Normalize();
        }
        
        /// <summary>
        /// Ground movement with Quake 3 friction and acceleration
        /// </summary>
        private void GroundMove()
        {
            // Apply friction
            ApplyFriction();
            
            // Ground acceleration
            float wishSpeed = _move.magnitude * moveSpeed;
            Accelerate(wishDir, wishSpeed, groundAccelerate);
            
            // Cap speed
            Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
            if (horizontalVel.magnitude > maxmoveSpeed)
            {
                horizontalVel = horizontalVel.normalized * maxmoveSpeed;
                velocity.x = horizontalVel.x;
                velocity.z = horizontalVel.z;
            }
        }
        
        /// <summary>
        /// Air movement with strafe jumping capability
        /// </summary>
        private void AirMove()
        {
            if (!enableStrafeJumping)
            {
                return;
            }
            
            float accel = airAccelerate;
            float wishSpeed = _move.magnitude * moveSpeed;
            
            // Check for strafe input
            bool isStrafing = Mathf.Abs(_move.x) > 0.1f;
            if (isStrafing)
            {
                wishSpeed *= strafeMultiplier;
            }
            
            AirAccelerate(wishDir, wishSpeed, accel);
            
            // Cap strafe speed
            Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
            if (horizontalVel.magnitude > maxStrafeSpeed)
            {
                horizontalVel = horizontalVel.normalized * maxStrafeSpeed;
                velocity.x = horizontalVel.x;
                velocity.z = horizontalVel.z;
            }
        }
        
        /// <summary>
        /// Quake 3 friction algorithm
        /// </summary>
        private void ApplyFriction()
        {
            Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
            float speed = horizontalVel.magnitude;
            
            if (speed < 0.1f)
            {
                velocity.x = 0;
                velocity.z = 0;
                return;
            }
            
            float drop = 0;
            float control = speed < stopSpeed ? stopSpeed : speed;
            drop = control * friction * Time.fixedDeltaTime;
            
            float newSpeed = Mathf.Max(speed - drop, 0);
            if (speed > 0)
            {
                newSpeed /= speed;
            }
            
            velocity.x *= newSpeed;
            velocity.z *= newSpeed;
        }
        
        /// <summary>
        /// Quake 3 acceleration formula
        /// </summary>
        private void Accelerate(Vector3 targetDir, float targetSpeed, float accel)
        {
            float currentSpeed = Vector3.Dot(velocity, targetDir);
            float addSpeed = targetSpeed - currentSpeed;
            
            if (addSpeed <= 0)
                return;
            
            float accelSpeed = accel * targetSpeed * Time.fixedDeltaTime;
            
            if (accelSpeed > addSpeed)
                accelSpeed = addSpeed;
            
            velocity.x += accelSpeed * targetDir.x;
            velocity.z += accelSpeed * targetDir.z;
        }
        
        /// <summary>
        /// Air acceleration for strafe jumping
        /// </summary>
        private void AirAccelerate(Vector3 targetDir, float targetSpeed, float accel)
        {
            float currentSpeed = Vector3.Dot(velocity, targetDir);
            float addSpeed = targetSpeed - currentSpeed;
            
            if (addSpeed <= 0)
                return;
            
            float accelSpeed = accel * targetSpeed * Time.fixedDeltaTime;
            
            if (accelSpeed > addSpeed)
                accelSpeed = addSpeed;
            
            velocity.x += accelSpeed * targetDir.x;
            velocity.z += accelSpeed * targetDir.z;
        }
        
        public float GetHorizontalSpeed()
        {
            Vector3 horizontalVel = new Vector3(_rbCompo.linearVelocity.x, 0, _rbCompo.linearVelocity.z);
            return horizontalVel.magnitude;
        }
        
        private void OnDrawGizmos()
        {
            if (_rbCompo == null) return;
            
            Gizmos.color = CheckGroundDetected() ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * jumpRaySize);
            
            if (useQuake3Physics)
            {
                // Draw velocity
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, _rbCompo.linearVelocity.normalized * 2f);
                
                // Draw wish direction
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, wishDir * 2f);
            }
        }
    }
}