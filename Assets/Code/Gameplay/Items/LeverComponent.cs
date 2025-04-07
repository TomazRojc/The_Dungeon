using UnityEngine;

namespace Code.Gameplay
{
    public class LeverComponent : InteractableEnvironmentBaseComponent
    {
        public override void Interact()
        {
            Debug.Log("Lever Interact");
        }
    }
}