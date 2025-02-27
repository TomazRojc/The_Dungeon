using System;
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

		private bool _UIActive;

		private void Start()
		{
			_gameplaySession = Main.GameplaySession;
			
			Main.UiManager.OnNavigate += HandleNavigate;
			Main.UiManager.OnSubmit += HandleSubmit;
			Main.UiManager.OnCancel += HandleCancel;
			Main.UiManager.OnEscape += HandleEscape;

			_UIActive = true;
			GoToMainMenu();
			_UICamera.SetActive(true);
		}
		
		private void Update()
		{
			_timer.Update(Time.deltaTime);
		}
		

		private void HandleNavigate(Direction inputDirection, int inputIndex)
		{
			if (!_UIActive) return;

			if (_lobby.NumJoinedPlayers == 0)
			{
				TryJoinPlayer(inputIndex);
			}
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

		private bool TryJoinPlayer(int inputIndex)
		{
			if (_lobby.TryJoinPlayer(inputIndex))
			{
				if (_lobby.IsActive) HandleSelectedButtonsOnPlayerJoined(inputIndex);
				return true;
			}
			return false;
		}

		private void HandleSubmit(int inputIndex)
		{
			if (!_UIActive) return;

			if (TryJoinPlayer(inputIndex)) return;
			
			var currentButton = _currentState.GetCurrentlySelectedButton(inputIndex);
			
			if (currentButton == null) return;

			if (!_currentState.HasButtonAuthority(inputIndex, currentButton)) return;

			currentButton.OnSubmit();
		}

		private void HandleSelectedButtonsOnPlayerJoined(int inputIndex)
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

			_currentState.ResetPlayerInControl();
			
			if (oldButton != null)
			{
				oldButton.OnDeselect();
			}
			newButton.OnSelect();
		}

		private void HandleCancel(int inputIndex)
		{
			if (!_UIActive) return;
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

			if (newState != null)
			{
				newState.OnEnter();
			}
			else
			{
				_UIActive = false;
			}

			_currentState = newState;
			
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
			// TODO JanR: move this
			_lobby.OnStartGame();
		}

		public void StartLevel(int levelNumber)
		{
			ChangeState(null);
			
			_loadingPanel.SetActive(true);
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