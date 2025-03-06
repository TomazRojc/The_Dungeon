using System;
using System.Collections.Generic;
using Code.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Code.UI.UiStates {
    public class LobbyUIState : MultiplayerUIState {

	    [Serializable]
	    private struct LobbyPlayerData {
		    public TMP_Text playerNameText;
		    public TMP_Text playerReadyText;
		    public Image playerAvatar;
		    public GameObject playerReadyButton;
	    }
	    
        [SerializeField]
        private List<LobbyPlayerData> playersData;
        
        [SerializeField]
        private ButtonBase startGameButton;
        
        [SerializeField]
        private List<Color> defaultPlayerColors;
        [SerializeField]
        private PlayerInputManager _playerInputManager;

        private GameplaySession _gameplaySession;
        private List<PlayerData> _players;
        
		
        public int NumJoinedPlayers => _gameplaySession.LobbyIndexToPlayerData.Count;
        
        private List<bool> _emptyLobbyIndices = new List<bool>(4);

        private void Awake()
        {
	        _gameplaySession = Main.GameplaySession;
	        _players = Main.GameplaySession.PlayersData;
            _emptyLobbyIndices = new List<bool> { true, true, true, true };
        }
        
        public override void OnEnter() {
            base.OnEnter();
            _playerInputManager.EnableJoining();

            UpdateDisplay();
            PlayerConnections.OnPlayerLeft += OnPlayerLeft;
        }
        
        public override void OnExit() {
            base.OnExit();
            
            _playerInputManager.DisableJoining();
            ResetPlayersReady();
            PlayerConnections.OnPlayerLeft -= OnPlayerLeft;
        }

        private void UpdateDisplay()
        {
            HandleReadyToStart(_players);

            ResetEmptyIndicesList();
            
            for (int i = 0; i < _players.Count; i++)
            {
                if (!_players[i].IsJoined) continue;

                var lobbyIndex = _players[i].LobbyIndex;

                _emptyLobbyIndices[lobbyIndex] = false;

                playersData[lobbyIndex].playerNameText.text = _players[i].DisplayName;
                playersData[lobbyIndex].playerReadyText.text = _players[i].IsReady
                    ? "<color=green>Ready</color>"
                    : "<color=red>Not Ready</color>";
                playersData[lobbyIndex].playerAvatar.gameObject.SetActive(true);
                playersData[lobbyIndex].playerAvatar.color = _players[i].Color;
                playersData[lobbyIndex].playerReadyButton.SetActive(true);
            }
            
            for (int i = 0; i < playersData.Count; i++)
            {
                if (!_emptyLobbyIndices[i]) continue;

                playersData[i].playerNameText.text = "Press <b>\u25a1</b> or F to join...";
                playersData[i].playerReadyText.text = string.Empty;
                playersData[i].playerAvatar.gameObject.SetActive(false);
                playersData[i].playerReadyButton.SetActive(false);
            }
        }

        private void ResetEmptyIndicesList()
        {
            for (int i = 0; i < 4; i++)
            {
                _emptyLobbyIndices[i] = true;
            }
        }

        public void HandleReadyToStart(List<PlayerData> players)
        {
            var allReady = true;
            var lobbyEmpty = true;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].IsJoined)
                {
                    lobbyEmpty = false;
                    if (!players[i].IsReady)
                    {
                        allReady = false;
                    }
                }
            }

            startGameButton.SetInteractable(allReady && !lobbyEmpty);
        }
        
        public bool TryJoinPlayer(int inputIndex) {
        
        	var playerData = _gameplaySession.GetPlayerData(inputIndex);
        	if (playerData.IsJoined) return false;

        	var lobbyIndex = GetFirstFreePanelIndex();
        	if (lobbyIndex == Int32.MaxValue)
        	{
        		Debug.LogWarning("Trying to join player but lobby is full.");
        		return false;
        	}

        	playerData.SetValues($"Player {lobbyIndex + 1}", defaultPlayerColors[lobbyIndex], true, false, lobbyIndex, inputIndex);
	        _gameplaySession.LobbyIndexToPlayerData.Add(lobbyIndex, playerData);
        	UpdateDisplay();
        	
        	return true;
        }

        private void OnPlayerLeft()
        {
        	UpdateDisplay();
        }

        private int GetFirstFreePanelIndex()
        {
        	var firstFreeIndex = -1;
        	for (int i = 0; i < 4; i++)
        	{
        		firstFreeIndex = i;
        		foreach (var player in _players)
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

        private void ResetPlayersReady()
        {
        	for (int i = 0; i < _players.Count; i++)
        	{
        		_players[i].IsReady = false;
        	}

        	UpdateDisplay();
        }

        public void OnPlayerReady(int lobbyIndex)
        {
	        _gameplaySession.LobbyIndexToPlayerData[lobbyIndex].IsReady = !_gameplaySession.LobbyIndexToPlayerData[lobbyIndex].IsReady;
	        UpdateDisplay();
        }

        public void OnStartGame()
        {
        	_gameplaySession.OnEnterLevelsGameplay(Main.Instance.GameplayConfig.PlayerPrefab);
        }
    }
}