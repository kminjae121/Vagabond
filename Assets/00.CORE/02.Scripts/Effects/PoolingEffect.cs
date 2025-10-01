using Code.Core.Debug;
using GondrLib.ObjectPool.RunTime;
using UnityEngine;

namespace Code.Core.Effects
{
    public class PoolingEffect : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        
        [SerializeField] private GameObject effectObject;
        
        public GameObject GameObject => gameObject;

        private Pool _myPool;
        private IPlayableVFX _playableVFX;
        
        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
            _playableVFX = effectObject.GetComponent<IPlayableVFX>(); 
        }

        public void ResetItem()
        {
            _playableVFX.StopVFX();
        }

        public void PlayVFX(Vector3 position, Quaternion rotation)
        {
            _playableVFX.PlayVFX(position, rotation);
        }
        
        private void OnValidate()
        {
            if (effectObject == null)
                return;
            
            _playableVFX = effectObject.GetComponent<IPlayableVFX>();
            
            if (_playableVFX == null)
            {
                UnityLogger.LogWarning($"Input object {effectObject.name} does not have {nameof(IPlayableVFX)} component.");
                effectObject = null;
            }
        }
    }
}