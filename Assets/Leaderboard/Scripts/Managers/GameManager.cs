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
        // 맵 씬에 진입했을 때 모든 UI 패널만 Close (Canvas 유지)
        HideAllPanels();
        FindAndAssignClearZoneTrigger();
    }

    private void HideAllPanels()
    {
        PanelManager.Close("start");
        PanelManager.Close("auth");
        PanelManager.Close("loading");
        PanelManager.Close("main");
        PanelManager.Close("leaderboards");
        PanelManager.Close("clearresult");
        PanelManager.Close("settings");
        PanelManager.Close("credits");
        PanelManager.Close("exitconfirm");
        PanelManager.Close("error");
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
            Debug.LogWarning("ClearZoneTrigger를 찾을 수 없습니다.");
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
            
            // 메뉴 씬으로 로드
            SceneManager.LoadScene("TestUGS");
            
            // 씬 로드 완료 후 UI 표시
            StartCoroutine(ShowClearResultAfterSceneLoad(recordedTime));
        }
    }

    private IEnumerator ShowClearResultAfterSceneLoad(float recordedTime)
    {
        // 씬이 완전히 로드될 때까지 대기
        yield return new WaitForSeconds(0.5f);
        
        // PanelManager 재초기화
        PanelManager.Singleton.ForceReinitialize();
        
        // UI 활성화
        CanvasManager.ShowUI();
        
        yield return null;
        yield return null;

        // 모든 패널 닫기
        HideAllPanels();

        // leaderboards 패널에 기록 저장
        LeaderboardsMenu leaderboardMenu = (LeaderboardsMenu)PanelManager.GetSingleton("leaderboards");
        if (leaderboardMenu != null)
        {
            leaderboardMenu.RecordTimeAsync(recordedTime);
            Debug.Log("LeaderboardsMenu 찾음");
        }

        // clearresult 패널 표시
        ClearResultMenu clearResultMenu = (ClearResultMenu)PanelManager.GetSingleton("clearresult");
        if (clearResultMenu != null)
        {
            clearResultMenu.ShowClearResult(recordedTime);
            Debug.Log("ClearResultMenu 찾음");
        }
    }

    public bool IsGameStarted()
    {
        return gameStarted;
    }
}