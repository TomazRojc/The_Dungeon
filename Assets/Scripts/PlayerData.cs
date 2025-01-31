using UnityEngine;

public class PlayerData
{
    public string DisplayName;
    public Color Color;
    public bool IsJoined;
    public bool IsReady;

    public PlayerData(string displayName, Color color, bool isJoined, bool isReady)
    {
        DisplayName = displayName;
        Color = color;
        IsJoined = isJoined;
        IsReady = isReady;
    }
    
    public PlayerData() : this("Player", Color.white, false, false) { }
    
    public void SetValues(string displayName, Color color, bool isJoined, bool isReady)
    {
        DisplayName = displayName;
        Color = color;
        IsJoined = isJoined;
        IsReady = isReady;
    }
}