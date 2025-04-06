using Code.Gameplay;
using UnityEditor;
using UnityEngine;

namespace Code.Editor
{
    [CustomEditor(typeof(RendererSortingOrderingComponent))]
    public class RendererSortingOrderingEditor : UnityEditor.Editor
    {

        private RendererSortingOrderingComponent _targetComponent;
        private int _allChangeValue;
    
        public override void OnInspectorGUI()
        {
            // Draw default fields
            DrawDefaultInspector();

            if (_targetComponent == null)
            {
                _targetComponent = (RendererSortingOrderingComponent)target;
            }
        
            GUILayout.Space(10f);
            GUILayout.Label("Update Sorting Values");
            GUILayout.Space(5f);
            foreach (var spriteRenderer in _targetComponent.SpriteRenderers)
            {
                spriteRenderer.sortingOrder = EditorGUILayout.IntField($"{spriteRenderer.name} renderer", spriteRenderer.sortingOrder);
            }

            foreach (var trailRenderer in _targetComponent.TrailRenderers)
            {
                trailRenderer.sortingOrder = EditorGUILayout.IntField($"{trailRenderer.name} renderer", trailRenderer.sortingOrder);
            }

            foreach (var particleSystemRenderers in _targetComponent.ParticleSystemRenderers)
            {
                particleSystemRenderers.sortingOrder = EditorGUILayout.IntField($"{particleSystemRenderers.name} renderer", particleSystemRenderers.sortingOrder);
            }
        
            DrawUpdateAllRenderersGUI();
        }

        private void DrawUpdateAllRenderersGUI()
        {
            GUILayout.BeginHorizontal();
            var newVal = EditorGUILayout.IntField("Change All", _allChangeValue);
            _allChangeValue = newVal;
            if (GUILayout.Button("Add To Sorting Order"))
            {
                _targetComponent.ChangeOrderInLayer(newVal);
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();
        }
    }
}
