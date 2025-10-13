using Code.Core.Debugs;
using Code.Entities;
using Code.Entities.Combat;
using Code.Interfaces;
using GondrLib.ObjectPool.RunTime;
using UnityEngine;

namespace Code.Enemies
{
    public class ArcherAttackCompo : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Transform firePos;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO arrowPrefab;
        [SerializeField] private float arrowSpeed = 20f;
        [SerializeField] private float arrowDamage = 10f;
        
        private Entity _entity;
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
        }

        public void ShootArrow(Vector3 dir)
        {
            var arrow = poolManager.Pop(arrowPrefab) as Arrow;
            
            arrow.transform.position = firePos.transform.position;
            arrow.transform.rotation = Quaternion.LookRotation(dir);

            arrow.Initialize(dir, arrowSpeed, arrowDamage, _entity);
            
            UnityLogger.Log("arrow shoot");
        }
    }
}