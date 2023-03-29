#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public class UnityLeaderboardRankUpdateEvent : UnityEvent<LeaderboardEntry>
    { }
}
#endif
