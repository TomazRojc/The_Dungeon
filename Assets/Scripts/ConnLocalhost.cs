using UnityEngine;
using UnityEngine.UI;


public class ConnLocalhost : MonoBehaviour
{
	[SerializeField] private MyNetworkManager networkManager = null;
	[SerializeField] private GameObject mainMenuUI = null;

	private void OnEnable()
	{
		MyNetworkManager.OnClientConnected += HandleClientConnected;
		MyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
	}

	private void OnDisable()
	{
		MyNetworkManager.OnClientConnected -= HandleClientConnected;
		MyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
	}

	public void HostLobby()
	{
		mainMenuUI.SetActive(false);

		networkManager.StartHost();
	}

	public void JoinLobby()
	{
		networkManager.networkAddress = "localhost";
		networkManager.StartClient();

		mainMenuUI.SetActive(false);
	}

	private void HandleClientConnected()
	{
		mainMenuUI.SetActive(false);
	}

	private void HandleClientDisconnected()
	{
		mainMenuUI.SetActive(true);
	}
}
