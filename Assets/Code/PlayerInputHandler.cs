using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerController _player;
        
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private int _inputIndex;

        public int InputIndex => _inputIndex;

        public Action<int> onDeviceLost;
        public Action<int> onDeviceRegained;

        private void Awake()
        {
            _inputIndex = GetComponent<PlayerInput>().playerIndex;
        }

        void Update()
        {
            if (_player == null) return;
            
            // invoke player movement/look
            _player.HandleMoveInput(_moveInput);
            _player.HandleLookInput(_lookInput);
        }

        public void ConnectPlayerController(PlayerController player)
        {
            _player = player;
        }

        // WASD or Left Stick
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        // Arrows or Right Stick
        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }

        // Q or North Button
        public void OnDropItem(InputAction.CallbackContext context)
        {
            if (_player == null) return;

            if (context.started)
            {
                Debug.Log("Drop Item started");
            }

            if (context.canceled)
            {
                Debug.Log("Drop Item ended");
            }
        }

        // E or West Button
        public void OnUseItem(InputAction.CallbackContext context)
        {
            if (_player == null) return;

            if (context.started)
            {
                Debug.Log("Use Item started");
            }

            if (context.canceled)
            {
                Debug.Log("Use Item ended");
            }
        }

        // Ctrl or East Button
        public void OnDash(InputAction.CallbackContext context)
        {
            if (_player == null) return;

            if (context.started)
            {
                _player.HandleDashInput();
            }
        }

        // Space or South Button
        public void OnJump(InputAction.CallbackContext context)
        {
            if (_player == null) return;

            if (context.started)
            {
                _player.HandleJumpInput();
            }
        }

        public void OnDeviceLost(PlayerInput playerInput)
        {
            onDeviceLost?.Invoke(playerInput.playerIndex);
        }

        public void OnDeviceRegained(PlayerInput playerInput)
        {
            onDeviceRegained?.Invoke(playerInput.playerIndex);
        }
    }
}