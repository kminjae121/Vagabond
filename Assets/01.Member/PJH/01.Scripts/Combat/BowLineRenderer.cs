using UnityEngine;
using DG.Tweening;

namespace Code.Entities.Combat
{
    public class BowLineRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer bowstringLine;
        [SerializeField] private Transform bowstringTop;
        [SerializeField] private Transform bowstringMiddle;
        [SerializeField] private Transform bowstringBottom;
        [SerializeField] private Transform drawTargetPoint; 
        
        private Vector3 _originalMiddlePos;
        private Tween _drawTween;

        private void Awake()
        {
            if (bowstringMiddle != null)
                _originalMiddlePos = bowstringMiddle.position;
        }

        private void LateUpdate()
        {
            CreateBowstring();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            CreateBowstring();
        }
#endif 
        
        public void DrawBowstring(float duration)
        {
            _drawTween?.Kill();

            if (drawTargetPoint != null)
            {
                _drawTween = bowstringMiddle.DOMove(drawTargetPoint.position, duration)
                                            .SetEase(Ease.OutQuad);
            }
        }

        public void ReleaseBowstring(float duration)
        {
            _drawTween?.Kill();
            
            _drawTween = bowstringMiddle.DOMove(_originalMiddlePos, duration)
                                        .SetEase(Ease.OutBack);
        }

        private void CreateBowstring()
        {
            if (!bowstringLine || !bowstringTop || !bowstringMiddle || !bowstringBottom)
                return;
            
            bowstringLine.positionCount = 3;
            bowstringLine.SetPosition(0, bowstringTop.position);
            bowstringLine.SetPosition(1, bowstringMiddle.position);
            bowstringLine.SetPosition(2, bowstringBottom.position);
        }

        private void OnDestroy()
        {
            _drawTween?.Kill();
        }
    }
}