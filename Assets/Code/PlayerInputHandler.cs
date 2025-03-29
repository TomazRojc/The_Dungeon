using Code.Gameplay;
using Code.UI;
using Code.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerController _player;
        private ItemController _playerItemController;
        
        private Vector2 _moveInput;
        private Direction _navigateUIDirection;
        private int _inputIndex;

        private bool _navigationEnabled = true;
        private float _joystickDirectionThreshold = 0.9f;
        private float _onNavigateCooldownShort = 0.2f;
        private float _onNavigateCooldownLong = 0.35f;
        private bool _longCooldownFinished;
        private SimpleTimer _onNavigateCooldownTimer;

        public int InputIndex => _inputIndex;
        public PlayerController Player => _player;

        private void Awake()
        {
            _inputIndex = GetComponent<PlayerInput>().playerIndex;
            _onNavigateCooldownTimer = new SimpleTimer();
            _longCooldownFinished = false;
        }

        void Update()
        {
            HandleUIInputOnUpdate();

            HandlePlayerInputOnUpdate();
        }
        
        private void HandleUIInputOnUpdate()
        {
            if (!UIController.UIActive) return;

            if (_navigateUIDirection == Direction.Center)
            {
                _navigationEnabled = true;
                _longCooldownFinished = false;
                _onNavigateCooldownTimer.Stop();
                return;
            }

            _onNavigateCooldownTimer.Update(Time.deltaTime);

            if (!_navigationEnabled) return;

            _onNavigateCooldownTimer.OnComplete += () =>
            {
                _longCooldownFinished = true;
                _navigationEnabled = true;
            };

            var cooldown = _longCooldownFinished ? _onNavigateCooldownShort : _onNavigateCooldownLong;
            _onNavigateCooldownTimer.Start(cooldown);

            _navigationEnabled = false;
            Main.UiEventBus.OnNavigate?.Invoke(_navigateUIDirection, _inputIndex);
        }
        
        private void HandlePlayerInputOnUpdate()
        {
            if (UIController.UIActive || _player == null) return;
            
            // invoke player movement/look
            _player.HandleMoveInput(_moveInput);
        }

        public void ConnectPlayerController(PlayerController player)
        {
            _player = player;
            _playerItemController = _player.GetComponent<ItemController>();
        }

        public void DisconnectPlayerController()
        {
            _player = null;
            _playerItemController = null;
        }

        // WASD or Left Stick
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        // Q or North Button
        public void OnPickUpOrDropItem(InputAction.CallbackContext context)
        {
            if (UIController.UIActive || !context.performed || _player == null) return;
            
            _playerItemController.TryPickUpOrDropItem();
        }

        // E or West Button
        public void OnUseItem(InputAction.CallbackContext context)
        {
            if (UIController.UIActive || !context.performed || _player == null) return;
            
            _playerItemController.TryUseItem();
        }

        // Ctrl or East Button
        public void OnDash(InputAction.CallbackContext context)
        {
            if (UIController.UIActive || !context.performed || _player == null) return;

            _player.HandleDashInput();
        }

        // Space or South Button
        public void OnJump(InputAction.CallbackContext context)
        {
            if (UIController.UIActive || !context.performed || _player == null) return;

            _player.HandleJumpInput();
        }

#region UI

        public void OnNavigateUI(InputAction.CallbackContext context)
        {
            _navigateUIDirection = GetDirection(context.ReadValue<Vector2>());
        }

        public void OnSubmitUI(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            Main.UiEventBus.OnSubmit?.Invoke(_inputIndex);
        }
        
        public void OnJoinLobbyUI(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            Main.UiEventBus.OnJoinLobby?.Invoke(_inputIndex);
        }
        
        public void OnCancelUI(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            Main.UiEventBus.OnCancel?.Invoke(_inputIndex);
        }
        
        public void OnEscapeUI(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            Main.UiEventBus.OnEscape?.Invoke(_inputIndex);
        }
        
        #endregion

        private Direction GetDirection(Vector2 direction) {
            if (direction.x < -_joystickDirectionThreshold)
            {
                return Direction.Left;
            }
            if (direction.x > _joystickDirectionThreshold)
            {
                return Direction.Right;
            }
            if (direction.y < -_joystickDirectionThreshold)
            {
                return Direction.Down;
            }
            if (direction.y > _joystickDirectionThreshold)
            {
                return Direction.Up;
            }
            
            return Direction.Center;
        }
    }
}