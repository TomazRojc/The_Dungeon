using Mirror;
using Steamworks;
using UnityEngine;


public class SteamLobby : MonoBehaviour
{
	[SerializeField] private GameObject mainMenuUI;
	[SerializeField] private GameObject joinGameUI;
	[SerializeField] private GameObject settingsUI;
	[SerializeField] private MyNetworkManager networkManager;
	[SerializeField] private Transport transport;

	protected Callback<LobbyCreated_t> lobbyCreated;
	protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
	protected Callback<LobbyEnter_t> lobbyEntered;

	private const string HostAddressKey = "HostAddress";


	private void Start() {
		if (!SteamManager.Initialized) { return; }

		lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
		gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
		lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
	}

	public void HostLobby()
	{
		mainMenuUI.SetActive(false);
		networkManager.SetTransport(transport);
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
	}

	private void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult != EResult.k_EResultOK)
		{
			mainMenuUI.SetActive(true);
			return;
		}

		networkManager.StartHost();

		SteamMatchmaking.SetLobbyData(
			new CSteamID(callback.m_ulSteamIDLobby),
			HostAddressKey,
			SteamUser.GetSteamID().ToString());
	}

	private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
	}

	private void OnLobbyEntered(LobbyEnter_t callback)
	{
		if (NetworkServer.active) { return; }

		string hostAddress = SteamMatchmaking.GetLobbyData(
			new CSteamID(callback.m_ulSteamIDLobby),
			HostAddressKey);

		networkManager.networkAddress = hostAddress;
		networkManager.StartClient();

		mainMenuUI.SetActive(false);
		joinGameUI.SetActive(false);
		settingsUI.SetActive(false);
	}
}
