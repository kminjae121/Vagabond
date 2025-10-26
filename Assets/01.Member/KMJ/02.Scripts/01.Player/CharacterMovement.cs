using Code.Core.Debugs;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class CharacterMovement : MonoBehaviour, IEntityComponent
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 12.0f;
        [SerializeField] private float jumpSpeed = 10.0f;
        [SerializeField] private float gravity = 25.0f;
        
        [Header("Acceleration")]
        [SerializeField] private float runAcceleration = 25.0f;
        [SerializeField] private float runDeacceleration = 15.0f;
        [SerializeField] private float airAcceleration = 12.0f;
        [SerializeField] private float airDecceleration = 10.0f;
        [SerializeField] private float airControl = 1.0f;
        
        [Header("Strafe Settings")]
        [SerializeField] private float sideStrafeSpeed = 1.0f;
        [SerializeField] private float sideStrafeAcceleration = 50.0f;
        
        [Header("Friction")]
        [SerializeField] private float friction = 4.0f;
        
        [Header("Ground Slide")]
        [SerializeField] private float groundSlideSpeedBoost = 10.0f;
        [SerializeField] private float groundSlideFriction = 1.0f;
        [SerializeField] private float groundSlideAcceleration = 10.0f;
        
        [Header("Wall Slide")]
        [field: SerializeField] public float wallSlideForwardSpeed { get; set; } = 10.0f;
        [SerializeField] private float wallJumpAwayForce = 5.0f;
        
        [Header("Speed Limits")]
        [SerializeField] private float _maxmoveSpeed = 20f;
        public float maxmoveSpeed 
        { 
            get => _maxmoveSpeed; 
            set => _maxmoveSpeed = value; 
        }
        
        [Header("Ground Detection")]
        [SerializeField] private float groundCheckRadius = 0.4f;
        [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask whatIsGround;
        
        [Header("Jump Settings")]
        [SerializeField] private int _maxJumpCnt = 2;
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        [SerializeField] private float jumpGroundIgnoreDuration = 0.1f;
        [SerializeField] private bool showJumpDebug = false;
        
        [Header("Debug Display")]
        [SerializeField] private bool showSpeedDebug = true;
        [SerializeField] private GUIStyle debugStyle;
        [SerializeField] private float fpsDisplayRate = 4.0f;
        
        public Vector3 _move;
        public int _jumpCnt { get; set; }
        public int maxJumpCnt 
        { 
            get => _maxJumpCnt; 
            set => _maxJumpCnt = value; 
        }
        
        public float baseSpeed { get; private set; }
        public bool isGrounded => isGroundedCached;
        public bool isGroundSliding { get; private set; }
        public bool isWallSliding { get; private set; }
        public bool isImpulseActive { get; private set; }
        public bool isGuidedMovement { get; private set; }

        private float originGravity;
        
        private Entity _entity;
        private Player _player;
        private CharacterController _controller;
        
        private Vector3 playerVelocity = Vector3.zero;
        private Vector3 moveDirectionNorm = Vector3.zero;
        private Vector3 wallNormal = Vector3.zero;
        private bool wishJump = false;
        private float playerFriction = 0.0f;
        
        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private bool wasGrounded;
        private bool canUseCoyoteTime;
        private bool isGroundedCached;
        
        private bool pendingJump = false;
        private bool pendingWallJump = false;
        private bool pendingGroundSlideJump = false;
        private Vector3 pendingWallJumpDirection;
        private float ignoreGroundTime = 0f;
        
        private Vector3 impulseVelocity = Vector3.zero;
        private float impulseDuration = 0f;
        private float impulseTimer = 0f;
        
        private Transform guidedTarget;
        private float guidedSpeed;
        private float guidedStopDistance;
        
        private float playerTopVelocity = 0.0f;
        private int frameCount = 0;
        private float dt = 0.0f;
        private float fps = 0.0f;

        public float GetCurrentMoveSpeed()
        {
            return moveSpeed;
        }
        
        
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _player = entity as Player;
            _controller = entity.GetComponent<CharacterController>();
            
            if (_controller == null)
            {
                UnityLogger.LogError("CharacterController가 엔티티에 없습니다. CharacterController를 추가해주세요.");
                return;
            }
            
            if (_player == null)
            {
                UnityLogger.LogError("CharacterMovement는 Player 엔티티에만 사용할 수 있습니다.");
                return;
            }
            
            SetupCharacterController();
            baseSpeed = moveSpeed;
            originGravity = gravity;
        }
        
        private void SetupCharacterController()
        {
            _controller.minMoveDistance = 0f;
            _controller.skinWidth = 0.08f;
            _controller.stepOffset = 0.3f;
            _controller.slopeLimit = 45f;
        }
        
        public void SetMove(float XMove, float ZMove)
        {
            _move.x = XMove;
            _move.z = ZMove;
        }
        
        private void UpdateMoveFromInput()
        {
            if (_player != null && _player.inputReader != null)
            {
                Vector2 moveValue = _player.inputReader.MoveValue;
                _move.x = moveValue.x;
                _move.z = moveValue.y;
            }
        }
        
        public bool CheckGroundDetected()
        {
            if (_controller == null) return false;
            
            Vector3 spherePosition = transform.position + Vector3.down * groundCheckDistance;
            return Physics.CheckSphere(spherePosition, groundCheckRadius, whatIsGround);
        }
        
        public void StartGroundSlide()
        {
            if (!isGroundedCached) return;
            
            isGroundSliding = true;
            isWallSliding = false;
        }
        
        public void StopGroundSlide()
        {
            isGroundSliding = false;
        }
        
        public void StartWallSlide(Vector3 normal)
        {
            isWallSliding = true;
            isGroundSliding = false;
            wallNormal = normal;
            playerVelocity.y = 0;
        }
        
        public void StopWallSlide()
        {
            isWallSliding = false;
        }

        public void SetGravityZero()
        {
            gravity = 0;
        }

        public void SetOriginGravity()
        {
            gravity = originGravity;
        }
        
        public void ApplyImpulse(Vector3 direction, float force, float duration)
        {
            impulseVelocity = direction.normalized * force;
            impulseDuration = duration;
            impulseTimer = 0f;
            isImpulseActive = true;
            
            if (showJumpDebug)
            {
                UnityLogger.Log($"Impulse 적용: 방향={direction}, 힘={force}, 지속시간={duration}");
            }
        }
        
        public void ApplyWallKick(Vector3 wallNormal, Transform playerTransform, float awayForce, float forwardForce, float upForce)
        {
            Vector3 kickDirection = wallNormal.normalized;
            kickDirection.y = 0;
            
            Vector3 kickVelocity = kickDirection * awayForce;
            
            Vector3 forwardDirection = playerTransform.forward;
            forwardDirection.y = 0;
            forwardDirection.Normalize();
            
            kickVelocity += forwardDirection * forwardForce;
            
            kickVelocity.y = upForce;
            
            float totalForce = kickVelocity.magnitude;
            float duration = 0.3f;
            
            ApplyImpulse(kickVelocity.normalized, totalForce, duration);

            if (showJumpDebug)
            {
                UnityLogger.Log($"벽 킥: 벽반대힘={awayForce}, 전진힘={forwardForce}, 수직힘={upForce}, 방향={kickVelocity.normalized}");
            }
        }
        
        public void StopImpulse()
        {
            isImpulseActive = false;
            impulseVelocity = Vector3.zero;
            impulseTimer = 0f;
        }
        
        public void SetGuidedMovement(Transform target, float speed, float stopDistance)
        {
            if (target == null)
            {
                UnityLogger.LogError("Guided Movement 타겟이 null입니다.");
                return;
            }
            
            guidedTarget = target;
            guidedSpeed = speed;
            guidedStopDistance = stopDistance;
            isGuidedMovement = true;
            
            if (showJumpDebug)
            {
                UnityLogger.Log($"Guided Movement 시작: 타겟={target.name}, 속도={speed}");
            }
        }
        
        public void StopGuidedMovement()
        {
            isGuidedMovement = false;
            guidedTarget = null;
            
            if (showJumpDebug)
            {
                UnityLogger.Log("Guided Movement 종료");
            }
        }
        
        public bool IsGuidedMovementComplete()
        {
            if (!isGuidedMovement || guidedTarget == null) return true;
            
            float distance = Vector3.Distance(transform.position, guidedTarget.position);
            return distance <= guidedStopDistance;
        }
        
        public void RequestJump()
        {
            jumpBufferCounter = jumpBufferTime;
            
            if (showJumpDebug)
            {
                UnityLogger.Log("점프 요청 수신 - 버퍼 활성화");
            }
            
            Jump();
        }
        
        public void Jump()
        {
            if (isWallSliding)
            {
                WallJump();
                return;
            }
            
            if (isGroundSliding)
            {
                GroundSlideJump();
                return;
            }
            
            bool canJumpFromGround = isGroundedCached || (coyoteTimeCounter > 0 && canUseCoyoteTime);
            
            if (canJumpFromGround)
            {
                pendingJump = true;
                jumpBufferCounter = 0;
                _jumpCnt = 1;
                
                if (!isGroundedCached && coyoteTimeCounter > 0)
                {
                    canUseCoyoteTime = false;
                    if (showJumpDebug)
                    {
                        UnityLogger.Log("코요테 타임 점프 실행!");
                    }
                }
                else if (showJumpDebug)
                {
                    UnityLogger.Log("지상 점프 실행!");
                }
            }
            else if (_jumpCnt < maxJumpCnt)
            {
                pendingJump = true;
                jumpBufferCounter = 0;
                _jumpCnt++;
                
                if (showJumpDebug)
                {
                    UnityLogger.Log($"공중 점프 #{_jumpCnt} 실행!");
                }
            }
            else
            {
                if (showJumpDebug)
                {
                    UnityLogger.Log($"점프 불가: 점프 횟수 {_jumpCnt}/{maxJumpCnt}, 지상: {isGroundedCached}, 코요테: {coyoteTimeCounter:F2}");
                }
            }
        }
        
        private void WallJump()
        {
            Vector3 jumpDirection = wallNormal.normalized;
            jumpDirection.y = 0;
            
            pendingWallJump = true;
            pendingWallJumpDirection = jumpDirection;
            jumpBufferCounter = 0;
            _jumpCnt = 1;
            
            StopWallSlide();
            
            if (showJumpDebug)
            {
                UnityLogger.Log("벽 점프 실행!");
            }
        }
        
        private void GroundSlideJump()
        {
            pendingGroundSlideJump = true;
            jumpBufferCounter = 0;
            _jumpCnt = 1;
            
            StopGroundSlide();
            
            if (showJumpDebug)
            {
                UnityLogger.Log("슬라이드 점프 실행!");
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
                    
                    if (showJumpDebug)
                    {
                        UnityLogger.Log("버퍼에서 점프 실행!");
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
                
                if (!wasGrounded)
                {
                    _jumpCnt = 0;
                }
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
            
            wasGrounded = isGroundedCached;
        }
        
        public void StopMoving()
        {
            playerVelocity = Vector3.zero;
            StopImpulse();
            StopGuidedMovement();
        }
        
        public void SetSpeed(float targetSpeedValue)
        {
            moveSpeed = targetSpeedValue;
            
            if (showJumpDebug)
            {
                UnityLogger.Log($"[CharacterMovement] moveSpeed 변경: {targetSpeedValue}");
            }
        }
        
        public void SetReturnOriginMoveSpeed()
        {
            moveSpeed = baseSpeed;
            
            if (showJumpDebug)
            {
                UnityLogger.Log($"[CharacterMovement] moveSpeed 원래 속도로 복구: {baseSpeed}");
            }
        }
        
        private void Update()
        {
            if (_controller == null) return;
            
            if (ignoreGroundTime > 0)
            {
                ignoreGroundTime -= Time.deltaTime;
                isGroundedCached = false;
            }
            else
            {
                isGroundedCached = _controller.isGrounded || CheckGroundDetected();
            }
            
            ProcessCoyoteTime();
            ProcessJumpBuffer();
            UpdateMoveFromInput();
            QueueJump();
            UpdateFPS();
            
            if (isImpulseActive)
            {
                ImpulseMove();
            }
            else if (isGuidedMovement)
            {
                GuidedMove();
            }
            else if (isWallSliding)
            {
                WallSlideMove();
            }
            else if (isGroundSliding)
            {
                GroundSlideMove();
            }
            else if (isGroundedCached)
            {
                GroundMove();
            }
            else
            {
                AirMove();
            }
            
            ProcessPendingJumps();
            
            _controller.Move(playerVelocity * Time.deltaTime);
            
            CheckCollisionStop();
            
            Vector3 udp = playerVelocity;
            udp.y = 0.0f;
            if (udp.magnitude > playerTopVelocity)
            {
                playerTopVelocity = udp.magnitude;
            }
            
            if (_player != null && _player.camCompo != null)
            {
                _player.camCompo.UpdateCameraEffects(GetHorizontalSpeed(), isGroundedCached, wasGrounded);
            }
        }
        
        private void ImpulseMove()
        {
            impulseTimer += Time.deltaTime;
            
            if (impulseTimer >= impulseDuration)
            {
                StopImpulse();
                if (_player != null)
                {
                    _player.ChangeState("IDLE");
                }
                return;
            }
            
            playerVelocity = impulseVelocity;
            playerVelocity.y -= gravity * Time.deltaTime;
        }
        
        private void GuidedMove()
        {
            if (guidedTarget == null)
            {
                StopGuidedMovement();
                return;
            }
            
            Vector3 direction = (guidedTarget.position - transform.position).normalized;
            playerVelocity = direction * guidedSpeed;
            
            float distance = Vector3.Distance(transform.position, guidedTarget.position);
            if (distance <= guidedStopDistance)
            {
                StopGuidedMovement();
                if (_player != null)
                {
                    _player.ChangeState("IDLE");
                }
            }
        }
        
        private void CheckCollisionStop()
        {
            if (isImpulseActive && (_controller.collisionFlags & CollisionFlags.Sides) != 0)
            {
                StopImpulse();
                if (_player != null)
                {
                    _player.ChangeState("IDLE");
                }
                
                if (showJumpDebug)
                {
                    UnityLogger.Log("벽 충돌로 Impulse 중단");
                }
            }
        }
        
        private void ProcessPendingJumps()
        {
            if (pendingJump)
            {
                playerVelocity.y = jumpSpeed;
                pendingJump = false;
                wishJump = false;
                ignoreGroundTime = jumpGroundIgnoreDuration;
                
                if (_player != null && _player.camCompo != null)
                {
                    _player.camCompo.OnJump();
                }
                
                if (showJumpDebug)
                {
                    UnityLogger.Log($"점프 적용: velocity.y = {jumpSpeed}");
                }
            }
            else if (pendingWallJump)
            {
                playerVelocity = pendingWallJumpDirection * wallJumpAwayForce;
                playerVelocity.y = jumpSpeed;
                pendingWallJump = false;
                wishJump = false;
                ignoreGroundTime = jumpGroundIgnoreDuration;
                
                if (showJumpDebug)
                {
                    UnityLogger.Log("벽 점프 적용");
                }
            }
            else if (pendingGroundSlideJump)
            {
                Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
                playerVelocity = horizontalVelocity;
                playerVelocity.y = jumpSpeed;
                pendingGroundSlideJump = false;
                wishJump = false;
                ignoreGroundTime = jumpGroundIgnoreDuration;
                
                if (_player != null && _player.camCompo != null)
                {
                    _player.camCompo.OnJump();
                }
                
                if (showJumpDebug)
                {
                    UnityLogger.Log($"슬라이드 점프 적용: 속도 보존 {horizontalVelocity.magnitude:F2}");
                }
            }
        }
        
        private void UpdateFPS()
        {
            frameCount++;
            dt += Time.deltaTime;
            if (dt > 1.0f / fpsDisplayRate)
            {
                fps = Mathf.Round(frameCount / dt);
                frameCount = 0;
                dt -= 1.0f / fpsDisplayRate;
            }
        }
        
        private void QueueJump()
        {
            wishJump = jumpBufferCounter > 0;
        }
        
        private void GroundMove()
        {
            if (!wishJump)
            {
                ApplyFriction(1.0f);
            }
            else
            {
                ApplyFriction(0);
            }
            
            Vector3 wishdir = new Vector3(_move.x, 0, _move.z);
            wishdir = transform.TransformDirection(wishdir);
            wishdir.Normalize();
            moveDirectionNorm = wishdir;
            
            float wishspeed = wishdir.magnitude;
            wishspeed *= moveSpeed;
            
            Accelerate(wishdir, wishspeed, runAcceleration);
            
            if (playerVelocity.y <= 0)
            {
                playerVelocity.y = -gravity * Time.deltaTime;
            }
            
            if (wishJump)
            {
                playerVelocity.y = jumpSpeed;
                wishJump = false;
                _jumpCnt = 1;
            }
        }
        
        private void GroundSlideMove()
        {
            if (!wishJump)
            {
                ApplyFriction(groundSlideFriction / friction);
            }
            else
            {
                ApplyFriction(0);
            }
            
            Vector3 wishdir = new Vector3(_move.x, 0, _move.z);
            wishdir = transform.TransformDirection(wishdir);
            wishdir.Normalize();
            moveDirectionNorm = wishdir;
            
            float wishspeed = wishdir.magnitude;
            wishspeed *= (moveSpeed + groundSlideSpeedBoost);
            
            Accelerate(wishdir, wishspeed, groundSlideAcceleration);
            
            if (playerVelocity.y <= 0)
            {
                playerVelocity.y = -gravity * Time.deltaTime;
            }
            
            if (wishJump)
            {
                Vector3 horizontalVelocity = new Vector3(playerVelocity.x, 0, playerVelocity.z);
                playerVelocity = horizontalVelocity;
                playerVelocity.y = jumpSpeed;
                wishJump = false;
                _jumpCnt = 1;
                StopGroundSlide();
            }
        }

        public float GetWallSlideSpeed()
        {
            return wallSlideForwardSpeed;
        }
        
        private void WallSlideMove()
        {
            Vector3 wishdir = new Vector3(_move.x, 0, _move.z);
            wishdir = transform.TransformDirection(wishdir);
            wishdir.Normalize();
            moveDirectionNorm = wishdir;
            
            float wishspeed = wishdir.magnitude * wallSlideForwardSpeed;

            wallSlideForwardSpeed -= Time.deltaTime * 2f;
            
            Vector3 horizontalVel = new Vector3(playerVelocity.x, 0, playerVelocity.z);
            Vector3 targetVel = wishdir * wishspeed;
            
            horizontalVel = Vector3.Lerp(horizontalVel, targetVel, Time.deltaTime * 10f);
            
            playerVelocity.x = horizontalVel.x;
            playerVelocity.z = horizontalVel.z;
            playerVelocity.y = 0;
            
            if (wishJump)
            {
                Vector3 jumpDirection = wallNormal.normalized;
                jumpDirection.y = 0;
                
                playerVelocity = jumpDirection * wallJumpAwayForce;
                playerVelocity.y = jumpSpeed;
                wishJump = false;
                _jumpCnt = 1;
                StopWallSlide();
            }
        }
        
        private void AirMove()
        {
            Vector3 wishdir = new Vector3(_move.x, 0, _move.z);
            wishdir = transform.TransformDirection(wishdir);
            
            float wishspeed = wishdir.magnitude;
            wishspeed *= moveSpeed;
            
            wishdir.Normalize();
            moveDirectionNorm = wishdir;
            
            float wishspeed2 = wishspeed;
            float accel;
            
            if (Vector3.Dot(playerVelocity, wishdir) < 0)
            {
                accel = airDecceleration;
            }
            else
            {
                accel = airAcceleration;
            }
            
            if (_move.z == 0 && _move.x != 0)
            {
                if (wishspeed > sideStrafeSpeed)
                {
                    wishspeed = sideStrafeSpeed;
                }
                accel = sideStrafeAcceleration;
            }
            
            Accelerate(wishdir, wishspeed, accel);
            
            if (airControl > 0)
            {
                AirControl(wishdir, wishspeed2);
            }
            
            playerVelocity.y -= gravity * Time.deltaTime;
        }
        
        private void AirControl(Vector3 wishdir, float wishspeed)
        {
            if (Mathf.Abs(_move.z) < 0.001f || Mathf.Abs(wishspeed) < 0.001f)
            {
                return;
            }
            
            float zspeed = playerVelocity.y;
            playerVelocity.y = 0;
            
            float speed = playerVelocity.magnitude;
            playerVelocity.Normalize();
            
            float dot = Vector3.Dot(playerVelocity, wishdir);
            float k = 32;
            k *= airControl * dot * dot * Time.deltaTime;
            
            if (dot > 0)
            {
                playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
                playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
                playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;
                
                playerVelocity.Normalize();
                moveDirectionNorm = playerVelocity;
            }
            
            playerVelocity.x *= speed;
            playerVelocity.y = zspeed;
            playerVelocity.z *= speed;
        }
        
        private void ApplyFriction(float t)
        {
            Vector3 vec = playerVelocity;
            vec.y = 0.0f;
            float speed = vec.magnitude;
            float drop = 0.0f;
            
            if (isGroundedCached)
            {
                float control = speed < runDeacceleration ? runDeacceleration : speed;
                drop = control * friction * Time.deltaTime * t;
            }
            
            float newspeed = speed - drop;
            playerFriction = newspeed;
            
            if (newspeed < 0)
            {
                newspeed = 0;
            }
            
            if (speed > 0)
            {
                newspeed /= speed;
            }
            
            playerVelocity.x *= newspeed;
            playerVelocity.z *= newspeed;
        }
        
        private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
        {
            float currentspeed = Vector3.Dot(playerVelocity, wishdir);
            float addspeed = wishspeed - currentspeed;
            
            if (addspeed <= 0)
            {
                return;
            }
            
            float accelspeed = accel * Time.deltaTime * wishspeed;
            
            if (accelspeed > addspeed)
            {
                accelspeed = addspeed;
            }
            
            playerVelocity.x += accelspeed * wishdir.x;
            playerVelocity.z += accelspeed * wishdir.z;
        }
        
        public float GetHorizontalSpeed()
        {
            Vector3 horizontalVel = new Vector3(playerVelocity.x, 0, playerVelocity.z);
            return horizontalVel.magnitude;
        }
        
        public Vector3 GetVelocity()
        {
            return playerVelocity;
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
        
        private void OnGUI()
        {
            if (!showSpeedDebug) return;

            GUIStyle originalStyle = debugStyle ?? GUI.skin.label;

            GUIStyle style = new GUIStyle(originalStyle);

            style.normal.textColor = Color.white; 
            
            GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + fps, style);
    
            Vector3 ups = playerVelocity;
            ups.y = 0;
            GUI.Label(new Rect(0, 15, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100 + " ups", style);
            GUI.Label(new Rect(0, 30, 400, 100), "Top Speed: " + Mathf.Round(playerTopVelocity * 100) / 100 + " ups", style);
            GUI.Label(new Rect(0, 45, 400, 100), "MoveSpeed: " + moveSpeed.ToString("F1"), style);
    
            if (showJumpDebug)
            {
                GUI.Label(new Rect(0, 60, 400, 100), "Grounded: " + isGroundedCached, style);
                GUI.Label(new Rect(0, 75, 400, 100), "Velocity.Y: " + playerVelocity.y.ToString("F2"), style);
                GUI.Label(new Rect(0, 90, 400, 100), "Coyote: " + coyoteTimeCounter.ToString("F2") + "s", style);
                GUI.Label(new Rect(0, 105, 400, 100), "Buffer: " + jumpBufferCounter.ToString("F2") + "s", style);
                GUI.Label(new Rect(0, 120, 400, 100), "Jump Count: " + _jumpCnt, style);
                GUI.Label(new Rect(0, 135, 400, 100), "Ground Slide: " + isGroundSliding, style);
                GUI.Label(new Rect(0, 150, 400, 100), "Wall Slide: " + isWallSliding, style);
                GUI.Label(new Rect(0, 165, 400, 100), "Impulse Active: " + isImpulseActive, style);
                GUI.Label(new Rect(0, 180, 400, 100), "Guided Movement: " + isGuidedMovement, style);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (_controller == null) return;
            
            Gizmos.color = isGroundedCached ? Color.green : Color.red;
            Vector3 spherePosition = transform.position + Vector3.down * groundCheckDistance;
            Gizmos.DrawWireSphere(spherePosition, groundCheckRadius);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, playerVelocity.normalized * 2f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, moveDirectionNorm * 2f);
            
            if (isWallSliding)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(transform.position, wallNormal * 2f);
            }
            
            if (isGuidedMovement && guidedTarget != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, guidedTarget.position);
                Gizmos.DrawWireSphere(guidedTarget.position, guidedStopDistance);
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