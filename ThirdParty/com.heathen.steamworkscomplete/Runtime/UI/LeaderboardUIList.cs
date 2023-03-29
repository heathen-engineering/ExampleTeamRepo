#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class LeaderboardUIList : MonoBehaviour
    {
        public Transform collection;
        public GameObject template;
        
        [Header("Events")]
        public UnityEvent Enabled;

        private List<GameObject> createdRecords = new List<GameObject>();

        private void OnEnable()
        {
            Enabled.Invoke();
        }

        public void Display(LeaderboardEntry[] entries)
        {
            foreach(var entry in createdRecords)
            {
                Destroy(entry);
            }
            createdRecords.Clear();

            foreach(var entry in entries)
            {
                var go = Instantiate(template, collection);
                createdRecords.Add(go);

                var display = go.GetComponent<ILeaderboardEntryDisplay>();
                if (display != null)
                    display.Entry = entry;
            }
        }
    }
}
#endif