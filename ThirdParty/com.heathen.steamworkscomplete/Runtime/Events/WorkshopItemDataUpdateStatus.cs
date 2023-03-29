#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct WorkshopItemDataUpdateStatus
    {
        public bool hasError;
        public string errorMessage;
        public WorkshopItemData data;
        public SubmitItemUpdateResult_t? submitItemUpdateResult;
    }
}
#endif