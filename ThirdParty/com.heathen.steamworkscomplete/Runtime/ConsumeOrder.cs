#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET

namespace HeathenEngineering.SteamworksIntegration
{
    public struct ConsumeOrder
    {
        public Steamworks.SteamItemDetails_t detail;
        public uint quantity;
    }
}
#endif