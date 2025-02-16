using Code.Gameplay;
using UnityEngine;

namespace Code
{
    public class Main : MonoBehaviour
    {
        public static Main Instance;

        [SerializeField]
        private GameplayConfig _gameplayConfig;
        [SerializeField]
        private LevelsConfig _levelsConfig;
        
        private GameplaySession _gameplaySession;
        private LevelManager _levelManager;
        
        public GameplayConfig GameplayConfig => _gameplayConfig;
        public GameplaySession GameplaySession => _gameplaySession;
        public LevelManager LevelManager => _levelManager;

        private void Awake()
        {
            Instance = this;
            _gameplaySession = new GameplaySession(_gameplayConfig.MaxPlayers);
            _levelManager = new LevelManager(_levelsConfig.LevelPrefabs);
        }
    }
}