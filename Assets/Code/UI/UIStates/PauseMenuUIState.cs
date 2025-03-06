using UnityEngine;

namespace Code.UI.UiStates {
    public class PauseMenuUIState : BaseUIState {

        public override void OnEnter() {
            base.OnEnter();
            Time.timeScale = 0f;
        }
        
        public override void OnExit() {
            base.OnExit();
            Time.timeScale = 1f;
        }
    }
}