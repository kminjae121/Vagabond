using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPersist : MonoBehaviour
{
    private void Awake()
    {
        // 루트 오브젝트가 아니면 루트로 변경
        if (transform.parent != null)
        {
            transform.SetParent(null);
            Debug.Log(gameObject.name + " 루트로 변경됨");
        }
        
        DontDestroyOnLoad(gameObject);
        Debug.Log(gameObject.name + " DontDestroyOnLoad 설정됨");
    }
}