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
		private UIController uiController;
		[SerializeField]
		private List<Color> defaultPlayerColors;
		[SerializeField]
		private PlayerInputManager _playerInputManager;

		private GameplaySession _gameplaySession;
		private List<PlayerData> _players;
		private bool _active;
		
		private void Awake()
		{
			_gameplaySession = Main.GameplaySession;
			_players = Main.GameplaySession.PlayersData;
		}

		public void OnEnter()
		{
			_active = true;

			_playerInputManager.EnableJoining();


			_lobbyUI.UpdateDisplay(_players);
		}

		public void OnExit()
		{
			_playerInputManager.DisableJoining();
			ResetPlayersReady();
			_active = false;
		}

		public void TryJoinPlayer(int inputIndex)
		{
			if (!_active) return;

			var playerDataIdx = GetPlayerDataIdx(inputIndex);
			if (_players[playerDataIdx].IsJoined) return;

			var lobbyIndex = GetFirstFreePanelIndex();
			if (lobbyIndex == Int32.MaxValue)
			{
				Debug.LogWarning("Trying to join player but lobby is full.");
				return;
			}

			_players[playerDataIdx].SetValues($"Player {lobbyIndex + 1}", defaultPlayerColors[lobbyIndex], true, false, lobbyIndex, inputIndex);
			_lobbyUI.UpdateDisplay(_players);
		}

		public void OnPlayerLeft()
		{
			if (_active)
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

		private int GetPlayerDataIdx(int inputIndex)
		{
			for (int i = 0; i < _players.Count; i++)
			{
				if (_players[i].InputIndex == inputIndex)
				{
					return i;
				}
			}
			return -1;
		}

		private void ResetPlayersReady()
		{
			for (int i = 0; i < _players.Count; i++)
			{
				_players[i].IsReady = false;
			}

			if (_active)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
		}

		public void OnPlayerReady(int buttonIndex)
		{
			_players[buttonIndex].IsReady = !_players[buttonIndex].IsReady;
			if (_active)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
		}

		public void GoToLobby()
		{
			uiController.GoToLobby();
			OnEnter();
		}
		
		public void StartGame()
		{
			_gameplaySession.OnEnterLevelsGameplay(Main.Instance.GameplayConfig.PlayerPrefab);
			uiController.GoToLevelSelection();
			OnExit();
		}

		public void BackToMainMenu()
		{
			uiController.GoBackToMainMenu();
			OnExit();
		}
	}
}