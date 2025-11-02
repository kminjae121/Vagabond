using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExitConfirmMenu : Panel
{
    [SerializeField] private Button confirmButton = null;
    [SerializeField] private Button cancelButton = null;
    [SerializeField] private TextMeshProUGUI confirmText = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        
        confirmButton.onClick.AddListener(ConfirmExit);
        cancelButton.onClick.AddListener(CancelExit);

        if (confirmText != null)
        {
            confirmText.text = "Are you sure you want to            the game?";
        }

        base.Initialize();
    }

    private void ConfirmExit()
    {
        Debug.Log("게임 종료");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void CancelExit()
    {
        Close();
    }
}