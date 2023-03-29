#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET

namespace HeathenEngineering.SteamworksIntegration
{
    public interface ILeaderboardEntryDisplay
    {
        LeaderboardEntry Entry { get; set; }
    }
}
#endif