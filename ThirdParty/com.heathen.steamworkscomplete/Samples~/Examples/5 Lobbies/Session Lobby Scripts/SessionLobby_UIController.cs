#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.UI;
using System.Collections.Generic;
using System;
#if ENABLE_INPUT_SYSTEM
#endif

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class SessionLobby_UIController : MonoBehaviour
    {
        [Header("References")]
        public LobbyManager lobbyManager;
        public GameObject sessionPanel;
        public GameObject[] ownerPip;
        public GameObject[] readyPip;
        public GameObject[] waitingPip;
        public SetUserAvatar[] memberSlotAvatars;
        public GameObject[] emptySlotButtons;
        public TMPro.TextMeshProUGUI peerMessage;
        public GameObject readyButton;
        public GameObject readyButtonWaiting;
        public GameObject playButtonWaiting;
        public TMPro.TextMeshProUGUI gameCreatedMessage;

        // cash of the members of the lobby other than my self
        private List<LobbyMemberData> partyMembersOtherThanMe = new List<LobbyMemberData>();

        /// <summary>
        /// Occures when we have entered a lobby .. that is when we have joined a lobby we didn't create
        /// </summary>
        public void OnJoinedALobby()
        {
            //Hide the proccessing animation on teh PlayButton
            playButtonWaiting.SetActive(false);
            peerMessage.text = "... Waiting ...";

            UpdateSlotInfo();

            //Check if the server has already been set for this lobby
            if (lobbyManager.HasServer)
                //If so update the user about that
                OnGameCreated(lobbyManager.GameServer);
        }
        /// <summary>
        /// Occurs when we create a lobby ... we know this only happens when we are the lobby "owner"
        /// </summary>
        /// <param name="lobby"></param>
        public void OnLobbyCreated(LobbyData lobby)
        {
            Debug.Log("The OnLobbyCreated event was called!");

            //Hide the proccessing animation on teh PlayButton
            playButtonWaiting.SetActive(false);
            gameCreatedMessage.text = string.Empty;
            playButtonWaiting.SetActive(false);

            UpdateSlotInfo();
        }
        public void HandleOnMemberLeft(UserLobbyLeaveData lobbyLeaveData)
        {
            Debug.Log($"A user named {lobbyLeaveData.user.Nickname} left the lobby");
            UpdateSlotInfo();
        }
        /// <summary>
        /// Occurs when the player clicks the "Ready" or "Wait" button
        /// </summary>
        public void SetReady()
        {
            lobbyManager.IsPlayerReady = !lobbyManager.IsPlayerReady;
        }
        /// <summary>
        /// This occurs when the owner clicks "Start Session"
        /// </summary>
        public void StartGame()
        {
            playButtonWaiting.SetActive(true);

            if (!lobbyManager.Lobby.AllPlayersReady)
            {
                Debug.Log($"You chose to start the session before all players where marked ready");
            }

            Debug.Log($"If this was a real game the owner would need to start up the network session.\n" +
                $"For a typical P2P game this would mean loading the network scene, calling StartHost or similar on your networking HLAPI and setting up the environment to make it ready for others to join.\n" +
                $"For a Client/Server game this would mean allocating a server, connecting to it and configuring it to make it ready for others to join.\n" +
                $"Once the network session is set up and ready for others to connect ... the owner of the lobby should call SetGameServer as demonstrated in our source code.");

            gameCreatedMessage.text = $"You chose to start the session before all players where marked ready\n\n" +
                $"If this was a real game the owner would need to start up the network session.\n" +
                $"For a typical P2P game this would mean loading the network scene, calling StartHost or similar on your networking HLAPI and setting up the environment to make it ready for others to join.\n" +
                $"For a Client/Server game this would mean allocating a server, connecting to it and configuring it to make it ready for others to join.\n" +
                $"Once the network session is set up and ready for others to connect ... the owner of the lobby should call SetGameServer as demonstrated in our source code.";

            //This will notify all other members on the lobby that the network session is ready to connect to
            lobbyManager.Lobby.SetGameServer();
        }
        /// <summary>
        /// Occurs when the server info has been set on this lobby
        /// </summary>
        /// <param name="server"></param>
        public void OnGameCreated(LobbyGameServer server)
        {
            //If we are the owner ... we already know we set this and can skip this
            if (lobbyManager.Lobby.IsOwner)
                return;

            peerMessage.text = "... Connecting ...";

            Debug.Log($"The owner of the session lobby has notified us that the server is ready to connect to and that the address of the server is\n\n" +
                $"CSteamID:{server.id}\n" +
                $"IP:{server.IpAddress}\n" +
                $"Port:{server.port}\n\n" +
                $"Yes its normal for the IP and Port to be 0 ... this is a P2P session and is assuming your using a Steam based transport which uses the CSteamID as the address");

            gameCreatedMessage.text = $"The owner of the session lobby has notified us that the server is ready to connect to and that the address of the server is\n\n" +
                $"CSteamID:{server.id}\n" +
                $"IP:{server.IpAddress}\n" +
                $"Port:{server.port}\n\n" +
                $"Yes its normal for the IP and Port to be 0 ... this is a P2P session and is assuming your using a Steam based transport which uses the CSteamID as the address";
        }
        /// <summary>
        /// We call this anytime there has been some change to the membership or data of the lobby
        /// </summary>
        public void UpdateSlotInfo()
        {
            //If we have no lobby clear all data and return
            if (!lobbyManager.HasLobby)
            {
                foreach (var pip in ownerPip)
                    pip.SetActive(false);

                foreach (var button in emptySlotButtons)
                    button.SetActive(true);

                sessionPanel.SetActive(false);

                //Clear all pips
                ownerPip[1].SetActive(false);
                readyPip[1].SetActive(false);
                waitingPip[1].SetActive(false);

                ownerPip[2].SetActive(false);
                readyPip[2].SetActive(false);
                waitingPip[2].SetActive(false);

                ownerPip[3].SetActive(false);
                readyPip[3].SetActive(false);
                waitingPip[3].SetActive(false);

                playButtonWaiting.SetActive(false);
                gameCreatedMessage.text = string.Empty;
                peerMessage.text = "... Waiting ...";

                return;
            }

            //If we got here then we have a lobby so lets update our view of it
            //Get the tracked lobby
            var lobby = lobbyManager.Lobby;
            sessionPanel.SetActive(true);

            if (lobby.IsOwner)
            {
                peerMessage.gameObject.SetActive(false);
                readyButtonWaiting.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(true);
            }
            else
            {
                peerMessage.gameObject.SetActive(true);
                readyButtonWaiting.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(false);
            }

            //Set my owner pip ... 
            ownerPip[0].SetActive(lobby.IsOwner);
            //Set my ready pip
            readyPip[0].SetActive(lobby.IsReady);
            //Set my waiting pip
            waitingPip[0].SetActive(!lobby.IsReady);

            //Get the members in the lobby
            var members = lobby.Members;

            //Clear our member cash
            partyMembersOtherThanMe.Clear();

            //Rebuild the cash
            foreach (var member in members)
            {
                //If this is not me, then add it to the cash
                if (member.user != UserData.Me)
                    partyMembersOtherThanMe.Add(member);
            }

            //Clear all pips
            ownerPip[1].SetActive(false);
            readyPip[1].SetActive(false);
            waitingPip[1].SetActive(false);

            ownerPip[2].SetActive(false);
            readyPip[2].SetActive(false);
            waitingPip[2].SetActive(false);

            ownerPip[3].SetActive(false);
            readyPip[3].SetActive(false);
            waitingPip[3].SetActive(false);

            //Set slot 1 information
            //If we have a member in slot 1 e.g. we have more than 0 members
            if (partyMembersOtherThanMe.Count > 0)
            {
                //Set the ownership pip ... 
                ownerPip[1].SetActive(partyMembersOtherThanMe[0].IsOwner);
                //Set the read pip ...
                readyPip[1].SetActive(partyMembersOtherThanMe[0].IsReady);
                //Set the waiting pip ...
                waitingPip[1].SetActive(!partyMembersOtherThanMe[0].IsReady);
                //Set the avatar
                memberSlotAvatars[0].UserData = partyMembersOtherThanMe[0].user;
                //Hide the "empty button"
                emptySlotButtons[0].SetActive(false);
            }

            //Set slot 2 information
            //If we have a member in slot 2 e.g. we have more than 1 members
            if (partyMembersOtherThanMe.Count > 1)
            {
                //Set the ownership pip ...
                ownerPip[2].SetActive(partyMembersOtherThanMe[1].IsOwner);
                //Set the read pip ...
                readyPip[2].SetActive(partyMembersOtherThanMe[1].IsReady);
                //Set the waiting pip ...
                waitingPip[2].SetActive(!partyMembersOtherThanMe[1].IsReady);
                //Set the avatar
                memberSlotAvatars[1].UserData = partyMembersOtherThanMe[1].user;
                //Hide the "empty button"
                emptySlotButtons[1].SetActive(false);
            }

            //Set slot 3 information
            //If we have a member in slot 3 e.g. we have more than 2 members
            if (partyMembersOtherThanMe.Count > 2)
            {
                //Set the ownership pip ...
                ownerPip[3].SetActive(partyMembersOtherThanMe[2].IsOwner);
                //Set the read pip ...
                readyPip[3].SetActive(partyMembersOtherThanMe[2].IsReady);
                //Set the waiting pip ...
                waitingPip[3].SetActive(!partyMembersOtherThanMe[2].IsReady);
                //Set the avatar
                memberSlotAvatars[2].UserData = partyMembersOtherThanMe[2].user;
                //Hide the "empty button"
                emptySlotButtons[2].SetActive(false);
            }
        }

        [ContextMenu("Test Running ON Steam Deck")]
        public void TestIsOnSteamDeck()
        {
            try
            {
                Debug.Log($"Running on Steamdeck: {HeathenEngineering.SteamworksIntegration.API.Utilities.Client.IsSteamRunningOnSteamDeck}");
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void OnLobbyLeave()
        {
            Debug.Log("Lobby leave invoked!");
        }
    }
}
#endif
