using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsComponent : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> spawnPoints;

    public List<GameObject> SpawnPoints => spawnPoints;
}
