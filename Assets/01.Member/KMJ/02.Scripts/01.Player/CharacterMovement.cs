using System;
using _00.CORE._02.Scripts;
using _00.CORE._02.Scripts.Input;
using _01.Member.KMJ._00.Core._01.Entity._05.Interface;
using UnityEngine;
using UnityEngine.Serialization;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class CharacterMovement : MonoBehaviour, IEntityComponent
    {
        [Header("Setting")]
        [SerializeField] private StatSO moveSpeedStat;
        [SerializeField] private StatSO jumpSpeedStat;
        [SerializeField] private StatSO maxMoveSpeedStat;
        [SerializeField] private float jumpRaySize;
        [SerializeField] private LayerMask whatIsGround;
        
        [Header("ModifierValue")]
        [SerializeField] private float moveModifierSpeed;
        
        [field : SerializeField] public InputReader _inputReader { get; private set; }
        public Vector3 _move;

        public int _jumpCnt { get; set; }
        
        public float moveSpeed { get; private set; }= 8f;
        public float baseSpeed { get; private set; }= 8f;
        
        public float maxmoveSpeed { get; private set; }= 10f;
        public float targetSpeed { get; private set; }= 8f;
        
        public float jumpSpeed { get; private set; }

        
        private Entity _entity;
        private EntityStatCompo _statCompo;
        
        private Rigidbody _rbCompo;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStatCompo>();
            _rbCompo = GetComponent<Rigidbody>();
            AfterInitialize();
        }
        


        public void SetMove(float XMove, float ZMove)
        {
            _move.x = XMove;
            _move.z = ZMove;
        }

        public bool CheckGroundDetected()
        {
            bool hit = Physics.Raycast(transform.position, Vector3.down, jumpRaySize, whatIsGround);
            
            return hit;
        }
        
        public void Jump()
        {
            if (CheckGroundDetected() == true)
            {
                _jumpCnt = 0;
                _rbCompo.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                _jumpCnt++;
            }
            else if (_jumpCnt < 2)
            {
                _rbCompo.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                _jumpCnt++;
            }
        }
        
        public void AfterInitialize()
        {
            moveSpeed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 4f);
            
            baseSpeed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 4f);
            
            jumpSpeed = _statCompo.SubscribeStat(jumpSpeedStat, HandleJumpPowerChange, 3f);
            
            maxmoveSpeed = _statCompo.SubscribeStat(maxMoveSpeedStat, HandleMaxMoveSpeedChange, 3f);
        }

        private void OnDestroy()
        {
            _statCompo.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
        }

        private void HandleMoveSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            moveSpeed = currentvalue;
            baseSpeed = currentvalue;
        }
        
        
        private void HandleJumpPowerChange(StatSO stat, float currentvalue, float previousvalue)
        {
            jumpSpeed = currentvalue;
        } 
        private void HandleMaxMoveSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            maxmoveSpeed = currentvalue;
        }

        public void StopMoving()
        {
            _rbCompo.linearVelocity = Vector3.zero;
        }
        
        private void SmoothMoveSpeed()
        {
            moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * moveModifierSpeed);
        }
        
        public void SetSpeed(float targetSpeedValue)
        {
            targetSpeed = targetSpeedValue;
        }

        public void SetReturnOriginMoveSpeed()
        {
            moveSpeed = baseSpeed;
        }

        private void Update()
        {
            SmoothMoveSpeed();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.down);
            Gizmos.color = Color.white;
        }
        
    }
}