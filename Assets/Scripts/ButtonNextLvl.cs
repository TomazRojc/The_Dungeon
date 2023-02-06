using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNextLvl : MonoBehaviour
{

	private MyNetworkManager room;
	private MyNetworkManager Room
	{
		get
		{
			if (room != null) { return room; }
			return room = NetworkManager.singleton as MyNetworkManager;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void nextLvl()
	{
		Room.StartLevel(2);
	}
}
