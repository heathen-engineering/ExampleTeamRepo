#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class ClanList : MonoBehaviour
    {
        public enum Filter
        {
            Any,
            OfficalGroups,
            PublicGroups,
            NonOfficalGroups,
            PrivateGroups, 
            Followed
        }

        [SerializeField]
        private Filter filter = Filter.Any;
        [SerializeField]
        private Transform content;
        [SerializeField]
        private GameObject recordTemplate;

        public Filter ActiveFilter
        {
            get => filter;
            set
            {
                filter = value;
                UpdateDisplay();
            }
        }

        private Dictionary<ClanData, ClanProfile> records = new Dictionary<ClanData, ClanProfile>();

        private void OnEnable()
        {
            if (API.App.Initialized)
            {
                UpdateDisplay();
            }
            else
            {
                API.App.evtSteamInitialized.AddListener(DelayUpdate);
            }
        }

        private void OnDisable()
        {
            Clear();
        }

        private void DelayUpdate()
        {
            UpdateDisplay();
            API.App.evtSteamInitialized.RemoveListener(DelayUpdate);
        }

        private void Remove(ClanData clan)
        {
            if (records.ContainsKey(clan))
            {
                var target = records[clan];
                records.Remove(clan);
                Destroy(target.gameObject);
            }
        }

        private void Add(ClanData clan)
        {
            //Add the user and then resort the display
            if (!records.ContainsKey(clan))
            {
                AddNewRecord(clan);
                SortRecords();
            }
            else
                records[clan].Clan = clan;
        }

        private void AddNewRecord(ClanData clan)
        {
            var go = Instantiate(recordTemplate, content);
            var comp = go.GetComponent<ClanProfile>();
            comp.Clan = clan;
            records.Add(clan, comp);
        }

        private void SortRecords()
        {
            var keys = records.Keys.ToList();
            keys.Sort((a, b) => { return a.Name.CompareTo(b.Name); });

            foreach (var key in keys)
            {
                records[key].transform.SetAsLastSibling();
            }
        }

        public void Clear()
        {
            if (content.childCount > 0)
                try
                {
                    foreach (GameObject go in content)
                        try
                        {
                            Destroy(go);
                        }
                        catch { }
                }
                catch { }
        }

        public void UpdateDisplay()
        {
            Clear();


            List<ClanData> filtered = new List<ClanData>();
            var clans = new List<ClanData>(API.Clans.Client.GetClans());
            var followed = new List<ClanData>();

            API.Friends.Client.GetFollowed((r) =>
            {
                if(r != null && r.Length > 0)
                {
                    var subset = r.Where(p => p.GetEAccountType() == Steamworks.EAccountType.k_EAccountTypeClan);
                    if (subset.Count() > 0)
                    {
                        foreach (var id in subset)
                        {
                            clans.Add(id);
                            followed.Add(id);
                        }
                    }
                }

                if (filter == Filter.Followed)
                {
                    foreach (var clan in followed)
                        if (!records.ContainsKey(clan))
                            AddNewRecord(clan);
                }
                else
                {
                    foreach (var clan in clans)
                    {
                        if (MatchFilter(clan))
                            filtered.Add(clan);
                    }

                    foreach (var clan in filtered)
                        if (!records.ContainsKey(clan))
                            AddNewRecord(clan);
                }

                SortRecords();
            });
        }

        public bool MatchFilter(ClanData clan)
        {
            switch(filter)
            {
                case Filter.Any:
                    return true;
                case Filter.NonOfficalGroups:
                    return !clan.IsOfficalGameGroup;
                case Filter.OfficalGroups:
                    return clan.IsOfficalGameGroup;
                case Filter.PrivateGroups:
                    return !clan.IsPublic && !clan.IsOfficalGameGroup;
                case Filter.PublicGroups:
                    return clan.IsPublic;
                default:
                    return false;
            }
        }
    }
}
#endif