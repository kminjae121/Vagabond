using Code.Core.GameEvent;
using UnityEngine;

namespace Code.Enemies
{
    public class CommonEnemy : Enemy
    {
        [field: SerializeField] public GameEventChannelSO PlayerChannel { get; private set; }
        [field: SerializeField] public bool IsBattleState { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            
        }

        protected override void Start()
        {
            base.Start();
            
            
        }

        private void OnDestroy()
        {
            
        }

        private void HandleDeadEvent()
        {
            if (IsDead)
                return;

            IsDead = true;
        }

        public void SetBattleState()
        {
            if (IsBattleState || IsDead)
                return;

            IsBattleState = true;
            
            var stateVariable = GetBlackboardVariable<EnemyState>("CurrentState");
            
            //if (stateVariable != null && stateVariable.Value != EnemyState.HIT)
                
        }
    }
}