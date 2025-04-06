using System;
using UnityEngine;

namespace Code.Gameplay
{
    public abstract class ItemBaseComponent : MonoBehaviour
    {
        [SerializeField]
        private RenderSortOrderingComponent _sortOrdering;
        
        [SerializeField]
        private GameObject _attachPoint;
        
        public GameObject AttachPoint => _attachPoint;

        public void PickUpItem()
        {
            _sortOrdering.ChangeOrderInLayer(100);
        }

        public void DropItem()
        {
            _sortOrdering.ChangeOrderInLayer(-100);
        }
        public abstract void UseItem();
    }
}
