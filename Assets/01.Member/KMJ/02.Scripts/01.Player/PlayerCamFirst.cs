using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class PlayerCamFirst : MonoBehaviour
    {
        [SerializeField] private float _sensX;
        [SerializeField] private float _sensY;

        [Header("Tilt Settings")]
        [SerializeField] private float tiltSpeed = 5f;  
        [SerializeField] private float camMoveSpeed = 5f;  

        public Transform orientation;

        private float _xRotation;
        private float _yRotation;

        public float slideAngle { get; private set; }     
        
        private float _targetSlideAngle = 0f;
        [SerializeField] private Vector3 _targetPos;

        private void Start()
        {
            _targetPos = transform.parent.position;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            CamSetting();
            SmoothTilt();
            //SetCamTrm();
        }

        private void CamSetting()
        {
            float MouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _sensX; 
            float MouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _sensY;

            _yRotation += MouseX;
            _xRotation -= MouseY;

            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, slideAngle);
            orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
        }

        public void SetCamTrm()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * camMoveSpeed);
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
    }
}