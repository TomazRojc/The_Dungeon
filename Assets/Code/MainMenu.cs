using UnityEngine;

public class MainMenu : MonoBehaviour
{
	
	[SerializeField]
	private Lobby _lobby;

	[Header("UIs")]
	[SerializeField] private GameObject _mainMenuPanel;
	[SerializeField] private GameObject _lobbyPanel;
	[SerializeField] private GameObject _settingsPanel;

	public void GoToLobby()
	{
		_lobbyPanel.SetActive(true);
		_lobby.OnEnter();
	}

	public void GoToSettings()
	{
		_settingsPanel.SetActive(true);
	}

	public void GoBackToMainMenu()
	{
		_mainMenuPanel.SetActive(true);
		_lobbyPanel.SetActive(false);
		_settingsPanel.SetActive(false);
		_lobby.OnExit();
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
