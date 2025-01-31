using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
	private Button startGameButton;
	[SerializeField] 
	private GameObject nameInputField;
	[SerializeField] 
	private GameObject colorPickerObject;
	[SerializeField] 
	private GameObject playerAvatarPreview;

	private List<PlayerData> _players;

	public void Awake()
	{
		startGameButton.interactable = false;
		
		InputField input = nameInputField.GetComponent<InputField>();
		input.onEndEdit.AddListener(delegate { InputEntered(input); });

		_players = Main.Instance.PlayersData;
		_players[0].SetValues("Player 1", new Color(0,1,0,0.5f), true, false);
		// UpdateDisplay();
	}

	public void OnEnable()
	{
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		for (int i = 0; i < _players.Count; i++)
		{
			if (!_players[i].IsJoined)
			{
				playerNameTexts[i].text = "Press any key to join...";
				playerReadyTexts[i].text = string.Empty;
				playerAvatars[i].SetActive(false);
			}
			else
			{
				playerNameTexts[i].text = _players[i].DisplayName;
				playerReadyTexts[i].text = _players[i].IsReady ?
					"<color=green>Ready</color>" :
					"<color=red>Not Ready</color>";
				playerAvatars[i].SetActive(true);
				playerAvatars[i].GetComponent<Image>().color = _players[i].Color;
			}
			
		}
	}

	public void HandleReadyToStart(bool readyToStart)
	{
		startGameButton.interactable = readyToStart;
	}

	public void EnableNameInput()
	{
		nameInputField.SetActive(true);
		nameInputField.GetComponent<InputField>().Select();
	}
	
	public void InputEntered(InputField input)
	{
		nameInputField.SetActive(false);
	}

	public void ShowLevelsUI()
	{
		GameObject.Find("LevelsUI").transform.GetChild(0).gameObject.SetActive(true);
	}

}
