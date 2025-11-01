using System.Collections.Generic;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Entities.UI
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField] private Button startBtn;
        [SerializeField] private Button settingBtn;
        [SerializeField] private Button quitBtn;

        [Header("Hover Effect")] [SerializeField]
        private float hoverScale = 1.2f;

        [SerializeField] private float hoverDuration = 0.2f;

        private Vector3 _originalScale;

        private void Awake()
        {
            if (startBtn == null || settingBtn == null || quitBtn == null)
            {
                UnityLogger.LogError("타이틀 UI 버튼을 찾을 수 없습니다.");
                return;
            }

            _originalScale = startBtn.transform.localScale;

            startBtn.onClick.AddListener(OnStartButtonClicked);
            settingBtn.onClick.AddListener(OnSettingButtonClicked);
            quitBtn.onClick.AddListener(OnQuitButtonClicked);

            List<Button> btnList = new() { startBtn, settingBtn, quitBtn };

            foreach (var btn in btnList)
                AddHoverEffect(btn);
        }

        private void AddHoverEffect(Button btn)
        {
            var hover = btn.gameObject.AddComponent<UIHoverEffect>();
            hover.Init(_originalScale, hoverScale, hoverDuration);
        }

        private void OnDestroy()
        {
            startBtn.onClick.RemoveListener(OnStartButtonClicked);
            settingBtn.onClick.RemoveListener(OnSettingButtonClicked);
            quitBtn.onClick.RemoveListener(OnQuitButtonClicked);
        }

        private void OnStartButtonClicked()
        {
            //SceneManager.LoadSceneAsync();
        }

        private void OnSettingButtonClicked()
        {
            // 설정창 열기
        }

        private void OnQuitButtonClicked() => Application.Quit();
    }
}