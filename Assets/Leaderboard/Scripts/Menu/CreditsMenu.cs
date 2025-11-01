using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreditsMenu : Panel
{
    [SerializeField] private Button closeButton = null;
    [SerializeField] private TextMeshProUGUI creditsText = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseCredits);
        }

        if (creditsText != null)
        {
            creditsText.text = "=== CREDITS ===\n\n" +
                               "Game Design\n" +
                               "Development Team\n\n" +
                               "Art & Animation\n" +
                               "Art Team\n\n" +
                               "Audio\n" +
                               "Sound Designer\n\n" +
                               "Special Thanks\n" +
                               "All Players\n\n" +
                               "© 2025 Game Studio\n" +
                               "All Rights Reserved";
        }

        base.Initialize();
    }

    private void CloseCredits()
    {
        Close();
    }
}