using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay
{
    public class LevelComponent : MonoBehaviour
    {
        [SerializeField] private SpawnPointsComponent spawnPointsComponent;

        public void SpawnPlayers(List<GameObject> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = spawnPointsComponent.SpawnPoints[i].transform.position;
            }
        }
    }
}