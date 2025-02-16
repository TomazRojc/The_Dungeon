using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay
{
    public class SpawnPointsComponent : MonoBehaviour
    {
        [SerializeField] private List<GameObject> spawnPoints;

        public List<GameObject> SpawnPoints => spawnPoints;
    }
}