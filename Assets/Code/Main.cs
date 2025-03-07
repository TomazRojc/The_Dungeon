using Code.Configs;
using Code.Gameplay;
using Code.UI;
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
        private static UIEventBus _uiEventBus;
        
        public GameplayConfig GameplayConfig => _gameplayConfig;
        
        public static GameplaySession GameplaySession => _gameplaySession;
        public static LevelManager LevelManager => _levelManager;
        public static UIEventBus UiEventBus => _uiEventBus;

        private void Awake()
        {
            Instance = this;
            _gameplaySession = new GameplaySession(_gameplayConfig.MaxPlayers);
            _levelManager = new LevelManager(_levelsConfig, _gameplayConfig);
            _uiEventBus = new UIEventBus();
        }
    }
}