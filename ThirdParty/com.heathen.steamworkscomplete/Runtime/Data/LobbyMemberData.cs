#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct LobbyMemberData: IEquatable<LobbyMemberData>
    {
        public LobbyData lobby;
        public UserData user;

        public string this[string metadataKey]
        {
            get
            {
                return API.Matchmaking.Client.GetLobbyMemberData(lobby, user, metadataKey);
            }
            set
            {
                if (user == API.User.Client.Id)
                    API.Matchmaking.Client.SetLobbyMemberData(lobby, metadataKey, value);
            }
        }

        public bool IsReady
        {
            get => this[LobbyData.DataReady] == "true";
            set => this[LobbyData.DataReady] = value.ToString().ToLower();
        }

        public string GameVersion
        {
            get => this[LobbyData.DataVersion];
            set => this[LobbyData.DataVersion] = value;
        }
        public bool IsOwner => lobby.Owner.Equals(this);

        public bool Equals(LobbyMemberData other)
        {
            return other.lobby == lobby && other.user == user;
        }

        public void Kick() => lobby.KickMember(user);

        public static LobbyMemberData Get(LobbyData lobby, UserData user) => new LobbyMemberData() { lobby = lobby, user = user };
    }
}
#endif