using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class PlayerCamFirst : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera mainCamera;
        
        [Header("Tilt Settings")]
        [SerializeField] private float tiltSpeed = 5f;
        [SerializeField] private float climbingTiltSmoothness = 7.5f;
        [SerializeField] private Transform _target;
        
        [Header("Bloodthief Style - FOV")]
        [SerializeField] private float baseFOV = 60f;
        [SerializeField] private float maxFOV = 80f;
        [SerializeField] private float wallSlideFOV = 55f;
        [SerializeField] private float climbingFOV = 58f;
        [SerializeField] private float fovSpeed = 5f;
        [SerializeField] private float fovSpeedThreshold = 10f;
        [SerializeField] private float fovMaxSpeed = 20f;
        
        [Header("Bloodthief Style - Camera Lag")]
        [SerializeField] private float cameraLagAmount = 0.1f;
        [SerializeField] private float cameraLagSpeed = 10f;
        [SerializeField] private float wallSlideLagAmount = 0.15f;
        
        [Header("Bloodthief Style - Landing Punch")]
        [SerializeField] private float landingPunchAmount = 0.5f;
        [SerializeField] private float landingPunchSpeed = 8f;
        
        [Header("Position Settings")]
        [SerializeField] private Vector3 _targetPos;
        
        public float slideAngle { get; private set; }
        private float _targetSlideAngle = 0f;
        private float currentFOV;
        private float targetFOV;
        
        private Vector3 lagOffset = Vector3.zero;
        private Vector3 lastVelocity = Vector3.zero;
        
        private float landingPunchOffset = 0f;
        private float landingPunchVelocity = 0f;
        
        private float lastYVelocity = 0f;
        private bool wasInAir = false;
        
        private bool isWallSlideMode = false;
        private bool isClimbingMode = false;
        private float climbingTiltAngle = 0f;
        private float currentClimbingTilt = 0f;
        
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            if (mainCamera != null)
            {
                currentFOV = mainCamera.fieldOfView;
                targetFOV = baseFOV;
            }
        }

        private void Start()
        {
            _targetPos = transform.position;
        }
        
        private void Update()
        {
            ApplyWorldTilt();
            SmoothTilt();
            UpdateLandingPunch();
        }

        private void LateUpdate()
        {
            ApplyCameraEffects();
        }
        
        private void ApplyWorldTilt()
        {
            if (_target == null) return;

            float yRot = _target.eulerAngles.y;
            float radY = yRot * Mathf.Deg2Rad;

            float sinY = Mathf.Sin(radY);
            float cosY = Mathf.Cos(radY);

            float xRot = sinY * slideAngle;
            float zRot = cosY * slideAngle;
            
            if (isClimbingMode)
            {
                currentClimbingTilt = Mathf.Lerp(currentClimbingTilt, climbingTiltAngle, Time.deltaTime * climbingTiltSmoothness);
                xRot = -currentClimbingTilt;
                zRot = 0f;
            }
            else
            {
                currentClimbingTilt = Mathf.Lerp(currentClimbingTilt, 0f, Time.deltaTime * climbingTiltSmoothness);
            }

            Quaternion tiltRotation = Quaternion.Euler(xRot, 0f, zRot);
            transform.localRotation = tiltRotation;
        }
        
        private void SmoothTilt()
        {
            slideAngle = Mathf.Lerp(slideAngle, _targetSlideAngle, Time.deltaTime * tiltSpeed);
        }
        
        private void ApplyCameraEffects()
        {
            Vector3 targetPosition = _targetPos + lagOffset;
            targetPosition.y += landingPunchOffset;
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            
            if (mainCamera != null)
            {
                currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovSpeed);
                mainCamera.fieldOfView = currentFOV;
            }
        }
        
        public void UpdateCameraEffects(float currentSpeed, bool isGrounded, bool wasGrounded)
        {
            UpdateFOV(currentSpeed);
            UpdateCameraLag(currentSpeed);
            CheckLanding(isGrounded, wasGrounded);
        }
        
        private void UpdateFOV(float speed)
        {
            if (isClimbingMode)
            {
                targetFOV = climbingFOV;
                return;
            }
            
            if (isWallSlideMode)
            {
                targetFOV = wallSlideFOV;
                return;
            }
            
            if (speed < fovSpeedThreshold)
            {
                targetFOV = baseFOV;
            }
            else
            {
                float speedPercent = Mathf.Clamp01((speed - fovSpeedThreshold) / (fovMaxSpeed - fovSpeedThreshold));
                targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedPercent);
            }
        }
        
        private void UpdateCameraLag(float speed)
        {
            float currentLagAmount = isWallSlideMode ? wallSlideLagAmount : cameraLagAmount;
            
            Vector3 currentVelocity = new Vector3(speed, 0, 0);
            Vector3 velocityDelta = currentVelocity - lastVelocity;
            
            Vector3 targetLag = -velocityDelta * currentLagAmount;
            lagOffset = Vector3.Lerp(lagOffset, targetLag, Time.deltaTime * cameraLagSpeed);
            
            lastVelocity = currentVelocity;
        }
        
        private void CheckLanding(bool isGrounded, bool wasGrounded)
        {
            if (isGrounded && !wasGrounded && lastYVelocity < -5f)
            {
                OnLanding(Mathf.Abs(lastYVelocity));
            }
            
            wasInAir = !isGrounded;
        }
        
        private void UpdateLandingPunch()
        {
            landingPunchOffset = Mathf.SmoothDamp(landingPunchOffset, 0f, ref landingPunchVelocity, 1f / landingPunchSpeed);
        }
        
        public void SetWallSlideMode(bool active)
        {
            isWallSlideMode = active;
        }
        
        public void SetClimbingMode(bool active, float tiltAngle)
        {
            isClimbingMode = active;
            climbingTiltAngle = tiltAngle;
            
            if (!active)
            {
                currentClimbingTilt = 0f;
            }
        }
        
        public void OnJump()
        {
        }
        
        public void OnLanding(float impactVelocity)
        {
            float impactForce = Mathf.Clamp01(impactVelocity / 20f);
            landingPunchOffset = -landingPunchAmount * impactForce;
        }

        public void SetCamTrm()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * 10);
        }

        public void SetCamPos(Vector3 pos)
        {
            _targetPos = pos;
        }

        public void SetTilt(float targetAngle)
        {
            _targetSlideAngle = targetAngle;
        }

        public void ReturnOwnTilt()
        {
            _targetSlideAngle = 0;
        }
        
        public void SetLastYVelocity(float yVelocity)
        {
            lastYVelocity = yVelocity;
        }
    }
}