using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Collider clearPoint = null;
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
        if (clearPoint != null)
        {
            clearZoneTrigger = clearPoint.GetComponent<ClearZoneTrigger>();
            if (clearZoneTrigger == null)
            {
                clearZoneTrigger = clearPoint.gameObject.AddComponent<ClearZoneTrigger>();
            }
            clearZoneTrigger.OnClearZoneEntered += OnPlayerClearedGame;
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
            if (leaderboardMenu != null)
            {
                leaderboardMenu.RecordTimeAsync(recordedTime);
            }
        }
    }

    public void SetClearPoint(Collider clearPointCollider)
    {
        clearPoint = clearPointCollider;
        if (clearPoint != null)
        {
            clearZoneTrigger = clearPoint.GetComponent<ClearZoneTrigger>();
            if (clearZoneTrigger == null)
            {
                clearZoneTrigger = clearPoint.gameObject.AddComponent<ClearZoneTrigger>();
            }
            clearZoneTrigger.OnClearZoneEntered += OnPlayerClearedGame;
        }
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }
}