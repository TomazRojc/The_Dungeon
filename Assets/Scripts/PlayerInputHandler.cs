using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private int _playerIndex;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerIndex = _playerInput.playerIndex;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("Move: " + context.ReadValue<Vector2>());
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("Look: " + context);
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire: " +  context);
    }
}
