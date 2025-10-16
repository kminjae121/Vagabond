using Code.Core.Debugs;
using Code.Entities.Combat;
using GondrLib.ObjectPool.RunTime;
using UnityEngine;

namespace Code.Enemies
{
    public class EnemyArcher : CommonEnemy
    {
        [SerializeField] private Transform firePos;
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO arrowPrefab;
        [SerializeField] private float arrowSpeed = 20f;
        [SerializeField] private float arrowDamage = 10f;
        
        public void ShootArrow(Vector3 dir)
        {
            var arrow = poolManager.Pop(arrowPrefab) as Arrow;
            
            arrow.transform.position = firePos.transform.position;
            arrow.transform.rotation = Quaternion.LookRotation(dir);

            arrow.Initialize(dir, arrowSpeed, arrowDamage, this);
            
            UnityLogger.Log("arrow shoot");
        }
    }
}