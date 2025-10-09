using UnityEngine;

namespace _01.Member.KDH._02.Scripts.UI
{
    public class MenuManager : MonoBehaviour
    {
    
        private bool initialized = false;
    
        private static MenuManager singleton = null;

        public static MenuManager Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = FindFirstObjectByType<MenuManager>();
                    singleton.Initialize();
                }
                return singleton; 
            }
        }

        private void Initialize()
        {
            if (initialized) { return; }
            initialized = true;
        }
    
        private void OnDestroy()
        {
            if (singleton == this)
            {
                singleton = null;
            }
        }

        private void Awake()
        {
            Application.runInBackground = true;
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open("Something happened !!!", "Cool");
        }
    
    }
}