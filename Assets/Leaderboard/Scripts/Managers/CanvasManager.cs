using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private static Canvas mainUICanvas = null;

    private void Awake()
    {
        // Canvas는 부모 또는 자신
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
        
        if (canvas != null)
        {
            mainUICanvas = canvas;
            Debug.Log("CanvasManager 초기화 - Canvas: " + canvas.gameObject.name);
        }
    }

    public static Canvas MainUICanvas
    {
        get
        {
            if (mainUICanvas == null)
            {
                mainUICanvas = FindFirstObjectByType<Canvas>();
            }
            return mainUICanvas;
        }
    }

    public static void ShowUI()
    {
        if (MainUICanvas != null)
        {
            MainUICanvas.gameObject.SetActive(true);
            Debug.Log("UI 표시됨");
        }
    }

    public static void HideUI()
    {
        if (MainUICanvas != null)
        {
            MainUICanvas.gameObject.SetActive(false);
            Debug.Log("UI 숨김");
        }
    }
}