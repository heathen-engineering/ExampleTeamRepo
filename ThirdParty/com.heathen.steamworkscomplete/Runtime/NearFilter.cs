#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct NearFilter
    {
        public string key;
        public int value;
    }
}
#endif