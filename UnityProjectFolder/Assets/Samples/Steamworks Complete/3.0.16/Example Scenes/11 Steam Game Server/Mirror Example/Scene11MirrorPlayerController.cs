#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS && MIRROR
using UnityEngine;
using Mirror;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene11MirrorPlayerController : NetworkBehaviour
    {
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            Authentication.GetAuthSessionTicket((result, ioError) =>
            {
                if (!ioError)
                    CmdAuthenticate(UserData.Me, result.Data);
            });
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            Authentication.CancelAllTickets();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            Authentication.EndAllSessions();
        }

        [Command]
        public void CmdAuthenticate(ulong userId, byte[] data)
        {
            var result = Authentication.BeginAuthSession(data, new Steamworks.CSteamID(userId), (session) =>
            {
                Debug.Log("Session started for " + userId + ", responce: " + session.Response);
                TargetAuthenticated(connectionToClient, session.Response);
            });

            Debug.Log("Request to begin auth session was called with a responce of: " + result.ToString());
        }

        [TargetRpc]
        public void TargetAuthenticated(NetworkConnection target, Steamworks.EAuthSessionResponse responce)
        {
            Debug.Log("The server told us our Auth Responce is: " + responce);
        }
    }
}
#endif