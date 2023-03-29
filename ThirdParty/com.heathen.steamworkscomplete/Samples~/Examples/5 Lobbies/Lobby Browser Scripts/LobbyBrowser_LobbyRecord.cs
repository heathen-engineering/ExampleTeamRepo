#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using System;
#if ENABLE_INPUT_SYSTEM
#endif

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class LobbyBrowser_LobbyRecord : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI nameOrID;
        public TMPro.TextMeshProUGUI buttonLabel;

        private LobbyData target;
        private LobbyManager lobbyManager;

        private void OnDestroy()
        {
            if (lobbyManager != null)
            {
                lobbyManager.evtEnterSuccess.RemoveListener(HandleLobbyEnter);
                lobbyManager.evtLeave.RemoveListener(HandleLobbyLeave);
            }
        }

        public void SetLobby(LobbyData lobby, LobbyManager parent)
        {
            target = lobby;
            lobbyManager = parent;

            if (lobbyManager.Lobby == target)
                buttonLabel.text = "Leave";
            else
                buttonLabel.text = "Join";

            nameOrID.text = lobby.Name;
            if(string.IsNullOrEmpty(nameOrID.text))
                nameOrID.text = lobby.ToString();

            parent.evtEnterSuccess.AddListener(HandleLobbyEnter);
            parent.evtLeave.AddListener(HandleLobbyLeave);
        }

        private void HandleLobbyLeave()
        {
            if (lobbyManager.Lobby == target)
                buttonLabel.text = "Leave";
            else
                buttonLabel.text = "Join";
        }

        private void HandleLobbyEnter(LobbyData arg0)
        {
            if (target.IsAMember(UserData.Me))
                buttonLabel.text = "Leave";
            else
                buttonLabel.text = "Join";
        }

        public void JoinOrExit()
        {
            if (lobbyManager.Lobby == target)
                lobbyManager.Leave();
            else
                lobbyManager.Join(target);
        }
    }
}
#endif
