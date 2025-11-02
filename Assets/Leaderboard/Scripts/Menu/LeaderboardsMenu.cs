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
    [SerializeField] private TextMeshProUGUI titleText = null;

    private int currentPage = 1;
    private int totalPages = 0;
    private string currentLeaderboardId = "VGTimeRk";
    private Button[] filterButtons = null;
    private bool isMapSpecific = false;
    private MapData currentMap = null;
    private bool filterButtonsCreated = false;

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
        
        // 한 번만 필터 버튼 생성
        if (!filterButtonsCreated)
        {
            CreateFilterButtons();
            filterButtonsCreated = true;
        }
        
        base.Initialize();
    }
    
    public override void Open()
    {
        isMapSpecific = false;
        currentMap = null;
        
        if (titleText != null)
        {
            titleText.text = "Leaderboards";
        }
        
        pageText.text = "-";
        nextButton.interactable = false;
        prevButton.interactable = false;
        base.Open();
        ClearPlayersList();
        currentPage = 1;
        totalPages = 0;
        LoadPlayers(1);
    }

    public void OpenForMap(MapData map)
    {
        isMapSpecific = true;
        currentMap = map;
        currentLeaderboardId = map.leaderboardId;
        
        if (titleText != null)
        {
            titleText.text = map.mapName + " Leaderboard";
        }
        
        currentPage = 1;
        totalPages = 0;
        pageText.text = "-";
        nextButton.interactable = false;
        prevButton.interactable = false;
        ClearPlayersList();
        LoadPlayers(1);
        base.Open();
    }

    private void CreateFilterButtons()
    {
        MapData[] allMaps = MapManager.Instance.GetAllMaps();
        
        if (allMaps == null || allMaps.Length == 0)
        {
            Debug.LogWarning("맵 데이터가 없습니다!");
            return;
        }

        filterButtons = new Button[allMaps.Length + 1];

        // ALL MAP 버튼 생성
        Button allButton = Instantiate(filterButtonPrefab, filterButtonsContainer);
        TextMeshProUGUI allButtonText = allButton.GetComponentInChildren<TextMeshProUGUI>();
        if (allButtonText != null)
        {
            allButtonText.text = "ALL MAP";
        }
        allButton.onClick.AddListener(() => FilterByLeaderboard("VGTimeRk", false));
        filterButtons[0] = allButton;

        // 각 맵별 버튼 생성 - 모두 "ALL MAP"으로 표시
        for (int i = 0; i < allMaps.Length; i++)
        {
            Button mapButton = Instantiate(filterButtonPrefab, filterButtonsContainer);
            TextMeshProUGUI mapButtonText = mapButton.GetComponentInChildren<TextMeshProUGUI>();
            
            if (mapButtonText != null)
            {
                mapButtonText.text = "ALL MAP";
            }
            
            string leaderboardId = allMaps[i].leaderboardId;
            mapButton.onClick.AddListener(() => FilterByLeaderboard(leaderboardId, true));
            filterButtons[i + 1] = mapButton;
        }
        
        Debug.Log("필터 버튼 생성 완료: " + filterButtons.Length + "개");
    }

    private void FilterByLeaderboard(string leaderboardId, bool isMapSpecific)
    {
        currentLeaderboardId = leaderboardId;
        this.isMapSpecific = isMapSpecific;
        
        if (titleText != null)
        {
            titleText.text = isMapSpecific ? "Map Leaderboard" : "Leaderboards";
        }
        
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