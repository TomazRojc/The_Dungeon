using System;
using System.Collections.Generic;
using Code.Utils;
using UnityEngine;
using UnityEngine.Events;

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
        private int _lobbyIndex;
        [SerializeField]
        private List<ButtonBase> _buttonsUp;
        [SerializeField]
        private List<ButtonBase> _buttonsDown;
        [SerializeField]
        private List<ButtonBase> _buttonsLeft;
        [SerializeField]
        private List<ButtonBase> _buttonsRight;

        protected SimpleTimer _animationTimer;
        
        private bool _isSelected;
        private SimpleTimer _idleBreakTimer;
        
        public bool IsSharedButton => _isSharedButton;
        public int LobbyIndex => _lobbyIndex;
        
        protected abstract void PlayEnterAnimation();
        protected abstract void PlayExitAnimation();
        protected abstract void PlayIdleBreakAnimation();
        protected abstract void ResetButton();

        protected virtual void Awake()
        {
            _idleBreakTimer.OnLoopComplete += PlayIdleBreakAnimation;
        }
        
        protected virtual void OnDisable() {
            _idleBreakTimer.Stop();
            ResetButton();
        }
        
        protected virtual void Update()
        {
            if (_isSelected)
            {
                _idleBreakTimer.Update(Time.deltaTime);
            }
            _animationTimer.Update(Time.deltaTime);
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

        public List<ButtonBase> GetNextButtons(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return _buttonsUp;
                case Direction.Down:
                    return _buttonsDown;
                case Direction.Left:
                    return _buttonsLeft;
                case Direction.Right:
                    return _buttonsRight;
            }
            throw new ArgumentException($"Invalid Direction for button {name}");
        }
    }
    
}