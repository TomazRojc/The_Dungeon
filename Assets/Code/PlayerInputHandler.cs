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

    // WASD or Left Stick
    public void OnMove(InputAction.CallbackContext context)
    {
        // context.started and context.cancelled are called on the first and last frame of performing an action (this function is called twice on those frames)
        // if (!context.performed) return;
        Debug.Log("Move: " + context.ReadValue<Vector2>());
        Debug.Log(context);
        
        // handle movement based on _playerInput.playerIndex
    }
    
    // Arrows or Right Stick
    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("Look: " + context.ReadValue<Vector2>());
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
