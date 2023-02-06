using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerSpawnSystem : NetworkBehaviour
{
	private static List<Transform> spawnPoints = new List<Transform>();

	private int nextIndex = 0;

	public static void AddSpawnPoint(Transform transform)
	{
		spawnPoints.Add(transform);

		spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
	}
	public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

	public override void OnStartServer() => MyNetworkManager.OnServerReadied += SpawnPlayer;

	[ServerCallback]
	private void OnDestroy() => MyNetworkManager.OnServerReadied -= SpawnPlayer;

	[Server]
	public void SpawnPlayer(NetworkConnection conn)
	{
		Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

		if (spawnPoint == null)
		{
			Debug.LogError($"Missing spawn point for player {nextIndex}");
			return;
		}

		GameObject [] playerObjects = GameObject.FindGameObjectsWithTag("Player");
		foreach (var playerObject in playerObjects) {
			GamePlayer gamePlayer = playerObject.GetComponent<GamePlayer>();
			gamePlayer.TargetTeleportPlayer(conn, spawnPoint.position.x, spawnPoint.position.y);
		}

		nextIndex++;
		if (spawnPoints.Count == nextIndex) nextIndex = 0;
	}
}
