using System;
using System.Collections.Generic;
using Code.Gameplay;
using Code.Utils;
using UnityEngine;

namespace Code.UI
{

	public class UIController : MonoBehaviour
	{
		[SerializeField]
		private GameObject _UICamera;

		[SerializeField]
		private Lobby _lobby;

		[Header("UIs")]
		[SerializeField]
		private StateUI _mainMenuPanelState;
		[SerializeField]
		private StateUI _settingsPanelState;
		[SerializeField]
		private StateUI _lobbyPanelState;
		[SerializeField]
		private StateUI _levelsPanelState;
		[SerializeField]
		private GameObject _loadingPanel;
		[SerializeField]
		private GameObject _levelsPanel;

		private GameplaySession _gameplaySession;
		
		private StateUI _currentState;

		private SimpleTimer _timer;

		private void Start()
		{
			_gameplaySession = Main.GameplaySession;
			
			Main.UiManager.OnNavigate += HandleNavigate;
			Main.UiManager.OnSubmit += HandleSubmit;
			Main.UiManager.OnCancel += HandleCancel;
			Main.UiManager.OnEscape += HandleEscape;

			GoToMainMenu();
			_UICamera.SetActive(true);
		}
		
		private void Update()
		{
			_timer.Update(Time.deltaTime);
		}
		

		private void HandleNavigate(Direction inputDirection, int inputIndex)
		{
			var currentButton = _currentState.GetCurrentlySelectedButton(inputIndex);
			
			if (currentButton == null) return;

			var nextButtons = currentButton.GetNextButtons(inputDirection);
			if (nextButtons.Count == 0) return;

			var nextButton = _currentState.GetButtonWithAuthority(inputIndex, nextButtons);
			if (nextButton == null) return;

			if (nextButton.IsSharedButton)
			{
				_currentState.SetPlayerInControl(inputIndex);
			}

			if (currentButton.IsSharedButton && !nextButton.IsSharedButton)
			{
				_currentState.ResetPlayerInControl();
			}
			
			_currentState.SetCurrentlySelectedButton(inputIndex, nextButton);
			currentButton.OnDeselect();
			nextButton.OnSelect();
		}
        
		private void HandleSubmit(int inputIndex)
		{
			if (_lobby.TryJoinPlayer(inputIndex))
			{
				HandlePlayerJoined(inputIndex);
				return;
			}
			var currentButton = _currentState.GetCurrentlySelectedButton(inputIndex);
			
			if (currentButton == null) return;

			if (!_currentState.HasButtonAuthority(inputIndex, currentButton)) return;

			currentButton.OnSubmit();
		}

		private void HandlePlayerJoined(int inputIndex)
		{
			var playerData = _gameplaySession.GetPlayerData(inputIndex);
			if (playerData == null) return;

			var lobbyIndex = playerData.LobbyIndex;
			if (lobbyIndex == -1)
			{
				throw new ArgumentException("Player not joined!");
			}
			var oldButton = _currentState.GetCurrentlySelectedButton(inputIndex);
			var newButton = _currentState.SelectPlayerSpecificButton(inputIndex, lobbyIndex);

			if (oldButton != null)
			{
				oldButton.OnDeselect();
			}
			newButton.OnSelect();
		}

		private void HandleCancel(int inputIndex)
		{
			throw new NotImplementedException();
		}
        
		private void HandleEscape(int inputIndex)
		{
			throw new NotImplementedException();
		}
		
		private void ChangeState(StateUI newState)
		{
			if (_currentState == newState) return;

			if (_currentState != null)
			{
				_currentState.OnExit();
			}
			_currentState = newState;
			newState.OnEnter();
			
			// TODO JanR: move this
			if (_currentState == _lobbyPanelState) _lobby.OnExit();
			if (newState == _lobbyPanelState) _lobby.OnEnter();
		}
		
		public void GoToLobby()
		{
			ChangeState(_lobbyPanelState);
		}

		public void GoToLevelSelection()
		{
			ChangeState(_levelsPanelState);
		}

		public void StartLevel(int levelNumber)
		{
			_loadingPanel.SetActive(true);
			_levelsPanel.SetActive(false);
			_timer.OnComplete += () =>
			{
				_loadingPanel.SetActive(false);
			};
			_timer.Start(0.7f);
			
			Main.LevelManager.StartLevel(levelNumber-1);
		}

		public void GoToSettings()
		{
			ChangeState(_settingsPanelState);
		}

		public void GoToMainMenu()
		{
			ChangeState(_mainMenuPanelState);
		}

		public void ExitGame()
		{
			Application.Quit();
		}
	}
}