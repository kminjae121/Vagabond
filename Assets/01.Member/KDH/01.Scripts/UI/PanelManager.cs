using System.Collections.Generic;
using UnityEngine;

namespace _01.Member.KDH._02.Scripts.UI
{
    public class PanelManager : MonoBehaviour
    {
        private readonly Dictionary<string, Panel> panels = new();
        private bool initialized;
        private Canvas[] canvases;
        private static PanelManager singleton;
    
        public static PanelManager Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = FindFirstObjectByType<PanelManager>();
                    
                    if (singleton == null)
                        singleton = new GameObject("PanelManager").AddComponent<PanelManager>();
                    
                    singleton.Initialize();
                }
                
                return singleton; 
            }
        }

        private void Initialize()
        {
            if (initialized) 
                return;
            
            initialized = true;
            panels.Clear();
            
            canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            if (canvases == null)
                return;
            
            foreach (var canvas in canvases)
            {
                var list = canvas.gameObject.GetComponentsInChildren<Panel>(true);

                if (list == null)
                    continue;
                
                foreach (var panel in list)
                {
                    if (string.IsNullOrEmpty(panel.ID) || panels.ContainsKey(panel.ID))
                        continue;
                    
                    panel.Initialize();
                    panel.Canvas = canvas;
                    panels.Add(panel.ID, panel);
                }
            }
        }
    
        private void OnDestroy()
        {
            if (singleton == this)
                singleton = null;
        }
    
        public static Panel GetSingleton(string id)
        {
            return Singleton.panels.ContainsKey(id) ?
                Singleton.panels[id] : null;
        }
    
        public static void Open(string id)
        {
            var panel = GetSingleton(id);
            
            if (panel != null)
                panel.Open();
        }
    
        public static void Close(string id)
        {
            var panel = GetSingleton(id);
            
            if (panel != null)
                panel.Close();
        }
    
        public static bool IsOpen(string id)
        {
            return Singleton.panels.ContainsKey(id) && Singleton.panels[id].IsOpen;
        }
    
        public static void CloseAll()
        {
            foreach (var panel in Singleton.panels)
                if (panel.Value != null)
                    panel.Value.Close();
        }
    }
}