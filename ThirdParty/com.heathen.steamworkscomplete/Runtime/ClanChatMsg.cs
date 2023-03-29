#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct ClanChatMsg
    {
        /// <summary>
        /// The room this message relates to
        /// </summary>
        /// <remarks>
        /// The room.id will always be populated however undersome conditions it is possible to recieve a clan chat room message from a room the internal system is not aware of.
        /// In that event the clan.id will be invalid and the room.enterResponse will be Failed
        /// </remarks>
        public ChatRoom room;
        public EChatEntryType type;
        public string message;
        public UserData user;
    }
}
#endif