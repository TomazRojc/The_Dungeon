using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject _playerInputManager;
	[SerializeField]
	private GameObject _lobby;

	[Header("UIs")]
	[SerializeField] private GameObject _mainMenuPanel;
	[SerializeField] private GameObject _lobbyPanel;
	[SerializeField] private GameObject _settingsPanel;

	public void GoToLobby()
	{
		_lobbyPanel.SetActive(true);
		_playerInputManager.SetActive(true);
		_lobby.SetActive(true);
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
		_playerInputManager.SetActive(false);
		_lobby.SetActive(false);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
