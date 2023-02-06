using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RoomPlayer : NetworkBehaviour
{
	[Header("UI")]
	[SerializeField] private GameObject lobbyUI = null;
	[SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
	[SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
	[SerializeField] private GameObject[] playerAvatars = new GameObject[4];

	[SerializeField] private Button startGameButton = null;
	[SerializeField] private GameObject nameInputField = null;
	[SerializeField] private GameObject colorPickerObject = null;
	[SerializeField] private GameObject playerAvatarPreview = null;

	[SyncVar(hook = nameof(HandleDisplayNameChanged))]
	public string DisplayName = "Player";
	[SyncVar(hook = nameof(HandleReadyStatusChanged))]
	public bool IsReady = false;
	[SyncVar(hook = nameof(HandleColorChanged))]
	public Color MyColor;

	private bool isLeader;
	public bool IsLeader
	{
		set
		{
			isLeader = value;
			// startGameButton.gameObject.SetActive(value);
		}
	}

	private MyNetworkManager room;
	private MyNetworkManager Room
	{
		get
		{
			if (room != null) { return room; }
			return room = NetworkManager.singleton as MyNetworkManager;
		}
	}

	void Update()
	{
		if (colorPickerObject.activeSelf)
		{
			playerAvatarPreview.GetComponent<Image>().color = colorPickerObject.GetComponent<FlexibleColorPicker>().color;
		}
	}

	public override void OnStartAuthority()
	{
		lobbyUI.SetActive(true);
		startGameButton.interactable = false;
		GameObject.Find("Main Menu UI").transform.GetChild(0).gameObject.SetActive(true);
		
		InputField input = nameInputField.GetComponent<InputField>();
		input.onEndEdit.AddListener(delegate { InputEntered(input); });

		int idx = Room.RoomPlayers.Count + 1;	// my player is not saved in the room yet, but others are
		CmdSetDisplayName("Player " + idx);
	}

	public override void OnStartClient()
	{
		Room.RoomPlayers.Add(this);
		UpdateDisplay();
	}

	public override void OnStopClient()
	{
		Room.RoomPlayers.Remove(this);

		UpdateDisplay();
	}

	public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
	public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
	public void HandleColorChanged(Color oldValue, Color newValue) => UpdateDisplay();
	
	private void UpdateDisplay()
	{
		if (!hasAuthority)
		{
			foreach (var player in Room.RoomPlayers)
			{
				if (player.hasAuthority)
				{
					player.UpdateDisplay();
					break;
				}
			}

			return;
		}

		for (int i = 0; i < playerNameTexts.Length; i++)
		{
			playerNameTexts[i].text = "Waiting for player...";
			playerReadyTexts[i].text = string.Empty;
			playerAvatars[i].SetActive(false);
		}

		for (int i = 0; i < Room.RoomPlayers.Count; i++)
		{
			playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
			playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
				"<color=green>Ready</color>" :
				"<color=red>Not Ready</color>";
			playerAvatars[i].SetActive(true);
			playerAvatars[i].GetComponent<Image>().color = Room.RoomPlayers[i].MyColor;
		}
	}

	public void HandleReadyToStart(bool readyToStart)
	{
		if (!isLeader) { return; }

		startGameButton.interactable = readyToStart;
	}

	public void DisconnectAndGoBack()
	{
		if (isLeader) {
			Room.StopHost();
		} else {
			Room.StopClient();
		}
	}

	public void EnableNameInput()
	{
		nameInputField.SetActive(true);
		nameInputField.GetComponent<InputField>().Select();
	}
	
	public void InputEntered(InputField input)
	{
		nameInputField.SetActive(false);
		CmdSetDisplayName(input.text);
	}

	public void EnableColorPicker() => colorPickerObject.SetActive(true);

	public void ApplyColor()
	{
		CmdSetColor(colorPickerObject.GetComponent<FlexibleColorPicker>().color);
		colorPickerObject.SetActive(false);
	}

	[Command]
	private void CmdSetDisplayName(string displayName)
	{
		DisplayName = displayName;
	}

	[Command]
	public void CmdReadyUp()
	{
		IsReady = !IsReady;

		Room.NotifyPlayersOfReadyState();
	}

	[Command]
	private void CmdSetColor(Color color)
	{
		MyColor = color;
	}

	[Command]
	public void CmdShowLevelsUI()
	{
		RpcShowLevelsUI();
		
		// if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

		// Room.StartGame();
	}

	[ClientRpc]
	public void RpcShowLevelsUI()
	{
		GameObject.Find("LevelsUI").transform.GetChild(0).gameObject.SetActive(true);
	}
}
