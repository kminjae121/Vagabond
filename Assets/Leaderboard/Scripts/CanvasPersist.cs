using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPersist : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}