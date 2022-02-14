using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[Header("UIs")]
	[SerializeField] private GameObject mainMenuUI = null;
	[SerializeField] private GameObject joinGameUI = null;
	[SerializeField] private GameObject settingsUI = null;
	

    public void GoToJoinGame()
    {
        joinGameUI.SetActive(true);
    }
	
	public void GoToSettings()
    {
        settingsUI.SetActive(true);
    }

	public void GoBack()
    {
		mainMenuUI.SetActive(true);
        joinGameUI.SetActive(false);
		settingsUI.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
