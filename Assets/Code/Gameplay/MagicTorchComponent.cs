using UnityEngine;

namespace Code.Gameplay
{
    public class MagicTorchComponent : ItemBaseComponent
    {
        private void Awake()
        {
            onUseItem += OnUseItem;
        }
        
        private void OnUseItem()
        {
            Debug.Log("MagicTorch OnUseItem");
        }
    }
}
