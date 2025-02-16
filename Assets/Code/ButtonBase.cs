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
        
        protected abstract void PlayEnterAnimation();
        protected abstract void PlayExitAnimation();
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PlayEnterAnimation();
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
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