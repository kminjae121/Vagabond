using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    
    private Dictionary<string, Panel> panels = new Dictionary<string, Panel>();
    private bool initialized = false;
    private Canvas[] canvas = null;
    private static PanelManager singleton = null;
    
    public static PanelManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindFirstObjectByType<PanelManager>();
                if (singleton == null)
                {
                    singleton = new GameObject("PanelManager").AddComponent<PanelManager>();
                }
                singleton.Initialize();
            }
            return singleton; 
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Initialize()
    {
        if (initialized) { return; }
        initialized = true;
        RefreshPanels();
    }

    private void RefreshPanels()
    {
        panels.Clear();
        canvas = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        Debug.Log("Canvas 개수: " + canvas.Length);
        
        if (canvas != null)
        {
            for (int i = 0; i < canvas.Length; i++)
            {
                Panel[] list = canvas[i].gameObject.GetComponentsInChildren<Panel>(true);
                if (list != null)
                {
                    for (int j = 0; j < list.Length; j++)
                    {
                        if (string.IsNullOrEmpty(list[j].ID) == false && panels.ContainsKey(list[j].ID) == false)
                        {
                            list[j].Initialize();
                            list[j].Canvas = canvas[i];
                            panels.Add(list[j].ID, list[j]);
                            Debug.Log("패널 등록됨: " + list[j].ID);
                        }
                    }
                }
            }
        }
        
        Debug.Log("총 등록된 패널: " + panels.Count);
    }

    public void ForceReinitialize()
    {
        initialized = false;
        RefreshPanels();
    }
    
    private void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }
    
    public static Panel GetSingleton(string id)
    {
        var panel = Singleton.panels.ContainsKey(id) ? Singleton.panels[id] : null;
        if (panel == null)
        {
            Debug.LogError("패널을 찾을 수 없습니다: " + id);
        }
        return panel;
    }
    
    public static void Open(string id)
    {
        var panel = GetSingleton(id);
        if (panel != null)
        {
            panel.Open();
        }
    }
    
    public static void Close(string id)
    {
        var panel = GetSingleton(id);
        if (panel != null)
        {
            panel.Close();
        }
    }
    
    public static bool IsOpen(string id)
    {
        if (Singleton.panels.ContainsKey(id))
        {
            return Singleton.panels[id].IsOpen;
        }
        return false;
    }
    
    public static void CloseAll()
    {
        foreach (var panel in Singleton.panels)
        {
            if (panel.Value != null)
            {
                panel.Value.Close();
            }
        }
    }
    
}