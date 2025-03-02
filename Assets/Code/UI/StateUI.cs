using System;
using System.Collections.Generic;
using Code.Gameplay;
using UnityEngine;

namespace Code.UI
{
    public class StateUI : MonoBehaviour
    {
        [SerializeField]
        private ButtonBase _defaultButton;

        [SerializeField]
        private List<ButtonBase> _defaultPlayerButtons;

        private GameplaySession _gameplaySession;
        
        private Dictionary<int, ButtonBase> _inputIndexToSelectedButton = new Dictionary<int, ButtonBase>(4);
        
        private int _inputIndexInControl;
        
        public ButtonBase DefaultButton => _defaultButton;

        private void Awake()
        {
            _gameplaySession = Main.GameplaySession;
        }

        public void OnEnter()
        {
            gameObject.SetActive(true);

            _inputIndexToSelectedButton.Clear();
            _inputIndexInControl = -1;

            foreach (var playerData in _gameplaySession.PlayersData)
            {
                ButtonBase button = null;
                // first try to assign a player specific button to the player
                if (playerData.IsJoined && playerData.LobbyIndex < _defaultPlayerButtons.Count)
                {
                    button = _defaultPlayerButtons[playerData.LobbyIndex];
                }

                if (button == null)
                {
                    if (_defaultButton == null)
                    {
                        throw new ArgumentException($"There are no possible player specific buttons to select and '_firstSelectedButton' is not set on UI state: {name}");
                    }
                    button = _defaultButton;
                }

                _inputIndexToSelectedButton[playerData.InputIndex] = button;
                button.OnSelect();
            }
        }
        
        public void OnExit()
        {
            gameObject.SetActive(false);
        }

        public void ResetPlayerInControl()
        {
            SetPlayerInControl(-1);
        }
        
        public void SetPlayerInControl(int inputIndex)
        {
            _inputIndexInControl = inputIndex;
        }

        public ButtonBase GetCurrentlySelectedButton(int inputIndex)
        {
            _inputIndexToSelectedButton.TryGetValue(inputIndex, out var button);
            return button;
        }

        public void SetCurrentlySelectedButton(int inputIndex, ButtonBase button)
        {
            _inputIndexToSelectedButton[inputIndex] = button;
        }

        public ButtonBase SelectPlayerSpecificButton(int inputIndex, int lobbyIndex)
        {
            var button = _defaultPlayerButtons[lobbyIndex];
            if (button == null)
            {
                throw new ArgumentException($"Player specific button not linked in {name}");
            }

            SetCurrentlySelectedButton(inputIndex, button);
            return button;
        }

        public bool HasButtonAuthority(int inputIndex, ButtonBase button)
        {
            if (!button.IsSharedButton)
            {
                var lobbyIndex = _gameplaySession.GetPlayerData(inputIndex).LobbyIndex;
                return lobbyIndex == button.LobbyIndex;
            }
            return _inputIndexInControl == -1 || _inputIndexInControl == inputIndex;
        }
        public ButtonBase GetButtonWithAuthority(int inputIndex, List<ButtonBase> buttons)
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
    }
}