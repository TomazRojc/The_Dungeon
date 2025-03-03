using Code.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class ArrowButton : ButtonBase
    {
        [SerializeField]
        private ArrowButtonConfig buttonConfig;
        [SerializeField]
        private Transform animationTransform;
        [SerializeField]
        private Image arrowImage;
        
        protected override void ChangeInteractableState(bool isInteractable)
        {
            if (_isSelected) return;

            Color color;
            if (isInteractable)
            {
                color = buttonConfig.DefaultColor;
            }
            else
            {
                color = buttonConfig.NonInteractableColor;
            }

            arrowImage.color = color;
        }
        
        protected override void PlayEnterAnimation(Color? highlightColor)
        {
            PlayAnimation(buttonConfig.SelectedScale, highlightColor ?? buttonConfig.DefaultHighlightedColor);
        }
        
        protected override void PlayExitAnimation()
        {
            PlayAnimation(1f, buttonConfig.DefaultColor);
        }
        
        protected override void PlayIdleBreakAnimation()
        {
            var startScale = animationTransform.localScale;
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(buttonConfig.IdleBreakAnimationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var eval = buttonConfig.AnimationCurve.Evaluate(normalizedTime);
                var scaleEval = buttonConfig.ScaleCurve.Evaluate(normalizedTime);
                animationTransform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -360f), eval));
                animationTransform.localScale = startScale + Vector3.one * ((buttonConfig.IdleBreakArrowScale - startScale.x) * scaleEval);
            }
        }

        protected override void ResetButton() {
            animationTransform.localScale = Vector3.one;
            animationTransform.localRotation = Quaternion.identity;
            arrowImage.color = buttonConfig.DefaultColor;
        }
        
        private void PlayAnimation(float buttonScale, Color color)
        {
            var startColor = arrowImage.color;
            var startScale = animationTransform.localScale.x;
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(buttonConfig.AnimationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var eval = buttonConfig.AnimationCurve.Evaluate(normalizedTime);
                arrowImage.color = Color.Lerp(startColor, color, eval);
                animationTransform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * buttonScale, eval);
            }
        }

        private void OnValidate()
        {
            arrowImage.color = buttonConfig.DefaultColor;
        }
    }
    
}