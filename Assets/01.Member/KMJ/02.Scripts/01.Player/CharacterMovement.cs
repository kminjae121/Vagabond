using _00.CORE._02.Scripts.Input;
using Code.Core.Debugs;
using Code.Core.Stats;
using Code.Interfaces;
using UnityEngine;

namespace Code.Entities
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
        [field: SerializeField] public float moveModifierSpeed { get; set; }
        
        [field : SerializeField] public InputReader _inputReader { get; private set; }
        public Vector3 _move;

        public int _jumpCnt { get; set; }
        
        public float moveSpeed { get;  set; }= 8f;
        public float baseSpeed { get; private set; }= 8f;

        public float maxmoveSpeed { get; set; } = 15f;
        public float targetSpeed { get; private set; }= 0;
        
        public float jumpSpeed { get; private set; }

        public int maxJumpCnt { get; set; } = 2;
        
        private Entity _entity;
        private EntityStatCompo _statCompo;
        
        private Rigidbody _rbCompo;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStatCompo>();
            _rbCompo = entity.GetComponent<Rigidbody>();
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
                
                Vector3 velocity = _rbCompo.linearVelocity;
                velocity.y = 0;
                _rbCompo.linearVelocity = velocity;
    
                _rbCompo.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                
                _jumpCnt++;
            }
            else if (_jumpCnt < maxJumpCnt)
            {
                Vector3 velocity = _rbCompo.linearVelocity;
                velocity.y = 0;
                _rbCompo.linearVelocity = velocity;
    
                _rbCompo.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                
                _jumpCnt++;
            }
        }
        
        public void AfterInitialize()
        {
            moveSpeed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 4f);
            
            jumpSpeed = _statCompo.SubscribeStat(jumpSpeedStat, HandleJumpPowerChange, 3f);
            
            maxmoveSpeed = _statCompo.SubscribeStat(maxMoveSpeedStat, HandleMaxMoveSpeedChange, 3f);

            baseSpeed = moveSpeed;
        }

        private void OnDestroy()
        {
            _statCompo.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
        }

        private void HandleMoveSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            moveSpeed = currentvalue;
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

            //if (moveSpeed >= maxmoveSpeed)
            //{
            //    moveSpeed = maxmoveSpeed;
            //}
        }
        
        public void SetSpeed(float targetSpeedValue)
        {
            targetSpeed = targetSpeedValue;
        }

        public void SetReturnOriginMoveSpeed()
        {
            targetSpeed = baseSpeed;
        }

        public void SetSpeedZero()
        {
            targetSpeed = 0;
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