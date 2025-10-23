using Code.Core.Debugs;
using Code.Entities;
using Unity.Behavior;
using UnityEngine;

namespace Code.Enemies
{
    public abstract class Enemy : Entity
    {
        [field: SerializeField] public EntityFinderSO PlayerFinder { get; private set; }

        public BehaviorGraphAgent BTAgent { get; private set; }

        public float detectRange;
        public float attackRange;

        protected override void AddComponents()
        {
            base.AddComponents();

            BTAgent = GetComponent<BehaviorGraphAgent>();
            Debug.Assert(BTAgent != null,
                $"{gameObject.name}이 behavior graph agent를 가지고 있지 않습니다.");
        }

        protected virtual void Start()
        {
            var target = GetBlackboardVariable<Transform>("Target");

            if (target == null)
            {
                UnityLogger.LogError($"{gameObject.name}의 타겟이 존재하지 않습니다.");
                return;
            }
            
            target.Value = PlayerFinder.Target.transform;
        }

        public BlackboardVariable<T> GetBlackboardVariable<T>(string key)
            => BTAgent.GetVariable(key, out BlackboardVariable<T> result) ? result : null;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}