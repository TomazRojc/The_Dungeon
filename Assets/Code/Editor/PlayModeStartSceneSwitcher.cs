using UnityEditor;
using UnityEditor.SceneManagement;

namespace Code.Editor {
    
    [InitializeOnLoad]
    public class PlayModeStartSceneSwitcher {
        
        static PlayModeStartSceneSwitcher()
        {
            EditorSceneManager.playModeStartScene = GetPlayModeStartScene();
        }
        
        private static SceneAsset GetPlayModeStartScene()
        {
            var scenes = AssetDatabase.FindAssets("t:scene MainMenu");
            var startupScene = scenes.Length == 1 ? AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(scenes[0])) : null;
            return startupScene;
        }
    }
}