using System.Collections.Generic;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace Code
{

    public class PlayerConnections : MonoBehaviour
    {
        [SerializeField]
        private Lobby lobby;

        [SerializeField]
        private GameObject _playerInputPrefab;

        private Dictionary<InputDevice, PlayerInputHandler> deviceToInputHandler = new Dictionary<InputDevice, PlayerInputHandler>(4);
        
        private GameplaySession _gameplaySession;

        private void Awake()
        {
            _gameplaySession = Main.GameplaySession;
            InputSystem.onDeviceChange += OnDeviceChange;
            AssignExistingDevices();
        }
        
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Removed:
                    RemoveDeviceAndPlayerInput(device);
                    break;
                case InputDeviceChange.Added:
                    AssignDeviceToPlayerInput(device);
                    break;
            }
        }

        private void AssignExistingDevices()
        {
            foreach (var device in InputSystem.devices)
            {
                AssignDeviceToPlayerInput(device);
            }
        }
        
        private void RemoveDeviceAndPlayerInput(InputDevice device)
        {
            if (deviceToInputHandler.TryGetValue(device, out var playerInputHandler))
            {
                deviceToInputHandler.Remove(device);
                _gameplaySession.RemovePlayerData(playerInputHandler.InputIndex);
                lobby.OnPlayerLeft(playerInputHandler.InputIndex);
                _gameplaySession.RemovePlayerInput(playerInputHandler);
                _gameplaySession.DespawnPlayer(playerInputHandler);
                Destroy(playerInputHandler.gameObject);
            }
        }

        private void AssignDeviceToPlayerInput(InputDevice device)
        {
            PlayerInput playerInput = null;
            if (device is Keyboard)
            {
                playerInput = PlayerInput.Instantiate(_playerInputPrefab, controlScheme: "Keyboard&Mouse");
            }
            else if (device is Gamepad)
            {
                playerInput = PlayerInput.Instantiate(_playerInputPrefab, controlScheme: "Gamepad", pairWithDevice: device);
            }

            if (playerInput == null)
            {
                Debug.LogWarning($"Input device {device} not supported");
                return;
            }

            var playerInputHandler = playerInput.GetComponent<PlayerInputHandler>();
            deviceToInputHandler.Add(device, playerInputHandler);
            _gameplaySession.AddPlayerData(playerInputHandler.InputIndex);
            _gameplaySession.AddPlayerInput(playerInputHandler);
        }
    }
}