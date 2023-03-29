#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct StatData : IEquatable<StatData>, IEquatable<string>, IComparable<StatData>, IComparable<string>
    {
        /// <summary>
        /// The API Name as it appears in the Steamworks portal.
        /// </summary>
        [SerializeField]
        private string id;

        public float FloatValue()
        {
            API.StatsAndAchievements.Client.GetStat(id, out float value);
            return value;
        }

        public int IntValue()
        {
            API.StatsAndAchievements.Client.GetStat(id, out int value);
            return value;
        }
        /// <summary>
        /// Asynchronously downloads stats and achievements for the specified user from the server.
        /// </summary>
        /// <remarks>
        /// To keep from using too much memory, an least recently used cache (LRU) is maintained and other user's stats will occasionally be unloaded. When this happens a UserStatsUnloaded_t callback is sent. After receiving this callback the user's stats will be unavailable until this function is called again.
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="callback"></param>
        public void RequestUserStats(UserData user, Action<UserStatsReceived_t, bool> callback) => API.StatsAndAchievements.Client.RequestUserStats(user, callback);
        public bool GetValue(UserData user, out int value) => API.StatsAndAchievements.Client.GetStat(this, out value);
        public bool GetValue(UserData user, out float value) => API.StatsAndAchievements.Client.GetStat(this, out value);

        public void Set(float value) => API.StatsAndAchievements.Client.SetStat(id, value);
        public void Set(int value) => API.StatsAndAchievements.Client.SetStat(id, value);
        public void Set(float value, double length) => API.StatsAndAchievements.Client.UpdateAvgRateStat(id, value, length);

        public static void Store() => API.StatsAndAchievements.Client.StoreStats();

        #region Boilerplate
        public bool Equals(string other)
        {
            return id.Equals(other);
        }

        public bool Equals(StatData other)
        {
            return id.Equals(other.id);
        }

        public override bool Equals(object obj)
        {
            return id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public int CompareTo(StatData other)
        {
            return id.CompareTo(other.id);
        }

        public int CompareTo(string other)
        {
            return id.CompareTo(other);
        }

        public static bool operator ==(StatData l, StatData r) => l.id == r.id;
        public static bool operator ==(string l, StatData r) => l == r.id;
        public static bool operator ==(StatData l, string r) => l.id == r;
        public static bool operator !=(StatData l, StatData r) => l.id != r.id;
        public static bool operator !=(string l, StatData r) => l != r.id;
        public static bool operator !=(StatData l, string r) => l.id != r;

        public static implicit operator string(StatData c) => c.id;
        public static implicit operator StatData(string id) => new StatData { id = id };
        #endregion
    }
}
#endif