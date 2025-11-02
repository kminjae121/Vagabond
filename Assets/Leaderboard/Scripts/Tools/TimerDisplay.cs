using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText = null;
    private bool isDisplayActive = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        isDisplayActive = true;
    }

    private void Update()
    {
        if (isDisplayActive && TimerManager.Instance != null)
        {
            float currentTime = TimerManager.Instance.GetElapsedTime();
            timerText.text = TimerManager.FormatTime(currentTime);
        }
    }

    public void SetDisplayActive(bool active)
    {
        isDisplayActive = active;
        gameObject.SetActive(active);
    }

    public bool IsDisplayActive()
    {
        return isDisplayActive;
    }
}