using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Leaderboards;

public class LeaderboardsMenu : Panel
{

    [SerializeField] private int playersPerPage = 25;
    [SerializeField] private LeaderboardsPlayerItem playerItemPrefab = null;
    [SerializeField] private RectTransform playersContainer = null;
    [SerializeField] public TextMeshProUGUI pageText = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;
    [SerializeField] private Button closeButton = null;
    [SerializeField] private RectTransform filterButtonsContainer = null;
    [SerializeField] private Button filterButtonPrefab = null;

    private int currentPage = 1;
    private int totalPages = 0;
    private string currentLeaderboardId = "VGTimeRk";
    private Button[] filterButtons = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        ClearPlayersList();
        closeButton.onClick.AddListener(ClosePanel);
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);
        CreateFilterButtons();
        base.Initialize();
    }
    
    public override void Open()
    {
        pageText.text = "-";
        nextButton.interactable = false;
        prevButton.interactable = false;
        base.Open();
        ClearPlayersList();
        currentPage = 1;
        totalPages = 0;
        LoadPlayers(1);
    }

    private void CreateFilterButtons()
    {
        MapData[] allMaps = MapManager.Instance.GetAllMaps();
        filterButtons = new Button[allMaps.Length + 1];

        Button allButton = Instantiate(filterButtonPrefab, filterButtonsContainer);
        allButton.GetComponentInChildren<TextMeshProUGUI>().text = "All";
        allButton.onClick.AddListener(() => FilterByLeaderboard("test"));
        filterButtons[0] = allButton;

        for (int i = 0; i < allMaps.Length; i++)
        {
            Button mapButton = Instantiate(filterButtonPrefab, filterButtonsContainer);
            mapButton.GetComponentInChildren<TextMeshProUGUI>().text = allMaps[i].mapName;
            string leaderboardId = allMaps[i].leaderboardId;
            mapButton.onClick.AddListener(() => FilterByLeaderboard(leaderboardId));
            filterButtons[i + 1] = mapButton;
        }
    }

    private void FilterByLeaderboard(string leaderboardId)
    {
        currentLeaderboardId = leaderboardId;
        currentPage = 1;
        totalPages = 0;
        pageText.text = "-";
        nextButton.interactable = false;
        prevButton.interactable = false;
        ClearPlayersList();
        LoadPlayers(1);
    }

    public async void RecordTimeAsync(float elapsedTime)
    {
        try
        {
            MapData selectedMap = MapManager.Instance.GetSelectedMap();
            string leaderboardId = selectedMap != null ? selectedMap.leaderboardId : "VGTimeRk";
            
            long timeScore = (long)(elapsedTime * 1000f);
            
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, timeScore);
            Debug.Log("리더보드에 기록됨: " + TimerManager.FormatTime(elapsedTime));
            LoadPlayers(currentPage);
        }
        catch (Exception exception)
        {
            Debug.Log("리더보드 기록 실패: " + exception.Message);
        }
    }

    private async void LoadPlayers(int page)
    {
        nextButton.interactable = false;
        prevButton.interactable = false;
        try
        {
            GetScoresOptions options = new GetScoresOptions();
            options.Offset = (page - 1) * playersPerPage;
            options.Limit = playersPerPage;
            var scores = await LeaderboardsService.Instance.GetScoresAsync(currentLeaderboardId, options);
            ClearPlayersList();
            for (int i = 0; i < scores.Results.Count; i++)
            {
                LeaderboardsPlayerItem item = Instantiate(playerItemPrefab, playersContainer);
                item.Initialize(scores.Results[i]);
            }
            totalPages = Mathf.CeilToInt((float)scores.Total / (float)scores.Limit);
            currentPage = page;
        }
        catch (Exception exception)
        {
            Debug.Log("플레이어 로드 실패: " + exception.Message);
        }
        pageText.text = currentPage.ToString() + "/" + totalPages.ToString();
        nextButton.interactable = currentPage < totalPages && totalPages > 1;
        prevButton.interactable = currentPage > 1 && totalPages > 1;
    }

    private void NextPage()
    {
        if (currentPage + 1 > totalPages)
        {
            LoadPlayers(1);
        }
        else
        {
            LoadPlayers(currentPage + 1);
        }
    }

    private void PrevPage()
    {
        if (currentPage - 1 <= 0)
        {
            LoadPlayers(totalPages);
        }
        else
        {
            LoadPlayers(currentPage - 1);
        }
    }

    private void ClosePanel()
    {
        Close();
    }

    private void ClearPlayersList()
    {
        LeaderboardsPlayerItem[] items = playersContainer.GetComponentsInChildren<LeaderboardsPlayerItem>();
        if (items != null)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Destroy(items[i].gameObject);
            }
        }
    }

}