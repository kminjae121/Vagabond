using System;
using _00.CORE._02.Scripts.Input;
using _01.Member.KMJ._00.Core._01.Entity._01.EntityState;
using _01.Member.KMJ._02.Scripts._01.Player.AttackCompo;
using _01.Member.KMJ._02.Scripts._01.Player.PlayerWeapon;
using _01.Member.KMJ._02.Scripts._01.Player.SlidingCompo;
using _01.Member.KMJ._02.Scripts._01.Player.State;
using _01.Member.KMJ._02.Scripts._02.System._01.BloodFlower;
using Code.Core.Debugs;
using Code.Entities;
using GondrLib.Dependencies;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class Player : Entity, IDependencyProvider
    {
        [Header("State Settings")]
        [SerializeField] private StateDataSO[] stateDataList;
        
        [Header("Input & Camera")]
        [field: SerializeField] public InputReader inputReader { get; private set; }
        [field: SerializeField] public Transform cameraTrm { get; set; }
        [field: SerializeField] public PlayerCamFirst camCompo { get; set; }
        
        [Header("Transform")]
        [SerializeField] private Transform parentTrm;
        
        [Header("Debug")]
        [SerializeField] private bool showJumpDebug = false;
        
        #region PlayerComponent

        public WallClimbingCompo climbingComponent { get; private set; }
        public PlayerAttack atkComponent { get; private set; }
        public PlayerAutoAiming aimmingComponent { get; private set; }
        
        private WallSliding _wallSlidingCompo;
        public GroundSliding _groundSlideCompo { get; private set; }
        
        public CharacterMovement movementCompo { get; private set; }
        
        public PlayerSword swordCompo { get; private set; }
        public BloodFlowerSystem bloodSystemCompo { get; private set; }
        
        
        #endregion
        
        private EntityStateMachine _stateMachine;
        private bool isJumping = true;
        public bool isSliding = false;

        [Provide]
        public Player GetPlayer() => this;
        
        protected override void Awake()
        {
            base.Awake();
            
            _stateMachine = new EntityStateMachine(this, stateDataList);

            InitializeComponents();
            ValidateComponents();
            SubscribeInputEvents();
        }

        private void InitializeComponents()
        {
            climbingComponent = GetCompo<WallClimbingCompo>();
            atkComponent = GetCompo<PlayerAttack>();
            aimmingComponent = GetComponent<PlayerAutoAiming>();
            _groundSlideCompo = GetCompo<GroundSliding>();
            _wallSlidingCompo = GetCompo<WallSliding>();
            movementCompo = GetCompo<CharacterMovement>();
            swordCompo = GetComponentInChildren<PlayerSword>();
            bloodSystemCompo = GetComponent<BloodFlowerSystem>();
        }

        private void ValidateComponents()
        {
            if (inputReader == null)
            {
                Debug.LogError($"[Player] InputReader가 할당되지 않았습니다. {gameObject.name}");
            }
            
            if (aimmingComponent == null)
            {
                Debug.LogWarning($"[Player] PlayerAutoAiming 컴포넌트를 찾을 수 없습니다. {gameObject.name}");
            }
            
            if (movementCompo == null)
            {
                Debug.LogError($"[Player] CharacterMovement 컴포넌트를 찾을 수 없습니다. {gameObject.name}");
            }
            
            if (atkComponent == null)
            {
                Debug.LogWarning($"[Player] PlayerAttack 컴포넌트를 찾을 수 없습니다. {gameObject.name}");
            }
        }

        private void SubscribeInputEvents()
        {
            if (inputReader != null)
            {
                inputReader.JumpKeyEvent += HandleJump;
                
                if (showJumpDebug)
                {
                    Debug.Log("[Player] InputReader JumpKeyEvent 구독 완료");
                }
            }
            else
            {
                Debug.LogError("[Player] InputReader가 null이어서 점프 이벤트를 구독할 수 없습니다.");
            }
        }

        private void OnDestroy()
        {
            UnsubscribeInputEvents();
        }

        private void UnsubscribeInputEvents()
        {
            if (inputReader != null)
            {
                inputReader.JumpKeyEvent -= HandleJump;
            }
        }

        private void Start()
        {
            _stateMachine.ChangeState("IDLE");
        }

        private void HandleJump()
        {
            if (showJumpDebug)
            {
                Debug.Log($"[Player] HandleJump 호출됨 - isJumping: {isJumping}, movementCompo: {movementCompo != null}");
            }
            
            if (!isJumping)
            {
                if (showJumpDebug)
                {
                    Debug.Log("[Player] 점프 불가: isJumping이 false");
                }
                return;
            }

            if (climbingComponent != null && climbingComponent.CanClimbWall())
            {
                if (showJumpDebug)
                {
                    Debug.Log("[Player] 벽 오르기 실행");
                }
                climbingComponent.ClimingWall();
            }
            else if (movementCompo != null)
            {
                if (showJumpDebug)
                {
                    Debug.Log("[Player] movementCompo.RequestJump() 호출");
                }
                movementCompo.RequestJump();
            }
            else
            {
                Debug.LogError("[Player] movementCompo가 null입니다!");
            }
        }

        public void SetJumping(bool isJump)
        {
            isJumping = isJump;
            
            if (showJumpDebug)
            {
                Debug.Log($"[Player] SetJumping: {isJump}");
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
            UpdatePlayerRotation();
        }

        private void UpdatePlayerRotation()
        {
            if (cameraTrm == null) return;

            Vector3 angles = transform.localEulerAngles;
            angles.y = cameraTrm.localEulerAngles.y;
            transform.localEulerAngles = angles;
        }

        public void ChangeState(string newStateName, bool force = false)
        {
            _stateMachine?.ChangeState(newStateName, force);
        }
    }
}