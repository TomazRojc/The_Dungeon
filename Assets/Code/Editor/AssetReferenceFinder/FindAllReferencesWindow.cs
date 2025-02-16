using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.AssetReferenceFinder {

    public class FindAllReferencesWindow : EditorWindow {

        private static bool s_reinitialize = false;

        private static PrefabReferences s_prefabReferences;

        private const string WindowTitle = "Find References in Project";
        
        private const string MenuItem = "Tools/" + WindowTitle + " #F12";
        
        private const float RowHeight = 18f;
        
        private const float ReferencesButtonWidth = 120f;
        
        private Vector2 _scrollPosition;

        private FindAllReferencesModel _model;

        [MenuItem(MenuItem, false, 20)]
        public static void FindReferencesInProject() {
            var window = GetWindow<FindAllReferencesWindow>(WindowTitle);
            window.Initialize();
        }

        [MenuItem(MenuItem, true, 20)]
        public static bool FindReferencesInProjectValidation() {
            return Selection.activeObject != null;
        }

        private void OnDestroy() {
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        private void Initialize() {
            _model = new FindAllReferencesModel();
            s_reinitialize = false;
        }

        private void OnGUI() {
            if (_model == null) {
                return;
            }

            if (s_reinitialize) {
                Initialize();
            }

            GUILayout.BeginVertical();

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
            
            foreach (var result in _model.Results) {
                DrawResult(result);
            }

            if (_model.HasAnyUnusedResult) {
                DrawRemoveUnusedAssetsButton();
            }

            GUILayout.Space(10);
            GUILayout.Label($"Time needed: {_model.TimeSpent} ms");

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private static void DrawResult(FindAllReferencesWindowResult result) {
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Found {result.Usages.Count} usages for asset:", GUILayout.ExpandWidth(false));

            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            
            if (AssetButton(result.Target.Asset)) {
                SelectAsset(result.Target.Asset);
            }
            
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            GUILayout.EndHorizontal();

            foreach (var usage in result.Usages) {
                DrawUsage(usage, result.Target);
                DrawPrefabLinkButtons(usage);
            }
        }

        private static void DrawUsage(FindAllReferencesWindowAsset usage, FindAllReferencesWindowAsset target) {
            GUILayout.BeginHorizontal();

            if (FindAssetReferencesButton()) {
                SelectAsset(usage.Asset);
                s_reinitialize = true;
            }

            if (AssetButton(usage.Asset))
            {
                SelectAsset(usage.Asset);
                
                if (usage.Asset is GameObject)
                {
                    HandlePrefabSelected(usage.Asset, target.Asset);
                }
            }

            GUILayout.Label($"{usage.Usages}", GUILayout.Width(32f));
            GUILayout.EndHorizontal();
        }

        private static void HandlePrefabSelected(Object usageAsset, Object targetAsset)
        {
            AssetDatabase.OpenAsset((GameObject) usageAsset);
            var prefabInHierarchy = PrefabStageUtility.GetCurrentPrefabStage()?.prefabContentsRoot;
                    
            if (prefabInHierarchy != null)
            {
                // Set prefab references so they are drawn OnGUI
                if (s_prefabReferences.prefabAsset != usageAsset)
                {
                    s_prefabReferences.prefabAsset = usageAsset;
                    s_prefabReferences.referencingComponents = AssetFinderUtils.FindReferencesInPrefab(prefabInHierarchy, targetAsset);
                }

                // Instantly highlight the game object with the first link
                if (s_prefabReferences.referencingComponents.Count > 0)
                {
                    Selection.activeGameObject = s_prefabReferences.referencingComponents[0].gameObject;
                }
            }
            
        }

        private static void DrawPrefabLinkButtons(FindAllReferencesWindowAsset usage)
        {
            if (s_prefabReferences.prefabAsset != usage.Asset) return;

            if (s_prefabReferences.referencingComponents.Count > 1)
            {
                for (int i = 0; i < s_prefabReferences.referencingComponents.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(ReferencesButtonWidth + 20f);
                    if (PrefabReferenceButton(i, s_prefabReferences.referencingComponents[i]))
                    {
                        Selection.activeGameObject = s_prefabReferences.referencingComponents[i].gameObject;
                    }
                    GUILayout.Space(50f);
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(2f);
            }
        }

        private void DrawRemoveUnusedAssetsButton() {
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Remove All Unused Assets", GUILayout.ExpandWidth(true), GUILayout.Height(RowHeight))) {
                foreach (var result in _model.Results) {
                    if (result.Usages.Count == 0) {
                        AssetDatabase.DeleteAsset(result.Target.AssetPath);
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        private static void SelectAsset(Object asset) {
            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
            s_prefabReferences.prefabAsset = null;
        }

        private static bool AssetButton(Object asset)
        {
            if (asset == null) return false;
            
            var gc = EditorGUIUtility.ObjectContent(asset, asset.GetType());
            gc.text = asset.name;
            return GUILayout.Button(gc, GUILayout.ExpandWidth(true), GUILayout.Height(RowHeight));
        }

        private static bool FindAssetReferencesButton() {
            return GUILayout.Button("Find References", GUILayout.Width(ReferencesButtonWidth), GUILayout.Height(RowHeight));
        }

        private static bool PrefabReferenceButton(int index, Component component) {
            return GUILayout.Button($"{index+1}. {component}", GUILayout.ExpandWidth(true), GUILayout.Height(RowHeight));
        }

    }

    public struct PrefabReferences
    {
        public Object prefabAsset;
        public List<Component> referencingComponents;
    }
    

}
