#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using System.Collections.Generic;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public class CreateArguments
    {
        public enum UseHintOptions
        {
            None,
            Group,
            Session
        }
        /// <summary>
        /// How will this lobby be used? This is an optional feature. If set to Group or Session then features of the LobbyData object can be used in code to fetch the created lobby such as LobbyData.GetGroup(...)
        /// </summary>
        [UnityEngine.Tooltip("How will this lobby be used? This is an optional feature. If set to Group or Session then features of the LobbyData object can be used in code to fetch the created lobby such as LobbyData.GetGroup(...)")]
        public UseHintOptions usageHint = UseHintOptions.None;
        /// <summary>
        /// The name to assign to the lobby when it is created
        /// </summary>
        [UnityEngine.Tooltip("The name to assign to the lobby when it is created")]
        public string name;
        /// <summary>
        /// The number of slots the newly created lobby should have
        /// </summary>
        [UnityEngine.Tooltip("The number of slots the newly created lobby should have")]
        public int slots;
        /// <summary>
        /// The type of lobby to create
        /// </summary>
        [UnityEngine.Tooltip("The type of lobby to create")]
        public ELobbyType type;
        /// <summary>
        /// The metadata to add to the lobby after creation. This is a dictionary and fields will not be repeated
        /// </summary>
        [UnityEngine.Tooltip("The metadata to add to the lobby after creation. This is a dictionary and fields will not be repeated")]
        public List<MetadataTempalate> metadata = new List<MetadataTempalate>();
    }
}
#endif