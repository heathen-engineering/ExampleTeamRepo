#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class LeaderboardEntryUIRecord : MonoBehaviour, ILeaderboardEntryDisplay
    {
        [SerializeField]
        private SetUserAvatar avatar;
        [SerializeField]
        private SetUserName userName;
        [SerializeField]
        private TMPro.TextMeshProUGUI score;
        [SerializeField]
        private TMPro.TextMeshProUGUI rank;

        private LeaderboardEntry _entery;
        public LeaderboardEntry Entry 
        { 
            get => _entery;
            set => SetEntry(value);
        }

        private void SetEntry(LeaderboardEntry entry)
        {
            avatar.UserData = entry.User;
            userName.UserData = entry.User;
            score.text = entry.Score.ToString();
            rank.text = entry.Rank.ToString();

            _entery = entry;
        }
    }
}
#endif