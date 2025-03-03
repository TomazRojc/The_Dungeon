using System.Collections.Generic;
using UnityEngine;

namespace Code.UI
{
    public class StateUI : MonoBehaviour
    {
        [SerializeField]
        private ButtonBase _defaultButton;

        [SerializeField]
        private List<ButtonBase> _defaultPlayerButtons;

        public ButtonBase DefaultButton => _defaultButton;
        
        public List<ButtonBase> DefaultPlayerButtons => _defaultPlayerButtons;

        public void OnEnter()
        {
            gameObject.SetActive(true);
        }
        
        public void OnExit()
        {
            gameObject.SetActive(false);
        }
    }
}