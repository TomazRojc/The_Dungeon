using UnityEngine;

namespace Code.Gameplay
{
    public class BasicItem : ItemBaseComponent
    {
        public override void UseItem()
        {
            Debug.Log("Basic item has no effect on UseItem");
        }
    }
}
