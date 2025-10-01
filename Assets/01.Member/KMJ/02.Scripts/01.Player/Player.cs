using System;
using _00.CORE._02.Scripts.Input;
using _01.Member.KMJ._00.Core._01.Entity._01.EntityState;
using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using _01.Member.KMJ._02.Scripts._01.Player.State;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class Player : Entity
    {
        [SerializeField] private StateDataSO[] stateDataList;
        
        private EntityStateMachine _stateMachine;
        [SerializeField] private InputReader _inputReader;

        private WallSliding _wallSlidingCompo;
        
        public PlayerCamFirst _camCompo { get; set; }

        private CharacterMovement _movementCompo;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, stateDataList);
            _wallSlidingCompo = GetComponent<WallSliding>();
            _camCompo = GetComponentInChildren<PlayerCamFirst>();
            _movementCompo = GetComponent<CharacterMovement>();  

            _inputReader.JumpKeyEvent += HandleJump;
            _inputReader.SlidingEvent += HandleWallSliding;
        }

        private void HandleWallSliding(bool isSliding)
        {
            if (_wallSlidingCompo.CanSlidingWall() != "None")
            {
                if (isSliding && _wallSlidingCompo.CanSlidingWall() == "Left" && !_wallSlidingCompo._isWallSliding)
                {
                    _movementCompo._jumpCnt = 0;
                    ChangeState("LEFTSLIDING");
                }
                else if(isSliding && _wallSlidingCompo.CanSlidingWall() == "Right" && !_wallSlidingCompo._isWallSliding)
                {
                    _movementCompo._jumpCnt = 0;
                    ChangeState("RIGHTSLIDING");
                }
                else if (!isSliding)
                {
                    ChangeState("JUMP");
                }   
            }
        }


        private void Start()
        {
            const string idle = "IDLE";
            _stateMachine.ChangeState(idle);
        }

        private void HandleJump()
        {
            if (_wallSlidingCompo.CanSlidingWall() == "None")
            {
                ChangeState("JUMP");
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

        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);

        
    }
}