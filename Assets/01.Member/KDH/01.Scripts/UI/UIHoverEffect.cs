using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _01.Member.KDH._01.Scripts.UI
{
    public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Vector3 _originalScale;
        private Tween _tween;
        
        private float _hoverScale;
        private float _duration;

        public void Init(Vector3 originalScale, float hoverScale, float duration)
        {
            _originalScale = originalScale;
            _hoverScale = hoverScale;
            _duration = duration;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tween?.Kill();
            
            _tween = transform.DOScale(_originalScale * _hoverScale, _duration)
                .SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tween?.Kill();
            
            _tween = transform.DOScale(_originalScale, _duration)
                .SetEase(Ease.OutBack);
        }
    }
}