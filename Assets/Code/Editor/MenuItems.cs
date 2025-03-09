using UnityEditor;

namespace Code.Editor
{
    public static class MenuItems
    {
        [MenuItem("Outfit7/Play Mode/Run _F5")]
        public static void Run()
        {
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }
}