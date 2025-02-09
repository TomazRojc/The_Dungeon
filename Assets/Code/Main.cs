using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;

    [SerializeField]
    private int maxPlayers = 4;

    public int MaxPlayers => maxPlayers;
    public List<PlayerData> PlayersData;

    private void Awake()
    {
        Instance = this;
        PlayersData = new List<PlayerData>(maxPlayers);
        for (int i = 0; i < maxPlayers; i++)
        {
            PlayersData.Add(new PlayerData());
        }
    }
}