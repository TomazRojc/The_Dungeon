using Code.Utils;
using UnityEngine;

namespace Code
{

	public class UIController : MonoBehaviour
	{

		[SerializeField] private Lobby _lobby;

		[Header("UIs")]
		[SerializeField]
		private GameObject _loadingPanel;
		[SerializeField]
		private GameObject _mainMenuPanel;
		[SerializeField]
		private GameObject _settingsPanel;
		[SerializeField]
		private GameObject _lobbyPanel;
		[SerializeField]
		private GameObject _levelsPanel;

		private SimpleTimer _timer;

		private void Update()
		{
			_timer.Update(Time.deltaTime);
		}

		public void GoToLobby()
		{
			_lobbyPanel.SetActive(true);
			_lobby.OnEnter();
		}

		public void GoToLevelSelection()
		{
			_mainMenuPanel.SetActive(false);
			_lobbyPanel.SetActive(false);
			_levelsPanel.SetActive(true);
		}

		public void StartLevel(int levelNumber)
		{
			_loadingPanel.SetActive(true);
			_levelsPanel.SetActive(false);
			_timer.OnComplete += () =>
			{
				_loadingPanel.SetActive(false);
			};
			_timer.Start(0.7f);
			
			Main.Instance.LevelManager.StartLevel(levelNumber-1);
		}

		public void GoToSettings()
		{
			_settingsPanel.SetActive(true);
		}

		public void GoBackToMainMenu()
		{
			_mainMenuPanel.SetActive(true);
			_settingsPanel.SetActive(false);
			_lobbyPanel.SetActive(false);
		}

		public void ExitGame()
		{
			Application.Quit();
		}
	}
}