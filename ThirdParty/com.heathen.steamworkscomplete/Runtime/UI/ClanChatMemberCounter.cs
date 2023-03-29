#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;
using Steamworks;
using System.Collections;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class ClanChatMemberCounter : MonoBehaviour
    {
        [SerializeField]
        private ulong clanId;
        [SerializeField]
        internal string prefix;
        [SerializeField]
        internal string suffix;

        public ClanData Clan
        {
            get => clanId;
            set => Apply(value);
        }

        private TMPro.TextMeshProUGUI label;

        private void OnEnable()
        {
            label = GetComponent<TMPro.TextMeshProUGUI>();
            API.Clans.Client.EventGameConnectedChatJoin.AddListener(HandleJoin);
            API.Clans.Client.EventGameConnectedChatLeave.AddListener(HandleLeve);
        }

        private void Start()
        {
            if (API.App.Initialized)
            {
                if (clanId > 0)
                    Refresh();
            }
            else
            {
                API.App.evtSteamInitialized.AddListener(DelayUpdate);
            }
        }

        private void DelayUpdate()
        {
            if (clanId > 0)
                Refresh();

            API.App.evtSteamInitialized.RemoveListener(DelayUpdate);
        }

        private void HandleLeve(UserLeaveData data)
        {
            if (data.room.clan == clanId)
                Refresh();
        }

        private void HandleJoin(ChatRoom room, UserData user)
        {
            if (room.clan == clanId)
                Refresh();
        }

        public void Apply(ClanData clan)
        {
            clanId = clan;
            Refresh();
        }

        public void Refresh()
        {
            if (clanId > 0)
                label.text = prefix + API.Clans.Client.GetChatMemberCount(Clan).ToString() + suffix;
        }
    }
}
#endif