using System;
using _00.CORE._02.Scripts.Input;
using _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon;
using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using _01.Member.KMJ._02.Scripts._01.Player.State;
using Code.Entities;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class Player : Entity
    {
        [SerializeField] private StateDataSO[] stateDataList;
        
        private EntityStateMachine _stateMachine;
        [SerializeField] private InputReader inputReader;

        private WallSliding _wallSlidingCompo;
        
        public PlayerCamFirst camCompo { get; set; }

        public CharacterMovement movementCompo { get; private set; }
        
        public PlayerSword swordCompo { get; private set; }
        
        private bool isJumping = true;
        
        public BloodFlowerSystem bloodSystemCompo { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStateMachine(this, stateDataList);
            _wallSlidingCompo = GetCompo<WallSliding>();
            camCompo = GetComponentInChildren<PlayerCamFirst>();
            movementCompo = GetCompo<CharacterMovement>();
            swordCompo = GetComponentInChildren<PlayerSword>();
            bloodSystemCompo = GetComponent<BloodFlowerSystem>();

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

        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);

        
    }
}