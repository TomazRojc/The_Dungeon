using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text[] playerNameTexts = new TMP_Text[4];
        [SerializeField]
        private TMP_Text[] playerReadyTexts = new TMP_Text[4];
        [SerializeField]
        private GameObject[] playerAvatars = new GameObject[4];
        [SerializeField]
        private GameObject[] playerReadyButtons = new GameObject[4];
        
        [SerializeField]
        private ButtonBase startGameButton;
        
        private List<bool> _emptyLobbyIndices = new List<bool>(4);


        private void Awake()
        {
            _emptyLobbyIndices = new List<bool> { true, true, true, true };
        }

        public void UpdateDisplay(List<PlayerData> players)
        {
            HandleReadyToStart(players);

            ResetEmptyIndicesList();
            
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].IsJoined) continue;

                var lobbyIndex = players[i].LobbyIndex;

                _emptyLobbyIndices[lobbyIndex] = false;

                playerNameTexts[lobbyIndex].text = players[i].DisplayName;
                playerReadyTexts[lobbyIndex].text = players[i].IsReady
                    ? "<color=green>Ready</color>"
                    : "<color=red>Not Ready</color>";
                playerAvatars[lobbyIndex].SetActive(true);
                playerAvatars[lobbyIndex].GetComponent<Image>().color = players[i].Color;
                playerReadyButtons[lobbyIndex].SetActive(true);
            }
            
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                if (!_emptyLobbyIndices[i]) continue;

                playerNameTexts[i].text = "Press <b>\u25a1</b> or F to join...";
                playerReadyTexts[i].text = string.Empty;
                playerAvatars[i].SetActive(false);
                playerReadyButtons[i].SetActive(false);
            }
        }

        private void ResetEmptyIndicesList()
        {
            for (int i = 0; i < 4; i++)
            {
                _emptyLobbyIndices[i] = true;
            }
        }

        public void HandleReadyToStart(List<PlayerData> players)
        {
            var allReady = true;
            var lobbyEmpty = true;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].IsJoined)
                {
                    lobbyEmpty = false;
                    if (!players[i].IsReady)
                    {
                        allReady = false;
                    }
                }
            }

            // TODO JanR: fix this to work with new buttons
            // startGameButton.interactable = allReady && !lobbyEmpty;
        }
    }
}