using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private Vector2 _moveInput;
        private Vector2 _lookInput;

        public Action<int> onDeviceLost;
        public Action<int> onDeviceRegained;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        void Update()
        {
            // invoke player movement/look
            Debug.Log("Move: " + _moveInput);
            Debug.Log("Look: " + _lookInput);
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
            if (context.started)
            {
                Debug.Log("Dash started");
            }

            if (context.canceled)
            {
                Debug.Log("Dash ended");
            }
        }

        // Space or South Button
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("Jump started");
            }

            if (context.canceled)
            {
                Debug.Log("Jump ended");
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