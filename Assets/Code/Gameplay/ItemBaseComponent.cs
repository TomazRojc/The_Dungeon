using System;
using UnityEngine;

namespace Code.Gameplay
{
    public abstract class ItemBaseComponent : MonoBehaviour
    {
        [SerializeField]
        private GameObject _attachPoint;
        
        public Action onPickUpItem;
        public Action onDropItem;
        public Action onUseItem;
        
        public GameObject AttachPoint => _attachPoint;
    }
}
