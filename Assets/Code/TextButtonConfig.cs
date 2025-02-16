using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "ButtonConfig", menuName = "ScriptableObjects/ButtonConfig", order = 2)]
    public class TextButtonConfig : ScriptableObject
    {
        [SerializeField]
        private Color defaultColor;
        [SerializeField]
        private Color highlightedColor;
        [SerializeField]
        private float animationDuration;
        [SerializeField]
        private float selectedScale;
        [SerializeField]
        private Vector3 selectedOffset;
        [SerializeField]
        private AnimationCurve animationCurve;

        public Color DefaultColor => defaultColor;
        public Color HighlightedColor => highlightedColor;
        public float AnimationDuration => animationDuration;
        public float SelectedScale => selectedScale;
        public Vector3 SelectedOffset => selectedOffset;
        public AnimationCurve AnimationCurve => animationCurve;
    }
}