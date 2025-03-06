using System.Collections.Generic;
using UnityEngine;

namespace Code.UI.UiStates {
    public class MultiplayerUIState : BaseUIState {
        
        [SerializeField]
        private List<ButtonBase> _defaultPlayerButtons;
        
        public List<ButtonBase> DefaultPlayerButtons => _defaultPlayerButtons;
    }
}