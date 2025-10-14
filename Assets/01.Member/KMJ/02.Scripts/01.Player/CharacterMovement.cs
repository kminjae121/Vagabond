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
        
        [Header("Bhop Physics")]
        [SerializeField] public bool useBhopPhysics = true;
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
        
        [Header("Jump Feel Enhancement")]
        [Tooltip("Time window after leaving ground where jump is still allowed (seconds)")]
        [SerializeField] private float coyoteTime = 0.15f;
        
        [Tooltip("Time window to buffer jump input before landing (seconds)")]
        [SerializeField] private float jumpBufferTime = 0.2f;
        
        [Tooltip("Enable visual/debug feedback for jump mechanics")]
        [SerializeField] private bool showJumpDebug = false;
        
        [Header("Ground Check")]
        [SerializeField] private float jumpRaySize = 1.2f;
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
        
        private Vector3 velocity;
        private Vector3 wishDir;
        private bool wasGrounded;
        private float currentHorizontalSpeed;
        
        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private bool isGroundedCached;
        private bool canUseCoyoteTime;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStatCompo>();
            _rbCompo = entity.GetComponent<Rigidbody>();
            
            if (_rbCompo != null && useBhopPhysics)
            {
                _rbCompo.useGravity = true;
                _rbCompo.freezeRotation = true;
            }
            
            AfterInitialize();
            
            if (_inputReader != null)
            {
                _inputReader.JumpKeyEvent += OnJumpInputReceived;
            }
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
        
        private void OnJumpInputReceived()
        {
            jumpBufferCounter = jumpBufferTime;
            
            if (showJumpDebug)
            {
                UnityLogger.Log("Jump input received - Buffer activated");
            }
        }
        
        public void Jump()
        {
            bool canJumpFromGround = isGroundedCached || (coyoteTimeCounter > 0 && canUseCoyoteTime);
            
            if (useBhopPhysics)
            {
                BhopJump(canJumpFromGround);
            }
            else
            {
                StandardJump(canJumpFromGround);
            }
        }
        
        private void ProcessJumpBuffer()
        {
            if (jumpBufferCounter > 0)
            {
                jumpBufferCounter -= Time.deltaTime;
                
                if (isGroundedCached && jumpBufferCounter > 0)
                {
                    Jump();
                    jumpBufferCounter = 0;
                    
                    if (showJumpDebug)
                    {
                        UnityLogger.Log("Jump executed from buffer!");
                    }
                }
            }
        }

        private void ProcessCoyoteTime()
        {
            if (isGroundedCached)
            {
                coyoteTimeCounter = coyoteTime;
                canUseCoyoteTime = true;

                if (wasGrounded == false)
                {
                    _jumpCnt = 0;
                }
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
        }
        
        private void StandardJump(bool canJumpFromGround)
        {
            if (canJumpFromGround)
            {
                ExecuteJump();
                
                if (!isGroundedCached && coyoteTimeCounter > 0)
                {
                    canUseCoyoteTime = false;
                    if (showJumpDebug)
                    {
                        UnityLogger.Log("Coyote time jump executed!");
                    }
                }
            }
            else if (_jumpCnt < maxJumpCnt)
            {
                ExecuteJump();
            }
        }
        
        private void BhopJump(bool canJumpFromGround)
        {
            if (canJumpFromGround)
            {
                Vector3 currentVel = _rbCompo.linearVelocity;
                
                if (enableAutoBhop && wasGrounded && currentHorizontalSpeed > baseSpeed)
                {
                    float retainedSpeed = currentHorizontalSpeed * bhopSpeedRetention;
                    Vector3 horizontalDir = new Vector3(currentVel.x, 0, currentVel.z).normalized;
                    currentVel.x = horizontalDir.x * retainedSpeed;
                    currentVel.z = horizontalDir.z * retainedSpeed;
                }
                
                currentVel.y = jumpSpeed;
                _rbCompo.linearVelocity = currentVel;
                _jumpCnt++;
                
                if (!isGroundedCached && coyoteTimeCounter > 0)
                {
                    canUseCoyoteTime = false;
                    if (showJumpDebug)
                    {
                        UnityLogger.Log($"Coyote time jump! Speed: {currentHorizontalSpeed:F2}");
                    }
                }
            }
            else if (_jumpCnt < maxJumpCnt)
            {
                Vector3 velocity = _rbCompo.linearVelocity;
                velocity.y = jumpSpeed;
                _rbCompo.linearVelocity = velocity;
                _jumpCnt++;
                
                if (showJumpDebug)
                {
                    UnityLogger.Log($"Air jump #{_jumpCnt}");
                }
            }
        }
        
        private void ExecuteJump()
        {
            _jumpCnt = 0;
            Vector3 velocity = _rbCompo.linearVelocity;
            velocity.y = 0;
            _rbCompo.linearVelocity = velocity;
            _rbCompo.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            _jumpCnt++;
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
            _statCompo?.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
            _statCompo?.UnSubscribeStat(jumpSpeedStat, HandleJumpPowerChange);
            _statCompo?.UnSubscribeStat(maxMoveSpeedStat, HandleMaxMoveSpeedChange);
            
            if (_inputReader != null)
            {
                _inputReader.JumpKeyEvent -= OnJumpInputReceived;
            }
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
            isGroundedCached = CheckGroundDetected();
            
            currentHorizontalSpeed = GetHorizontalSpeed();
            
            ProcessCoyoteTime();
            ProcessJumpBuffer();
            
            if (useBhopPhysics && showJumpDebug)
            {
                string debugInfo = $"Speed: {currentHorizontalSpeed:F2} | Max: {maxmoveSpeed:F2}";
                debugInfo += $"\nCoyote: {coyoteTimeCounter:F2}s | Buffer: {jumpBufferCounter:F2}s";
                debugInfo += $"\nGrounded: {isGroundedCached} | JumpCount: {_jumpCnt}";
                UnityLogger.Log(debugInfo);
            }
        }
        
        private void FixedUpdate()
        {
            if (useBhopPhysics)
            {
                BhopMovement();
            }
            
            wasGrounded = isGroundedCached;
        }

        private void BhopMovement()
        {
            velocity = _rbCompo.linearVelocity;

            CalculateWishDirection();
            
            if (isGroundedCached)
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
            wishDir = transform.TransformDirection(_move);
            wishDir.y = 0;
            wishDir.Normalize();
        }
        
        private void GroundMove()
        {
            ApplyFriction();

            float wishSpeed = _move.magnitude * moveSpeed;
            Accelerate(wishDir, wishSpeed, groundAccelerate);
            
            Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
            if (horizontalVel.magnitude > maxmoveSpeed)
            {
                horizontalVel = horizontalVel.normalized * maxmoveSpeed;
                velocity.x = horizontalVel.x;
                velocity.z = horizontalVel.z;
            }
        }
        
        private void AirMove()
        {
            if (!enableStrafeJumping)
            {
                return;
            }
            
            float accel = airAccelerate;
            float wishSpeed = _move.magnitude * moveSpeed;

            bool isStrafing = Mathf.Abs(_move.x) > 0.1f;
            if (isStrafing)
            {
                wishSpeed *= strafeMultiplier;
            }
            
            AirAccelerate(wishDir, wishSpeed, accel);

            Vector3 horizontalVel = new Vector3(velocity.x, 0, velocity.z);
            if (horizontalVel.magnitude > maxStrafeSpeed)
            {
                horizontalVel = horizontalVel.normalized * maxStrafeSpeed;
                velocity.x = horizontalVel.x;
                velocity.z = horizontalVel.z;
            }
        }

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
        
        public float GetCoyoteTimeRemaining()
        {
            return Mathf.Max(0, coyoteTimeCounter);
        }
        
        public float GetJumpBufferRemaining()
        {
            return Mathf.Max(0, jumpBufferCounter);
        }
        
        public bool IsCoyoteTimeActive()
        {
            return !isGroundedCached && coyoteTimeCounter > 0 && canUseCoyoteTime;
        }

        public bool IsJumpBufferActive()
        {
            return jumpBufferCounter > 0;
        }
        
        private void OnDrawGizmos()
        {
            if (_rbCompo == null) return;
            
            Gizmos.color = isGroundedCached ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down * jumpRaySize);
            
            if (useBhopPhysics)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, _rbCompo.linearVelocity.normalized * 2f);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, wishDir * 2f);
            }

            if (showJumpDebug)
            {
                if (IsCoyoteTimeActive())
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.3f);
                }

                if (IsJumpBufferActive())
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, 0.3f);
                }
            }
        }
    }
}