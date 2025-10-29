using Code.Core._02.Sound;
using Code.Core.GameEvent;
using Code.Entities;
using Code.Interfaces;
using GameEvents;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo
{
    public class WallSliding : MonoBehaviour, IEntityComponent
    {
        [Header("Wall Detection")]
        [SerializeField] private LayerMask _whatIsWall;
        [SerializeField] private Transform leftPos;
        [SerializeField] private Transform rightPos;
        [SerializeField] private Vector3 checkSize;
        [SerializeField] private float rayDistance = 1f;

        public bool _isWallSliding { get; set; }

        private CharacterMovement _movementCompo;
        private Player _player;
        [SerializeField] private SoundSO slideSound;
        [SerializeField] private GameEventChannelSO _soundChannel;
        private bool isSlidingKeyHeld = false;
        private Vector3 currentWallNormal = Vector3.zero;
        private string currentWallSide = "None";

        public void Initialize(Entity entity)
        {
            _movementCompo = entity.GetCompo<CharacterMovement>();
            _player = entity as Player;
        }

        private void Start()
        {
            if (_player != null && _player.inputReader != null)
            {
                _player.inputReader.SlidingEvent += OnSlidingInput;
            }
        }

        private void OnDisable()
        {
            if (_player != null && _player.inputReader != null)
            {
                _player.inputReader.SlidingEvent -= OnSlidingInput;
            }
        }

        private void Update()
        {
            UpdateWallSlideState();
        }

        private void OnSlidingInput(bool isHeld)
        {
            isSlidingKeyHeld = isHeld;
        }

        private void UpdateWallSlideState()
        {
            if (_movementCompo == null) return;

            string wallSide = CanSlidingWall();

            if (isSlidingKeyHeld && wallSide != "None" && !_movementCompo.CheckGroundDetected())
            {
                if (!_isWallSliding)
                {
                    StartWallSlide();
                }
            }
            else
            {
                if (_isWallSliding)
                {
                    EndWallSlide();
                }
            }
        }

        public string CanSlidingWall()
        {
            if (CheckWall(leftPos.position, out Vector3 leftNormal))
            {
                currentWallNormal = leftNormal;
                currentWallSide = "Left";
                return "Left";
            }

            if (CheckWall(rightPos.position, out Vector3 rightNormal))
            {
                currentWallNormal = rightNormal;
                currentWallSide = "Right";
                return "Right";
            }

            currentWallSide = "None";
            return "None";
        }

        public void PlaySlideSound()
        {
            var sfxEvt = SoundEvents.PlayLongSFXEvent.Initializer(transform.position, slideSound, 1);
            _soundChannel.RaiseEvent(sfxEvt);
        }

        public void StopSlideSound()
        {
            _soundChannel.RaiseEvent(SoundEvents.StopLongSFXEvent.Initializer(1));
        }

        public string GetWallSide()
        {
            return DetermineWallSideByPlayerView();
        }
        
        /// <summary>
        /// 플레이어의 시야(Forward) 벡터를 기준으로 벽이 오른쪽인지 왼쪽인지 판단
        /// 벽이 플레이어의 오른쪽에 있으면 "Right", 왼쪽에 있으면 "Left"
        /// </summary>
        private string DetermineWallSideByPlayerView()
        {
            if (_player == null) return "None";
            
            Vector3 playerRight = _player.transform.right;
            
            // 현재 벽의 위치 (왼쪽 또는 오른쪽 감지 위치의 중점)
            Vector3 wallPosition = (leftPos.position + rightPos.position) / 2f;
            Vector3 playerToWall = (wallPosition - _player.transform.position).normalized;
            
            // 플레이어의 오른쪽 벡터와의 내적 계산
            float dotRight = Vector3.Dot(playerToWall, playerRight);
            
            // 내적 값이 양수면 벽이 오른쪽, 음수면 왼쪽
            if (dotRight > 0)
            {
                return "Right";
            }
            else
            {
                return "Left";
            }
        }
        
        public Vector3 GetWallNormal()
        {
            return currentWallNormal;
        }

        private bool CheckWall(Vector3 position, out Vector3 normal)
        {
            normal = Vector3.zero;

            Collider[] hits = Physics.OverlapBox(position, checkSize, Quaternion.identity, _whatIsWall);

            if (hits.Length > 0)
            {
                Vector3 directionToWall = (hits[0].transform.position - transform.position).normalized;
                directionToWall.y = 0;

                if (Physics.Raycast(position, directionToWall, out RaycastHit hit, rayDistance, _whatIsWall))
                {
                    normal = hit.normal;
                    return true;
                }

                normal = -directionToWall;
                return true;
            }

            return false;
        }

        public void StartWallSlide()
        {
            _isWallSliding = true;
            _movementCompo.StartWallSlide(currentWallNormal);
            _player.ChangeState("WALLSLIDE");
        }

        public void EndWallSlide()
        {
            _isWallSliding = false;
            _movementCompo.StopWallSlide();
        }

        private void OnDrawGizmos()
        {
            if (leftPos == null || rightPos == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(leftPos.position, checkSize);
            Gizmos.DrawWireCube(rightPos.position, checkSize);

            if (_isWallSliding && currentWallNormal != Vector3.zero)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(transform.position, currentWallNormal * 2f);
            }
        }
    }
}