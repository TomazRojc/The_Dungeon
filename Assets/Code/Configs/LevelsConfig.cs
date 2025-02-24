using System.Collections.Generic;
using UnityEngine;

namespace Code.Configs
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "ScriptableObjects/LevelsConfig", order = 1)]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> levelPrefabs;

        public List<GameObject> LevelPrefabs => levelPrefabs;
    }
}