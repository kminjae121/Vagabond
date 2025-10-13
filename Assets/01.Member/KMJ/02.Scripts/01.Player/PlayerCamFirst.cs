using System;
using Unity.Cinemachine;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class PlayerCamFirst : MonoBehaviour
    {
        [Header("Tilt Settings")]
        [SerializeField] private float tiltSpeed = 5f;
        [SerializeField] private float maxTiltAngle = 15f;
        [SerializeField] private bool enableVelocityTilt = true;
        [SerializeField] private float velocityTiltMultiplier = 0.5f;
        
        [Header("Camera Position")]
        [SerializeField] private Vector3 _targetPos;
        [SerializeField] private Transform _target;
        [SerializeField] private float positionLerpSpeed = 10f;
        
        [Header("FOV Settings")]
        [SerializeField] private bool enableSpeedFOV = true;
        [SerializeField] private float baseFOV = 90f;
        [SerializeField] private float maxSpeedFOV = 110f;
        [SerializeField] private float speedForMaxFOV = 20f;
        [SerializeField] private float fovChangeSpeed = 5f;
        
        [Header("Head Bob")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private float bobFrequency = 2f;
        [SerializeField] private float bobHorizontalAmplitude = 0.05f;
        [SerializeField] private float bobVerticalAmplitude = 0.1f;
        
        public float slideAngle { get; private set; }
        private float _targetSlideAngle = 0f;
        private Camera _camera;
        
        // Head bob
        private float _bobTimer = 0f;
        private Vector3 _bobOffset = Vector3.zero;
        
        // Velocity calculation
        private Vector3 _lastPosition;
        private Vector3 _currentVelocity;
        
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _camera = GetComponent<Camera>();
            if (_camera != null)
            {
                _camera.fieldOfView = baseFOV;
            }
        }

        private void Start()
        {
            _targetPos = transform.position;
            _lastPosition = transform.position;
        }

        private void Update()
        {
            CalculateVelocity();
            UpdateAutoTilt();
            UpdateFOV();
            ApplyWorldTilt();
            SmoothTilt();
        }

        private void LateUpdate()
        {
            if (enableHeadBob && _currentVelocity.magnitude > 0.1f)
            {
                ApplyHeadBob();
            }
            else
            {
                _bobOffset = Vector3.Lerp(_bobOffset, Vector3.zero, Time.deltaTime * 5f);
            }
        }

        /// <summary>
        /// 현재 속도 계산 (물리 기반 효과용)
        /// </summary>
        private void CalculateVelocity()
        {
            _currentVelocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;
        }

        /// <summary>
        /// 속도 기반 자동 틸트
        /// </summary>
        private void UpdateAutoTilt()
        {
            if (!enableVelocityTilt) return;
            
            // 좌우 이동 속도에 따른 자동 틸트
            float velocityTilt = -_currentVelocity.x * velocityTiltMultiplier;
            velocityTilt = Mathf.Clamp(velocityTilt, -maxTiltAngle, maxTiltAngle);
            
            // 수동 틸트와 자동 틸트 결합
            float combinedTilt = _targetSlideAngle + velocityTilt;
            combinedTilt = Mathf.Clamp(combinedTilt, -maxTiltAngle, maxTiltAngle);
            
            _targetSlideAngle = Mathf.Lerp(_targetSlideAngle, combinedTilt, Time.deltaTime * tiltSpeed * 0.5f);
        }

        /// <summary>
        /// 속도 기반 동적 FOV
        /// </summary>
        private void UpdateFOV()
        {
            if (!enableSpeedFOV || _camera == null) return;
            
            float horizontalSpeed = new Vector3(_currentVelocity.x, 0, _currentVelocity.z).magnitude;
            float speedRatio = Mathf.Clamp01(horizontalSpeed / speedForMaxFOV);
            float targetFOV = Mathf.Lerp(baseFOV, maxSpeedFOV, speedRatio);
            
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * fovChangeSpeed);
        }

        /// <summary>
        /// Head Bob 효과 (걷기/달리기 시)
        /// </summary>
        private void ApplyHeadBob()
        {
            float horizontalSpeed = new Vector3(_currentVelocity.x, 0, _currentVelocity.z).magnitude;
            
            if (horizontalSpeed > 0.1f)
            {
                _bobTimer += Time.deltaTime * bobFrequency * Mathf.Clamp(horizontalSpeed / 5f, 0.5f, 2f);
                
                float bobX = Mathf.Sin(_bobTimer) * bobHorizontalAmplitude;
                float bobY = Mathf.Abs(Mathf.Sin(_bobTimer * 2)) * bobVerticalAmplitude;
                
                _bobOffset = new Vector3(bobX, bobY, 0);
            }
            else
            {
                _bobTimer = 0;
            }
        }

        /// <summary>
        /// 월드 공간에서의 틸트 적용 (기존 로직 유지)
        /// </summary>
        private void ApplyWorldTilt()
        {
            if (_target == null) return;

            float yRot = _target.eulerAngles.y;
            float radY = yRot * Mathf.Deg2Rad;

            float sinY = Mathf.Sin(radY);
            float cosY = Mathf.Cos(radY);
            
            float xRot = sinY * slideAngle;
            float zRot = cosY * slideAngle;

            transform.localRotation = Quaternion.Euler(xRot, 0f, zRot);
        }

        /// <summary>
        /// 부드러운 틸트 전환
        /// </summary>
        private void SmoothTilt()
        {
            slideAngle = Mathf.Lerp(slideAngle, _targetSlideAngle, Time.deltaTime * tiltSpeed);
        }

        /// <summary>
        /// 카메라 위치 업데이트 (Head Bob 포함)
        /// </summary>
        public void SetCamTrm()
        {
            Vector3 targetWithBob = _targetPos + _bobOffset;
            transform.position = Vector3.Lerp(transform.position, targetWithBob, Time.deltaTime * positionLerpSpeed);
        }

        /// <summary>
        /// 카메라 목표 위치 설정
        /// </summary>
        public void SetCamPos(Vector3 pos)
        {
            _targetPos = pos;
        }

        /// <summary>
        /// 수동 틸트 각도 설정
        /// </summary>
        public void SetTilt(float targetAngle)
        {
            _targetSlideAngle = Mathf.Clamp(targetAngle, -maxTiltAngle, maxTiltAngle);
        }

        /// <summary>
        /// 틸트 리셋 (중립 상태로)
        /// </summary>
        public void ReturnOwnTilt()
        {
            _targetSlideAngle = 0;
        }

        /// <summary>
        /// 타겟 설정
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// 속도 기반 틸트 활성화/비활성화
        /// </summary>
        public void SetVelocityTiltEnabled(bool enabled)
        {
            enableVelocityTilt = enabled;
        }

        /// <summary>
        /// Head Bob 활성화/비활성화
        /// </summary>
        public void SetHeadBobEnabled(bool enabled)
        {
            enableHeadBob = enabled;
            if (!enabled)
            {
                _bobOffset = Vector3.zero;
                _bobTimer = 0f;
            }
        }

        /// <summary>
        /// 동적 FOV 활성화/비활성화
        /// </summary>
        public void SetSpeedFOVEnabled(bool enabled)
        {
            enableSpeedFOV = enabled;
            if (!enabled && _camera != null)
            {
                _camera.fieldOfView = baseFOV;
            }
        }

        /// <summary>
        /// 기본 FOV 설정
        /// </summary>
        public void SetBaseFOV(float fov)
        {
            baseFOV = fov;
            if (_camera != null && !enableSpeedFOV)
            {
                _camera.fieldOfView = baseFOV;
            }
        }

        /// <summary>
        /// Head Bob 강도 설정
        /// </summary>
        public void SetHeadBobIntensity(float horizontal, float vertical)
        {
            bobHorizontalAmplitude = horizontal;
            bobVerticalAmplitude = vertical;
        }

        /// <summary>
        /// 현재 속도 반환 (외부에서 사용 가능)
        /// </summary>
        public Vector3 GetCurrentVelocity()
        {
            return _currentVelocity;
        }

        /// <summary>
        /// 현재 수평 속도 반환
        /// </summary>
        public float GetHorizontalSpeed()
        {
            return new Vector3(_currentVelocity.x, 0, _currentVelocity.z).magnitude;
        }
    }
}