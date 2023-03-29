using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HeathenEngineering.SteamworksIntegration.GameServerBrowserManager;

namespace HeathenEngineering.DEMO
{
    public class GameServerBrowser_UIController : MonoBehaviour
    {
        public GameObject template;
        public Transform content;

        public void Clear()
        {
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }

        public void ServerResults(ResultData results)
        {
            if (results.entries != null)
            {
                foreach (var result in results.entries)
                {
                    if (result.m_bHadSuccessfulResponse)
                    {
                        var GO = Instantiate(template, content);
                        var comp = GO.GetComponent<GameServerBrowser_EntryUI>();
                        comp.Name = result.Name;
                        comp.Players = $"{result.PlayerCount}/{result.MaxPlayerCount}";
                        comp.Ping = result.Ping.ToString();
                        comp.SteamId = result.SteamId.ToString();
                    }
                }
            }
        }
    }
}
