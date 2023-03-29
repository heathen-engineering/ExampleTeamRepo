#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct PartyBeaconDetails
    {
        public PartyBeaconID_t id;
        public UserData owner;
        public SteamPartyBeaconLocation_t location;
        public string metadata;
    }
    //*/

}
#endif