using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private static Canvas mainUICanvas = null;

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

    private void Awake()
    {
        if (mainUICanvas == null)
        {
            mainUICanvas = GetComponent<Canvas>();
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