using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Code.AssetReferenceFinder {

    public class FindAllReferencesWindowAsset {
        
        public Object Asset { get; set; }
        
        public string AssetPath { get; set; }

        public int Usages { get; set; }

        public FindAllReferencesWindowAsset(string file, int usages) {
            AssetPath = file.Substring(file.IndexOf("Assets", StringComparison.Ordinal));
            Asset = AssetDatabase.LoadMainAssetAtPath(AssetPath);
            Usages = usages;
        }
        
        public FindAllReferencesWindowAsset() {
        }
        
    }

}
