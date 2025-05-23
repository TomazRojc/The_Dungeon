﻿using System;
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
        private GameObject _playerInputPrefab;

        private Dictionary<InputDevice, PlayerInputHandler> deviceToInputHandler = new Dictionary<InputDevice, PlayerInputHandler>(4);
        
        private GameplaySession _gameplaySession;
        private LevelManager _levelManager;
        
        public static event Action<int> OnPlayerLeft;

        private void Awake()
        {
            _gameplaySession = Main.GameplaySession;
            _levelManager = Main.LevelManager;
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
                var lobbyIndex = _gameplaySession.GetPlayerData(playerInputHandler.InputIndex).LobbyIndex;
                deviceToInputHandler.Remove(device);
                _gameplaySession.LobbyIndexToPlayerData.Remove(lobbyIndex);
                _gameplaySession.RemovePlayerData(playerInputHandler.InputIndex);
                _gameplaySession.RemovePlayerInput(playerInputHandler);
                _levelManager.DespawnPlayer(playerInputHandler);
                Destroy(playerInputHandler.gameObject);
                OnPlayerLeft?.Invoke(playerInputHandler.InputIndex);
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