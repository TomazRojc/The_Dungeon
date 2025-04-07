using UnityEngine;

namespace Code.Gameplay
{
    public abstract class ItemBaseComponent : MonoBehaviour
    {
        [SerializeField]
        private RenderSortOrderingComponent _sortOrdering;
        
        [SerializeField]
        private GameObject _attachPoint;

        public void PickUpItem(Transform parentTransform)
        {
            transform.parent = parentTransform;
                
            var localOffset = transform.InverseTransformPoint(_attachPoint.transform.position);
            var newPosition = parentTransform.position - localOffset;
            transform.position = newPosition;
            _sortOrdering.ChangeOrderInLayer(100);
        }

        public void DropItem()
        {
            transform.parent = Main.LevelManager.WorldGameObject.transform;
            _sortOrdering.ChangeOrderInLayer(-100);
        }
        public abstract void UseItem();
    }
}
