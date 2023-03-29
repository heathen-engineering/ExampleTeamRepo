#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct LeaderboardUGCSet
    {
        public Steamworks.LeaderboardUGCSet_t data;
        public EResult Result => data.m_eResult;
        public LeaderboardData Leaderboard => data.m_hSteamLeaderboard;

        public static implicit operator LeaderboardUGCSet(LeaderboardUGCSet_t native) => new LeaderboardUGCSet { data = native };
        public static implicit operator LeaderboardUGCSet_t(LeaderboardUGCSet heathen) => heathen.data;
    }
}
#endif