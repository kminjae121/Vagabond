
using Code.Entities;
using Code.Interfaces;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo
{
    public class GroundSliding : MonoBehaviour, IEntityComponent
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform _eyeTrm;
        [SerializeField] private Transform _slideTrm;
        [SerializeField] private Transform _ownTrm;
        [SerializeField] private float camMoveSpeed;

        [Header("Collider Settings")]
        [SerializeField] public CapsuleCollider playerCollider;

        private Player _player;
        private bool isSlideKeyHeld = false;
        
        public void Initialize(Entity entity)
        {
            _player = entity as Player;
        }

        private void Start()
        {
            if (_player != null && _player.inputReader != null)
            {
                _player.inputReader.SlideEvent += OnSlideInput;
            }
        }

        private void OnDisable()
        {
            if (_player != null && _player.inputReader != null)
            {
                _player.inputReader.SlideEvent -= OnSlideInput;
            }
        }

        private void Update()
        {
            UpdateCameraPosition();
            UpdateSlideState();
        }

        private void OnSlideInput()
        {
            isSlideKeyHeld = !isSlideKeyHeld;
        }

        private void UpdateSlideState()
        {
            if (_player == null || _player.movementCompo == null) return;

            if (isSlideKeyHeld && _player.movementCompo.CheckGroundDetected())
            {
                if (!_player.isSliding)
                {
                    StartSlide();
                }
            }
            else
            {
                if (_player.isSliding)
                {
                    EndSlide();
                }
            }
        }

        private void StartSlide()
        {
            _player.isSliding = true;
            _player.movementCompo.StartGroundSlide();
            _player.ChangeState("SLIDE");
        }

        private void EndSlide()
        {
            _player.isSliding = false;
            _player.movementCompo.StopGroundSlide();
        }

        private void UpdateCameraPosition()
        {
            if (_eyeTrm == null || _slideTrm == null || _ownTrm == null) return;

            Vector3 targetPos = _player.isSliding ? _slideTrm.position : _ownTrm.position;
            _eyeTrm.position = Vector3.Lerp(_eyeTrm.position, targetPos, Time.deltaTime * camMoveSpeed);
        }
    }
}