using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Gameplay
{
    public class GameplaySession
    {
        private List<PlayerInputHandler> _playerInputHandlers;
        
        public bool GameLevelActive;
        public List<PlayerController> PlayerControllers;
        public List<PlayerData> PlayersData;
        
        
        public GameplaySession(int maxPlayers)
        {
            GameLevelActive = false;
            PlayerControllers = new List<PlayerController>(maxPlayers);
            _playerInputHandlers = new List<PlayerInputHandler>(maxPlayers);
            PlayersData = new List<PlayerData>(maxPlayers);
            for (int i = 0; i < maxPlayers; i++)
            {
                PlayersData.Add(new PlayerData());
            }
        }

        public void AddPlayerInput(PlayerInputHandler inputHandler)
        {
            _playerInputHandlers.Add(inputHandler);
        }
        
        public void RemovePlayerInput(PlayerInputHandler inputHandler)
        {
            _playerInputHandlers.Remove(inputHandler);
        }

        public void OnEnterLevelsGameplay(GameObject playerPrefab)
        {
            var playerGameobjects = new List<GameObject>();
            foreach (var playerData in PlayersData)
            {
                if (!playerData.IsJoined) continue;
                
                var inputHandler = GetInputHandler(playerData.InputIndex);
                var playerGameobject = SpawnPlayer(playerPrefab, playerData.Color);
                playerGameobjects.Add(playerGameobject);
                
                var playerController = playerGameobject.GetComponent<PlayerController>();
                inputHandler.ConnectPlayerController(playerController);
                PlayerControllers.Add(playerController);
            }
            
            Main.Instance.LevelManager.Init(playerGameobjects);
        }

        private GameObject SpawnPlayer(GameObject playerPrefab, Color color)
        {
            var playerObject = Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            var spriteRenderer = playerObject.GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            return playerObject;
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
    }
}