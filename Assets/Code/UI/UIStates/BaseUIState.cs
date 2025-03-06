using System.Collections.Generic;
using UnityEngine;

namespace Code.UI.UiStates
{
    public class BaseUIState : MonoBehaviour
    {
        [SerializeField]
        private ButtonBase _defaultButton;

        public ButtonBase DefaultButton => _defaultButton;

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