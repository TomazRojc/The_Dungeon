using Mirror;
using UnityEngine;


public class ConnLocalhost : MonoBehaviour
{
	[SerializeField] private MyNetworkManager networkManager;
	[SerializeField] private GameObject mainMenuUI;
	[SerializeField] private Transport transport;

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
		networkManager.SetTransport(transport);
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
