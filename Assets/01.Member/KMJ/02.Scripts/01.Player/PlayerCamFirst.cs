using System;
using Unity.Cinemachine;
using UnityEngine;
namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class PlayerCamFirst : MonoBehaviour
    {
        [Header("Tilt Settings")]
        [SerializeField] private float tiltSpeed = 5f;
        public float slideAngle { get; private set; }
        private float _targetSlideAngle = 0f;
        [SerializeField] private Vector3 _targetPos;

        [SerializeField] private Transform _target;
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            _targetPos = transform.position;
        }
        private void Update()
        {
            ApplyWorldTilt();
            SmoothTilt();
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

            transform.localRotation = Quaternion.Euler(xRot, 0f, zRot);
        }

        private void LateUpdate()
        {
        }

        public void SetCamTrm()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * 10);
        }

        private void SmoothTilt()
        {
            slideAngle = Mathf.Lerp(slideAngle, _targetSlideAngle, Time.deltaTime * tiltSpeed);
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
    }
}