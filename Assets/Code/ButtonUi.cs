using TheDungeon.Utils;
using Unity.Mathematics;
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
        private Image lineImage;
        [SerializeField]
        private Image arrowImage;
        [SerializeField]
        private Text text;
        [SerializeField]
        private UnityEvent onClick;
        
        private SimpleTimer _animationTimer;

        protected override void Awake()
        {
            if (Application.isPlaying)
            {
                lineImage.transform.localScale = Vector3.zero;
                arrowImage.transform.localScale = Vector3.zero;
            }
        }
        
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
            PlayAnimation(buttonConfig.SelectedScale, buttonConfig.SelectedOffset, buttonConfig.HighlightedColor, 1f, -1f);
        }
        
        private void PlayExitAnimation()
        {
            PlayAnimation(1f, Vector3.zero, buttonConfig.DefaultColor, 0f, 1f);
        }
        
        private void OnClick()
        {
            onClick.Invoke();
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
                arrowImage.transform.rotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, new Vector3(0, 0, direction * 360f), eval));
                arrowImage.transform.localScale = Vector3.Lerp(Vector3.one * startImagesScale, Vector3.one * imagesScale, eval);
                text.color = Color.Lerp(startColor, color, eval);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            lineImage.color = buttonConfig.DefaultColor;
            arrowImage.color = buttonConfig.DefaultColor;
            text.color = buttonConfig.DefaultColor;
        }
    }
    
}