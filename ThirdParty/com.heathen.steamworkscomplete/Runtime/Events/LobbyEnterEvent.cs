#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
#if STEAMWORKSNET
    [System.Serializable]
    public class LobbyEnterEvent : UnityEvent<LobbyEnter_t> { }
#elif FACEPUNCH
    [System.Serializable]
    public class LobbyEnterEvent : UnityEvent<Lobby> { }
#endif
}
#endif