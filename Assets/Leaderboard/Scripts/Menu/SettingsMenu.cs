using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : Panel
{
    [SerializeField] private Button closeButton = null;

    private bool settingsOpen = false;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSettings);
        }
        
        base.Initialize();
    }

    public override void Open()
    {
        settingsOpen = true;
        base.Open();
    }

    public override void Close()
    {
        settingsOpen = false;
        base.Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsOpen)
            {
                CloseSettings();
            }
            else
            {
                Open();
            }
        }
    }

    private void CloseSettings()
    {
        Close();
    }
}