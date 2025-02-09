using UnityEngine;

public class PlayerData
{
    public string DisplayName;
    public Color Color;
    public bool IsJoined;
    public bool IsReady;
    public int InputIndex;

    public PlayerData(string displayName, Color color, bool isJoined, bool isReady, int inputIndex)
    {
        DisplayName = displayName;
        Color = color;
        IsJoined = isJoined;
        IsReady = isReady;
        InputIndex = inputIndex;
    }

    public PlayerData() : this("Player", Color.white, false, false, -1) { }

    public void SetValues(string displayName, Color color, bool isJoined, bool isReady, int inputIndex)
    {
        DisplayName = displayName;
        Color = color;
        IsJoined = isJoined;
        IsReady = isReady;
        InputIndex = inputIndex;
    }
}