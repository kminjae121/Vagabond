using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.Services.Leaderboards;

public class ClearResultMenu : Panel
{
    [SerializeField] private TextMeshProUGUI mapNameText = null;
    [SerializeField] private TextMeshProUGUI clearMessageText = null;
    [SerializeField] private TextMeshProUGUI recordedTimeText = null;
    [SerializeField] private TextMeshProUGUI bestScoreText = null;
    [SerializeField] private Button leaderboardButton = null;
    [SerializeField] private Button restartButton = null;
    [SerializeField] private Button mainMenuButton = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        
        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.AddListener(OpenLeaderboard);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoMainMenu);
        }

        base.Initialize();
    }

    public void ShowClearResult(float elapsedTime)
    {
        MapData selectedMap = MapManager.Instance.GetSelectedMap();

        if (mapNameText != null && selectedMap != null)
        {
            mapNameText.text = selectedMap.mapName;
        }

        if (clearMessageText != null)
        {
            clearMessageText.text = "Level Complete";
        }

        if (recordedTimeText != null)
        {
            recordedTimeText.text = "Time: " + TimerManager.FormatTime(elapsedTime);
        }

        if (bestScoreText != null)
        {
            bestScoreText.gameObject.SetActive(false);
        }

        if (selectedMap != null)
        {
            CheckBestScore(selectedMap, elapsedTime);
        }

        Open();
    }

    private async void CheckBestScore(MapData map, float currentTime)
    {
        try
        {
            var scoresOptions = new GetScoresOptions();
            scoresOptions.Limit = 1;
            var scores = await LeaderboardsService.Instance.GetScoresAsync(map.leaderboardId, scoresOptions);
            
            if (scores.Results.Count > 0)
            {
                float bestTime = (float)scores.Results[0].Score / 1000f;
                
                if (currentTime < bestTime)
                {
                    if (bestScoreText != null)
                    {
                        bestScoreText.text = "Best Score";
                        bestScoreText.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (bestScoreText != null)
                {
                    bestScoreText.text = "Best Score";
                    bestScoreText.gameObject.SetActive(true);
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log("최고기록 확인 실패: " + exception.Message);
        }
    }

    private void OpenLeaderboard()
    {
        Close();
        LeaderboardsMenu leaderboardsMenu = (LeaderboardsMenu)PanelManager.GetSingleton("leaderboards");
        if (leaderboardsMenu != null)
        {
            leaderboardsMenu.Open();
        }
    }

    private void RestartGame()
    {
        Close();
        TimerManager.Instance.ResetTimer();
        GameState.Instance.SetState(GameState.State.InGame);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoMainMenu()
    {
        Close();
        GameState.Instance.SetState(GameState.State.Menu);
        TimerManager.Instance.ResetTimer();
        SceneManager.LoadScene("Menu");
    }
}