using System;
using _00.CORE._02.Scripts.Input;
using _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon;
using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Core.Debugs;
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
        [field : SerializeField] public InputReader inputReader { get; private set; }
        [field: SerializeField] public Transform cameraTrm { get; set; }
        [SerializeField] private Transform parentTrm;
        
        #region PlayerComponent
        
        private WallSliding _wallSlidingCompo;
        
        [field:SerializeField] public PlayerCamFirst camCompo { get; set; }

        public CharacterMovement movementCompo { get; private set; }
        
        public PlayerSword swordCompo { get; private set; }
        public BloodFlowerSystem bloodSystemCompo { get; private set; }
        
        public PlayerDashComponent dashComponent { get; private set; }
        
        public GroundSliding _groundSlideCompo { get; private set; }
        
        #endregion
        
        private bool isJumping = true;

        public bool isSliding = false;

        [Provide]
        public Player GetPlayer() => this;
        
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, stateDataList);
            _groundSlideCompo = GetCompo<GroundSliding>();
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
            if (isJumping)
            {
                if (_wallSlidingCompo.CanSlidingWall() != "None")
                {
                    if (isSliding && _wallSlidingCompo.CanSlidingWall() == "Left" && !_wallSlidingCompo._isWallSliding)
                    {
                        movementCompo._jumpCnt = 0;
                        ChangeState("LEFTSLIDING");
                    }
                    else if(isSliding && _wallSlidingCompo.CanSlidingWall() == "Right" && !_wallSlidingCompo._isWallSliding)
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
        }

        public void SetJumping(bool isJump)
        {
            isJumping = isJump;
        }

        private void Start()
        {
            const string idle = "IDLE";
            _stateMachine.ChangeState(idle);
        }

        private void HandleJump()
        {
            if (isJumping)
            {
                if (_wallSlidingCompo.CanSlidingWall() == "None")
                {
                    ChangeState("JUMP");
                }   
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
            Vector3 angles = transform.localEulerAngles;
            angles.y = cameraTrm.localEulerAngles.y;    
            transform.localEulerAngles = angles;
        }

        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);

        
    }
}