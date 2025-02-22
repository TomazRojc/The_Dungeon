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
        public PlayerController Player => _player;

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

#region UI

        public void OnNavigateUI(InputAction.CallbackContext context) {
            if (context.performed) {
                Main.UiManager.OnNavigate?.Invoke(GetDirection(context.ReadValue<Vector2>()), _inputIndex);
            }
        }

        public void OnSubmitUI(InputAction.CallbackContext context) {
             if (context.performed) {
                Main.UiManager.OnSubmit?.Invoke(_inputIndex);
            }
        }
        
        public void OnBackUI(InputAction.CallbackContext context) {
            if (context.performed) {
                Main.UiManager.OnBack?.Invoke(_inputIndex);
            }
        }
        
        #endregion

        private Direction GetDirection(Vector2 direction) {
            if (direction.x < -0.9)
            {
                return Direction.Left;
            }
            if (direction.x > 0.9)
            {
                return Direction.Right;
            }
            if (direction.y < -0.9)
            {
                return Direction.Down;
            }
            
            return Direction.Up;
        }
    }
}