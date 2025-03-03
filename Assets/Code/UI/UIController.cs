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

		private static bool _UIActive;
		
		private int _inputIndexInControl;
		
		private Dictionary<int, ButtonBase> _inputIndexToSelectedButton = new Dictionary<int, ButtonBase>(4);

		private SimpleTimer _timer;
		
		public static bool UIActive => _UIActive;

		private void Start()
		{
			_gameplaySession = Main.GameplaySession;
			
			Main.UiEventBus.OnNavigate += HandleNavigate;
			Main.UiEventBus.OnSubmit += HandleSubmit;
			Main.UiEventBus.OnJoinLobby += HandleJoinLobby;
			Main.UiEventBus.OnCancel += HandleCancel;
			Main.UiEventBus.OnEscape += HandleEscape;

			GoToMainMenu();
		}
		
		private void Update()
		{
			_timer.Update(Time.deltaTime);
		}
		
		private void ToggleUI()
		{
			var turnOn = !_UIActive;
			_UIActive = turnOn;
			_UICamera.SetActive(turnOn);
		}

		private void HandleNavigate(Direction inputDirection, int inputIndex)
		{
			if (!_UIActive) return;

			_inputIndexToSelectedButton.TryGetValue(inputIndex, out var currentButton);

			if (currentButton == null)
			{
				currentButton = _currentState.DefaultButton;
				if (currentButton == null) throw new ArgumentException($"Player button not selected and DefaultButton not set on UI state {_currentState.name}");
			}

			var nextButtons = currentButton.GetNextButtons(inputDirection);
			if (nextButtons.Count == 0) return;

			var nextButton = GetButtonWithAuthority(inputIndex, nextButtons);
			if (nextButton == null) return;

			if (nextButton.IsSharedButton)
			{
				_inputIndexInControl = inputIndex;
			}

			if (currentButton.IsSharedButton && !nextButton.IsSharedButton)
			{
				_inputIndexInControl = -1;
			}
			
			_inputIndexToSelectedButton[inputIndex] = nextButton;
			
			currentButton.OnDeselect();
			nextButton.OnSelect();
		}

		private void HandleSubmit(int inputIndex)
		{
			if (!_UIActive) return;

			if (_lobby.NumJoinedPlayers == 0)
			{
				TryJoinPlayer(inputIndex);
			}

			_inputIndexToSelectedButton.TryGetValue(inputIndex, out var currentButton);
			
			if (currentButton == null || !currentButton.IsInteractable) return;

			if (!HasButtonAuthority(inputIndex, currentButton)) return;

			currentButton.OnSubmit();
		}

		private void HandleJoinLobby(int inputIndex)
		{
			if (!_UIActive) return;
			
			// TODO: JanR handle de-joining?
			TryJoinPlayer(inputIndex);
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
			
			if (_currentState == null || newState == null)
			{
				ToggleUI();
			}

			// TODO JanR: move this
			if (_currentState == _lobbyPanelState) _lobby.OnExit();
			if (newState == _lobbyPanelState) _lobby.OnEnter();

			ResetSelectedButtons(newState);

			_currentState = newState;
		}


		private bool TryJoinPlayer(int inputIndex)
		{
			if (!_lobby.TryJoinPlayer(inputIndex)) return false;

			if (_currentState == _lobbyPanelState)
			{
				UpdateSelectedButtonsOnPlayerJoined(inputIndex);
			}
			
			return true;
		}

		private void UpdateSelectedButtonsOnPlayerJoined(int inputIndex)
		{
			var playerData = _gameplaySession.GetPlayerData(inputIndex);

			var lobbyIndex = playerData.LobbyIndex;
			if (lobbyIndex == -1)
			{
				throw new ArgumentException("Player not joined!");
			}

			if (_inputIndexInControl == inputIndex)
			{
				_inputIndexInControl = -1;
			}

			// current button
			_inputIndexToSelectedButton.TryGetValue(inputIndex, out var currentButton);

			// next button
			var nextButton = _currentState.DefaultPlayerButtons[lobbyIndex];
			if (nextButton == null)
			{
				throw new ArgumentException($"Player specific button not linked in {_currentState.name}");
			}

			_inputIndexToSelectedButton[inputIndex] = nextButton;
			if (currentButton != null)
			{
				currentButton.OnDeselect();
			}
			nextButton.OnSelect();
		}

		private void ResetSelectedButtons(StateUI state)
		{
			if (state == null) return;
			
			_inputIndexToSelectedButton.Clear();
			_inputIndexInControl = -1;

			foreach (var playerData in _gameplaySession.PlayersData)
			{
				ButtonBase button = null;
				// first try to assign a player specific button to the player
				if (playerData.IsJoined && playerData.LobbyIndex < state.DefaultPlayerButtons.Count)
				{
					button = state.DefaultPlayerButtons[playerData.LobbyIndex];
				}

				if (button == null)
				{
					if (state.DefaultButton == null)
					{
						throw new ArgumentException($"There are no possible player specific buttons to select and '_firstSelectedButton' is not set on UI state: {state.name}");
					}
					button = state.DefaultButton;
				}

				_inputIndexToSelectedButton[playerData.InputIndex] = button;
				button.OnSelect();
			}
		}

		private bool HasButtonAuthority(int inputIndex, ButtonBase button)
		{
			if (!button.IsInteractable) return false;

			if (!button.IsSharedButton)
			{
				var lobbyIndex = _gameplaySession.GetPlayerData(inputIndex).LobbyIndex;
				return lobbyIndex == button.LobbyIndex;
			}
			return _inputIndexInControl == -1 || _inputIndexInControl == inputIndex;
		}
		private ButtonBase GetButtonWithAuthority(int inputIndex, List<ButtonBase> buttons)
		{
			foreach (var button in buttons)
			{
				if (HasButtonAuthority(inputIndex, button))
				{
					return button;
				}
			}
			return null;
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