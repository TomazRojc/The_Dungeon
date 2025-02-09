using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheDungeon.AssetReferenceFinder {

    public class FindAllReferencesModel {

        private static readonly List<string> s_searchPatterns = new List<string> { "*.unity", "*.prefab", "*.asset", "*.mat", "*.playable", "*.controller", "*.overrideController", "*.shadervariants" };

        public long TimeSpent { get; set; }

        public List<FindAllReferencesWindowResult> Results { get; } = new List<FindAllReferencesWindowResult>(5);

        public bool HasAnyUnusedResult { get; }
        
        public FindAllReferencesModel() : this(Selection.objects) { }

        public FindAllReferencesModel(Object[] objects) {
            var stopwatch = Stopwatch.StartNew();

            var currentDirectory = Directory.GetCurrentDirectory();
            var searchPath = Path.Combine(currentDirectory, "Assets");

            foreach (var selectedObject in objects) {
                var result = GetResult(selectedObject, searchPath);
                Results.Add(result);

                if (result.Usages.Count == 0) {
                    HasAnyUnusedResult = true;
                }
            }

            Results.Sort((a, b) => string.Compare(Path.GetFileNameWithoutExtension(a.Target.AssetPath), Path.GetFileNameWithoutExtension(b.Target.AssetPath), StringComparison.Ordinal));

            TimeSpent = stopwatch.ElapsedMilliseconds;
        }

        private static FindAllReferencesWindowResult GetResult(Object selectedObject, string searchPath) {
            var result = new FindAllReferencesWindowResult();

            var assetPath = AssetDatabase.GetAssetPath(selectedObject);

            result.Target = new FindAllReferencesWindowAsset {
                AssetPath = assetPath,
                Asset = selectedObject
            };

            var searchString = GetSearchString(selectedObject, assetPath);
            var files = AssetFinderUtils.GetAllFilesReferencingGivenSearchPattern(searchPath, s_searchPatterns, searchString);
            var usages = files.Select(file => new FindAllReferencesWindowAsset(file.Item1, file.Item2));
            result.Usages.AddRange(usages);

            return result;
        }

        private static string GetSearchString(Object selectedObject, string assetPath) {
            if (selectedObject is Sprite sprite) {
                return GetSpriteFileId(sprite.name, assetPath);
            }

            return AssetDatabase.AssetPathToGUID(assetPath);
        }

        private static string GetSpriteFileId(string spriteName, string atlasFilePath) {
            var atlasMetaFilePath = atlasFilePath + ".meta";

            if (!File.Exists(atlasMetaFilePath)) {
                throw new FileNotFoundException("Unable to find meta file for asset: " + atlasFilePath);
            }

            var atlasMetaFileContents = File.ReadAllText(atlasMetaFilePath);
            var searchString = $"name: {spriteName}";
            var nameIndex = atlasMetaFileContents.IndexOf(searchString, StringComparison.InvariantCulture);
            if (nameIndex < 0) {
                throw new KeyNotFoundException("Unable to find sprite file ID for " + spriteName);
            }

            var internalIdIndex = atlasMetaFileContents.IndexOf("internalID", nameIndex, StringComparison.InvariantCulture);
            if (internalIdIndex < 0) {
                throw new KeyNotFoundException("Unable to find sprite file ID for " + spriteName);
            }

            var newLineIndex = atlasMetaFileContents.IndexOf(Environment.NewLine, internalIdIndex, StringComparison.InvariantCulture);
            if (newLineIndex < 0) {
                throw new KeyNotFoundException("Unable to find sprite file ID for " + spriteName);
            }

            var length = newLineIndex - internalIdIndex;
            var line = atlasMetaFileContents.Substring(internalIdIndex, length);
            var components = line.Split(':');
            if (components.Length != 2) {
                throw new KeyNotFoundException("Unable to find sprite file ID for " + spriteName);
            }

            var internalId = components[1].Trim();
            return internalId;
        }

    }

}
