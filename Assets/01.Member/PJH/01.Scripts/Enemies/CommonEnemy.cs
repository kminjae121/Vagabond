using Code.Core.GameEvent;
using UnityEngine;

namespace Code.Enemies
{
    public class CommonEnemy : Enemy
    {
        [field: SerializeField] public GameEventChannelSO PlayerChannel { get; private set; }
        [field: SerializeField] public bool IsBattleState { get; set; }

        private StateChangeEvent _stateChangeChannel;
        
        protected override void Awake()
        {
            base.Awake();
            
            
        }

        protected override void Start()
        {
            base.Start();

            _stateChangeChannel = GetBlackboardVariable<StateChangeEvent>("StateChannel").Value;
            OnDeathEvent.AddListener(HandleDeadEvent);
        }

        private void OnDestroy()
        {
            OnDeathEvent.RemoveListener(HandleDeadEvent);
        }

        private void HandleDeadEvent()
        {
            if (IsDead)
                return;

            IsDead = true;
            _stateChangeChannel.SendEventMessage(EnemyState.DEAD);
        }

        public void SetBattleState()
        {
            if (IsBattleState || IsDead)
                return;

            IsBattleState = true;
            
            var stateVariable = GetBlackboardVariable<EnemyState>("CurrentState");
            
            if (stateVariable != null && stateVariable.Value != EnemyState.HIT)
                _stateChangeChannel.SendEventMessage(EnemyState.CHASE);
        }
    }
}