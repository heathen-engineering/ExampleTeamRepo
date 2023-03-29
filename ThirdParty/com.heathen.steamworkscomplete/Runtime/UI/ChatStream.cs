#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;
using Steamworks;
using System.Collections.Generic;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class ChatStream : MonoBehaviour
    {
        [Tooltip("How many message entries will be retained at a time.\nOldest entries are removed as this count is exceeded")]
        [SerializeField]
        private uint historyLength = 200;
        [Tooltip("The root object underwhich chat messages will be listed.")]
        [SerializeField]
        private Transform content;
        [SerializeField]
        private GameObject messageTemplate;

        private UnityEngine.UI.ScrollRect scrollRect;
        private Queue<GameObject> messages = new Queue<GameObject>();

        private void OnEnable()
        {
            scrollRect = GetComponentInChildren<UnityEngine.UI.ScrollRect>();
        }

        public void HandheldClanMessage(ClanChatMsg message)
        {
            var go = Instantiate(messageTemplate, content);
            var comp = go.GetComponent<IChatMessage>();
            comp.Initialize(message);

            messages.Enqueue(go);
            if(messages.Count > historyLength)
            {
                var target = messages.Dequeue();
                Destroy(target);
            }

            scrollRect.verticalNormalizedPosition = 0;
        }

        public void HandheldLobbyMessage(LobbyChatMsg message)
        {
            var go = Instantiate(messageTemplate, content);
            var comp = go.GetComponent<IChatMessage>();
            comp.Initialize(message);

            messages.Enqueue(go);
            if (messages.Count > historyLength)
            {
                var target = messages.Dequeue();
                Destroy(target);
            }

            scrollRect.verticalNormalizedPosition = 0;
        }

        public void HandheldMessage(UserData sender, string message, EChatEntryType type)
        {
            var go = Instantiate(messageTemplate, content);
            var comp = go.GetComponent<IChatMessage>();
            comp.Initialize(sender, message, type);

            messages.Enqueue(go);
            if (messages.Count > historyLength)
            {
                var target = messages.Dequeue();
                Destroy(target);
            }

            scrollRect.verticalNormalizedPosition = 0;
        }
    }
}
#endif