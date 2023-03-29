#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    public class ClanChatDirector : MonoBehaviour
    {
        [Header("Events")]
        public GameConnectedChatJoinEvent evtJoin;
        public GameConnectedClanChatMsgEvent evtRecieved;
        public GameConnectedChatLeaveEvent evtLeave;

        public UserData[] Members
        {
            get
            {
                if (InRoom)
                    return chatRoom.Value.Members;
                else
                    return new UserData[0];
            }
        }
        /// <summary>
        /// Checks if the Steam Group chat room is open in the Steam UI.
        /// </summary>
        public bool IsOpenInSteam
        {
            get
            {
                if (InRoom)
                    return chatRoom.Value.IsOpenInSteam;
                else
                    return false;
            }
        }
        public bool InRoom => chatRoom.HasValue && chatRoom.Value.enterResponse == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess;

        public ChatRoom ChatRoom => chatRoom.HasValue ? chatRoom.Value : default;

        private ChatRoom? chatRoom = null;

        private void OnEnable()
        {
            API.Clans.Client.EventChatMessageRecieved.AddListener(HandleNewMessage);
            API.Clans.Client.EventGameConnectedChatJoin.AddListener(HandleJoined);
            API.Clans.Client.EventGameConnectedChatLeave.AddListener(HandleLeave);
        }

        private void OnDisable()
        {
            API.Clans.Client.EventChatMessageRecieved.RemoveListener(HandleNewMessage);
            API.Clans.Client.EventGameConnectedChatJoin.RemoveListener(HandleJoined);
            API.Clans.Client.EventGameConnectedChatLeave.RemoveListener(HandleLeave);
        }

        public void Join(ClanData clan)
        {
            API.Clans.Client.JoinChatRoom(clan, (result, error) =>
            {
                if (!error)
                    chatRoom = result;
                else
                    Debug.LogWarning("Steam client responded with an IO error when attempting to join Clan chat for " + clan.ToString());
            });
        }

        public void Leave()
        {
            if (InRoom)
            {
                chatRoom.Value.Leave();
                chatRoom = null;
            }
        }

        public void Send(string message)
        {
            if (InRoom)
            {
                chatRoom.Value.SendMessage(message);
            }
        }

        public void OpenInSteam()
        {
            if (InRoom)
                chatRoom.Value.OpenChatWindowInSteam();
        }

        private void HandleLeave(UserLeaveData arg0)
        {
            if (InRoom && arg0.room.id == chatRoom.Value.id)
                evtLeave.Invoke(arg0);
        }

        private void HandleJoined(ChatRoom arg0, UserData arg1)
        {
            if (InRoom && arg0.id == chatRoom.Value.id)
                evtJoin.Invoke(arg0, arg1);
        }

        private void HandleNewMessage(ClanChatMsg arg0)
        {
            if (InRoom && arg0.room.id == chatRoom.Value.id)
                evtRecieved.Invoke(arg0);
        }
    }
}
#endif