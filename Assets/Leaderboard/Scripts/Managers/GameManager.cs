using System.Collections;
using System.Collections.Generic;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private ClearZoneTrigger clearZoneTrigger = null;
    private bool gameStarted = false;

    private static GameManager instance = null;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<GameManager>();
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

    private void Start()
    {
        FindAndAssignClearZoneTrigger();
    }

    private void FindAndAssignClearZoneTrigger()
    {
        clearZoneTrigger = FindFirstObjectByType<ClearZoneTrigger>();
        UnityLogger.Log("왜 안됨");
        
        if (clearZoneTrigger != null)
        {
            clearZoneTrigger.OnClearZoneEntered += OnPlayerClearedGame;
            StartGame();
            Debug.Log("ClearZoneTrigger 자동 할당됨");
        }
        else
        {
            Debug.LogWarning("ClearZoneTrigger를 찾을 수 없습니다. 씬에 ClearZoneTrigger.cs가 있는 오브젝트가 있는지 확인하세요.");
        }
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            TimerManager.Instance.StartTimer();
            Debug.Log("게임 시작됨!");
        }
    }

    private void OnPlayerClearedGame()
    {
        if (gameStarted)
        {
            gameStarted = false;
            float recordedTime = TimerManager.Instance.StopTimer();
            
            LeaderboardsMenu leaderboardMenu = (LeaderboardsMenu)PanelManager.GetSingleton("leaderboards");
            
            UnityLogger.Log(leaderboardMenu);
            
            if (leaderboardMenu != null)
            {
                leaderboardMenu.RecordTimeAsync(recordedTime);
            }

            ClearResultMenu clearResultMenu = (ClearResultMenu)PanelManager.GetSingleton("clearresult");
            UnityLogger.Log(clearResultMenu);
            if (clearResultMenu != null)
            {
                clearResultMenu.ShowClearResult(recordedTime);
            }
        }
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }
}