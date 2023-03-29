#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System;
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    [Obsolete("Replaced by WorkshopItem")]
    [Serializable]
    public class UGCCommunityItem : WorkshopItem
    {
        public UGCCommunityItem(SteamUGCDetails_t itemDetails) : base(itemDetails)
        {
        }
    }
}
#endif