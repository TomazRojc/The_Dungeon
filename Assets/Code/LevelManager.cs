using System.Collections.Generic;
using Code.Gameplay;
using UnityEngine;

namespace Code
{
    public class LevelManager
    {
        private GameObject _worldGameObject;
        private List<GameObject> _playerGameObjects;
        private List<GameObject> _levelPrefabs;

        public LevelManager(List<GameObject> levelPrefabs)
        {
            _levelPrefabs = levelPrefabs;
        }
        
        public void Init(List<GameObject> playerGameObjects)
        {
            _worldGameObject = new GameObject("3D");
            _worldGameObject.transform.position = new Vector3(2000, 0, 0);
            foreach (var player in playerGameObjects)
            {
                player.transform.SetParent(_worldGameObject.transform);
            }
            _playerGameObjects = playerGameObjects;
        }

        public void StartLevel(int levelIndex)
        {
            var level = Object.Instantiate(_levelPrefabs[levelIndex], _worldGameObject.transform);
            var levelComponent = level.GetComponent<LevelComponent>();
            levelComponent.SpawnPlayers(_playerGameObjects);
        }
    }
}