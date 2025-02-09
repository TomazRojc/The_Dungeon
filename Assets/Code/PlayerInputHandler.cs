using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;

    public Action<int> onDeviceLost;
    public Action<int> onDeviceRegained;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Move: " + context.ReadValue<Vector2>());
        // handle movement based on _playerInput.playerIndex
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("Look: " + context.ReadValue<Vector2>());
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire: " +  context);
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
