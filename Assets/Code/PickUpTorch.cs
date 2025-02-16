using UnityEngine;

namespace Code
{
	public class PickUpTorch : MonoBehaviour
	{
		public Transform destination;

		public void PickUp()
		{
			transform.position = destination.position;
			transform.parent = GameObject.Find("TestPlayer").transform;
		}

		public void Drop()
		{
			transform.parent = null;
		}
	}
}