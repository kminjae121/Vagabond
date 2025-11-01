using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText = null;

    private void Update()
    {
        if (timerText != null)
        {
            float currentTime = TimerManager.Instance.GetElapsedTime();
            timerText.text = TimerManager.FormatTime(currentTime);
            
            // 타이머가 실행 중이 아니면 불투명도 감소
            if (!TimerManager.Instance.IsRunning())
            {
                Color color = timerText.color;
                color.a = 0.5f;
                timerText.color = color;
            }
            else
            {
                Color color = timerText.color;
                color.a = 1f;
                timerText.color = color;
            }
        }
    }
    
    public void ResetDisplay()
    {
        if (timerText != null)
        {
            timerText.text = "00:00.000";
            Color color = timerText.color;
            color.a = 0.5f;
            timerText.color = color;
        }
    }
}