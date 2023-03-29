#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;
using System.Collections.Generic;
using System;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class ClanChatMemberList : MonoBehaviour
    {
        [SerializeField]
        private ulong clanId;
        [SerializeField]
        private Transform content;
        [SerializeField]
        private GameObject template;

        public ClanData Clan
        {
            get => clanId;
            set => Apply(value);
        }

        private void Start()
        {
            if (clanId > 0)
                Apply(clanId);
        }

        public void Apply(ClanData clan)
        {
            clanId = clan;
            Refresh();
        }

        public void Refresh()
        {
            foreach (GameObject obj in content)
                Destroy(obj);

            if (clanId > 0 && Clan.IsValid)
            {
                List<UserData> members = new List<UserData>();
                members.AddRange(API.Clans.Client.GetChatMembers(Clan));
                members.Sort((a, b) => { return a.Nickname.CompareTo(b.Nickname); });

                foreach(var user in members)
                {
                    var go = Instantiate(template, content);
                    var comp = go.GetComponent<IUserProfile>();
                    comp.Apply(user);
                }
            }
        }
    }
}
#endif