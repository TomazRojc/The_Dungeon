using System;
using System.Collections.Generic;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{
	public class Lobby : MonoBehaviour
	{
		[SerializeField]
		private LobbyUI _lobbyUI;
		[SerializeField]
		private MainMenu _mainMenu;
		[SerializeField]
		private List<Color> defaultPlayerColors;
		[SerializeField]
		private PlayerInputManager _playerInputManager;

		private GameplaySession GameplaySession;
		private List<PlayerData> _players;
		private bool _active;
		
		public bool Active => _active;

		private void Awake()
		{
			GameplaySession = Main.Instance.GameplaySession;
		}

		public void OnEnter()
		{
			_active = true;

			_playerInputManager.EnableJoining();

			if (_players == null)
			{
				_players = Main.Instance.GameplaySession.PlayersData;
			}

			_lobbyUI.UpdateDisplay(_players);
		}

		public void OnExit()
		{
			_playerInputManager.DisableJoining();
			ResetPlayersReady();
			_active = false;
		}

		public void OnPlayerJoined(int playerInputIndex)
		{
			var firstFreeIdx = -1;
			for (int i = 0; i < _players.Count; i++)
			{
				if (firstFreeIdx == -1 && !_players[i].IsJoined)
				{
					firstFreeIdx = i;
				}

				// player with this input index was already joined before
				if (_players[i].InputIndex == playerInputIndex)
				{
					_players[i].IsJoined = true;
					return;
				}
			}

			if (firstFreeIdx == -1)
			{
				throw new ArgumentException("Cannot join a player in a full lobby");
			}

			// new player joined
			_players[firstFreeIdx].SetValues($"Player {firstFreeIdx + 1}", defaultPlayerColors[firstFreeIdx], true,
				false, playerInputIndex);
			
			if (_active)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
		}

		public void OnPlayerLeft(int playerInputIndex)
		{
			for (var i = 0; i < _players.Count; i++)
			{
				if (_players[i].InputIndex == playerInputIndex)
				{
					_players[i].IsJoined = false;
					_players[i].IsReady = false;
					break;
				}
			}

			if (_active)
			{
				_lobbyUI.UpdateDisplay(_players);
			}
		}

		public void ResetPlayersReady()
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
		
		public void StartGame()
		{
			GameplaySession.OnEnterLevelsGameplay(Main.Instance.GameplayConfig.PlayerPrefab);
			_mainMenu.GoToLevelSelection();
			OnExit();
		}

		public void BackToMainMenu()
		{
			_mainMenu.GoBackToMainMenu();
			OnExit();
		}
	}
}