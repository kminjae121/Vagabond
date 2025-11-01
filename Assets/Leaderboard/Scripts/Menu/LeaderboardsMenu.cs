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

    private int currentPage = 1;
    private int totalPages = 0;

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
    
    public async void RecordTimeAsync(float elapsedTime)
    {
        try
        {
            // 시간을 점수로 변환 (낮은 점수가 빠른 시간)
            // 예: 1분 30초 = 90.5초 → 이를 점수로 저장
            long timeScore = (long)(elapsedTime * 1000f); // 밀리초 단위로 저장
            
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("test", timeScore);
            Debug.Log($"리더보드에 기록됨: {TimerManager.FormatTime(elapsedTime)}");
            LoadPlayers(currentPage);
        }
        catch (Exception exception)
        {
            Debug.Log($"리더보드 기록 실패: {exception.Message}");
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
            var scores = await LeaderboardsService.Instance.GetScoresAsync("VGTimeRk", options);
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
            Debug.Log($"플레이어 로드 실패: {exception.Message}");
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