using TheDungeon.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public class ButtonUi : Selectable
    {
        [SerializeField]
        private float animationDuration;
        [SerializeField]
        private float selectedScale;
        [SerializeField]
        private Vector3 selectedOffset;
        [SerializeField]
        private AnimationCurve animationCurve;
        [SerializeField]
        private Transform animationTransform;
        
        private SimpleTimer _animationTimer;
        
        private void Update()
        {
            _animationTimer.Update(Time.deltaTime);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PlayEnterAnimation();
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            PlayExitAnimation();
        }
        
        private void PlayEnterAnimation()
        {
            PlayAnimation(selectedScale, selectedOffset);
        }
        
        private void PlayExitAnimation()
        {
            PlayAnimation(1f, Vector3.zero);
        }
        
        private void PlayAnimation(float endScale, Vector3 endPos)
        {
            var startScale = animationTransform.localScale.x;
            var startPos = animationTransform.localPosition;
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(animationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var vectorStartScale = Vector3.one * startScale;
                var vectorEndScale = Vector3.one * endScale;
                var eval = animationCurve.Evaluate(normalizedTime);
                animationTransform.localScale = Vector3.Lerp(vectorStartScale, vectorEndScale, eval);
                animationTransform.localPosition = Vector3.Lerp(startPos, endPos, eval);
            }
        }
    }
    
}