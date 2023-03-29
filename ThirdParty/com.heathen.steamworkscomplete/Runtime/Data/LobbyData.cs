#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct LobbyData : IEquatable<CSteamID>, IEquatable<ulong>, IEquatable<LobbyData>
    {
        private ulong id;
        public CSteamID SteamId
        {
            get => new CSteamID(id);
            set => id = value.m_SteamID;
        }
        public AccountID_t AccountId
        {
            get => SteamId.GetAccountID();
            set
            {
                SteamId = new CSteamID(value, 393216, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeChat);
            }
        }
        public uint FriendId
        {
            get => AccountId.m_AccountID;
            set
            {
                SteamId = new CSteamID(new AccountID_t(value), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeChat);
            }
        }
        /// <summary>
        /// Is this Lobby value a valid value.
        /// This does not indicate it is a lobby simply that structurally the data is possibly a lobby
        /// </summary>
        public bool IsValid
        {
            get
            {
                var sId = SteamId;
                if (sId == CSteamID.Nil
                    || sId.GetEAccountType() != EAccountType.k_EAccountTypeChat
                    || sId.GetEUniverse() != EUniverse.k_EUniversePublic)
                    return false;
                else
                    return true;
            }
        }
        /// <summary>
        /// Get or set the lobby name
        /// </summary>
        /// <remarks>
        /// <para>
        /// The lobby name is a metadata field whoes key is "name". Setting this field will update the lobby metadata accordinly and this update will be reflected to all members.
        /// Only the owner of the lobby can set this value.
        /// </para>
        /// </remarks>
        public string Name
        {
            get => this[DataName];
            set => this[DataName] = value;
        }
        /// <summary>
        /// The current owner of the lobby.
        /// </summary>
        public LobbyMemberData Owner
        {
            get => new LobbyMemberData { lobby = this, user = API.Matchmaking.Client.GetLobbyOwner(id) };
            set => API.Matchmaking.Client.SetLobbyOwner(id, value.user);
        }
        /// <summary>
        /// The member data for this user
        /// </summary>
        public LobbyMemberData Me => new LobbyMemberData { lobby = this, user = API.User.Client.Id };
        [Obsolete("Please use Me instead.")]
        public LobbyMemberData User => Me;
        /// <summary>
        /// The collection of all members of this lobby including the owner of the lobby.
        /// </summary>
        public LobbyMemberData[] Members => API.Matchmaking.Client.GetLobbyMembers(id);
        /// <summary>
        /// True if the data type metadata is set
        /// </summary>
        public bool IsTypeSet => !string.IsNullOrEmpty(API.Matchmaking.Client.GetLobbyData(id, DataType));
        /// <summary>
        /// Returns the type of the lobby if set, if not set this will default to Private, you can check if the type is set with <see cref="IsTypeSet"/>
        /// </summary>
        public ELobbyType Type
        {
            get
            {
                if (int.TryParse(API.Matchmaking.Client.GetLobbyData(id, DataType), out int enumVal))
                {
                    return (ELobbyType)enumVal;
                }
                else
                    return ELobbyType.k_ELobbyTypePrivate;
            }
            set => API.Matchmaking.Client.SetLobbyType(id, value);
        }
        /// <summary>
        /// Gets or sets the version of the game the lobby is configured for ... this should match the owners version
        /// </summary>
        public string GameVersion
        {
            get => this[DataVersion];
            set => this[DataVersion] = value;
        }
        /// <summary>
        /// Is the user the host of this lobby
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calls <see cref="SteamMatchmaking.GetLobbyOwner(CSteamID)"/> and compares the results to <see cref="SteamUser.GetSteamID()"/>.
        /// This returns true if the provided lobby ID is a legitimate ID and if Valve indicates that the lobby has members and if the owner of the lobby is the current player.
        /// </para>
        /// </remarks>
        public bool IsOwner
        {
            get
            {
                return SteamUser.GetSteamID() == SteamMatchmaking.GetLobbyOwner(this);
            }
        }
        /// <summary>
        /// Indicates rather or not this lobby is a party lobby
        /// </summary>
        public bool IsGroup
        {
            get
            {
                return this[DataMode] == "Group";
            }
            set
            {
                if (IsOwner)
                {
                    if (value)
                    {
                        SetType(ELobbyType.k_ELobbyTypeInvisible);
                        this[DataMode] = "Group";
                    }
                    else
                    {
                        this[DataMode] = "General";
                    }
                }
            }
        }
        /// <summary>
        /// Indicates rather or not this lobby is a party lobby
        /// </summary>
        public bool IsSession
        {
            get
            {
                return this[DataMode] == "Session";
            }
            set
            {
                if (IsOwner)
                {
                    if (value)
                    {
                        this[DataMode] = "Session";
                    }
                    else
                    {
                        this[DataMode] = "General";
                    }
                }
            }
        }
        /// <summary>
        /// Does this lobby have a game server registered to it
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calls <see cref="SteamMatchmaking.GetLobbyGameServer(CSteamID, out uint, out ushort, out CSteamID)"/> and cashes the data to <see cref="GameServer"/>.
        /// It is not usually nessisary to check this value since the set game server callback from Steamworks will automatically update these values if the user was connected to the lobby when the set game server data was called.
        /// </para>
        /// </remarks>
        public bool HasServer => SteamMatchmaking.GetLobbyGameServer(this, out _, out _, out _);
        public LobbyGameServer GameServer => API.Matchmaking.Client.GetLobbyGameServer(id);
        /// <summary>
        /// Returns true if all of the players 'IsReady' is true
        /// </summary>
        /// <remarks>
        /// <para>
        /// This can be used to determin if the players are ready to play the game.
        /// </para>
        /// </remarks>
        public bool AllPlayersReady
        {
            get
            {
                //If we have any that are not ready then return false ... else return true
                return Members.Any(p => !p.IsReady) ? false : true;
            }
        }
        /// <summary>
        /// Returns true if all of the players 'IsReady' is false
        /// </summary>
        /// <remarks>
        /// <para>
        /// This can be used to determin if all players have reset the ready flag such as when some change is made after a previous ready check had already passed.
        /// </para>
        /// </remarks>
        public bool AllPlayersNotReady
        {
            get
            {
                //If we have any that are not ready then return false ... else return true
                return Members.Any(p => p.IsReady) ? false : true;
            }
        }
        public bool IsReady
        {
            get => API.Matchmaking.Client.GetLobbyMemberData(id, API.User.Client.Id, DataReady) == "true";
            set => API.Matchmaking.Client.SetLobbyMemberData(id, DataReady, value.ToString().ToLower());
        }
        public bool Full => API.Matchmaking.Client.GetLobbyMemberLimit(id) <= SteamMatchmaking.GetNumLobbyMembers(this);
        public int MaxMembers
        {
            get => API.Matchmaking.Client.GetLobbyMemberLimit(id);
            set => API.Matchmaking.Client.SetLobbyMemberLimit(id, value);
        }
        public int MemberCount => Steamworks.SteamMatchmaking.GetNumLobbyMembers(this);
        /// <summary>
        /// Read and write metadata values to the lobby
        /// </summary>
        /// <param name="metadataKey">The key of the value to be read or writen</param>
        /// <returns>The value of the key if any otherwise returns and empty string.</returns>
        public string this[string metadataKey]
        {
            get
            {
                return API.Matchmaking.Client.GetLobbyData(id, metadataKey);
            }
            set
            {
                API.Matchmaking.Client.SetLobbyData(id, metadataKey, value);
            }
        }
        public LobbyMemberData this[UserData user]
        {
            get
            {
                if (GetMember(user, out var member))
                    return member;
                else
                    return default;
            }
        }
        /// <summary>
        /// Get the LobbyMember object for a given user
        /// </summary>
        /// <param name="id">The ID of the member to fetch</param>
        /// <param name="member">The member found</param>
        /// <returns>True if the user is a member of the lobby, false if they are not</returns>
        public bool GetMember(UserData user, out LobbyMemberData member) => API.Matchmaking.Client.GetMember(this, user, out member);
        /// <summary>
        /// Checks if a user is a member of this lobby
        /// </summary>
        /// <param name="id">The user to check for</param>
        /// <returns>True if they are, false if not</returns>
        public bool IsAMember(CSteamID id) => API.Matchmaking.Client.IsAMember(this, id);
        /// <summary>
        /// Updates the lobby type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool SetType(ELobbyType type) => API.Matchmaking.Client.SetLobbyType(id, type);
        public bool SetJoinable(bool makeJoinable) => API.Matchmaking.Client.SetLobbyJoinable(id, makeJoinable);
        /// <summary>
        /// Gets the dictionary of metadata values assigned to this lobby.
        /// </summary>
        public Dictionary<string, string> GetMetadata()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var count = SteamMatchmaking.GetLobbyDataCount(this);

            for (int i = 0; i < count; i++)
            {
                SteamMatchmaking.GetLobbyDataByIndex(this, i, out string key, Constants.k_nMaxLobbyKeyLength, out string value, Constants.k_cubChatMetadataMax);
                result.Add(key, value);
            }

            return result;
        }
        public static void Create(ELobbyType type, int slots, Action<EResult, LobbyData, bool> callback) => API.Matchmaking.Client.CreateLobby(type, slots, callback);
        public static void CreateParty(int slots, Action<EResult, LobbyData, bool> callback)
        {
            API.Matchmaking.Client.CreateLobby(ELobbyType.k_ELobbyTypeInvisible, slots, (r,l,e) =>
            {
                if(!e && r == EResult.k_EResultOK)
                    l.IsGroup = true;

                callback?.Invoke(r, l, e);
            });
        }
        public static void CreateSession(ELobbyType type, int slots, Action<EResult, LobbyData, bool> callback)
        {
            API.Matchmaking.Client.CreateLobby(type, slots, (r, l, e) =>
            {
                if (!e && r == EResult.k_EResultOK)
                    l.IsSession = true;

                callback?.Invoke(r, l, e);
            });
        }
        public static void CreatePublicSession(int slots, Action<EResult, LobbyData, bool> callback)
        {
            API.Matchmaking.Client.CreateLobby(ELobbyType.k_ELobbyTypePublic, slots, (r, l, e) =>
            {
                if (!e && r == EResult.k_EResultOK)
                    l.IsSession = true;

                callback?.Invoke(r, l, e);
            });
        }
        public static void CreatePrivateSession(int slots, Action<EResult, LobbyData, bool> callback)
        {
            API.Matchmaking.Client.CreateLobby(ELobbyType.k_ELobbyTypePrivate, slots, (r, l, e) =>
            {
                if (!e && r == EResult.k_EResultOK)
                    l.IsSession = true;

                callback?.Invoke(r, l, e);
            });
        }
        public static void CreateFriendOnlySession(int slots, Action<EResult, LobbyData, bool> callback)
        {
            API.Matchmaking.Client.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, slots, (r, l, e) =>
            {
                if (!e && r == EResult.k_EResultOK)
                    l.IsSession = true;

                callback?.Invoke(r, l, e);
            });
        }
        /// <summary>
        /// Join this lobby
        /// </summary>
        /// <param name="callback">Handler(LobbyEnter_t result, bool IOError)</param>
        public void Join(Action<LobbyEnter, bool> callback)
        {
            API.Matchmaking.Client.JoinLobby(this, callback);
        }
        /// <summary>
        /// Leaves the current lobby if any
        /// </summary>
        public void Leave()
        {
            if (SteamId == CSteamID.Nil)
                return;

            API.Matchmaking.Client.LeaveLobby(this);

            SteamId = CSteamID.Nil;
        }
        public bool DeleteLobbyData(string dataKey) => API.Matchmaking.Client.DeleteLobbyData(id, dataKey);

        public bool InviteUserToLobby(UserData targetUser) => API.Matchmaking.Client.InviteUserToLobby(id, targetUser);

        public bool SendChatMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes(message);
            return SteamMatchmaking.SendLobbyChatMsg(this, MsgBody, MsgBody.Length);
        }
        public bool SendChatMessage(byte[] data)
        {
            if (data == null || data.Length < 1)
                return false;

            return SteamMatchmaking.SendLobbyChatMsg(this, data, data.Length);
        }
        public bool SendChatMessage(object jsonObject)
        {
            return SendChatMessage(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(jsonObject)));
        }
        /// <summary>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        public void SetGameServer(string address, ushort port, CSteamID gameServerId)
        {
            API.Matchmaking.Client.SetLobbyGameServer(id, GameServer.ipAddress, port, gameServerId);
        }
        /// <summary>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        public void SetGameServer(string address, ushort port)
        {
            API.Matchmaking.Client.SetLobbyGameServer(id, GameServer.ipAddress, port, CSteamID.Nil);
        }
        /// <summary>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        public void SetGameServer(CSteamID gameServerId)
        {
            API.Matchmaking.Client.SetLobbyGameServer(id, 0, 0, gameServerId);
        }
        /// <summary>
        /// <para>
        /// This overload uses the lobby owner's CSteamID as the server ID which is typical of P2P session.
        /// </para>
        /// <para>
        /// Sets the game server associated with the lobby.
        /// This can only be set by the owner of the lobby.
        /// Either the IP/Port or the Steamworks ID of the game server must be valid, depending on how you want the clients to be able to connect.
        /// A LobbyGameCreated_t callback will be sent to all players in the lobby, usually at this point, the users will join the specified game server.
        /// </para>
        /// </summary>
        public void SetGameServer()
        {
            API.Matchmaking.Client.SetLobbyGameServer(id, 0, 0, API.User.Client.Id);
        }
        /// <summary>
        /// Marks the user to be removed
        /// </summary>
        /// <param name="memberId"></param>
        /// <remarks>
        /// This creates an entry in the metadata named z_heathenKick which contains a string array of Ids of users that should leave the lobby.
        /// When users detect their ID in the string they will automatically leave the lobby on leaving the lobby the users ID will be removed from the array.
        /// </remarks>
        public bool KickMember(CSteamID memberId)
        {
            if (!IsOwner)
                return false;

            var kickList = API.Matchmaking.Client.GetLobbyData(id, DataKick);

            if (kickList == null)
                kickList = string.Empty;

            if (!kickList.Contains("[" + memberId.ToString() + "]"))
                kickList += "[" + memberId.ToString() + "]";

            return API.Matchmaking.Client.SetLobbyData(id, DataKick, kickList);
        }
        public bool KickListContains(CSteamID memberId)
        {
            var kickList = API.Matchmaking.Client.GetLobbyData(id, DataKick);
            return kickList.Contains("[" + memberId.ToString() + "]");
        }
        public bool RemoveFromKickList(CSteamID memberId)
        {
            if (!IsOwner)
                return false;

            var kickList = API.Matchmaking.Client.GetLobbyData(id, DataKick);

            kickList = kickList.Replace("[" + memberId.ToString() + "]", string.Empty);

            return API.Matchmaking.Client.SetLobbyData(id, DataKick, kickList);
        }
        public bool ClearKickList()
        {
            if (!IsOwner)
                return false;

            return API.Matchmaking.Client.DeleteLobbyData(id, DataKick);
        }
        /// <summary>
        /// Use this sparingly it requires string parcing and is not performant
        /// </summary>
        /// <returns></returns>
        public CSteamID[] GetKickList()
        {
            var list = API.Matchmaking.Client.GetLobbyData(id, DataKick);
            if (!string.IsNullOrEmpty(list))
            {
                var sArray = list.Split(new string[] { "][" }, StringSplitOptions.RemoveEmptyEntries);
                var resultList = new List<CSteamID>();
                for (int i = 0; i < sArray.Length; i++)
                {
                    if (ulong.TryParse(sArray[i].Replace("[", string.Empty).Replace("]", string.Empty), out ulong id))
                        resultList.Add(new CSteamID(id));
                }

                return resultList.ToArray();
            }
            else
                return new CSteamID[0];
        }
        /// <summary>
        /// Sets metadata for the player on the first lobby
        /// </summary>
        /// <param name="key">The key of the metadata to set</param>
        /// <param name="value">The value of the metadata to set</param>
        public void SetMemberMetadata(string key, string value)
        {
            API.Matchmaking.Client.SetLobbyMemberData(id, key, value);
        }
        /// <summary>
        /// Returns the metadata field of the local user
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMemberMetadata(string key)
        {
            return API.Matchmaking.Client.GetLobbyMemberData(id, API.User.Client.Id, key);
        }
        /// <summary>
        /// Returns the metadata field of the user indicated by <paramref name="memberId"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMemberMetadata(CSteamID memberId, string key)
        {
            return API.Matchmaking.Client.GetLobbyMemberData(id, memberId, key);
        }
        /// <summary>
        /// Returns the metadata field of the user indicated by <paramref name="member"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetMemberMetadata(LobbyMemberData member, string key)
        {
            return API.Matchmaking.Client.GetLobbyMemberData(id, member.user, key);
        }
        public static LobbyData Get(string accountId)
        {
            if (uint.TryParse(accountId, out uint result))
                return Get(result);
            else
                return CSteamID.Nil;
        }
        /// <summary>
        /// Get the lobby represented by this account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static LobbyData Get(uint accountId) => new CSteamID(new AccountID_t(accountId), 393216, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeChat);
        /// <summary>
        /// Get the lobby represented by this account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static LobbyData Get(AccountID_t accountId) => new CSteamID(accountId, 393216, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeChat);
        /// <summary>
        /// Get the lobby represented by this CSteamID value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static LobbyData Get(ulong id) => new LobbyData { id = id };
        /// <summary>
        /// Get the lobby represented by this CSteamID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static LobbyData Get(CSteamID id) => new LobbyData { id = id.m_SteamID };
        /// <summary>
        /// Returns the first lobby where lobby.IsGroup was set to true
        /// </summary>
        /// <param name="lobby"></param>
        /// <returns></returns>
        public static bool GroupLobby(out LobbyData lobby)
        {
            lobby = API.Matchmaking.Client.memberOfLobbies.FirstOrDefault(p => p.IsGroup);
            return lobby.IsValid;
        }
        /// <summary>
        /// Returns the first lobby where lobby.IsSession was set to true
        /// </summary>
        /// <param name="lobby"></param>
        /// <returns></returns>
        public static bool SessionLobby(out LobbyData lobby)
        {
            lobby = API.Matchmaking.Client.memberOfLobbies.FirstOrDefault(p => p.IsSession);
            return lobby.IsValid;
        }
        /// <summary>
        /// Join the lobby represented by this account Id
        /// </summary>
        /// <param name="accountId">Must be a valid uint as a string</param>
        /// <param name="callback">Invoked when the process is complete, handler(LobbyEnter_t result, bool IOError)</param>
        public static void Join(string accountId, Action<LobbyEnter, bool> callback) => API.Matchmaking.Client.JoinLobby(Get(accountId), callback);
        /// <summary>
        /// Join the lobby
        /// </summary>
        /// <param name="lobby">The lobby to join</param>
        /// <param name="callback">Invoked when the process is complete, handler(LobbyEnter_t result, bool IOError)</param>
        public static void Join(LobbyData lobby, Action<LobbyEnter, bool> callback) => API.Matchmaking.Client.JoinLobby(lobby, callback);
        /// <summary>
        /// Join the lobby represented by this account Id
        /// </summary>
        /// <param name="accountId">Must be a valid uint as a string</param>
        /// <param name="callback">Invoked when the process is complete, handler(LobbyEnter_t result, bool IOError)</param>
        public static void Join(AccountID_t accountId, Action<LobbyEnter, bool> callback) => API.Matchmaking.Client.JoinLobby(Get(accountId), callback);

    #region Constants
        /// <summary>
        /// Standard metadata field representing the name of the lobby.
        /// This field is typically only used in lobby metadata
        /// </summary>
        public const string DataName = "name";
        /// <summary>
        /// Heathen standard metadata field representing the version of the game.
        /// This field is commonly used in lobby and member metadata
        /// </summary>
        public const string DataVersion = "z_heathenGameVersion";
        /// <summary>
        /// Heathen standard metadata field indicating that the user is ready to play.
        /// This field is commonly only used on member metadata
        /// </summary>
        public const string DataReady = "z_heathenReady";
        /// <summary>
        /// Heathen standard metadata field indicating that these users should leave the lobby.
        /// This is a string containing each CSteamID of members that should not join this lobby and if present should leave it.
        /// Data in this list is in the form of [ + CSteamID + ] e.g. [123456789][987654321] would indicate 2 users that should leave
        /// This field is commonly only used on lobby metadata
        /// </summary>
        public const string DataKick = "z_heathenKick";
        /// <summary>
        /// Heathen standard metadata field indicating the mode of the lobby e.g. group or general
        /// If this is blank its assumed to be general
        /// </summary>
        public const string DataMode = "z_heathenMode";
        /// <summary>
        /// Heathen standard metadata field indicating the type of lobby e.g. private, friend, public or invisible
        /// </summary>
        public const string DataType = "z_heathenType";
    #endregion

    #region Boilerplate
        public int CompareTo(CSteamID other)
        {
            return id.CompareTo(other);
        }

        public int CompareTo(ulong other)
        {
            return id.CompareTo(other);
        }

        public override string ToString()
        {
            return id.ToString();
        }

        public bool Equals(CSteamID other)
        {
            return id.Equals(other);
        }

        public bool Equals(ulong other)
        {
            return id.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public bool Equals(LobbyData other)
        {
            return id.Equals(other.id);
        }

        public static bool operator ==(LobbyData l, LobbyData r) => l.id == r.id;
        public static bool operator ==(CSteamID l, LobbyData r) => l.m_SteamID == r.id;
        public static bool operator ==(LobbyData l, CSteamID r) => l.id == r.m_SteamID;
        public static bool operator ==(LobbyData l, ulong r) => l.id == r;
        public static bool operator ==(ulong l, LobbyData r) => l == r.id;
        public static bool operator !=(LobbyData l, LobbyData r) => l.id != r.id;
        public static bool operator !=(CSteamID l, LobbyData r) => l.m_SteamID != r.id;
        public static bool operator !=(LobbyData l, CSteamID r) => l.id != r.m_SteamID;
        public static bool operator !=(LobbyData l, ulong r) => l.id != r;
        public static bool operator !=(ulong l, LobbyData r) => l != r.id;

        public static implicit operator CSteamID(LobbyData c) => c.SteamId;
        public static implicit operator LobbyData(CSteamID id) => new LobbyData { id = id.m_SteamID };
        public static implicit operator ulong(LobbyData id) => id.id;
        public static implicit operator LobbyData(ulong id) => new LobbyData { id = id };

    #endregion
    }
}
#endif