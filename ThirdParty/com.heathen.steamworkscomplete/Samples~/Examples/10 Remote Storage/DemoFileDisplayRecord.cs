#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using HeathenEngineering.SteamworksIntegration.API;
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class DemoFileDisplayRecord : MonoBehaviour
    {
        [SerializeField]
        private DemoDataModel model;
        [SerializeField]
        private TMPro.TextMeshProUGUI title;

        private RemoteStorageFile record;

        public void Initialize(RemoteStorageFile file)
        {
            record = file;
            title.text = record.name;
        }

        public void Delete()
        {
            RemoteStorage.Client.FileDelete(record.name);
        }

        public void Load()
        {
            model.LoadFileAddress(record);
        }
    }
}
#endif
