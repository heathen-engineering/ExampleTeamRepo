#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct MetadataTempalate
    {
        /// <summary>
        /// The key or field name to be used. names will not be duplicated, if you add another field of the same name it will overwrite, not duplicate
        /// </summary>
        [UnityEngine.Tooltip("The key or field name to be used. names will not be duplicated, if you add another field of the same name it will overwrite, not duplicate")]
        public string key;
        /// <summary>
        /// The value of the field to be applied, empty values are ignored
        /// </summary>
        [UnityEngine.Tooltip("The value of the field to be applied, empty values are ignored")]
        public string value;
    }
}
#endif