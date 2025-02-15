using TheDungeon.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public class ButtonUi : Selectable
    {
        [SerializeField]
        private ButtonConfig buttonConfig;
        [SerializeField]
        private Transform animationTransform;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private Text text;
        [SerializeField]
        private UnityEvent onClick;
        
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
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnClick();
        }
        
        private void PlayEnterAnimation()
        {
            PlayAnimation(buttonConfig.SelectedScale, buttonConfig.SelectedOffset, buttonConfig.HighlightedColor);
        }
        
        private void PlayExitAnimation()
        {
            PlayAnimation(1f, Vector3.zero, buttonConfig.DefaultColor);
        }
        
        private void OnClick()
        {
            onClick.Invoke();
        }
        
        private void PlayAnimation(float endScale, Vector3 endPos, Color endColor)
        {
            var startScale = animationTransform.localScale.x;
            var startPos = animationTransform.localPosition;
            var startColor = backgroundImage.color;
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(buttonConfig.AnimationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var vectorStartScale = Vector3.one * startScale;
                var vectorEndScale = Vector3.one * endScale;
                var eval = buttonConfig.AnimationCurve.Evaluate(normalizedTime);
                animationTransform.localScale = Vector3.Lerp(vectorStartScale, vectorEndScale, eval);
                animationTransform.localPosition = Vector3.Lerp(startPos, endPos, eval);
                backgroundImage.color = Color.Lerp(startColor, endColor, eval);
                text.color = Color.Lerp(startColor, endColor, eval);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            backgroundImage.color = buttonConfig.DefaultColor;
            text.color = buttonConfig.DefaultColor;
        }
    }
    
}