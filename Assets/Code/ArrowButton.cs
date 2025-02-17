using Code.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class ArrowButton : ButtonBase
    {
        [SerializeField]
        private ArrowButtonConfig buttonConfig;
        [SerializeField]
        private Image arrowImage;
        
        private SimpleTimer _animationTimer;
        
        protected override void Update()
        {
            base.Update();
            _animationTimer.Update(Time.deltaTime);
        }
        
        protected override void PlayEnterAnimation()
        {
            PlayAnimation(buttonConfig.SelectedScale, buttonConfig.HighlightedColor);
        }
        
        protected override void PlayExitAnimation()
        {
            PlayAnimation(1f, buttonConfig.DefaultColor);
        }
        
        protected override void PlayIdleBreakAnimation()
        {
            var startScale = arrowImage.transform.localScale;
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(buttonConfig.IdleBreakAnimationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var eval = buttonConfig.AnimationCurve.Evaluate(normalizedTime);
                var scaleEval = buttonConfig.ScaleCurve.Evaluate(normalizedTime);
                arrowImage.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -360f), eval));
                arrowImage.transform.localScale = startScale + Vector3.one * ((buttonConfig.IdleBreakArrowScale - startScale.x) * scaleEval);
            }
        }
        
        private void PlayAnimation(float buttonScale, Color color)
        {
            var startColor = arrowImage.color;
            var startScale = arrowImage.transform.localScale.x;
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(buttonConfig.AnimationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var eval = buttonConfig.AnimationCurve.Evaluate(normalizedTime);
                arrowImage.color = Color.Lerp(startColor, color, eval);
                arrowImage.transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * buttonScale, eval);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            arrowImage.color = buttonConfig.DefaultColor;
        }
    }
    
}