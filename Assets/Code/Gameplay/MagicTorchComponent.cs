using UnityEngine;

namespace Code.Gameplay
{
    public class MagicTorchComponent : ItemBaseComponent
    {
        public override void OnUseItem()
        {
            Debug.Log("MagicTorch OnUseItem");
        }
    }
}
