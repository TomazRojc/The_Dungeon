using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Gameplay
{
    public class GameplaySession
    {
        private List<PlayerInputHandler> _playerInputHandlers;
        private Dictionary<int, PlayerData> _lobbyIndexToPlayerData = new Dictionary<int, PlayerData>(4);
        
        public bool GameLevelActive;
        public List<PlayerController> PlayerControllers;
        public List<PlayerData> PlayersData;
        public Dictionary<int, PlayerData> LobbyIndexToPlayerData => _lobbyIndexToPlayerData;
        
        
        public GameplaySession(int maxPlayers)
        {
            GameLevelActive = false;
            PlayerControllers = new List<PlayerController>(maxPlayers);
            _playerInputHandlers = new List<PlayerInputHandler>(maxPlayers);
            PlayersData = new List<PlayerData>(maxPlayers);
        }

        public void AddPlayerInput(PlayerInputHandler inputHandler)
        {
            _playerInputHandlers.Add(inputHandler);
        }
        
        public void RemovePlayerInput(PlayerInputHandler inputHandler)
        {
            _playerInputHandlers.Remove(inputHandler);
        }
        
        public void AddPlayerData(int inputIndex)
        {
            PlayersData.Add(new PlayerData("Player", Color.white, false, false, -1, inputIndex));
        }
        
        public void RemovePlayerData(int inputIndex)
        {
            for (int i = 0; i < PlayersData.Count; i++)
            {
                if (PlayersData[i].InputIndex == inputIndex)
                {
                    PlayersData.RemoveAt(i);
                    return;
                }
            }
        }

        public void OnEnterLevelsGameplay(GameObject playerPrefab)
        {
            var playerGameObjects = new List<GameObject>();
            foreach (var playerData in PlayersData)
            {
                if (!playerData.IsJoined) continue;
                
                var inputHandler = GetInputHandler(playerData.InputIndex);
                var playerGameObject = SpawnPlayer(playerPrefab, playerData.Color);
                playerGameObjects.Add(playerGameObject);
                
                var playerController = playerGameObject.GetComponent<PlayerController>();
                inputHandler.ConnectPlayerController(playerController);
                PlayerControllers.Add(playerController);
            }
            
            Main.LevelManager.Init(playerGameObjects);
        }

        private GameObject SpawnPlayer(GameObject playerPrefab, Color color)
        {
            var playerObject = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var spriteRenderer = playerObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            return playerObject;
        }

        public void DespawnPlayer(int inputIndex)
        {
            foreach (var playerInputHandler in _playerInputHandlers)
            {
                if (playerInputHandler.InputIndex != inputIndex) continue;
                DespawnPlayer(playerInputHandler);                
            }
        }
        
        public void DespawnPlayer(PlayerInputHandler playerInputHandler)
        {
            var player = playerInputHandler.Player;
            if (playerInputHandler.Player != null)
            {
                Object.Destroy(player);
            }
        }

        private PlayerInputHandler GetInputHandler(int inputIndex)
        {
            foreach (var playerInputHandler in _playerInputHandlers)
            {
                if (playerInputHandler.InputIndex == inputIndex)
                {
                    return playerInputHandler;
                }
            }

            return null;
        }
        
        public PlayerData GetPlayerData(int inputIndex)
        {
            foreach (var playerData in PlayersData)
            {
                if (playerData.InputIndex == inputIndex)
                {
                    return playerData;
                }
            }

            return null;
        }
    }
}