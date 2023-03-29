#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct ItemDetail
    {
        public SteamItemDetails_t itemDetails;
        public ItemProperty[] properties;
        public string dynamicProperties;
        public ItemTag[] tags;

        public SteamItemInstanceID_t ItemId => itemDetails.m_itemId;
        public ItemData Definition => itemDetails.m_iDefinition;
        public ushort Quantity => itemDetails.m_unQuantity;
        public ESteamItemFlags Flags => (ESteamItemFlags)itemDetails.m_unFlags;
    }
}
#endif