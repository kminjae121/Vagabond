using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private float elapsedTime = 0f;
    private bool isRunning = false;

    private static TimerManager instance = null;

    public static TimerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<TimerManager>();
                if (instance == null)
                {
                    instance = new GameObject("TimerManager").AddComponent<TimerManager>();
                }
            }
            return instance;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        if (!isRunning)
        {
            elapsedTime = 0f;
            isRunning = true;
            Debug.Log("타이머 시작됨");
        }
    }
    
    public float StopTimer()
    {
        if (isRunning)
        {
            isRunning = false;
            Debug.Log($"타이머 종료됨. 경과 시간: {FormatTime(elapsedTime)}");
            return elapsedTime;
        }
        return 0f;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
    
    public bool IsRunning()
    {
        return isRunning;
    }

    /// <summary>
    /// 경과 시간을 MM:SS.ms 형식으로 포맷팅합니다. (소수점 3자리)
    /// 예시: 00:00.000
    /// </summary>
    public static string FormatTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        int milliseconds = (int)((time % 1f) * 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
    
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
    }
}