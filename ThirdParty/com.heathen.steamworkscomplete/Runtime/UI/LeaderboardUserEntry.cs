#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class LeaderboardUserEntry : MonoBehaviour
    {
        public LeaderboardObject leaderboard;
        public TMPro.TextMeshProUGUI score;
        public TMPro.TextMeshProUGUI rank;

        public LeaderboardEntry Entry { get; private set; }

        private void Start()
        {
            leaderboard.UserEntryUpdated.AddListener(Refresh);
            //Refresh on a delay we do this to insure everything is loaded even if this is in the same scene as Steam Init
            Invoke(nameof(Refresh), 1.5f);
        }

        public void Refresh()
        {
            leaderboard.GetUserEntry((entry, error) =>
            {
                if (!error && entry != null)
                {
                    Refresh(entry);
                }
            });
        }

        public void Refresh(LeaderboardEntry entry)
        {
            if (entry == null)
                return;

            Entry = entry;
            score.text = entry.Score.ToString();
            rank.text = entry.Rank.ToString();
        }
    }
}
#endif