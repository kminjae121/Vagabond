using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _01.Member.KDH._01.Scripts.UI
{
    public class UIButtonBrightness : MonoBehaviour
    {
        public Button targetButton;
        public float brightnessAmount = 0.2f; // 밝기 증가치
        public float duration = 0.15f;

        private Image buttonImage;
        private Color originalColor;

        void Start()
        {
            buttonImage = targetButton.GetComponent<Image>();
            originalColor = buttonImage.color;
            targetButton.onClick.AddListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            // 색상 밝기 증가
            Color brighterColor = new Color(
                Mathf.Min(originalColor.r + brightnessAmount, 1f),
                Mathf.Min(originalColor.g + brightnessAmount, 1f),
                Mathf.Min(originalColor.b + brightnessAmount, 1f),
                originalColor.a
            );

            // 밝아졌다가 원래색으로 돌아오는 애니메이션
            buttonImage.DOColor(brighterColor, duration)
                .OnComplete(() =>
                {
                    buttonImage.DOColor(originalColor, duration);
                });
        }
    }
}