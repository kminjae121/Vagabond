using GondrLib.ObjectPool.RunTime;
using UnityEngine;

namespace Code.Enemies
{
    public class EnemyHeavy : CommonEnemy
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private PoolItemSO shockwaveItem;

        public void SpawnShockWave()
        {
            
        }
    }
}