using System; 
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
            transform.rotation = Quaternion.Euler(0, 0, slideAngle);
            SmoothTilt();
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
    } 
}