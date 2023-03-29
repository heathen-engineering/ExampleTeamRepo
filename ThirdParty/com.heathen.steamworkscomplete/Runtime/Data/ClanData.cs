#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using System.Linq;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration
{
    /// <summary>
    /// Steam Groups aka Steam Clans structure
    /// </summary>
    /// <remarks>
    /// This is interchangable with CSteamID and simply extends it with Clan related accessor features
    /// <para>
    /// You can fetch the list of available clans via API.Clans.Client.GetClans()
    /// </para>
    /// </remarks>
    [Serializable]
    public struct ClanData : IEquatable<CSteamID>, IEquatable<ClanData>, IEquatable<ulong>, IComparable<CSteamID>, IComparable<ClanData>, IComparable<ulong>
    {
        [SerializeField]
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
                SteamId = new CSteamID(value, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeClan);
            }
        }
        public uint FriendId
        {
            get => SteamId.GetAccountID().m_AccountID;
            set
            {
                SteamId = new CSteamID(new AccountID_t(value), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeClan);
            }
        }
        /// <summary>
        /// Is this Clan value a valid value.
        /// This does not indicate it is a clan simply that structurally the data is possibly a clan
        /// </summary>
        public bool IsValid
        {
            get
            {
                var sID = SteamId;

                if (sID == CSteamID.Nil
                    || sID.GetEAccountType() != EAccountType.k_EAccountTypeClan
                    || sID.GetEUniverse() != EUniverse.k_EUniversePublic)
                    return false;
                else
                    return true;
            }
        }
        public Texture2D Icon => API.Friends.Client.GetLoadedAvatar(this);
        public string Name => API.Clans.Client.GetName(this);
        public string Tag => API.Clans.Client.GetTag(this);
        public UserData Owner => API.Clans.Client.GetOwner(this);
        public UserData[] Officers => API.Clans.Client.GetOfficers(this);
        public int NumberOfMembersInChat => API.Clans.Client.GetChatMemberCount(this);
        public UserData[] MembersInChat => API.Clans.Client.GetChatMembers(this);
        public bool IsOfficalGameGroup => API.Clans.Client.IsClanOfficialGameGroup(this);
        public bool IsPublic => API.Clans.Client.IsClanPublic(this);
        public bool IsUserOwner => Owner == UserData.Me;
        public bool IsUserOfficer => Officers.Any(p => p == UserData.Me);
        /// <summary>
        /// Get all the clans this user is a member of
        /// </summary>
        /// <returns></returns>
        public static ClanData[] Get() => API.Clans.Client.GetClans();
        /// <summary>
        /// Get the clan represented by this account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static ClanData Get(uint accountId) => new CSteamID(new AccountID_t(accountId), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeClan);
        /// <summary>
        /// Get the clan represented by this account ID
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static ClanData Get(AccountID_t accountId) => new CSteamID(accountId, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeClan);
        /// <summary>
        /// Get the clan represented by this CSteamID value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ClanData Get(ulong id) => new ClanData { id = id };
        /// <summary>
        /// Get the clan represented by this CSteamID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ClanData Get(CSteamID id) => new ClanData { id = id.m_SteamID };
        /// <summary>
        /// Allows the user to join Steam group (clan) chats right within the game.
        /// </summary>
        /// <remarks>
        /// The joins the "old" group chat used in Steam not the new channel based chat system introduced 2018-2019.
        /// It is still usable but will not reflect the caht seen in the Steam client. 
        /// It can be used as a global chat for in-game players as connecting via the Steam API is the only known way to connect to this legacy chat system.
        /// </remarks>
        /// <param name="callback"></param>
        public void JoinChat(Action<ChatRoom, bool> callback) => API.Clans.Client.JoinChatRoom(id, callback);

        public void LoadIcon(Action<Texture2D> callback) => API.Friends.Client.GetFriendAvatar(SteamId, callback);

    #region Boilerplate
        public int CompareTo(CSteamID other)
        {
            return id.CompareTo(other.m_SteamID);
        }

        public int CompareTo(ClanData other)
        {
            return id.CompareTo(other.id);
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

        public bool Equals(ClanData other)
        {
            return id.Equals(other.id);
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

        public static bool operator ==(ClanData l, ClanData r) => l.id == r.id;
        public static bool operator !=(ClanData l, ClanData r) => l.id != r.id;
        public static bool operator ==(ClanData l, CSteamID r) => l.id == r.m_SteamID;
        public static bool operator !=(ClanData l, CSteamID r) => l.id != r.m_SteamID;
        public static bool operator ==(CSteamID l, ClanData r) => l.m_SteamID == r.id;
        public static bool operator !=(CSteamID l, ClanData r) => l.m_SteamID != r.id;
        public static bool operator <(ClanData l, ClanData r) => l.id < r.id;
        public static bool operator >(ClanData l, ClanData r) => l.id > r.id;
        public static bool operator <=(ClanData l, ClanData r) => l.id <= r.id;
        public static bool operator >=(ClanData l, ClanData r) => l.id >= r.id;
        public static implicit operator CSteamID(ClanData c) => c.SteamId;
        public static implicit operator ClanData(CSteamID id) => new ClanData { id = id.m_SteamID };
        public static implicit operator ulong(ClanData c) => c.id;
        public static implicit operator ClanData(ulong id) => new ClanData { id = id };
    #endregion
    }
}
#endif