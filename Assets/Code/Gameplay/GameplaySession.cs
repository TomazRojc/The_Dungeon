using System;
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
        public List<PlayerData> PlayersData;
        public Dictionary<int, PlayerData> LobbyIndexToPlayerData => _lobbyIndexToPlayerData;
        public int NumJoinedPlayers => LobbyIndexToPlayerData.Count;
        public List<PlayerInputHandler> PlayerInputHandlers => _playerInputHandlers;
        
        
        public GameplaySession(int maxPlayers)
        {
            GameLevelActive = false;
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
        
        public bool TryJoinPlayer(int inputIndex) {
        
            var playerData = GetPlayerData(inputIndex);
            if (playerData.IsJoined) return false;

            var lobbyIndex = GetFirstFreePanelIndex();
            if (lobbyIndex == Int32.MaxValue)
            {
                Debug.LogWarning("Trying to join player but lobby is full.");
                return false;
            }

            playerData.SetValues($"Player {lobbyIndex + 1}", Main.Instance.GameplayConfig.DefaultPlayerColors[lobbyIndex], true, false, lobbyIndex, inputIndex);
            LobbyIndexToPlayerData.Add(lobbyIndex, playerData);
            return true;
        }
        
        private int GetFirstFreePanelIndex()
        {
            var firstFreeIndex = -1;
            for (int i = 0; i < 4; i++)
            {
                firstFreeIndex = i;
                foreach (var player in PlayersData)
                {
                    if (player.LobbyIndex == firstFreeIndex)
                    {
                        firstFreeIndex = -1;
                        break;
                    }
                }
                if (firstFreeIndex != -1) return firstFreeIndex;
            }
        	
            return firstFreeIndex;
        }
    }
}