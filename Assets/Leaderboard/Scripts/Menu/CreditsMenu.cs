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
            creditsText.text = "Kim Min Jea\n" +
                               "Kim Dong Hyun\n" +
                               "Park Ji Ho\n" +
                               "Ahn Jun Su";
        }

        base.Initialize();
    }

    private void CloseCredits()
    {
        Close();
    }
}