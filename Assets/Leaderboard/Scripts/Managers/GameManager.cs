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
            
            // PanelManager 초기화 확인
            if (PanelManager.Singleton == null)
            {
                Debug.LogError("PanelManager를 찾을 수 없습니다.");
                return;
            }

            // leaderboards 패널 처리
            LeaderboardsMenu leaderboardMenu = (LeaderboardsMenu)PanelManager.GetSingleton("leaderboards");
            if (leaderboardMenu != null)
            {
                leaderboardMenu.RecordTimeAsync(recordedTime);
            }
            else
            {
                Debug.LogWarning("LeaderboardsMenu 패널을 찾을 수 없습니다. 패널 ID가 'leaderboards'인지 확인하세요.");
            }

            // clearresult 패널 처리
            ClearResultMenu clearResultMenu = (ClearResultMenu)PanelManager.GetSingleton("clearresult");
            if (clearResultMenu != null)
            {
                clearResultMenu.ShowClearResult(recordedTime);
            }
            else
            {
                Debug.LogWarning("ClearResultMenu 패널을 찾을 수 없습니다. 패널 ID가 'clearresult'인지 확인하세요.");
            }
        }
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }
}
