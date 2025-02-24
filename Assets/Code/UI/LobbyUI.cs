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
        
        public void UpdateDisplay(List<PlayerData> players)
        {
            HandleReadyToStart(players);

            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Press X to join...";
                playerReadyTexts[i].text = string.Empty;
                playerAvatars[i].SetActive(false);
                playerReadyButtons[i].SetActive(false);
            }
            
            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].IsJoined) continue;

                var lobbyIndex = players[i].LobbyIndex;

                playerNameTexts[lobbyIndex].text = players[i].DisplayName;
                playerReadyTexts[lobbyIndex].text = players[i].IsReady
                    ? "<color=green>Ready</color>"
                    : "<color=red>Not Ready</color>";
                playerAvatars[lobbyIndex].SetActive(true);
                playerAvatars[lobbyIndex].GetComponent<Image>().color = players[i].Color;
                playerReadyButtons[lobbyIndex].SetActive(true);
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