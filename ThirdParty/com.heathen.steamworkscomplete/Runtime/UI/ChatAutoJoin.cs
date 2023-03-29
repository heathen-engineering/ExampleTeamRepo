#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System.Collections;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    [RequireComponent(typeof(ClanChatDirector))]
    public class ChatAutoJoin : MonoBehaviour
    {
        [SerializeField]
        private ulong clanId;

        private void Start()
        {
            if (API.App.Initialized)
            {
                var director = GetComponent<ClanChatDirector>();
                director.Join(clanId);
            }
            else
            {
                API.App.evtSteamInitialized.AddListener(DelayUpdate);
            }
        }

        private void DelayUpdate()
        {
            var director = GetComponent<ClanChatDirector>();
            director.Join(clanId);

            API.App.evtSteamInitialized.RemoveListener(DelayUpdate);
        }
    }
}
#endif