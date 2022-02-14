﻿using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MyNetworkManager : NetworkManager
{
    
	[SerializeField] private int minPlayers = 4;
    [SerializeField] private string menuScene = string.Empty;

    // [Header("Maps")]
    // [SerializeField] private int numberOfRounds = 1;
    // [SerializeField] private MapSet mapSet = null;

    [Header("Room")]
    [SerializeField] private RoomPlayer roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private GamePlayer gamePlayerPrefab = null;
    // [SerializeField] private GameObject playerSpawnSystem = null;
    // [SerializeField] private GameObject roundSystem = null;

    // private MapHandler mapHandler;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    // public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

	[SerializeField]
    public List<RoomPlayer> RoomPlayers { get; } = new List<RoomPlayer>();
    public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();

    // public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    // public override void OnStartClient()
    // {
    //     var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

    //     foreach (var prefab in spawnablePrefabs)
    //     {
    //         NetworkClient.RegisterPrefab(prefab);
    //     }
    // }

    public override void OnClientConnect(NetworkConnection conn)
    {		
		base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log("Server: OnServerConnect");
		if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
		Debug.Log("Server: OnServerAddPlayer, my logic to instantiate a player on the server");
        if (SceneManager.GetActiveScene().name == menuScene)
        {			
			bool isLeader = RoomPlayers.Count == 0;

            RoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            
			NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
			Debug.Log("Server: OnServerAddPlayer, added player for connection");
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<RoomPlayer>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            // mapHandler = new MapHandler(mapSet, numberOfRounds);

            // ServerChangeScene(mapHandler.NextMap);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_Map"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    // public override void OnServerSceneChanged(string sceneName)
    // {
    //     if (sceneName.StartsWith("Scene_Map"))
    //     {
    //         GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
    //         NetworkServer.Spawn(playerSpawnSystemInstance);

    //         GameObject roundSystemInstance = Instantiate(roundSystem);
    //         NetworkServer.Spawn(roundSystemInstance);
    //     }
    // }

    // public override void OnServerReady(NetworkConnection conn)
    // {
    //     base.OnServerReady(conn);

    //     OnServerReadied?.Invoke(conn);
    // }
	

	
}