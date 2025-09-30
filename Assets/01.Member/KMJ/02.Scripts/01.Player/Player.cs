using _01.Member.KMJ._00.Core._01.Entity._01.EntityState;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts._01.Player
{
    public class Player : Entity
    {
        
        [SerializeField] private StateDataSO[] stateDataList;
        
        private EntityStatemachine _stateMachine;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new EntityStatemachine(this, stateDataList);
        }
        
        private void Start()
        {
            const string idle = "IDLE";
            _stateMachine.ChangeState(idle);
        }
        
        private void Update()
        {
            _stateMachine.UpdateStateMachine();
        }
        
        public void ChangeState(string newStateName, bool force = false) 
            => _stateMachine.ChangeState(newStateName, force);

        protected override void InitializeComponents()
        {
            base.InitializeComponents();
        }
    }
}