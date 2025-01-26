using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
	
	private readonly struct PlayerData
	{
		public readonly string DisplayName;
		public readonly bool IsReady;
		public readonly Color MyColor;

		public PlayerData(string displayName, bool isReady, Color myColor)
		{
			DisplayName = displayName;
			IsReady = isReady;
			MyColor = myColor;
		}
	}
	
	[Header("UI")]
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

	private List<PlayerData> Players;

	public void Awake()
	{
		startGameButton.interactable = false;
		GameObject.Find("Main Menu UI").transform.GetChild(0).gameObject.SetActive(true);
		
		InputField input = nameInputField.GetComponent<InputField>();
		input.onEndEdit.AddListener(delegate { InputEntered(input); });

	}
	
	private void UpdateDisplay()
	{

		for (int i = 0; i < playerNameTexts.Length; i++)
		{
			playerNameTexts[i].text = "Waiting for player...";
			playerReadyTexts[i].text = string.Empty;
			playerAvatars[i].SetActive(false);
		}

		for (int i = 0; i < Players.Count; i++)
		{
			playerNameTexts[i].text = Players[i].DisplayName;
			playerReadyTexts[i].text = Players[i].IsReady ?
				"<color=green>Ready</color>" :
				"<color=red>Not Ready</color>";
			playerAvatars[i].SetActive(true);
			playerAvatars[i].GetComponent<Image>().color = Players[i].MyColor;
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
