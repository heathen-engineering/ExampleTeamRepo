#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;
#if ENABLE_INPUT_SYSTEM
#endif

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class LobbyBrowser_UIController : MonoBehaviour
    {
        public LobbyManager lobbyManager;
        public GameObject template;
        public Transform root;

        /// <summary>
        /// This is connected to the Lobby Found event of the LobbyManager located on the Managers GameObject
        /// It is invoked whenever the Lobby Manager completes a search and has new lobbies idenitifed
        /// </summary>
        /// <param name="results"></param>
        public void LobbyResults(LobbyData[] results)
        {
            //First we clean out the old records
            foreach (Transform tran in root)
                Destroy(tran.gameObject);

            //Next we spawn new records for each lobby
            foreach(var lobby in results)
            {
                var GO = Instantiate(template, root);
                var com = GO.GetComponent<LobbyBrowser_LobbyRecord>();
                com.SetLobby(lobby, lobbyManager);
            }
        }
    }
}
#endif
