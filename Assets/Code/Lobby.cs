using System;
using System.Collections.Generic;
using Code.Gameplay;
using Code.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{
	public class Lobby : MonoBehaviour
	{
		[SerializeField]
		private LobbyUI _lobbyUI;
		[SerializeField]
		private List<Color> defaultPlayerColors;
		[SerializeField]
		private PlayerInputManager _playerInputManager;

		private bool _isActive;
		private GameplaySession _gameplaySession;
		private List<PlayerData> _players;
		private Dictionary<int, PlayerData> _lobbyIndexToPlayerData = new Dictionary<int, PlayerData>(4);
		
		public int NumJoinedPlayers => _lobbyIndexToPlayerData.Count;
		
		public bool IsActive => _isActive;
		
		private void Awake()
		{
			_gameplaySession = Main.GameplaySession;
			_players = Main.GameplaySession.PlayersData;
		}

		public void OnEnter()
		{
			_isActive = true;

			_playerInputManager.EnableJoining();

			_lobbyUI.UpdateDisplay(_players);
		}

		public void OnExit()
		{
			_playerInputManager.DisableJoining();
			ResetPlayersReady();
			_isActive = false;
		}

		public bool TryJoinPlayer(int inputIndex)
		{

			var playerData = _gameplaySession.GetPlayerData(inputIndex);
			if (playerData.IsJoined) return false;

			var lobbyIndex = GetFirstFreePanelIndex();
			if (lobbyIndex == Int32.MaxValue)
			{
				Debug.LogWarning("Trying to join player but lobby is full.");
				return false;
			}

			playerData.SetValues($"Player {lobbyIndex + 1}", defaultPlayerColors[lobbyIndex], true, false, lobbyIndex, inputIndex);
			_lobbyIndexToPlayerData.Add(lobbyIndex, playerData);
			if (_isActive)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
			return true;
		}

		public void OnPlayerLeft(int inputIndex)
		{
			var lobbyIndex = _gameplaySession.GetPlayerData(inputIndex).LobbyIndex;
			_lobbyIndexToPlayerData.Remove(lobbyIndex);
			if (_isActive)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
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

			if (_isActive)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
		}

		public void OnPlayerReady(int lobbyIndex)
		{
			_lobbyIndexToPlayerData[lobbyIndex].IsReady = !_lobbyIndexToPlayerData[lobbyIndex].IsReady;
			if (_isActive)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
		}

		public void OnStartGame()
		{
			_gameplaySession.OnEnterLevelsGameplay(Main.Instance.GameplayConfig.PlayerPrefab);
		}

	}
}