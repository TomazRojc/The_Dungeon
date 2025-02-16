using UnityEngine;

namespace Code
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "ScriptableObjects/GameplayConfig", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField]
        private int maxPlayers;
        [SerializeField]
        private GameObject playerPrefab;

        public int MaxPlayers => maxPlayers;
        public GameObject PlayerPrefab => playerPrefab;
    }
}