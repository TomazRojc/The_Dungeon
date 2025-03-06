using System.Collections.Generic;
using UnityEngine;

namespace Code.UI.UiStates
{
    public enum StateUiName {
        MainMenu,
        Settings,
        Lobby,
        LevelSelection,
        PauseMenu,
    }
    public class StateUI : MonoBehaviour
    {
        [SerializeField]
        private StateUiName _stateUiName;
        [SerializeField]
        private ButtonBase _defaultButton;

        [SerializeField]
        private List<ButtonBase> _defaultPlayerButtons;

        public ButtonBase DefaultButton => _defaultButton;
        public StateUiName StateUiName => _stateUiName;
        
        public List<ButtonBase> DefaultPlayerButtons => _defaultPlayerButtons;

        public virtual void OnEnter()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void OnExit()
        {
            gameObject.SetActive(false);
        }
    }
}