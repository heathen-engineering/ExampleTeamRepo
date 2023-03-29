#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct RankChange
    {
        public string leaderboardName;
        public SteamLeaderboard_t leaderboardId;
        public LeaderboardEntry oldEntry;
        public LeaderboardEntry newEntry;
        public int rankDelta
        {
            get
            {
                if (oldEntry != null)
                    return newEntry.entry.m_nGlobalRank - oldEntry.entry.m_nGlobalRank;
                else
                    return newEntry.entry.m_nGlobalRank;
            }
        }

        public int scoreDeta
        {
            get
            {
                if (oldEntry != null)
                    return newEntry.entry.m_nScore - oldEntry.entry.m_nScore;
                else
                    return newEntry.entry.m_nScore;
            }
        }
    }
}
#endif