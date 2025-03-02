using UnityEngine;

namespace Code.UI
{
    [CreateAssetMenu(fileName = "TextButtonConfig", menuName = "ScriptableObjects/TextButtonConfig", order = 2)]
    public class TextButtonConfig : ScriptableObject
    {
        [SerializeField]
        private Color defaultColor;
        [SerializeField]
        private Color nonInteractableColor;
        [SerializeField]
        private Color defaultHighlightedColor;
        [SerializeField]
        private float animationDuration;
        [SerializeField]
        private float idleBreakAnimationDuration;
        [SerializeField]
        private float selectedScale;
        [SerializeField]
        private Vector3 selectedOffset;
        [SerializeField]
        private float idleBreakArrowScale;
        [SerializeField]
        private AnimationCurve animationCurve;
        [SerializeField]
        private AnimationCurve scaleCurve;

        public Color DefaultColor => defaultColor;
        public Color NonInteractableColor => nonInteractableColor;
        public Color DefaultHighlightedColor => defaultHighlightedColor;
        public float AnimationDuration => animationDuration;
        public float IdleBreakAnimationDuration => idleBreakAnimationDuration;
        public float SelectedScale => selectedScale;
        public Vector3 SelectedOffset => selectedOffset;
        public float IdleBreakArrowScale => idleBreakArrowScale;
        public AnimationCurve AnimationCurve => animationCurve;
        public AnimationCurve ScaleCurve => scaleCurve;
    }
}