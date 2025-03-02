using Code.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class TextButton : ButtonBase
    {
        [SerializeField]
        private TextButtonConfig buttonConfig;
        [SerializeField]
        private Transform animationTransform;
        [SerializeField]
        private Image lineImage;
        [SerializeField]
        private Image arrowImage;
        [SerializeField]
        private Text text;
        
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                lineImage.transform.localScale = Vector3.zero;
                arrowImage.transform.localScale = Vector3.zero;
            }
        }
        
        protected override void PlayEnterAnimation()
        {
            PlayAnimation(buttonConfig.SelectedScale, buttonConfig.SelectedOffset, buttonConfig.HighlightedColor, 1f, -1f);
        }
        
        protected override void PlayExitAnimation()
        {
            PlayAnimation(1f, Vector3.zero, buttonConfig.DefaultColor, 0f, 1f);
        }
        
        protected override void PlayIdleBreakAnimation()
        {
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(buttonConfig.IdleBreakAnimationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var eval = buttonConfig.AnimationCurve.Evaluate(normalizedTime);
                var scaleEval = buttonConfig.ScaleCurve.Evaluate(normalizedTime);
                arrowImage.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -360f), eval));
                arrowImage.transform.localScale = Vector3.one + Vector3.one * ((buttonConfig.IdleBreakArrowScale - 1) * scaleEval);
            }
        }
        
        protected override void ResetButton() {
            _animationTimer.Stop();
                
            animationTransform.localScale = Vector3.one;
            animationTransform.localPosition = Vector3.zero;
            lineImage.color = buttonConfig.DefaultColor;
            lineImage.transform.localScale = Vector3.zero;
            arrowImage.color = buttonConfig.DefaultColor;
            arrowImage.transform.localRotation = Quaternion.identity;
            arrowImage.transform.localScale = Vector3.zero;
            text.color = buttonConfig.DefaultColor;
        }
        
        private void PlayAnimation(float buttonScale, Vector3 position, Color color, float imagesScale, float direction)
        {
            var startPos = animationTransform.localPosition;
            var startColor = lineImage.color;
            var startImagesScale = lineImage.transform.localScale.x;
            var vectorStartScale = Vector3.one * animationTransform.localScale.x;
            var vectorEndScale = Vector3.one * buttonScale;
            _animationTimer = new SimpleTimer();
            _animationTimer.OnUpdate += UpdateAnimation;
            _animationTimer.Start(buttonConfig.AnimationDuration);

            void UpdateAnimation(float normalizedTime)
            {
                var eval = buttonConfig.AnimationCurve.Evaluate(normalizedTime);
                animationTransform.localScale = Vector3.Lerp(vectorStartScale, vectorEndScale, eval);
                animationTransform.localPosition = Vector3.Lerp(startPos, position, eval);
                lineImage.color = Color.Lerp(startColor, color, eval);
                lineImage.transform.localScale = new Vector3(Mathf.Lerp(startImagesScale, imagesScale, eval), 1, 1);
                arrowImage.color = Color.Lerp(startColor, color, eval);
                arrowImage.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, direction * 360f), eval));
                arrowImage.transform.localScale = Vector3.Lerp(Vector3.one * startImagesScale, Vector3.one * imagesScale, eval);
                text.color = Color.Lerp(startColor, color, eval);
            }
        }

        private void OnValidate()
        {
            lineImage.color = buttonConfig.DefaultColor;
            arrowImage.color = buttonConfig.DefaultColor;
            text.color = buttonConfig.DefaultColor;
        }
    }
    
}