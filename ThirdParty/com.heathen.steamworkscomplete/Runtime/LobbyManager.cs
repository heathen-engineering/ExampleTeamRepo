#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamworksIntegration
{
    /// <summary>
    /// Helps you find or create a lobby.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is meant to be attached to your lobby UI, party UI or similar and manages 1 single lobby.
    /// It can be used to search for a matching lobby and automatically join it, 
    /// to create a lobby or to browse for lobby that match its <see cref="searchArguments"/>.
    /// </para>
    /// <para>
    /// When you create or join a lobby using this tool it will link that joined or created lobby and provide access to that lobbies events. methods and members.
    /// You can then use this object as an interface between your UI and a specific lobby to create lobby windows, party windows, lobby chats and more.
    /// </para>
    /// </remarks>
    [HelpURL("https://kb.heathen.group/assets/steamworks/for-unity-game-engine/components/lobby-manager")]
    public class LobbyManager : MonoBehaviour
    {
        [Tooltip("The values to be used when searching for a lobby")]
        public SearchArguments searchArguments = new SearchArguments();
        [Tooltip("The values to be used when creating a new lobby")]
        public CreateArguments createArguments = new CreateArguments();

        [Header("Events")]
        public LobbyDataListEvent evtFound;
        /// <summary>
        /// Occurs when the local user enters a lobby as a responce to a join
        /// </summary>
        public LobbyDataEvent evtEnterSuccess;
        /// <summary>
        /// Occurs when the local user tried but failed to enter a lobby
        /// </summary>
        public LobbyResponceEvent evtEnterFailed;
        /// <summary>
        /// Occurs when the local user creates a lobby
        /// </summary>
        public LobbyDataEvent evtCreated;
        /// <summary>
        /// Occurs when the local user tried but failed to create a lobby
        /// </summary>
        public EResultEvent evtCreateFailed;
        /// <summary>
        /// Occurs when the local user attempts to quick match but fails to find a match or resolve the quick match
        /// </summary>
        public UnityEvent evtQuickMatchFailed;
        /// <summary>
        /// Occurs when any data is updated on the lobby be that lobby metadata or a members metadata
        /// </summary>
        public LobbyDataUpdateEvent evtDataUpdated;
        /// <summary>
        /// Occurs when the local user leaves the managed lobby
        /// </summary>
        public UnityEvent evtLeave;
        /// <summary>
        /// Occurs when the local user is asked to leave the lobby via Heathen's Kick system
        /// </summary>
        public UnityEvent evtAskedToLeave;
        /// <summary>
        /// Occurs when the <see cref="GameServer"/> information is first set on the lobby
        /// </summary>
        public GameServerSetEvent evtGameCreated;
        /// <summary>
        /// Occurs when the local user is a member of a lobby and a new member joins that lobby
        /// </summary>
        public UserDataEvent evtUserJoined;
        /// <summary>
        /// Occurts when the local user is a member of a lobby and another fellow member leveas the lobby
        /// </summary>
        public UserLeaveEvent evtUserLeft;
        /// <summary>
        /// The lobby this manager is currently managing
        /// </summary>
        /// <remarks>
        /// This will automatically be updated when you use the Lobby Manager to create, join or leave a lobby.
        /// If you manually create, join or leave a lobby you must update this field your self.
        /// To clear the value assign <see cref="CSteamID.Nil"/>
        /// </remarks>
        public LobbyData Lobby
        {
            get;
            set;
        }
        /// <summary>
        /// Returns true if the <see cref="Lobby"/> value is populatted and resolves to a non-empty lobby
        /// </summary>
        public bool HasLobby => Lobby != CSteamID.Nil.m_SteamID && SteamMatchmaking.GetNumLobbyMembers(Lobby) > 0;
        /// <summary>
        /// Returns true if the local user is the owner of the managed lobby
        /// </summary>
        public bool IsPlayerOwner => HasLobby && Lobby.IsOwner;
        /// <summary>
        /// Returns true if all members in the lobby have indicated that they are Read via the Heathen Ready Check system
        /// </summary>
        public bool AllPlayersReady => HasLobby && Lobby.AllPlayersReady;
        /// <summary>
        /// Is the local player ready
        /// </summary>
        /// <remarks>
        /// You can assigne this value to update the local player's LobbyMember accordingly for the Heathen Ready Check system
        /// </remarks>
        public bool IsPlayerReady
        {
            get => HasLobby && API.Matchmaking.Client.GetLobbyMemberData(Lobby, API.User.Client.Id, LobbyData.DataReady) == "true";
            set => API.Matchmaking.Client.SetLobbyMemberData(Lobby, LobbyData.DataReady, value.ToString().ToLower());
        }
        /// <summary>
        /// Returns true when the managed lobby is full e.g. unable to take more members
        /// </summary>
        public bool Full => HasLobby && Lobby.Full;
        public int Slots => HasLobby ? SteamMatchmaking.GetLobbyMemberLimit(Lobby) : 0;
        public int MemberCount => HasLobby ? SteamMatchmaking.GetNumLobbyMembers(Lobby) : 0;
        /// <summary>
        /// Returns true if the Heathen TypeSet feature has been populated to indicate the type of lobby this is
        /// </summary>
        public bool IsTypeSet => HasLobby && Lobby.IsTypeSet;
        /// <summary>
        /// Returns the type of lobby this is, this is a feature of Heathen's Lobby tools. Valve does not actually expose this so this will only work for lobbies
        /// created by Heathen's tools such as <see cref="LobbyManager"/> and of course <see cref="API.Matchmaking.Client"/>
        /// </summary>
        public ELobbyType Type
        {
            get => Lobby.Type;
            set
            {
                var l = Lobby;
                l.Type = value;
            }
        }
        /// <summary>
        /// The max number of members this lobby supports
        /// </summary>
        /// <remarks>
        /// The owner of the lobby can set this value to update the max allowed
        /// </remarks>
        public int MaxMembers
        {
            get => HasLobby ? API.Matchmaking.Client.GetLobbyMemberLimit(new CSteamID(Lobby)) : 0;
            set => API.Matchmaking.Client.SetLobbyMemberLimit(new CSteamID(Lobby), value);
        }
        /// <summary>
        /// Does the managed lobby have game server data set on it?
        /// </summary>
        /// <remarks>
        /// <see cref="LobbyData.SetGameServer"/> for more information
        /// </remarks>
        public bool HasServer => SteamMatchmaking.GetLobbyGameServer(Lobby, out _, out _, out _);
        /// <summary>
        /// The game server information set on the managed lobby if any
        /// </summary>
        public LobbyGameServer GameServer => API.Matchmaking.Client.GetLobbyGameServer(Lobby);

        public string this[string key]
        {
            get => Lobby[key];
            set
            {
                var lobby = Lobby;
                lobby[key] = value;
            }
        }

        public LobbyMemberData this[UserData user] => Lobby[user];

        private void OnEnable()
        {
            API.Matchmaking.Client.EventLobbyAskedToLeave.AddListener(HandleAskedToLeave);
            API.Matchmaking.Client.EventLobbyDataUpdate.AddListener(HandleLobbyDataUpdate);
            API.Matchmaking.Client.EventLobbyLeave.AddListener(HandleLobbyLeave);
            API.Matchmaking.Client.EventLobbyGameCreated.AddListener(HandleGameServerSet);
            API.Matchmaking.Client.EventLobbyChatUpdate.AddListener(HandleChatUpdate);
        }

        private void OnDisable()
        {
            API.Matchmaking.Client.EventLobbyAskedToLeave.RemoveListener(HandleAskedToLeave);
            API.Matchmaking.Client.EventLobbyDataUpdate.RemoveListener(HandleLobbyDataUpdate);
            API.Matchmaking.Client.EventLobbyLeave.RemoveListener(HandleLobbyLeave);
            API.Matchmaking.Client.EventLobbyGameCreated.RemoveListener(HandleGameServerSet);
            API.Matchmaking.Client.EventLobbyChatUpdate.RemoveListener(HandleChatUpdate);
        }

        private void HandleChatUpdate(LobbyChatUpdate_t arg0)
        {
            if(arg0.m_ulSteamIDLobby == Lobby)
            {
                var state = (EChatMemberStateChange)arg0.m_rgfChatMemberStateChange;
                if (state == EChatMemberStateChange.k_EChatMemberStateChangeEntered)
                    evtUserJoined?.Invoke(arg0.m_ulSteamIDUserChanged);
                else
                    evtUserLeft?.Invoke(new UserLobbyLeaveData { user = arg0.m_ulSteamIDUserChanged, state = state });
            }
        }

        private void HandleGameServerSet(LobbyGameCreated_t arg0)
        {
            if (arg0.m_ulSteamIDLobby == Lobby)
                evtGameCreated.Invoke(GameServer);
        }

        private void HandleLobbyLeave(LobbyData arg0)
        {
            if (arg0 == Lobby)
            {
                Lobby = default;
                evtLeave.Invoke();
            }
        }

        private void HandleAskedToLeave(LobbyData arg0)
        {
            if (arg0 == Lobby)
                evtAskedToLeave.Invoke();
        }

        private void HandleLobbyDataUpdate(LobbyDataUpdateEventData arg0)
        {
            if (arg0.lobby == Lobby)
                evtDataUpdated.Invoke(arg0);
        }

        /// <summary>
        /// Changes the type of the current lobby if any
        /// </summary>
        /// <remarks>
        /// This will also update the type in the <see cref="createArguments"/> record
        /// </remarks>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool SetType(ELobbyType type)
        {
            createArguments.type = type;
            return API.Matchmaking.Client.SetLobbyType(Lobby, type);
        }
        /// <summary>
        /// Sets the lobby joinable or not
        /// </summary>
        /// <param name="makeJoinable"></param>
        /// <returns></returns>
        public bool SetJoinable(bool makeJoinable) => API.Matchmaking.Client.SetLobbyJoinable(Lobby, makeJoinable);
        /// <summary>
        /// Searches for a match based on <see cref="searchArguments"/>, if none is found it will create a lobby matching the <see cref="createArguments"/>
        /// </summary>
        public void QuickMatch(bool createOnFail = true)
        {
            if (HasLobby)
            {
                Lobby.Leave();
                Lobby = CSteamID.Nil.m_SteamID;
            }

            API.Matchmaking.Client.AddRequestLobbyListDistanceFilter(searchArguments.distance);

            if (searchArguments.slots > 0)
                API.Matchmaking.Client.AddRequestLobbyListFilterSlotsAvailable(searchArguments.slots);

            foreach (var near in searchArguments.nearValues)
                API.Matchmaking.Client.AddRequestLobbyListNearValueFilter(near.key, near.value);

            foreach (var numeric in searchArguments.numericFilters)
                API.Matchmaking.Client.AddRequestLobbyListNumericalFilter(numeric.key, numeric.value, numeric.comparison);

            foreach (var text in searchArguments.stringFilters)
                API.Matchmaking.Client.AddRequestLobbyListStringFilter(text.key, text.value, text.comparison);

            API.Matchmaking.Client.AddRequestLobbyListResultCountFilter(1);

            API.Matchmaking.Client.RequestLobbyList((r, e) =>
            {
                if (!e && r.Length >= 1)
                {
                    API.Matchmaking.Client.JoinLobby(r[0], (r2, e2) =>
                    {
                        var responce = r2.Response;

                        if (!e2 && responce == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                        {
                            if (API.App.isDebugging)
                                Debug.Log("Quick match found, joined lobby: " + r2.Lobby.ToString());

                            Lobby = r2.Lobby;
                            evtFound?.Invoke(r);
                            evtEnterSuccess.Invoke(r[0]);
                        }
                        else
                        {
                            if (responce == EChatRoomEnterResponse.k_EChatRoomEnterResponseLimited)
                            {
                                Debug.LogError("This user is limited and cannot create or join lobbies or chats.");
                                evtEnterFailed.Invoke(responce);
                            }
                            else
                            {
                                if (createOnFail)
                                {
                                    if (API.App.isDebugging)
                                        Debug.LogError("Quick match failed, lobbies found but failed to join ... creating lobby.");

                                    Create();
                                }
                                else
                                    evtQuickMatchFailed.Invoke();
                            }
                        }
                    });
                }
                else
                {
                    if (createOnFail)
                    {
                        if (API.App.isDebugging)
                            Debug.Log("Quick match failed, no lobbies found ... creating lobby.");

                        Create();
                    }
                    else
                        evtQuickMatchFailed.Invoke();
                }
            });
        }
        /// <summary>
        /// Creates a new lobby with the data found in <see cref="createArguments"/>
        /// </summary>
        public void Create() => Create(null);
        public void Create(string name, ELobbyType type, int slots, params MetadataTempalate[] metadata)
        {
            createArguments.name = name;
            createArguments.type = type;
            createArguments.slots = slots;
            createArguments.metadata.Clear();
            createArguments.metadata.AddRange(metadata);
            Create(null);
        }
        public void Create(ELobbyType type, int slots, params MetadataTempalate[] metadata)
        {
            createArguments.name = string.Empty;
            createArguments.type = type;
            createArguments.slots = slots;
            createArguments.metadata.Clear();
            createArguments.metadata.AddRange(metadata);
            Create(null);
        }
        /// <summary>
        /// Creates a new lobby with the data found in <see cref="createArguments"/> and invokes the callback when complete
        /// </summary>
        public void Create(Action<EResult, LobbyData, bool> callback)
        {
            if (HasLobby)
            {
                Lobby.Leave();
                Lobby = CSteamID.Nil.m_SteamID;
            }

            API.Matchmaking.Client.CreateLobby(createArguments.type, createArguments.slots, (result, lobby, ioError) =>
            {
                if (!ioError)
                {
                    if (result == EResult.k_EResultOK)
                    {
                        if (API.App.isDebugging)
                            Debug.Log("New lobby created.");

                        Lobby = lobby;

                        if (createArguments.usageHint == CreateArguments.UseHintOptions.Group)
                            lobby.IsGroup = true;
                        else if (createArguments.usageHint == CreateArguments.UseHintOptions.Session)
                            lobby.IsSession = true;

                        lobby[LobbyData.DataName] = createArguments.name;
                        foreach (var data in createArguments.metadata)
                            lobby[data.key] = data.value;

                        evtCreated?.Invoke(lobby);
                    }
                    else
                    {
                        Debug.Log($"No lobby created Steam API responce code: {result}");
                        evtCreateFailed?.Invoke(result);
                    }
                }
                else
                {
                    Debug.LogError("Lobby creation failed with message: IOFailure\nSteam API responded with a general IO Failure.");
                    evtCreateFailed?.Invoke(EResult.k_EResultIOFailure);
                }

                callback?.Invoke(result, lobby, ioError);
            });
        }
        /// <summary>
        /// Searches for lobbies that match the <see cref="searchArguments"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Remimber Lobbies are a matchmaking feature, the first lobby returned is generally he best, lobby search is not intended to return all possible results simply the best matching options.
        /// </para>
        /// </remarks>
        /// <param name="maxResults">The maximum number of lobbies to return. lower values are better.</param>
        public void Search(int maxResults)
        {
            if (maxResults <= 0)
                return;

            API.Matchmaking.Client.AddRequestLobbyListDistanceFilter(searchArguments.distance);

            if (searchArguments.slots > 0)
                API.Matchmaking.Client.AddRequestLobbyListFilterSlotsAvailable(searchArguments.slots);

            foreach (var near in searchArguments.nearValues)
                API.Matchmaking.Client.AddRequestLobbyListNearValueFilter(near.key, near.value);

            foreach (var numeric in searchArguments.numericFilters)
                API.Matchmaking.Client.AddRequestLobbyListNumericalFilter(numeric.key, numeric.value, numeric.comparison);

            foreach (var text in searchArguments.stringFilters)
                API.Matchmaking.Client.AddRequestLobbyListStringFilter(text.key, text.value, text.comparison);

            API.Matchmaking.Client.AddRequestLobbyListResultCountFilter(maxResults);

            API.Matchmaking.Client.RequestLobbyList((r, e) =>
            {
                if (!e)
                {
                    evtFound?.Invoke(r);
                }
                else
                {
                    evtFound?.Invoke(new LobbyData[0]);
                }
            });
        }
        /// <summary>
        /// Joins the indicated steam lobby
        /// </summary>
        /// <param name="lobby"></param>
        public void Join(LobbyData lobby)
        {
            if (HasLobby)
            {
                Lobby.Leave();
                Lobby = CSteamID.Nil.m_SteamID;
            }

            API.Matchmaking.Client.JoinLobby(lobby, (r, e) =>
            {
                if(!e)
                {
                    if (r.Response == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                    {
                        if (API.App.isDebugging)
                            Debug.Log("Joined lobby: " + lobby.ToString());

                        Lobby = r.Lobby;
                        evtEnterSuccess.Invoke(lobby);
                    }
                    else
                        evtEnterFailed.Invoke(r.Response);
                }
                else
                    evtEnterFailed.Invoke(EChatRoomEnterResponse.k_EChatRoomEnterResponseError);
            });
        }

        public void Join(ulong lobby)
        {
            if (HasLobby)
            {
                Lobby.Leave();
                Lobby = CSteamID.Nil.m_SteamID;
            }

            API.Matchmaking.Client.JoinLobby(lobby, (r, e) =>
            {
                if (!e)
                {
                    if (r.Response == EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                    {
                        if (API.App.isDebugging)
                            Debug.Log("Joined lobby: " + lobby.ToString());

                        Lobby = r.Lobby;
                        evtEnterSuccess.Invoke(lobby);
                    }
                    else
                        evtEnterFailed.Invoke(r.Response);
                }
                else
                    evtEnterFailed.Invoke(EChatRoomEnterResponse.k_EChatRoomEnterResponseError);
            });
        }
        public void Join(string lobbyIdAsString)
        {
            if (HasLobby)
            {
                Lobby.Leave();
                Lobby = CSteamID.Nil.m_SteamID;
            }

            if (ulong.TryParse(lobbyIdAsString, out ulong result))
                Join(result);
        }
        public void Leave()
        {
            Lobby.Leave();
            Lobby = CSteamID.Nil.m_SteamID;
        }
        public bool SetLobbyData(string key, string value) => API.Matchmaking.Client.SetLobbyData(Lobby, key, value);
        public void SetMemberData(string key, string value) => API.Matchmaking.Client.SetLobbyMemberData(Lobby, key, value);
        public LobbyMemberData GetLobbyMember(UserData member) => new LobbyMemberData { lobby = Lobby, user = member };
        public string GetLobbyData(string key) => API.Matchmaking.Client.GetLobbyData(Lobby, key);
        public string GetMemberData(UserData member, string key) => API.Matchmaking.Client.GetLobbyMemberData(Lobby, member, key);
        public bool IsMemberReady(UserData member) => API.Matchmaking.Client.GetLobbyMemberData(Lobby, member, LobbyData.DataReady) == "true";
        public void KickMember(UserData member) => Lobby.KickMember(member);
        public void Invite(UserData user) => API.Matchmaking.Client.InviteUserToLobby(Lobby, user);
        public void Invite(uint FriendId) => API.Matchmaking.Client.InviteUserToLobby(Lobby, UserData.Get(FriendId));
        public LobbyMemberData[] Members => API.Matchmaking.Client.GetLobbyMembers(Lobby);
    }
}
#endif