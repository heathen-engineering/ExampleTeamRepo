﻿#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct WorkshopItemDataCreateStatus
    {
        public bool hasError;
        public string errorMessage;
        public WorkshopItemData data;
        public CreateItemResult_t? createItemResult;
        public SubmitItemUpdateResult_t? submitItemUpdateResult;
    }
}
#endif