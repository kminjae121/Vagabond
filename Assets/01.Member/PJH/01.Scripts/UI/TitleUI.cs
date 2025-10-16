using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Entities.UI
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button quitButton;
        
        private void Awake()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            settingButton.onClick.AddListener(OnSettingButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnDestroy()
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
            settingButton.onClick.RemoveListener(OnSettingButtonClicked);
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            //SceneManager.LoadSceneAsync();
        }

        private void OnSettingButtonClicked()
        {
            // 설정창
        }

        private void OnQuitButtonClicked()
        {
            Application.Quit();
        }
    }
}