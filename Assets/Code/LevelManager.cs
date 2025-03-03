using System.Collections.Generic;
using Code.Configs;
using Code.Gameplay;
using UnityEngine;

namespace Code
{
    public class LevelManager
    {
        private GameObject _worldGameObject;
        private List<GameObject> _playerGameObjects;
        private LevelsConfig _levelsConfig;

        public LevelManager(LevelsConfig levelsConfig)
        {
            _levelsConfig = levelsConfig;
        }
        
        public void Init(List<GameObject> playerGameObjects)
        {
            _worldGameObject = new GameObject("3D");
            _worldGameObject.transform.position = new Vector3(0, 0, 0);
            foreach (var player in playerGameObjects)
            {
                player.transform.SetParent(_worldGameObject.transform);
            }
            _playerGameObjects = playerGameObjects;
        }

        public void StartLevel(int levelIndex)
        {
            var level = Object.Instantiate(_levelsConfig.LevelPrefabs[levelIndex], _worldGameObject.transform);
            var camera = Object.Instantiate(_levelsConfig.LevelCameraPrefab, level.transform).GetComponent<CameraFollow>();
            var levelComponent = level.GetComponent<LevelComponent>();
            levelComponent.StartLevel(_playerGameObjects, camera);
        }
    }
}