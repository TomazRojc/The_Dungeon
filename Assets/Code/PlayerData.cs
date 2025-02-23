using UnityEngine;

namespace Code
{
    public class PlayerData
    {
        public string DisplayName;
        public Color Color;
        public bool IsJoined;
        public bool IsReady;
        public int LobbyIndex;
        public int InputIndex;

        public PlayerData(string displayName, Color color, bool isJoined, bool isReady, int lobbyIndex, int inputIndex)
        {
            DisplayName = displayName;
            Color = color;
            IsJoined = isJoined;
            IsReady = isReady;
            LobbyIndex = lobbyIndex;
            InputIndex = inputIndex;
        }

        public void SetValues(string displayName, Color color, bool isJoined, bool isReady, int lobbyIndex, int inputIndex)
        {
            DisplayName = displayName;
            Color = color;
            IsJoined = isJoined;
            IsReady = isReady;
            LobbyIndex = lobbyIndex;
            InputIndex = inputIndex;
        }
    }
}