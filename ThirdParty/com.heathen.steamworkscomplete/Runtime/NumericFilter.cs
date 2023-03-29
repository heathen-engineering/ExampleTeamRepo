#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct NumericFilter
    {
        public string key;
        public int value;
        public ELobbyComparison comparison;
    }
}
#endif