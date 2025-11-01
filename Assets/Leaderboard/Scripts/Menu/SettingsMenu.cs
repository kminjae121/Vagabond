using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : Panel
{
    [SerializeField] private Button closeButton = null;
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private string masterGroupName;
    [SerializeField] private string bgmGroupName;
    [SerializeField] private string sfxGroupName;
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
    
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat(masterGroupName, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }


    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat(bgmGroupName, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }


    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(sfxGroupName, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }
}