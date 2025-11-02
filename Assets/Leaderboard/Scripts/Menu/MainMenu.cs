using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine.UI;
using System;
using Unity.Services.Leaderboards;

public class MainMenu : Panel
{
    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private ScrollRect mapScrollRect = null;
    [SerializeField] private RectTransform mapListContainer = null;
    [SerializeField] private MapSelectionItem mapItemPrefab = null;
    
    [SerializeField] private Image mapDetailImage = null;
    [SerializeField] private TextMeshProUGUI mapDetailNameText = null;
    [SerializeField] private TextMeshProUGUI bestRecordText = null;
    [SerializeField] private Button playButton = null;
    [SerializeField] private Button leaderboardButton = null;

    private MapData selectedMap = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayMap);
        }

        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.AddListener(OpenMapLeaderboard);
        }
        
        base.Initialize();
    }
    
    public override void Open()
    {
        UpdatePlayerNameUI();
        LoadMaps();
        base.Open();
    }

    private void UpdatePlayerNameUI()
    {
        nameText.text = AuthenticationService.Instance.PlayerName;
    }

    private void LoadMaps()
    {
        foreach (Transform child in mapListContainer)
        {
            Destroy(child.gameObject);
        }

        MapData[] allMaps = MapManager.Instance.GetAllMaps();
        foreach (MapData map in allMaps)
        {
            MapSelectionItem item = Instantiate(mapItemPrefab, mapListContainer);
            item.Initialize(map, SelectMapDetail);
        }
    }

    public void SelectMapDetail(MapData map)
    {
        selectedMap = map;
        MapManager.Instance.SelectMap(map);
        LoadMapDetails();
    }

    private void LoadMapDetails()
    {
        if (selectedMap != null)
        {
            if (mapDetailNameText != null)
            {
                mapDetailNameText.text = selectedMap.mapName;
            }

            if (mapDetailImage != null && selectedMap.mapImage != null)
            {
                mapDetailImage.sprite = selectedMap.mapImage;
            }

            LoadBestRecord(selectedMap);
        }
    }

    private async void LoadBestRecord(MapData map)
    {
        try
        {
            var scoresOptions = new GetScoresOptions();
            scoresOptions.Limit = 1;
            var scores = await LeaderboardsService.Instance.GetScoresAsync(map.leaderboardId, scoresOptions);
            
            if (scores.Results.Count > 0 && bestRecordText != null)
            {
                float timeInSeconds = (float)scores.Results[0].Score / 1000f;
                bestRecordText.text = "Best Record: " + TimerManager.FormatTime(timeInSeconds);
            }
            else if (bestRecordText != null)
            {
                bestRecordText.text = "Best Record: --:--.---";
            }
        }
        catch (Exception exception)
        {
            Debug.Log("최고기록 로드 실패: " + exception.Message);
            if (bestRecordText != null)
            {
                bestRecordText.text = "Best Record: --:--.---";
            }
        }
    }

    private void PlayMap()
    {
        if (selectedMap != null)
        {
            MapManager.Instance.PlayMap(selectedMap);
        }
    }

    private void OpenMapLeaderboard()
    {
        if (selectedMap != null)
        {
            MapLeaderboardMenu mapLeaderboardMenu = (MapLeaderboardMenu)PanelManager.GetSingleton("mapleaderboard");
            if (mapLeaderboardMenu != null)
            {
                mapLeaderboardMenu.OpenForMap(selectedMap);
            }
        }
    }
}