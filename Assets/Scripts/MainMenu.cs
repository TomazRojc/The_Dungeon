using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[Header("UIs")]
	[SerializeField] private GameObject mainMenuUI;
	[SerializeField] private GameObject lobbyUI;
	[SerializeField] private GameObject settingsUI;
	

	public void GoToLobby()
	{
		lobbyUI.SetActive(true);
	}
	
	public void GoToSettings()
	{
		settingsUI.SetActive(true);
	}

	public void GoBack()
	{
		mainMenuUI.SetActive(true);
		lobbyUI.SetActive(false);
		settingsUI.SetActive(false);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
