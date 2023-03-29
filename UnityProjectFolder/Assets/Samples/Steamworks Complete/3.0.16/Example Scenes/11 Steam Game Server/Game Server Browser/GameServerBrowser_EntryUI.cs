using UnityEngine;

namespace HeathenEngineering.DEMO
{
    public class GameServerBrowser_EntryUI : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI displayName;
        [SerializeField]
        private TMPro.TextMeshProUGUI players;
        [SerializeField]
        private TMPro.TextMeshProUGUI ping;
        [SerializeField]
        private TMPro.TextMeshProUGUI steamId;

        public string Name
        {
            get => displayName.text;
            set => displayName.text = value;
        }
        public string Players
        {
            get => players.text;
            set => players.text = value;
        }
        public string Ping
        {
            get => ping.text;
            set => ping.text = value;
        }
        public string SteamId
        {
            get => steamId.text;
            set => steamId.text = value;
        }
    }
}
