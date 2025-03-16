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

        public void TryPickUpItem()
        {
            if (_currentItem != null)
            {
                _currentItem.transform.parent = Main.LevelManager.WorldGameObject.transform;
                _currentItem = null;
                return;
            }
            
            _overlappingColliders.Clear();
            Physics2D.OverlapCollider(_playerCollider, _colliderFilter, _overlappingColliders);

            if (_overlappingColliders.Count > 0)
            {
                _currentItem = _overlappingColliders[0].GetComponent<ItemBaseComponent>();
                _currentItem.transform.parent = _itemSnapPoint.transform;
            }
        }
        
        public void TryUseItem()
        {
            _currentItem?.OnUseItem();
        }
    }
}