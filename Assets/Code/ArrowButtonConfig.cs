using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "ArrowButtonConfig", menuName = "ScriptableObjects/ArrowButtonConfig", order = 3)]
    public class ArrowButtonConfig : ScriptableObject
    {
        [SerializeField]
        private Color defaultColor;
        [SerializeField]
        private Color highlightedColor;
        [SerializeField]
        private float animationDuration;
        [SerializeField]
        private float idleBreakAnimationDuration;
        [SerializeField]
        private float selectedScale;
        [SerializeField]
        private float idleBreakArrowScale;
        [SerializeField]
        private AnimationCurve animationCurve;
        [SerializeField]
        private AnimationCurve scaleCurve;

        public Color DefaultColor => defaultColor;
        public Color HighlightedColor => highlightedColor;
        public float AnimationDuration => animationDuration;
        public float IdleBreakAnimationDuration => idleBreakAnimationDuration;
        public float SelectedScale => selectedScale;
        public float IdleBreakArrowScale => idleBreakArrowScale;
        public AnimationCurve AnimationCurve => animationCurve;
        public AnimationCurve ScaleCurve => scaleCurve;
    }
}