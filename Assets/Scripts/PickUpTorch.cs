using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpTorch : MonoBehaviour
{
	public Transform destination;

	public void PickUp() {
		this.transform.position = destination.position;
		this.transform.parent = GameObject.Find("TestPlayer").transform;
	}

	public void Drop() {
		this.transform.parent = null;
	}
}
