using Code.Configs;
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
        
        private static GameplaySession _gameplaySession;
        private static LevelManager _levelManager;
        private static UiManager _uiManager;
        
        public GameplayConfig GameplayConfig => _gameplayConfig;
        
        public static GameplaySession GameplaySession => _gameplaySession;
        public static LevelManager LevelManager => _levelManager;
        public static UiManager UiManager => _uiManager;

        private void Awake()
        {
            Instance = this;
            _gameplaySession = new GameplaySession(_gameplayConfig.MaxPlayers);
            _levelManager = new LevelManager(_levelsConfig.LevelPrefabs);
            _uiManager = new UiManager();
        }
    }
}