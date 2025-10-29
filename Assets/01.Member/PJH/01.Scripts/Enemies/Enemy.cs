using Code.Core.Debugs;
using Code.Entities;
using GondrLib.ObjectPool.RunTime;
using Unity.Behavior;
using UnityEngine;

namespace Code.Enemies
{
    public abstract class Enemy : Entity, IPoolable
    {
        [field: SerializeField] public EntityFinderSO PlayerFinder { get; private set; }
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }

        public BehaviorGraphAgent BTAgent { get; private set; }
        public GameObject GameObject => gameObject;

        public float detectRange;
        public float attackRange;

        protected Pool _myPool;
        
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
        
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
        }

        public void ResetItem()
        {
        }
    }
}