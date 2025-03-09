using System.Collections.Generic;
using Code.Configs;
using Code.Gameplay;
using UnityEngine;

namespace Code
{
    public class LevelManager
    {
        private GameObject _worldGameObject;
        private List<PlayerController> _playerControllers;
        private LevelsConfig _levelsConfig;
        private GameplayConfig _gameplayConfig;

        public LevelManager(LevelsConfig levelsConfig, GameplayConfig gameplayConfig)
        {
            _levelsConfig = levelsConfig;
            _gameplayConfig = gameplayConfig;
        }
        
        public void OnGameplayEnter(int levelIndex)
        {
            _worldGameObject = new GameObject("3D");
            _worldGameObject.transform.position = new Vector3(0, 0, 0);
            SpawnPlayers(_gameplayConfig.PlayerPrefab);
            
            var level = Object.Instantiate(_levelsConfig.LevelPrefabs[levelIndex], _worldGameObject.transform);
            var camera = Object.Instantiate(_levelsConfig.LevelCameraPrefab, level.transform).GetComponent<CameraFollow>();
            var levelComponent = level.GetComponent<LevelComponent>();
            levelComponent.StartLevel(_playerControllers, camera);
        }
        
        public void OnGameplayExit() {
            DespawnPlayers();
            Object.Destroy(_worldGameObject);
        }

        private void SpawnPlayers(GameObject playerPrefab) {
            _playerControllers = new List<PlayerController>();
            foreach (var playerData in Main.GameplaySession.PlayersData)
            {
                if (!playerData.IsJoined) continue;
                
                var inputHandler = GetInputHandler(playerData.InputIndex);
                var playerController = InstantiatePlayerController(playerPrefab, playerData.Color);
                _playerControllers.Add(playerController);
                inputHandler.ConnectPlayerController(playerController);
            }
        }
        
        private PlayerController InstantiatePlayerController(GameObject playerPrefab, Color color)
        {
            var playerObject = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var playerController = playerObject.GetComponent<PlayerController>();
            playerController.transform.SetParent(_worldGameObject.transform);
            playerController.Init(color);
            return playerController;
        }
        
        private void DespawnPlayers()
        {
            foreach (var playerInputHandler in Main.GameplaySession.PlayerInputHandlers)
            {
                DespawnPlayer(playerInputHandler);                
            }
            _playerControllers.Clear();
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
    }
}