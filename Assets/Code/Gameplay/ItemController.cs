using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField]
        GameObject _itemSnapPoint;
        
        [SerializeField]
        private LayerMask _itemsLayer;
        
        private ItemBaseComponent _currentItem;

        private BoxCollider2D _playerCollider;
        private List<Collider2D> _overlappingColliders;
        private ContactFilter2D _colliderFilter;

        private void Awake()
        {
            var playerController = GetComponent<PlayerController>();
            _playerCollider = playerController.BoxCollider;
            
            _colliderFilter = new ContactFilter2D();
            _colliderFilter.SetLayerMask(_itemsLayer);
            _overlappingColliders = new List<Collider2D>(1);
        }

        public void TryPickUpOrDropItem()
        {
            var overlappingCollider = GetOverlappingCollider();
            
            if (overlappingCollider != null)
            {
                var interactable = overlappingCollider.GetComponent<InteractableEnvironmentBaseComponent>();
                if (interactable != null)
                {
                    interactable.Interact();
                    return;
                }
            }

            if (_currentItem != null)
            {
                DropItem();
            }

            if (overlappingCollider != null)
            {
                var item = overlappingCollider.GetComponent<ItemBaseComponent>();
                if (item != null)
                {
                    PickUpItem(item);
                }
            }
            
        }

        private Collider2D GetOverlappingCollider()
        {
            _overlappingColliders.Clear();
            Physics2D.OverlapCollider(_playerCollider, _colliderFilter, _overlappingColliders);
            var currentItemCollider = _currentItem != null? _currentItem.GetComponent<Collider2D>() : null;
            
            foreach (var candidateCollider in _overlappingColliders)
            {
                if (candidateCollider != currentItemCollider)
                {
                    return candidateCollider;
                }
            }
            
            return null;
        }

        private void PickUpItem(ItemBaseComponent item)
        {
            _currentItem = item;
            _currentItem.PickUpItem(_itemSnapPoint.transform);
        }

        private void DropItem()
        {
            _currentItem.DropItem();
            _currentItem = null;
        }

        public void TryUseItem()
        {
            _currentItem?.UseItem();
        }
    }
}