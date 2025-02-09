using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
	[SerializeField]
	private TMP_Text[] playerNameTexts = new TMP_Text[4];
	[SerializeField]
	private TMP_Text[] playerReadyTexts = new TMP_Text[4];
	[SerializeField]
	private GameObject[] playerAvatars = new GameObject[4];
	[SerializeField]
	private GameObject[] playerReadyButtons = new GameObject[4];

	[SerializeField]
	private List<Color> defaultPlayerColors;

	[SerializeField]
	private Button startGameButton;

	[SerializeField]
	private GameObject levelsUI;

	[SerializeField]
	private PlayerInputManager _playerInputManager;

	private List<PlayerData> _players;

	public void OnEnter()
	{
		_playerInputManager.EnableJoining();
			
		_players = Main.Instance.PlayersData;

		HandleReadyToStart();
		UpdateDisplay();
	}

	public void OnExit()
	{
		_playerInputManager.DisableJoining();
	}

	private void UpdateDisplay()
	{
		for (int i = 0; i < _players.Count; i++)
		{
			if (!_players[i].IsJoined)
			{
				playerNameTexts[i].text = "Press X to join...";
				playerReadyTexts[i].text = string.Empty;
				playerAvatars[i].SetActive(false);
				playerReadyButtons[i].SetActive(false);
			}
			else
			{
				playerNameTexts[i].text = _players[i].DisplayName;
				playerReadyTexts[i].text = _players[i].IsReady ?
					"<color=green>Ready</color>" :
					"<color=red>Not Ready</color>";
				playerAvatars[i].SetActive(true);
				playerAvatars[i].GetComponent<Image>().color = _players[i].Color;
				playerReadyButtons[i].SetActive(true);
			}
		}
	}

	public void OnPlayerJoined(int playerInputIndex)
	{
		var firstFreeIdx = -1;
		for (int i = 0; i < _players.Count; i++)
		{
			if (firstFreeIdx == -1 && !_players[i].IsJoined)
			{
				firstFreeIdx = i;
			}

			// player with this input index was already joined before
			if (_players[i].InputIndex == playerInputIndex)
			{
				_players[i].IsJoined = true;
				return;
			}
		}

		if (firstFreeIdx == -1)
		{
			throw new ArgumentException("Cannot join a player in a full lobby");
		}

		// new player joined
		_players[firstFreeIdx].SetValues($"Player {firstFreeIdx+1}", defaultPlayerColors[firstFreeIdx], true, false, playerInputIndex);
		UpdateDisplay();
	}

	public void OnPlayerLeft(int playerInputIndex)
	{
		for (var i = 0; i < _players.Count; i++)
		{
			if (_players[i].InputIndex == playerInputIndex)
			{
				_players[i].IsJoined = false;
				_players[i].IsReady = false;
				break;
			}
		}
		UpdateDisplay();
	}

	public void OnPlayerReady(int buttonIndex)
	{
		_players[buttonIndex].IsReady = !_players[buttonIndex].IsReady;
		HandleReadyToStart();
		UpdateDisplay();
	}

	public void HandleReadyToStart()
	{
		var readyToStart = true;
		for (int i = 0; i < _players.Count; i++)
		{
			if (_players[i].IsJoined && !_players[i].IsReady)
			{
				readyToStart = false;
			}
		}
		startGameButton.interactable = readyToStart;
	}

	public void ShowLevelsUI()
	{
		levelsUI.SetActive(true);
	}

}
