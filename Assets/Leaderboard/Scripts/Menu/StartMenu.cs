using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Authentication;

public class StartMenu : Panel
{
    [SerializeField] private Button startButton = null;
    [SerializeField] private Button settingsButton = null;
    [SerializeField] private Button creditsButton = null;
    [SerializeField] private Button exitButton = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        startButton.onClick.AddListener(StartGameFlow);
        settingsButton.onClick.AddListener(OpenSettings);
        creditsButton.onClick.AddListener(OpenCredits);
        exitButton.onClick.AddListener(ExitGame);
        base.Initialize();
    }

    public override void Open()
    {
        // Canvas 활성화
        Canvas uiCanvas = FindFirstObjectByType<Canvas>();
        if (uiCanvas != null)
        {
            uiCanvas.gameObject.SetActive(true);
        }
        
        base.Open();
    }

    private void StartGameFlow()
    {
        PanelManager.CloseAll();
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            GameState.Instance.SetState(GameState.State.Menu);
            PanelManager.Open("main");
        }
        else
        {
            PanelManager.Open("auth");
        }
    }

    private void OpenSettings()
    {
        SettingsMenu settingsMenu = (SettingsMenu)PanelManager.GetSingleton("settings");
        if (settingsMenu != null)
        {
            settingsMenu.Open();
        }
    }

    private void OpenCredits()
    {
        CreditsMenu creditsMenu = (CreditsMenu)PanelManager.GetSingleton("credits");
        if (creditsMenu != null)
        {
            creditsMenu.Open();
        }
    }

    private void ExitGame()
    {
        ExitConfirmMenu exitConfirmMenu = (ExitConfirmMenu)PanelManager.GetSingleton("exitconfirm");
        if (exitConfirmMenu != null)
        {
            exitConfirmMenu.Open();
        }
    }
}