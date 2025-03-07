using System.Collections.Generic;
using UnityEngine;

namespace Code.Configs
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "ScriptableObjects/GameplayConfig", order = 1)]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField]
        private int maxPlayers;
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private List<Color> defaultPlayerColors;

        public int MaxPlayers => maxPlayers;
        public GameObject PlayerPrefab => playerPrefab;
        public List<Color> DefaultPlayerColors => defaultPlayerColors;
    }
}