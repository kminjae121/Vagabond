using _00.CORE._02.Scripts.Input;
using _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon;
using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using Code.Dash;
using Code.Entities;
using GondrLib.Dependencies;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class Player : Entity, IDependencyProvider
    {
        [SerializeField] private StateDataSO[] stateDataList;
        
        private EntityStateMachine _stateMachine;
        [SerializeField] private InputReader inputReader;
        
        [field: SerializeField] public Transform cameraTrm { get; set; }
        [SerializeField] private Transform parentTrm;
        
        #region PlayerComponent
        
        private WallSliding _wallSlidingCompo;
        
        [field:SerializeField] public PlayerCamFirst camCompo { get; set; }

        public CharacterMovement movementCompo { get; private set; }
        
        public PlayerSword swordCompo { get; private set; }
        public BloodFlowerSystem bloodSystemCompo { get; private set; }
        
        public PlayerDashComponent dashComponent { get; private set; }
        
        #endregion
        
        private bool isJumping = true;
        private bool isMovementEnabled = true;

        [Provide]
        public Player GetPlayer() => this;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, stateDataList);
            _wallSlidingCompo = GetCompo<WallSliding>();
            movementCompo = GetCompo<CharacterMovement>();
            swordCompo = GetComponentInChildren<PlayerSword>();
            bloodSystemCompo = GetComponent<BloodFlowerSystem>();
            dashComponent = GetComponent<PlayerDashComponent>();

            inputReader.JumpKeyEvent += HandleJump;
            inputReader.SlidingEvent += HandleWallSliding;
        }

        private void HandleWallSliding(bool isSliding)
        {
            if (!isJumping) return;
            
            string slidingDirection = _wallSlidingCompo.CanSlidingWall();
            
            if (slidingDirection != "None")
            {
                if (isSliding && slidingDirection == "Left" && !_wallSlidingCompo._isWallSliding)
                {
                    movementCompo._jumpCnt = 0;
                    ChangeState("LEFTSLIDING");
                }
                else if (isSliding && slidingDirection == "Right" && !_wallSlidingCompo._isWallSliding)
                {
                    movementCompo._jumpCnt = 0;
                    ChangeState("RIGHTSLIDING");
                }
                else if (!isSliding)
                {
                    ChangeState("JUMP");
                }
            }
        }

        public void SetJumping(bool isJump)
        {
            isJumping = isJump;
        }
        
        public void SetMovementEnabled(bool enabled)
        {
            isMovementEnabled = enabled;
            
            if (!enabled)
            {
                movementCompo.SetMove(0, 0);
            }
        }

        private void Start()
        {
            const string idle = "IDLE";
            _stateMachine.ChangeState(idle);
        }

        private void HandleJump()
        {
            if (!isJumping) return;
            
            if (_wallSlidingCompo.CanSlidingWall() == "None")
            {
                movementCompo.Jump();
            }
        }
        
        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdateMachine();
        }

        private void LateUpdate()
        {
            // 카메라 방향에 맞춰 플레이어 회전
            Vector3 angles = transform.localEulerAngles;
            angles.y = cameraTrm.localEulerAngles.y;    
            transform.localEulerAngles = angles;
        }

        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);

        private void OnDestroy()
        {
            if (inputReader != null)
            {
                inputReader.JumpKeyEvent -= HandleJump;
                inputReader.SlidingEvent -= HandleWallSliding;
            }
        }
    }
}