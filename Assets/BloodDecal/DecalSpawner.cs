using UnityEngine;

public class DecalSpawner : MonoBehaviour
{
    public GameObject decalPrefab;

    public float lifetime = 10f;
    
    public float minSize = 0.8f;
    public float maxSize = 1.5f;
    
    public void SpawnDecal(RaycastHit hitInfo)
    {
        if (decalPrefab == null)
        {
            Debug.LogError("Decal Prefab이 할당되지 않았습니다!");
            return;
        }
        
        GameObject decal = Instantiate(decalPrefab, hitInfo.point, Quaternion.LookRotation(-hitInfo.normal));
        
        float randomRotation = Random.Range(0f, 360f);
        decal.transform.Rotate(Vector3.forward, randomRotation);
        
        float randomSize = Random.Range(minSize, maxSize);
        decal.transform.localScale = Vector3.one * randomSize;
        
        decal.transform.SetParent(hitInfo.collider.transform);

        Destroy(decal, lifetime);
    }
}