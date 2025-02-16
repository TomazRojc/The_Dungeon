using System;
using Code.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public abstract class ButtonBase : Selectable
    {
        [SerializeField]
        private UnityEvent onClick;
        [SerializeField]
        private float idleBreakTime;
        
        private bool _isSelected;
        
        private SimpleTimer _idleBreakTimer;
        
        protected abstract void PlayEnterAnimation();
        protected abstract void PlayExitAnimation();
        protected abstract void PlayIdleBreakAnimation();

        protected override void Awake()
        {
            base.Awake();
            _idleBreakTimer.OnLoopComplete += PlayIdleBreakAnimation;
        }
        
        protected virtual void Update()
        {
            if (_isSelected)
            {
                _idleBreakTimer.Update(Time.deltaTime);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            _isSelected = true;
            PlayEnterAnimation();
            _idleBreakTimer.Start(idleBreakTime, true);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            _isSelected = false;
            PlayExitAnimation();
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnClick();
        }
        
        protected virtual void OnClick()
        {
            onClick.Invoke();
        }
    }
    
}