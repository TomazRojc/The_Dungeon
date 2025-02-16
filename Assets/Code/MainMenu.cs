using TheDungeon.Utils;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	
	[SerializeField]
	private Lobby _lobby;

	[Header("UIs")]
	[SerializeField] private GameObject _mainMenuPanel;
	[SerializeField] private GameObject _settingsPanel;
	[SerializeField] private GameObject _lobbyPanel;
	[SerializeField] private GameObject _levelsPanel;

	private SimpleTimer _timer = new SimpleTimer();

	private void Update()
	{
		_timer.Update(Time.deltaTime);
	}

	public void GoToLobby()
	{
		_lobbyPanel.SetActive(true);
		_lobby.OnEnter();
	}

	private void TryExitLobby()
	{
		if (_lobby.Active)
		{
			_lobbyPanel.SetActive(false);
			_lobby.OnExit();
		}
	}

	public void GoToLevelSelection()
	{
		_mainMenuPanel.SetActive(false);
		TryExitLobby();
		
		_timer.OnComplete += () =>
		{
			_levelsPanel.SetActive(true);
		};
		_timer.Start(0.7f);
	}
	
	public void GoToSettings()
	{
		_settingsPanel.SetActive(true);
	}

	public void GoBackToMainMenu()
	{
		_mainMenuPanel.SetActive(true);
		_settingsPanel.SetActive(false);
		TryExitLobby();
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
