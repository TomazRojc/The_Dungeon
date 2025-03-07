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
        private GameplayConfig _gameplayConfig;

        public LevelManager(LevelsConfig levelsConfig, GameplayConfig gameplayConfig)
        {
            _levelsConfig = levelsConfig;
            _gameplayConfig = gameplayConfig;
        }
        
        private void OnGameplayEnter(GameObject playerPrefab)
        {
            _worldGameObject = new GameObject("3D");
            _worldGameObject.transform.position = new Vector3(0, 0, 0);
            SpawnPlayers(playerPrefab);
        }
        
        public void OnGameplayExit() {
            DespawnPlayers();
            Object.Destroy(_worldGameObject);
        }

        private void SpawnPlayers(GameObject playerPrefab) {
            _playerGameObjects = new List<GameObject>();
            foreach (var playerData in Main.GameplaySession.PlayersData)
            {
                if (!playerData.IsJoined) continue;
                
                var inputHandler = GetInputHandler(playerData.InputIndex);
                var playerGameObject = InstantiatePlayerGameObject(playerPrefab, playerData.Color);
                playerGameObject.transform.SetParent(_worldGameObject.transform);
                _playerGameObjects.Add(playerGameObject);
                
                var playerController = playerGameObject.GetComponent<PlayerController>();
                inputHandler.ConnectPlayerController(playerController);
            }
        }
        
        private GameObject InstantiatePlayerGameObject(GameObject playerPrefab, Color color)
        {
            var playerObject = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var spriteRenderer = playerObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            return playerObject;
        }
        
        private void DespawnPlayers()
        {
            foreach (var playerInputHandler in Main.GameplaySession.PlayerInputHandlers)
            {
                DespawnPlayer(playerInputHandler);                
            }
            _playerGameObjects.Clear();
        }
        
        public void DespawnPlayer(PlayerInputHandler playerInputHandler)
        {
            var player = playerInputHandler.Player;
            if (playerInputHandler.Player != null) {
                playerInputHandler.DisconnectPlayerController();
                Object.Destroy(player);
            }
        }

        private PlayerInputHandler GetInputHandler(int inputIndex)
        {
            foreach (var playerInputHandler in Main.GameplaySession.PlayerInputHandlers)
            {
                if (playerInputHandler.InputIndex == inputIndex)
                {
                    return playerInputHandler;
                }
            }

            return null;
        }

        public void StartLevel(int levelIndex)
        {
            OnGameplayEnter(_gameplayConfig.PlayerPrefab);
            
            var level = Object.Instantiate(_levelsConfig.LevelPrefabs[levelIndex], _worldGameObject.transform);
            var camera = Object.Instantiate(_levelsConfig.LevelCameraPrefab, level.transform).GetComponent<CameraFollow>();
            var levelComponent = level.GetComponent<LevelComponent>();
            levelComponent.StartLevel(_playerGameObjects, camera);
        }
    }
}