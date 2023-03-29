#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct ItemTag
    {
        public string category;
        public string tag;

        public override string ToString()
        {
            return category + ":" + tag;
        }
    }
}
#endif