#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct LeaderboardData : IEquatable<SteamLeaderboard_t>, IEquatable<ulong>, IEquatable<string>
    {
        /// <summary>
        /// What is the name of the board ... if this is not to be created at run time then this must match the name as it appears in Steamworks
        /// </summary>
        public string apiName;
        /// <summary>
        /// What is the leaderboard ID ... this is nullable if null then no leaderboard has been connected
        /// </summary>
        public SteamLeaderboard_t id;
        public string DisplayName => Steamworks.SteamUserStats.GetLeaderboardName(id);
        public bool Valid => id.m_SteamLeaderboard > 0;
        public int EntryCount => API.Leaderboards.Client.GetEntryCount(id);

        /// <summary>
        /// Returns the user entry for the local user
        /// </summary>
        /// <param name="callback">The deligate to invoke when the process is complete</param>
        public void GetUserEntry(int maxDetailEntries, Action<LeaderboardEntry, bool> callback)
        {
            API.Leaderboards.Client.DownloadEntries(id, new CSteamID[] { UserData.Me }, maxDetailEntries, (results, error) =>
            {
                if (error || results.Length == 0)
                    callback.Invoke(null, error);
                else
                    callback.Invoke(results[0], error);
            });
        }
        /// <summary>
        /// Returns the top number of entries on the board
        /// </summary>
        /// <param name="count">How many top entries to return</param>
        /// <param name="maxDetailEntries">How many detail entries should be read</param>
        /// <param name="callback">This will be invoked when the process is completed</param>
        public void GetTopEntries(int count, int maxDetailEntries, Action<LeaderboardEntry[], bool> callback) => GetEntries(ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, count, maxDetailEntries, callback);
        /// <summary>
        /// Invokes the callback with the query results
        /// </summary>
        /// <param name="request">The type of range to get from the board</param>
        /// <param name="start">The index to start downloading at</param>
        /// <param name="end">The index to end downloading at</param>
        /// <param name="callback">The deligate to invoke when the process is complete</param>
        public void GetEntries(ELeaderboardDataRequest request, int start, int end, int maxDetailEntries, Action<LeaderboardEntry[], bool> callback) => API.Leaderboards.Client.DownloadEntries(id, request, start, end, maxDetailEntries, callback);
        /// <summary>
        /// Invokes the callback with the query results 
        /// </summary>
        /// <param name="users">The users to get results for</param>
        /// <param name="callback">The deligate to invoke when the process is complete</param>
        public void GetEntries(UserData[] users, int maxDetailEntries, Action<LeaderboardEntry[], bool> callback) => API.Leaderboards.Client.DownloadEntries(id, Array.ConvertAll<UserData, CSteamID>(users, p => p.id), maxDetailEntries, callback);
        public void GetAllEntries(int maxDetailEntries, Action<LeaderboardEntry[], bool> callback)
        {
            GetEntries(ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, int.MaxValue, maxDetailEntries, callback);
        }

        /// <summary>
        /// Invokes the callback with the query results 
        /// </summary>
        /// <param name="users">The users to get results for</param>
        /// <param name="callback">The deligate to invoke when the process is complete</param>
        public void GetEntries(CSteamID[] users, int maxDetailEntries, Action<LeaderboardEntry[], bool> callback) => API.Leaderboards.Client.DownloadEntries(id, users, maxDetailEntries, callback);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback">(result, error)</param>
        public static void Get(string name, Action<LeaderboardData, bool> callback) => API.Leaderboards.Client.Find(name, callback);
        public static LeaderboardData Get(ulong id) => id;
        public static LeaderboardData Get(SteamLeaderboard_t id) => id;

        public struct GetAllRequest
        {
            public bool create;
            public string name;
            public ELeaderboardDisplayType type;
            public ELeaderboardSortMethod sort;
        }

        public static void GetAll(LeaderboardObject[] boards, Action<EResult> callback)
        {
            if (boards == null || boards.Length == 0)
            {
                callback?.Invoke(EResult.k_EResultOK);
                return;
            }

            if(SteamSettings.current != null && SteamSettings.current.isDebugging)
            {
                Debug.Log($"Begining GetAll for {boards.Length} boards.");
            }
            
            if(boards.Any(b => b == null || string.IsNullOrEmpty(b.apiName)))
            {
                Debug.LogError("Errors have been found with the Leaderboard Objects proivded. Please review your Leaderboard Objects and try again.");
                callback?.Invoke(EResult.k_EResultUnexpectedError);
                return;
            }

            try
            {
                var commands = new GetAllRequest[boards.Length];
                for (int i = 0; i < boards.Length; i++)
                {
                    commands[i] = new GetAllRequest
                    {
                        create = boards[i].createIfMissing,
                        name = boards[i].apiName,
                        sort = boards[i].sortMethod,
                        type = boards[i].displayType
                    };
                }

                var bgWorker = new BackgroundWorker();
                bgWorker.DoWork += BgWorker_DoWork;
                bgWorker.RunWorkerCompleted += (sender, arguments) =>
                {
                    if (arguments.Cancelled)
                        callback?.Invoke(EResult.k_EResultCancelled);
                    else if (arguments.Error != null)
                        callback?.Invoke(EResult.k_EResultUnexpectedError);
                    else
                    {
                        var results = arguments.Result as LeaderboardData[];
                        for (int i = 0; i < results.Length; i++)
                        {
                            boards[i].data = results[i];
                        }
                        callback?.Invoke(EResult.k_EResultOK);
                    }

                    bgWorker.Dispose();
                };
                bgWorker.RunWorkerAsync(commands);
            }
            catch (Exception ex)
            {
                Debug.LogError("Get All Leaderboards experienced and unhandled exception: " + ex.ToString());
                callback?.Invoke(EResult.k_EResultUnexpectedError);
            }
        }

        public static void GetAll(GetAllRequest[] commands, Action<LeaderboardData[], EResult> callback)
        {
            if (commands == null || commands.Length == 0)
            {
                callback?.Invoke(null, EResult.k_EResultOK);
                return;
            }

            var boards = new LeaderboardData[commands.Length];

            try
            {
                var bgWorker = new BackgroundWorker();
                bgWorker.DoWork += BgWorker_DoWork;
                bgWorker.RunWorkerCompleted += (sender, arguments) =>
                {
                    if (arguments.Cancelled)
                        callback?.Invoke(null, EResult.k_EResultCancelled);
                    else if (arguments.Error != null)
                        callback?.Invoke(null, EResult.k_EResultUnexpectedError);
                    else
                    {
                        var results = arguments.Result as LeaderboardData[];
                        for (int i = 0; i < results.Length; i++)
                        {
                            boards[i] = results[i];
                        }
                        callback?.Invoke(boards, EResult.k_EResultOK);
                    }

                    bgWorker.Dispose();
                };
                bgWorker.RunWorkerAsync(commands);
            }
            catch (Exception ex)
            {
                Debug.LogError("Get All Leaderboards experienced and unhandled exception: " + ex.ToString());
                callback?.Invoke(null, EResult.k_EResultUnexpectedError);
            }
        }

        private static void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var boards = e.Argument as GetAllRequest[];
            var results = new LeaderboardData[boards.Length];

            for (int i = 0; i < boards.Length; i++)
            {
                try
                {
                    var board = boards[i];
                    bool waiting = true;
                    if (board.create)
                        GetOrCreate(board.name, board.type, board.sort, (result, error) =>
                        {
                            results[i] = result;
                            waiting = false;
                        });
                    else
                        Get(board.name, (result, error) =>
                        {
                            results[i] = result;
                            waiting = false;
                        });

                    while (waiting)
                    {
                        Thread.Sleep(10);
                    }
                }
                catch
                {
                    results[i] = default;
                }
            }

            e.Result = results;
        }

        public static void GetOrCreate(string name, ELeaderboardDisplayType displayType, ELeaderboardSortMethod sortMethod, Action<LeaderboardData, bool> callback) => API.Leaderboards.Client.FindOrCreate(name, sortMethod, displayType, callback);

        /// <summary>
        /// Uploads a score for the player to this board
        /// </summary>
        /// <param name="score"></param>
        /// <param name="method"></param>
        /// <param name="callback">{ LeaderboardScoreUploaded_t result, bool error } optional callback that will pass the results to you, if error is true it indicates a failure.</param>
        public void UploadScore(int score, ELeaderboardUploadScoreMethod method, Action<LeaderboardScoreUploaded, bool> callback = null) => API.Leaderboards.Client.UploadScore(id, method, score, null, callback);

        /// <summary>
        /// Uploads a score for the player to this board
        /// </summary>
        /// <param name="score"></param>
        /// <param name="method"></param>
        /// <param name="callback">{ LeaderboardScoreUploaded_t result, bool error } optional callback that will pass the results to you, if error is true it indicates a failure.</param>
        public void UploadScore(int score, int[] scoreDetails, ELeaderboardUploadScoreMethod method, Action<LeaderboardScoreUploaded, bool> callback = null) => API.Leaderboards.Client.UploadScore(id, method, score, scoreDetails, callback);

        /// <summary>
        /// Attempts to save, share and attach an object to the leaderboard
        /// </summary>
        /// <remarks>
        /// Note that this depends on being able to save the file to the User's Remote Storage which is a limited resoruce so use this sparingly.
        /// </remarks>
        /// <param name="fileName">The name the file should be saved as. This must be unique on the user's storage</param>
        /// <param name="JsonObject">A JsonUtility serialisable object, we will serialize this to UTF8 format and then convert to byte[] for you and upload to Steam Remote Storage</param>
        /// <param name="callback">{ LeaderbaordUGCSet_t result, bool error } optional callback that will pass the results to you, if error is true it indicates a failure from Valve.</param>
        public void AttachUGC(string fileName, object jsonObject, System.Text.Encoding encoding, Action<LeaderboardUGCSet, bool> callback = null) => API.Leaderboards.Client.AttachUGC(id, fileName, jsonObject, encoding, callback);
        public void AttachUGC(string fileName, object jsonObject, Action<LeaderboardUGCSet, bool> callback = null) => API.Leaderboards.Client.AttachUGC(id, fileName, jsonObject, System.Text.Encoding.UTF8, callback);

        public void ForceUploadScore(string score)
        {
            if (int.TryParse(score, out int result))
                UploadScore(result, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
        }
        public void ForceUploadScore(int score) => UploadScore(score, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
        public void KeepBestUploadScore(string score)
        {
            if (int.TryParse(score, out int result))
                UploadScore(result, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest);
        }
        public void KeepBestUploadScore(int score) => UploadScore(score, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest);

        #region Boilerplate
        public override string ToString()
        {
            return apiName;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() + apiName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(SteamLeaderboard_t))
                return Equals((SteamLeaderboard_t)obj);
            else if (obj.GetType() == typeof(string))
                return Equals((string)obj);
            else if (obj.GetType() == typeof(ulong))
                return Equals((ulong)obj);
            else
                return id.Equals(obj);
        }

        public bool Equals(SteamLeaderboard_t other)
        {
            return id.Equals(other);
        }

        public bool Equals(ulong other)
        {
            return id.m_SteamLeaderboard.Equals(other);
        }

        public bool Equals(string other)
        {
            return apiName.Equals(other);
        }

        public static bool operator ==(LeaderboardData l, LeaderboardData r) => l.id == r.id;
        public static bool operator ==(LeaderboardData l, ulong r) => l.id.m_SteamLeaderboard == r;
        public static bool operator ==(LeaderboardData l, string r) => l.apiName == r;
        public static bool operator ==(LeaderboardData l, SteamLeaderboard_t r) => l.id == r;
        public static bool operator !=(LeaderboardData l, LeaderboardData r) => l.id != r.id;
        public static bool operator !=(LeaderboardData l, ulong r) => l.id.m_SteamLeaderboard != r;
        public static bool operator !=(LeaderboardData l, string r) => l.apiName != r;
        public static bool operator !=(LeaderboardData l, SteamLeaderboard_t r) => l.id != r;

        public static implicit operator ulong(LeaderboardData c) => c.id.m_SteamLeaderboard;
        public static implicit operator LeaderboardData(ulong id) => new LeaderboardData { id = new SteamLeaderboard_t(id), apiName = API.Leaderboards.Client.GetName(new SteamLeaderboard_t(id)) };
        public static implicit operator SteamLeaderboard_t(LeaderboardData c) => c.id;
        public static implicit operator LeaderboardData(SteamLeaderboard_t id) => new LeaderboardData { id = id };
        public static implicit operator string(LeaderboardData c) => c.apiName;
        #endregion
    }
}
#endif