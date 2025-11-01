using Code.Combat;
using Code.Core.Debugs;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using UnityEngine;

public class DecalSpawner : MonoBehaviour
{
    [SerializeField] private PoolItemSO decalItem;

    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float minSize = 0.8f;
    [SerializeField] private float maxSize = 1.5f;

    [Inject] private PoolManagerMono _poolManager;
    
    public async void SpawnDecal()
    {
        if (decalItem == null)
        {
            UnityLogger.LogError("Decal Item이 할당되지 않았습니다.");
            return;
        }
        
        var decal = _poolManager.Pop<BloodDecal>(decalItem);
        decal.transform.position = transform.position;
        
        float randomRotation = Random.Range(0f, 360f);
        decal.transform.Rotate(Vector3.forward, randomRotation);
        
        float randomSize = Random.Range(minSize, maxSize);
        decal.transform.localScale = Vector3.one * randomSize;
        
        decal.transform.SetParent(transform.root.transform);

        await Awaitable.WaitForSecondsAsync(lifetime);
        
        _poolManager.Push(decal);
    }
}