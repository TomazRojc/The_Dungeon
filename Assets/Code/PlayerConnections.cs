using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code
{

    public class PlayerConnections : MonoBehaviour
    {

        [SerializeField] private Lobby lobby;

        [UsedImplicitly]
        public void AddPlayerConnection(PlayerInput playerInput)
        {
            lobby.OnPlayerJoined(playerInput.playerIndex);

            var inputHandler = playerInput.GetComponent<PlayerInputHandler>();
            inputHandler.onDeviceLost += HandleDeviceLost;
            inputHandler.onDeviceRegained += HandleDeviceRegained;
        }

        [UsedImplicitly]
        public void RemovePlayerConnection(PlayerInput playerInput)
        {
            lobby.OnPlayerLeft(playerInput.playerIndex);

            var inputHandler = playerInput.GetComponent<PlayerInputHandler>();
            inputHandler.onDeviceLost -= HandleDeviceLost;
            inputHandler.onDeviceRegained -= HandleDeviceRegained;
        }

        private void HandleDeviceLost(int playerInputIndex)
        {
            lobby.OnPlayerLeft(playerInputIndex);
        }

        private void HandleDeviceRegained(int playerInputIndex)
        {
            lobby.OnPlayerJoined(playerInputIndex);
        }
    }
}