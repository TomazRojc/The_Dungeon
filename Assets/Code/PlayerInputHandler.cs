using Code.UI;
using Code.Utils;
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

        private bool _navigationEnabled = true;
        private float _onNavigateCooldown = 0.1f;
        private SimpleTimer _onNavigateCooldownTimer;

        public int InputIndex => _inputIndex;
        public PlayerController Player => _player;

        private void Awake()
        {
            _inputIndex = GetComponent<PlayerInput>().playerIndex;
            _onNavigateCooldownTimer = new SimpleTimer();
        }

        void Update()
        {
            _onNavigateCooldownTimer.Update(Time.deltaTime);
            
            if (UIController.UIActive || _player == null) return;
            
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
            if (UIController.UIActive || _player == null) return;

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
            if (UIController.UIActive || _player == null) return;

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
            if (UIController.UIActive || _player == null) return;

            if (context.started)
            {
                _player.HandleDashInput();
            }
        }

        // Space or South Button
        public void OnJump(InputAction.CallbackContext context)
        {
            if (UIController.UIActive || _player == null) return;

            if (context.started)
            {
                _player.HandleJumpInput();
            }
        }

#region UI

        public void OnNavigateUI(InputAction.CallbackContext context)
        {
            if (!context.performed || !_navigationEnabled) return;

            var direction = GetDirection(context.ReadValue<Vector2>());
            if (direction == Direction.Center) return;

            _navigationEnabled = false;

            _onNavigateCooldownTimer.OnComplete += () => { _navigationEnabled = true; };
            _onNavigateCooldownTimer.Start(_onNavigateCooldown);
            Main.UiEventBus.OnNavigate?.Invoke(direction, _inputIndex);
        }

        public void OnSubmitUI(InputAction.CallbackContext context)
        {
             if (context.performed)
             {
                Main.UiEventBus.OnSubmit?.Invoke(_inputIndex);
             }
        }
        
        public void OnCancelUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Main.UiEventBus.OnCancel?.Invoke(_inputIndex);
            }
        }
        
        public void OnEscapeUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Main.UiEventBus.OnEscape?.Invoke(_inputIndex);
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
            if (direction.y > 0.9)
            {
                return Direction.Up;
            }
            
            return Direction.Center;
        }
    }
}