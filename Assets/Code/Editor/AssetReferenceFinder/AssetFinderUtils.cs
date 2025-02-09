using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TheDungeon.AssetReferenceFinder {

    internal static class AssetFinderUtils {

        private static readonly string[] s_patterns = { "guid: ", "m_AssetGUID: ", "fileID: " };

        public static List<UnityEngine.Object> GetAllUnusedObjects(string[] assetGuids, HashSet<string> allGuidReferences) {
            var objects = new List<UnityEngine.Object>();
            foreach (var assetGuid in assetGuids) {
                if (!allGuidReferences.Contains(assetGuid)) {
                    var loadedAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(assetGuid));
                    objects.Add(loadedAsset);
                }
            }

            return objects;
        }

        public static HashSet<string> CollectAllGuidReferences(List<string[]> filePathsPerSearchPattern, object locker) {
            var guids = new HashSet<string>();

            void ExtractGuidReferencesFromFiles(string[] filePaths, HashSet<string> hashSet) {
                Parallel.For(0, filePaths.Length, i => {
                    string fileText = File.ReadAllText(filePaths[i]);
                    FillHashSetWithGuidReferencesFromFileContent(hashSet, fileText, "guid: ", locker);
                    FillHashSetWithGuidReferencesFromFileContent(hashSet, fileText, "m_AssetGUID: ", locker);
                });
            }

            foreach (var filePaths in filePathsPerSearchPattern) {
                ExtractGuidReferencesFromFiles(filePaths, guids);
            }

            return guids;
        }

        public static List<Tuple<string, int>> GetAllFilesReferencingGivenSearchPattern(string rootFolderPath, List<string> searchPatterns, string givenSearchPattern) {
            var results = new List<Tuple<string, int>>();

            var filePathsPerSearchPattern = new List<string[]>(20000);

            var locker = new object();

            Parallel.ForEach(searchPatterns, searchPattern => {
                var res = Directory.GetFiles(rootFolderPath, searchPattern, SearchOption.AllDirectories);
                lock (locker) {
                    filePathsPerSearchPattern.Add(res);
                }
            });

            void ExtractAllFilesWithReferencesToGivenGuid(string[] filePaths) {
                Parallel.ForEach(filePaths, filePath => {
                    string fileText = File.ReadAllText(filePath);
                    int amount = GetAmountOfSearchPatternReferenceInFileContent(fileText, givenSearchPattern);
                    if (amount > 0) {
                        lock (locker) {
                            results.Add(new Tuple<string, int>(filePath, amount));
                        }
                    }
                });
            }

            foreach (var filePaths in filePathsPerSearchPattern) {
                ExtractAllFilesWithReferencesToGivenGuid(filePaths);
            }

            return results;
        }

        private static void FillHashSetWithGuidReferencesFromFileContent(HashSet<string> hashSet, string fileContent, string guidTagPattern, object locker) {
            int index = 0;

            while (true) {
                index = fileContent.IndexOf(guidTagPattern, index, StringComparison.Ordinal);
                if (index < 0) {
                    return;
                }

                //Null reference case
                if (fileContent[index + guidTagPattern.Length] == '\n' || fileContent[index + guidTagPattern.Length] == '\r') {
                    index += guidTagPattern.Length;
                    continue;
                }

                string guid = fileContent.Substring(index + guidTagPattern.Length, 32);

                lock (locker) {
                    hashSet.Add(guid);
                }

                index = index + guidTagPattern.Length + 32;
            }
        }

        private static int GetAmountOfSearchPatternReferenceInFileContent(string fileContent, string guidReference) {
            int index = 0;
            int amount = 0;

            while (true) {
                index = fileContent.IndexOf(guidReference, index, StringComparison.Ordinal);
                if (index < 0) {
                    break;
                }

                foreach (var pattern in s_patterns) {
                    var patternMatch = fileContent.Substring(index - pattern.Length, pattern.Length);
                    if (patternMatch == pattern) {
                        amount++;
                        break;
                    }
                }

                index += guidReference.Length;
            }

            return amount;
        }

        public static string[] GetAllGuidsForAssetType(string rootFolderPath, string assetFileSearchPattern) {
            var metaFilePaths = Directory.GetFiles(rootFolderPath, assetFileSearchPattern, SearchOption.AllDirectories);
            var guids = new string[metaFilePaths.Length];

            Parallel.For(0, metaFilePaths.Length, i => {
                guids[i] = GetGuidFromMetaFile(metaFilePaths[i]);
            });

            return guids;
        }

        private static string GetGuidFromMetaFile(string metaFilePath) {
            var lines = File.ReadAllLines(metaFilePath);

            foreach (var line in lines) {
                if (line.StartsWith("guid:")) {
                    return line.Substring(6);
                }
            }

            throw new Exception("Meta file does not contain guid, something's not right...");
        }
        
        public static List<Component> FindReferencesInPrefab(GameObject prefab, UnityEngine.Object asset)
        {
            var components = prefab.GetComponentsInChildren<Component>(true);
            var results = new List<Component>();

            if (asset == null) return results;
            
            foreach (var component in components)
            {
                var serializedObject = new SerializedObject(component);
                var property = serializedObject.GetIterator();
                
                while (property.NextVisible(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    if (property.objectReferenceValue == asset)
                    {
                        results.Add(component);
                    }
                    else if (asset is GameObject)   // prefab or sprite
                    {
                        // Need to access the sprite inside the game object when searching for sprite links
                        var spriteRenderer = ((GameObject)asset).GetComponent<SpriteRenderer>();
                        if (spriteRenderer == null) continue;   // asset is a prefab
                        
                        if (property.objectReferenceValue == spriteRenderer.sprite)
                        {
                            results.Add(component);
                        }
                    }
                }

            }

            return results;
        }

    }

}
