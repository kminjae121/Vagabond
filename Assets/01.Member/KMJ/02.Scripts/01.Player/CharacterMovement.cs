using _00.CORE._02.Scripts.Input;
using Code.Core.Stats;
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class CharacterMovement : MonoBehaviour, IEntityComponent
    {
        [Header("Base Stats")]
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private StatSO jumpSpeedStat;
        [SerializeField] private StatSO maxMoveSpeedStat;
        
        [Header("Ground Detection")]
        [SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private float groundStickForce = 5f;
        
        [Header("Advanced Movement Settings")]
        [SerializeField] private bool enableAdvancedMovement = true;
        [SerializeField] private float gravity = 800f * 0.0254f; // Hammer scale
        [SerializeField] private float friction = 6f;
        [SerializeField] private float groundAcceleration = 14f;
        [SerializeField] private float groundDeceleration = 10f;
        [SerializeField] private float airAcceleration = 2f;
        [SerializeField] private float airControl = 0.3f;
        [SerializeField] private float airCap = 30f * 0.0254f;
        [SerializeField] private float sideStrafeSpeed = 1f;
        [SerializeField] private float sideStrafeAcceleration = 50f;
        [SerializeField] private bool autoBhop = true;
        
        [Header("Slope & Surface")]
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float surfSlope = 0.7f; // Surface angle threshold
        [SerializeField] private float stepSize = 0.48f;
        [SerializeField] private float movingUpRapidlyFactor = 0.85f;
        
        [Header("Collision Resolution")]
        [SerializeField] private int maxCollisionChecks = 128;
        [SerializeField] private float collisionOffset = 0.01f;
        
        [field: SerializeField] public InputReader _inputReader { get; private set; }
        
        // Movement data
        public Vector3 _move { get; private set; }
        private Vector3 _velocity;
        private Vector3 _baseVelocity;
        private Vector3 _moveDirectionNorm = Vector3.zero;
        private Vector3 _previousOrigin;
        private Vector3 _preGroundedVelocity;
        
        // Jump
        public int _jumpCnt { get; set; }
        public int maxJumpCnt { get; set; } = 2;
        private bool _wishJump = false;
        
        // Speed properties
        public float moveSpeed { get; private set; } = 8f;
        public float baseSpeed { get; private set; } = 8f;
        public float maxmoveSpeed { get; set; } = 15f;
        public float targetSpeed { get; private set; } = 0;
        public float jumpSpeed { get; private set; } = 8f;
        public float currentSpeed { get; private set; }
        public float topSpeed { get; private set; }
        
        // Ground state
        private bool _isGrounded;
        private bool _wasGrounded;
        private bool _justGrounded;
        private bool _justJumped;
        private RaycastHit _groundHit;
        private Vector3 _groundNormal = Vector3.up;
        private Vector3 _surfNormal = Vector3.zero;
        private float _groundAngle;
        private GameObject _groundObject;
        private bool _surfing;
        
        // Surface properties
        private float _surfaceFriction = 1f;
        
        // Components
        private Entity _entity;
        private EntityStatCompo _statCompo;
        private Rigidbody _rbCompo;
        private BoxCollider _collider;
        
        // Input
        private float _forwardInput;
        private float _rightInput;
        
        // Collision cache
        private static Collider[] _colliders = new Collider[128];
        private static RaycastHit[] _hitCache = new RaycastHit[32];
        
        private const float HammerScale = 0.0254f;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStatCompo>();
            _rbCompo = entity.GetComponent<Rigidbody>();
            _collider = entity.GetComponent<BoxCollider>();
            
            if (_collider == null)
            {
                _collider = entity.gameObject.AddComponent<BoxCollider>();
            }
            
            ConfigureRigidbody();
            AfterInitialize();
        }
        
        private void ConfigureRigidbody()
        {
            if (_rbCompo != null)
            {
                _rbCompo.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                _rbCompo.interpolation = RigidbodyInterpolation.Interpolate;
                _rbCompo.freezeRotation = true;
                _rbCompo.useGravity = false; // Manual gravity
                _rbCompo.isKinematic = false;
            }
        }

        public void SetMove(float xMove, float zMove)
        {
            _forwardInput = zMove;
            _rightInput = xMove;
            _move = new Vector3(xMove, 0, zMove);
        }

        public bool CheckGroundDetected()
        {
            _surfaceFriction = 1f;
            _surfing = false;
            
            var trace = BoxCastToFloor(0.1f, 0.99f);
            var movingUp = _velocity.y > 0;
            
            if (trace.collider != null)
            {
                _groundNormal = trace.normal;
                _groundAngle = Vector3.Angle(Vector3.up, _groundNormal);
                
                // Check if surfing (steep slope)
                if (_groundNormal.y <= surfSlope)
                {
                    _surfing = true;
                    _surfNormal = _groundNormal;
                }
                
                return _groundAngle <= maxSlopeAngle;
            }
            
            _groundNormal = Vector3.up;
            _groundAngle = 0f;
            return false;
        }
        
        private RaycastHit BoxCastToFloor(float distance = 0.05f, float extentModifier = 1.0f)
        {
            var extents = _collider.bounds.extents * extentModifier;
            var center = transform.position;
            center.y += extents.y + 0.02f;
            distance += 0.02f;
            
            if (_velocity.y < 0)
            {
                var dv = _velocity.y * -1.01f * Time.fixedDeltaTime;
                distance = Mathf.Max(distance, dv);
            }
            
            var count = Physics.BoxCastNonAlloc(
                center,
                extents,
                Vector3.down,
                _hitCache,
                Quaternion.identity,
                distance,
                whatIsGround,
                QueryTriggerInteraction.Ignore);
            
            var greatY = float.MinValue;
            RaycastHit bestHit = default;
            
            for (int i = 0; i < count; i++)
            {
                if (!_hitCache[i].collider.enabled || _hitCache[i].point == Vector3.zero)
                {
                    continue;
                }
                
                if (bestHit.collider == null)
                {
                    bestHit = _hitCache[i];
                    greatY = _hitCache[i].point.y;
                }
                
                if (_hitCache[i].normal.y <= surfSlope && _hitCache[i].normal.y > 0)
                {
                    return _hitCache[i];
                }
                
                if (_hitCache[i].point.y > greatY)
                {
                    bestHit = _hitCache[i];
                    greatY = _hitCache[i].point.y;
                }
            }
            
            return bestHit;
        }
        
        public void Jump()
        {
            _wishJump = true;
        }
        
        private void ProcessJump()
        {
            if (!_wishJump) return;
            
            if (_isGrounded)
            {
                _velocity.y = jumpSpeed;
                _justJumped = true;
                SetGround(null);
                _jumpCnt++;
            }
            else if (_jumpCnt < maxJumpCnt && _jumpCnt > 0)
            {
                _velocity.y = jumpSpeed;
                _jumpCnt++;
            }
            
            if (!autoBhop)
            {
                _wishJump = false;
            }
        }
        
        private void CheckJump()
        {
            if (!_wishJump || _groundObject == null)
            {
                return;
            }
            
            ProcessJump();
        }
        
        public void AfterInitialize()
        {
            moveSpeed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 8f);
            jumpSpeed = _statCompo.SubscribeStat(jumpSpeedStat, HandleJumpPowerChange, 8f);
            maxmoveSpeed = _statCompo.SubscribeStat(maxMoveSpeedStat, HandleMaxMoveSpeedChange, 15f);

            baseSpeed = moveSpeed;
            targetSpeed = baseSpeed;
        }

        private void OnDestroy()
        {
            if (_statCompo != null)
            {
                _statCompo.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
                _statCompo.UnSubscribeStat(jumpSpeedStat, HandleJumpPowerChange);
                _statCompo.UnSubscribeStat(maxMoveSpeedStat, HandleMaxMoveSpeedChange);
            }
        }

        private void HandleMoveSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            moveSpeed = currentvalue;
            baseSpeed = currentvalue;
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
            _velocity = Vector3.zero;
            _rbCompo.linearVelocity = Vector3.zero;
        }

        public void SetSpeed(float targetSpeedValue)
        {
            targetSpeed = targetSpeedValue;
        }

        public void SetReturnOriginMoveSpeed()
        {
            targetSpeed = baseSpeed;
        }

        private void Update()
        {
            UpdateGroundState();
            CalculateCurrentSpeed();
        }

        private void FixedUpdate()
        {
            _previousOrigin = transform.position;
            _justGrounded = false;
            _justJumped = false;
            
            if (enableAdvancedMovement)
            {
                CalculateMovement();
            }
            else
            {
                BasicMovement();
            }
            
            ResolveCollisions();
            ApplyVelocity();
        }
        
        private void CalculateMovement()
        {
            CheckParameters();
            ApplyGravity();
            CheckJump();
            
            if (_isGrounded)
            {
                GroundMove();
            }
            else
            {
                AirMove();
            }
            
            CheckSteps();
            ClampVelocity();
        }
        
        private void CheckParameters()
        {
            var spd = (_forwardInput * _forwardInput) +
                      (_rightInput * _rightInput);
            
            spd = Mathf.Sqrt(spd);
            if (spd != 0.0f && spd > moveSpeed)
            {
                float ratio = moveSpeed / spd;
                _forwardInput *= ratio;
                _rightInput *= ratio;
            }
        }
        
        private void UpdateGroundState()
        {
            _wasGrounded = _isGrounded;
            _isGrounded = CheckGroundDetected();
            
            if (_isGrounded && !_wasGrounded)
            {
                _jumpCnt = 0;
                OnLanded();
            }
        }
        
        private void SetGround(GameObject obj, Vector3 normal = default)
        {
            if (obj != null)
            {
                if (_groundObject == null)
                {
                    _justGrounded = true;
                    _preGroundedVelocity = _velocity;
                }
                _groundObject = obj;
                _groundNormal = normal;
            }
            else
            {
                _groundObject = null;
            }
        }
        
        private void OnLanded()
        {
            SetGround(_groundHit.collider?.gameObject, _groundHit.normal);
            
            if (_justGrounded && _groundHit.collider != null)
            {
                // Reflect velocity on landing
                if (_groundNormal.y > surfSlope && _groundNormal.y < 1f)
                {
                    ClipVelocity(_velocity, _groundNormal, ref _velocity, 1.0f);
                }
                _velocity.y = 0;
            }
        }
        
        private void CalculateCurrentSpeed()
        {
            Vector3 horizontalVelocity = new Vector3(_velocity.x, 0, _velocity.z);
            currentSpeed = horizontalVelocity.magnitude;
            
            if (currentSpeed > topSpeed)
            {
                topSpeed = currentSpeed;
            }
        }
        
        #region Movement Calculation
        
        private void ApplyGravity()
        {
            if (_groundObject == null)
            {
                _velocity.y -= gravity * Time.fixedDeltaTime;
            }
        }
        
        private void GroundMove()
        {
            // Apply friction if not jumping
            if (!_wishJump)
            {
                ApplyFriction(1.0f);
            }
            else
            {
                ApplyFriction(0f);
            }
            
            Vector3 wishDir = GetWishDirection();
            wishDir = AdjustDirectionToSlope(wishDir);
            wishDir.Normalize();
            _moveDirectionNorm = wishDir;
            
            float wishSpeed = wishDir.magnitude * moveSpeed;
            
            _velocity += Accelerate(_velocity, wishDir, wishSpeed, groundAcceleration);
            
            // Ground stick
            _velocity.y = -groundStickForce * Time.fixedDeltaTime;
        }
        
        private void AirMove()
        {
            Vector3 wishDir = GetWishDirection();
            wishDir = transform.TransformDirection(wishDir);
            
            float wishSpeed = moveSpeed;
            wishDir.Normalize();
            _moveDirectionNorm = wishDir;
            
            // Strafe jumping (only left/right, no forward)
            if (_forwardInput == 0 && _rightInput != 0)
            {
                if (wishSpeed > sideStrafeSpeed)
                {
                    wishSpeed = sideStrafeSpeed;
                }
                _velocity += AirAccelerate(_velocity, wishDir, wishSpeed, sideStrafeAcceleration, airCap);
            }
            else
            {
                _velocity += AirAccelerate(_velocity, wishDir, wishSpeed, airAcceleration, airCap);
            }
            
            // Air control
            if (airControl > 0 && Mathf.Abs(_forwardInput) > 0.001f)
            {
                ApplyAirControl(wishDir, wishSpeed);
            }
        }
        
        private Vector3 GetWishDirection()
        {
            return new Vector3(_rightInput, 0, _forwardInput);
        }
        
        private Vector3 AdjustDirectionToSlope(Vector3 direction)
        {
            if (!_isGrounded) return direction;
            return Vector3.ProjectOnPlane(direction, _groundNormal);
        }
        
        private void ApplyFriction(float multiplier)
        {
            Vector3 vec = _velocity;
            vec.y = 0;
            float speed = vec.magnitude;
            float drop = 0f;
            
            if (_isGrounded)
            {
                float control = speed < groundDeceleration ? groundDeceleration : speed;
                drop = control * friction * Time.fixedDeltaTime * multiplier;
            }
            
            float newSpeed = speed - drop;
            if (newSpeed < 0) newSpeed = 0;
            if (speed > 0) newSpeed /= speed;
            
            _velocity.x *= newSpeed;
            _velocity.z *= newSpeed;
        }
        
        private Vector3 Accelerate(Vector3 currentVelocity, Vector3 wishDir, float wishSpeed, float accel)
        {
            float currentSpeed = Vector3.Dot(currentVelocity, wishDir);
            float addSpeed = wishSpeed - currentSpeed;
            
            if (addSpeed <= 0) return Vector3.zero;
            
            float accelSpeed = accel * Time.fixedDeltaTime * wishSpeed * _surfaceFriction;
            
            if (accelSpeed > addSpeed)
            {
                accelSpeed = addSpeed;
            }
            
            return accelSpeed * wishDir;
        }
        
        private Vector3 AirAccelerate(Vector3 velocity, Vector3 wishDir, float wishSpeed, float accel, float airCapValue)
        {
            float wishSpd = wishSpeed;
            
            // Cap speed
            if (wishSpd > airCapValue)
            {
                wishSpd = airCapValue;
            }
            
            float currentSpeed = Vector3.Dot(velocity, wishDir);
            float addSpeed = wishSpd - currentSpeed;
            
            if (addSpeed <= 0) return Vector3.zero;
            
            float accelSpeed = accel * wishSpeed * Time.fixedDeltaTime;
            
            if (accelSpeed > addSpeed)
            {
                accelSpeed = addSpeed;
            }
            
            return accelSpeed * wishDir;
        }
        
        private void ApplyAirControl(Vector3 wishDir, float wishSpeed)
        {
            float zSpeed = _velocity.y;
            _velocity.y = 0;
            
            float speed = _velocity.magnitude;
            _velocity.Normalize();
            
            float dot = Vector3.Dot(_velocity, wishDir);
            float k = 32f * airControl * dot * dot * Time.fixedDeltaTime;
            
            if (dot > 0)
            {
                _velocity.x = _velocity.x * speed + wishDir.x * k;
                _velocity.y = _velocity.y * speed + wishDir.y * k;
                _velocity.z = _velocity.z * speed + wishDir.z * k;
                
                _velocity.Normalize();
                _moveDirectionNorm = _velocity;
            }
            
            _velocity.x *= speed;
            _velocity.y = zSpeed;
            _velocity.z *= speed;
        }
        
        private void ClipVelocity(Vector3 input, Vector3 normal, ref Vector3 output, float overbounce)
        {
            float backoff = Vector3.Dot(input, normal) * overbounce;
            
            for (int i = 0; i < 3; i++)
            {
                float change = normal[i] * backoff;
                output[i] = input[i] - change;
            }
            
            float adjust = Vector3.Dot(output, normal);
            if (adjust < 0.0f)
            {
                output -= (normal * adjust);
            }
        }
        
        private void CheckSteps()
        {
            if (_groundObject != null)
            {
                var extents = _collider.bounds.extents;
                extents.y = 0.15f;
                var nextPos = transform.position + _velocity * Time.fixedDeltaTime;
                var center = nextPos + new Vector3(0, _collider.bounds.size.y - extents.y, 0);
                var distance = 10f;
                
                if (Physics.BoxCast(
                    center: center,
                    halfExtents: extents,
                    direction: Vector3.down,
                    orientation: Quaternion.identity,
                    maxDistance: distance,
                    layerMask: whatIsGround,
                    queryTriggerInteraction: QueryTriggerInteraction.Ignore,
                    hitInfo: out RaycastHit hit))
                {
                    if (hit.collider.enabled && hit.point != Vector3.zero && hit.normal.y > surfSlope)
                    {
                        var stepHeight = Mathf.Abs(hit.point.y - transform.position.y);
                        if (transform.position.y > hit.point.y)
                        {
                            stepHeight -= HammerScale * 2f;
                        }
                        
                        if (stepHeight <= stepSize)
                        {
                            transform.position = new Vector3(transform.position.x, hit.point.y + HammerScale, transform.position.z);
                        }
                    }
                }
            }
        }
        
        private void ClampVelocity()
        {
            for (int i = 0; i < 3; i++)
            {
                _velocity[i] = Mathf.Clamp(_velocity[i], -maxmoveSpeed, maxmoveSpeed);
            }
        }
        
        #endregion
        
        #region Collision Resolution (FragSurf Style)
        
        private void ResolveCollisions()
        {
            var staticOrigin = transform.position + new Vector3(0, _collider.bounds.extents.y, 0);
            var numOverlaps = Physics.OverlapBoxNonAlloc(
                staticOrigin,
                _collider.bounds.extents,
                _colliders,
                Quaternion.identity,
                whatIsGround,
                QueryTriggerInteraction.Ignore);
            
            for (int i = 0; i < numOverlaps; i++)
            {
                if (!_colliders[i].enabled)
                {
                    continue;
                }
                
                bool penetration = Physics.ComputePenetration(
                    _collider,
                    transform.position,
                    Quaternion.identity,
                    _colliders[i],
                    _colliders[i].transform.position,
                    _colliders[i].transform.rotation,
                    out Vector3 direction,
                    out float distance);
                
                if (!penetration)
                {
                    continue;
                }
                
                var penetrationVec = direction * (distance + collisionOffset);
                var velocityVec = -Vector3.Project(_velocity, -direction);
                
                transform.position += penetrationVec;
                staticOrigin += penetrationVec;
                
                if (_surfing)
                {
                    ClipVelocity(_velocity, direction, ref _velocity, 1.0f);
                }
                else
                {
                    _velocity += velocityVec;
                }
            }
        }
        
        #endregion
        
        #region Basic Movement
        
        private void BasicMovement()
        {
            Vector3 moveDirection = transform.TransformDirection(_move);
            
            if (_isGrounded)
            {
                _velocity = moveDirection * moveSpeed;
                
                if (_wishJump)
                {
                    _velocity.y = jumpSpeed;
                    _wishJump = false;
                    _jumpCnt++;
                }
            }
            else
            {
                Vector3 horizontalVel = new Vector3(_velocity.x, 0, _velocity.z);
                horizontalVel = Vector3.Lerp(horizontalVel, moveDirection * moveSpeed,
                    airAcceleration * Time.fixedDeltaTime);
                
                _velocity.x = horizontalVel.x;
                _velocity.z = horizontalVel.z;
                
                _velocity.y -= gravity * Time.fixedDeltaTime;
            }
        }
        
        #endregion
        
        private void ApplyVelocity()
        {
            Vector3 horizontalVel = new Vector3(_velocity.x, 0, _velocity.z);
            if (horizontalVel.magnitude > maxmoveSpeed)
            {
                horizontalVel = horizontalVel.normalized * maxmoveSpeed;
                _velocity.x = horizontalVel.x;
                _velocity.z = horizontalVel.z;
            }
            
            _rbCompo.linearVelocity = _velocity;
        }

        private void OnDrawGizmos()
        {
            if (_collider == null) return;
            
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
            
            if (_surfing)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, transform.position + _surfNormal * 2f);
            }
            
            if (_groundHit.collider != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, _groundHit.point);
                Gizmos.DrawLine(_groundHit.point, _groundHit.point + _groundNormal);
            }
            
            // Velocity direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + _velocity.normalized * 2f);
        }
    }
}