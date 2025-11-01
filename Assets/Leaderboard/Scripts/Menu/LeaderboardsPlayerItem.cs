using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine.UI;

public class LeaderboardsPlayerItem : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI rankText = null;
    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] public TextMeshProUGUI timeText = null;
    [SerializeField] private Button selectButton = null;
    
    private LeaderboardEntry player = null;
    
    private void Start()
    {
        selectButton.onClick.AddListener(Clicked);
    }

    
    public void Initialize(LeaderboardEntry player)
    {
        this.player = player;
        rankText.text = (player.Rank + 1).ToString();
        nameText.text = player.PlayerName;
        
        // 점수를 시간으로 변환 (밀리초 단위로 저장된 값을 초 단위로 변환)
        float timeInSeconds = (float)player.Score / 1000f;
        timeText.text = TimerManager.FormatTime(timeInSeconds);
    }
    
    private void Clicked()
    {
        Debug.Log("TODO -> Open profile: " + player.PlayerName);
    }
    
}