using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace TheDungeon.AssetReferenceFinder {

    public class FindUnusedAssetsInBundlesWindow : EditorWindow {

        private Vector2 _scrollPos;

        private List<List<Object>> _unusedObjectReferencesByType;

        [MenuItem("Outfit7/Find Unused Assets In Bundles", false, 200)]
        public static void FindUnusedAssets() {
            var window = GetWindow<FindUnusedAssetsInBundlesWindow>("Find Unused Assets In Bundles");

            window.Initialize();
        }

        private static void RenderList(List<Object> objects) {
            var log = string.Empty;
            if (objects.Count > 0) {
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();

                var type = objects[0].GetType();
                GUILayout.Label($"UNUSED ASSETS OF TYPE {type} ({objects.Count})", GUILayout.MinWidth(300f), GUILayout.Height(18f));
                log += $"UNUSED ASSETS OF TYPE {type}\n\n";
                if (objects.Count > 1) {
                    if (GUILayout.Button("DELETE ALL", GUILayout.Width(100f), GUILayout.Height(18f))) {
                        foreach (var obj in objects) {
                            DeleteFile(obj);
                        }

                        objects.Clear();
                    }
                }

                GUILayout.EndHorizontal();
            }

            var deletedIndex = -1;
            for (int i = 0; i < objects.Count; i++) {
                var obj = objects[i];
                if (obj == null) {
                    continue;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                var guiContent = EditorGUIUtility.ObjectContent(obj, obj.GetType());
                guiContent.text = obj.name;

                log += $"{AssetDatabase.GetAssetPath(obj)}\n";
                if (GUILayout.Button(guiContent, GUILayout.MinWidth(300f), GUILayout.Height(18f))) {
                    EditorGUIUtility.PingObject(obj);
                    Selection.activeObject = obj;
                }

                if (GUILayout.Button("DELETE", GUILayout.Width(100f), GUILayout.Height(18f))) {
                    DeleteFile(obj);
                    deletedIndex = i;
                }

                GUILayout.EndHorizontal();
            }

            Debug.Log(log);
            if (deletedIndex >= 0) {
                objects.RemoveAt(deletedIndex);
            }
        }

        private static void DeleteFile(Object obj) {
            var path = AssetDatabase.GetAssetPath(obj);
            var metaPath = path + ".meta";
            FileUtil.DeleteFileOrDirectory(path);
            FileUtil.DeleteFileOrDirectory(metaPath);
        }

        private void Initialize() {
            var sw = new Stopwatch();
            sw.Start();

            string bundlesPath = Directory.GetCurrentDirectory() + "/Assets/Bundles";

            var searchPatternList = new List<string> { "*.asset.meta", "*.mat.meta", "*.prefab.meta", "*.png.meta", "*.fbx.meta", "*.mesh.meta", "*.shader.meta", "*.shadergraph.meta", "*.anim.meta", "*.playable.meta", "*..shadervariants.meta" };
            var listOfGuidsPerType = new List<string[]>();

            // Extract guids of all assets
            foreach (var searchPattern in searchPatternList) {
                listOfGuidsPerType.Add(AssetFinderUtils.GetAllGuidsForAssetType(bundlesPath, searchPattern));
            }

            var searchPatterns = new List<string> { "*.unity", "*.prefab", "*.asset", "*.mat", "*.playable", "*.controller", "*.overrideController", "*.shadervariants" };

            var locker = new object();
            var filePathsPerSearchPattern = new List<string[]>(searchPatterns.Count);
            Parallel.For(0, searchPatterns.Count, i => {
                var res = Directory.GetFiles(bundlesPath, searchPatterns[i], SearchOption.AllDirectories);
                lock (locker) {
                    filePathsPerSearchPattern.Add(res);
                }
            });

            // Extract all guid references from all serialized assets
            var allGuidReferences = AssetFinderUtils.CollectAllGuidReferences(filePathsPerSearchPattern, locker);

            sw.Stop();
            Debug.Log("Time spent: " + sw.Elapsed.TotalSeconds);

            _unusedObjectReferencesByType = new List<List<Object>>();
            foreach (var guids in listOfGuidsPerType) {
                _unusedObjectReferencesByType.Add(AssetFinderUtils.GetAllUnusedObjects(guids, allGuidReferences));
            }
        }

        private void OnGUI() {
            GUILayout.BeginVertical();

            GUILayout.Space(40);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, false);

            var alignment = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;

            for (int i = 0; i < _unusedObjectReferencesByType.Count; i++) {
                RenderList(_unusedObjectReferencesByType[i]);
            }

            GUI.skin.button.alignment = alignment;

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void OnDestroy() {
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

    }

}
