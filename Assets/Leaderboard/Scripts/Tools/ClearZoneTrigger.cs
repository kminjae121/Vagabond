using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearZoneTrigger : MonoBehaviour
{
    public delegate void ClearZoneDelegate();
    public event ClearZoneDelegate OnClearZoneEntered;

    private void OnTriggerEnter(Collider collision)
    {
        // 플레이어 태그가 "Player"라고 가정
        if (collision.CompareTag("Player"))
        {
            OnClearZoneEntered?.Invoke();
        }
    }
}