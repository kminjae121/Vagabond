using Unity.Cinemachine;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class PlayerCamFirst : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera mainCamera;
        
        [Header("Tilt Settings")]
        [SerializeField] private float tiltSpeed = 5f;
        [SerializeField] private Transform _target;
        
        [Header("Bloodthief Style - FOV")]
        [SerializeField] private float baseFOV = 60f;
        [SerializeField] private float maxFOV = 80f;
        [SerializeField] private float fovSpeed = 5f;
        [SerializeField] private float fovSpeedThreshold = 10f;
        [SerializeField] private float fovMaxSpeed = 20f;
        
        [Header("Bloodthief Style - Camera Shake")]
        [SerializeField] private float landingShakeAmount = 0.3f;
        [SerializeField] private float landingShakeDuration = 0.2f;
        [SerializeField] private float wallJumpShakeAmount = 0.2f;
        [SerializeField] private float wallJumpShakeDuration = 0.15f;
        
        [Header("Bloodthief Style - Camera Lag")]
        [SerializeField] private float cameraLagAmount = 0.1f;
        [SerializeField] private float cameraLagSpeed = 10f;
        
        [Header("Bloodthief Style - Landing Punch")]
        [SerializeField] private float landingPunchAmount = 0.5f;
        [SerializeField] private float landingPunchSpeed = 8f;
        
        [Header("Position Settings")]
        [SerializeField] private Vector3 _targetPos;
        
        public float slideAngle { get; private set; }
        private float _targetSlideAngle = 0f;
        private float currentFOV;
        private float targetFOV;
        
        private Vector3 shakeOffset = Vector3.zero;
        private float shakeTimer = 0f;
        private float shakeAmount = 0f;
        
        private Vector3 lagOffset = Vector3.zero;
        private Vector3 lastVelocity = Vector3.zero;
        
        private float landingPunchOffset = 0f;
        private float landingPunchVelocity = 0f;
        
        private float lastYVelocity = 0f;
        private bool wasInAir = false;
        
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
            UpdateCameraShake();
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

            Quaternion tiltRotation = Quaternion.Euler(xRot, 0f, zRot);
            Quaternion shakeRotation = Quaternion.Euler(shakeOffset.x * 10f, shakeOffset.y * 10f, shakeOffset.z * 10f);
            
            transform.localRotation = tiltRotation * shakeRotation;
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
            Vector3 currentVelocity = new Vector3(speed, 0, 0);
            Vector3 velocityDelta = currentVelocity - lastVelocity;
            
            Vector3 targetLag = -velocityDelta * cameraLagAmount;
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
        
        private void UpdateCameraShake()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                
                shakeOffset = Random.insideUnitSphere * shakeAmount;
                shakeAmount = Mathf.Lerp(shakeAmount, 0, Time.deltaTime * 5f);
            }
            else
            {
                shakeOffset = Vector3.Lerp(shakeOffset, Vector3.zero, Time.deltaTime * 10f);
            }
        }
        
        private void UpdateLandingPunch()
        {
            landingPunchOffset = Mathf.SmoothDamp(landingPunchOffset, 0f, ref landingPunchVelocity, 1f / landingPunchSpeed);
        }
        
        public void OnJump()
        {
        }
        
        public void OnWallJump()
        {
            TriggerShake(wallJumpShakeAmount, wallJumpShakeDuration);
        }
        
        public void OnLanding(float impactVelocity)
        {
            float impactForce = Mathf.Clamp01(impactVelocity / 20f);
            TriggerShake(landingShakeAmount * impactForce, landingShakeDuration);
            landingPunchOffset = -landingPunchAmount * impactForce;
        }
        
        private void TriggerShake(float amount, float duration)
        {
            shakeAmount = amount;
            shakeTimer = duration;
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