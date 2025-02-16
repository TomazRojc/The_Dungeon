using Code.Gameplay;
using UnityEngine;

namespace Code
{
    public class Main : MonoBehaviour
    {
        public static Main Instance;

        [SerializeField]
        private GameplayConfig _gameplayConfig;
        
        private GameplaySession _gameplaySession;
        
        public GameplayConfig GameplayConfig => _gameplayConfig;
        public GameplaySession GameplaySession => _gameplaySession;

        private void Awake()
        {
            Instance = this;
            _gameplaySession = new GameplaySession(_gameplayConfig.MaxPlayers);
        }
    }
}