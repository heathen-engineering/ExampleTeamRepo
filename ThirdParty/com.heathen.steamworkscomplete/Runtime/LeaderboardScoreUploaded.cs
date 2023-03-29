#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct LeaderboardScoreUploaded
    {
        public Steamworks.LeaderboardScoreUploaded_t data;
        public bool Success => data.m_bSuccess != 0;
        public bool ScoreChanged => data.m_bScoreChanged != 0;
        public LeaderboardData Leaderboard => data.m_hSteamLeaderboard;
        public int Score => data.m_nScore;
        public int GlobalRankNew => data.m_nGlobalRankNew;
        public int GlobalRankPrevious => data.m_nGlobalRankPrevious;

        public static implicit operator LeaderboardScoreUploaded(LeaderboardScoreUploaded_t native) => new LeaderboardScoreUploaded { data = native };
        public static implicit operator LeaderboardScoreUploaded_t(LeaderboardScoreUploaded heathen) => heathen.data;
    }
}
#endif