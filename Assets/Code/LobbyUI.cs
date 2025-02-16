using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code
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
        private Button startGameButton;
        
        public void UpdateDisplay(List<PlayerData> players)
        {
            HandleReadyToStart(players);

            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].IsJoined)
                {
                    playerNameTexts[i].text = "Press X to join...";
                    playerReadyTexts[i].text = string.Empty;
                    playerAvatars[i].SetActive(false);
                    playerReadyButtons[i].SetActive(false);
                }
                else
                {
                    playerNameTexts[i].text = players[i].DisplayName;
                    playerReadyTexts[i].text = players[i].IsReady
                        ? "<color=green>Ready</color>"
                        : "<color=red>Not Ready</color>";
                    playerAvatars[i].SetActive(true);
                    playerAvatars[i].GetComponent<Image>().color = players[i].Color;
                    playerReadyButtons[i].SetActive(true);
                }
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

            startGameButton.interactable = allReady && !lobbyEmpty;
        }
    }
}