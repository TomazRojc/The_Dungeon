using System;
using Code.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Code.UI
{
    public abstract class ButtonBase : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onSubmit;
        [SerializeField]
        private float idleBreakTime;
        [SerializeField]
        private bool _isSharedButton = true;
        [SerializeField]
        private ButtonBase _buttonUp;
        [SerializeField]
        private ButtonBase _buttonDown;
        [SerializeField]
        private ButtonBase _buttonLeft;
        [SerializeField]
        private ButtonBase _buttonRight;

        private bool _isSelected;

        private SimpleTimer _idleBreakTimer;
        
        public bool IsSharedButton => _isSharedButton;
        
        protected abstract void PlayEnterAnimation();
        protected abstract void PlayExitAnimation();
        protected abstract void PlayIdleBreakAnimation();

        protected virtual void Awake()
        {
            _idleBreakTimer.OnLoopComplete += PlayIdleBreakAnimation;
        }
        
        protected virtual void Update()
        {
            if (_isSelected)
            {
                _idleBreakTimer.Update(Time.deltaTime);
            }
        }

        public virtual void OnSelect()
        {
            _isSelected = true;
            PlayEnterAnimation();
            _idleBreakTimer.Start(idleBreakTime, true);
        }
        
        public virtual void OnDeselect()
        {
            _isSelected = false;
            PlayExitAnimation();
        }
        
        public virtual void OnSubmit()
        {
            onSubmit.Invoke();
        }

        public ButtonBase GetNextButton(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return _buttonUp;
                case Direction.Down:
                    return _buttonDown;
                case Direction.Left:
                    return _buttonLeft;
                case Direction.Right:
                    return _buttonRight;
            }
            throw new ArgumentException($"Invalid Direction for button {name}");
        }
    }
    
}