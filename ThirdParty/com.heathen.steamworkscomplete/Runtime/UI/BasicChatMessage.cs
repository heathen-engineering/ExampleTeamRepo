#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    [HelpURL("https://kb.heathen.group/assets/steamworks/for-unity-game-engine/interfaces/ichatmessage")]
    public class BasicChatMessage : MonoBehaviour, IChatMessage
    {
        [SerializeField]
        private SetUserAvatar avatar;
        [SerializeField]
        private SetUserName username;
        [SerializeField]
        private string dateTimeFormat = "HH:mm:ss";
        [SerializeField]
        private TMPro.TextMeshProUGUI datetime;
        [SerializeField]
        private TMPro.TextMeshProUGUI message;
        
        public UserData User { get; private set; }

        public byte[] Data { get; private set; }

        public string Message { get; private set; }

        public DateTime ReceivedAt { get; private set; }

        public EChatEntryType Type { get; private set; }

        public bool IsExpanded
        {
            get
            {
                if (avatar != null)
                    return avatar.gameObject.activeSelf;
                else if (datetime != null)
                    return datetime.gameObject.activeSelf;
                else
                    return false;
            }
            set
            {
                if (avatar != null)
                    avatar.gameObject.SetActive(value);
                if (datetime != null)
                    datetime.gameObject.SetActive(value);
            }
        }

        public GameObject GameObject => gameObject;

        public void Initialize(ClanChatMsg message)
        {
            User = message.user;
            Type = message.type;
            Message = message.message;
            Data = System.Text.Encoding.UTF8.GetBytes(message.message);
            ReceivedAt = DateTime.Now;

            if (avatar != null)
                avatar.UserData = User;

            if (username != null)
                username.UserData = User;

            this.message.text = Message;

            if (datetime != null)
                datetime.text = ReceivedAt.ToString(dateTimeFormat);
        }

        public void Initialize(LobbyChatMsg message)
        {
            User = message.sender;
            Type = message.type;
            Message = message.Message;
            Data = message.data;
            ReceivedAt = DateTime.Now;

            if (avatar != null)
                avatar.UserData = User;

            if (username != null)
                username.UserData = User;

            this.message.text = Message;

            if (datetime != null)
                datetime.text = ReceivedAt.ToString(dateTimeFormat);
        }

        public void Initialize(UserData sender, string message, EChatEntryType type)
        {
            User = sender;
            Type = type;
            Message = message;
            Data = System.Text.Encoding.UTF8.GetBytes(message);
            ReceivedAt = DateTime.Now;

            if (avatar != null)
                avatar.UserData = User;

            if (username != null)
                username.UserData = User;

            this.message.text = Message;

            if (datetime != null)
                datetime.text = ReceivedAt.ToString(dateTimeFormat);
        }
    }
}
#endif