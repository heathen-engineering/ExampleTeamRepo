#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
#endif

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class BasicLobbyMemberSlot : LobbyMemberSlot
    {
        public SetUserAvatar avatar;
        public Button inviteButton;
        public Button removeButton;
        public GameObject ownerPip;
        public GameObject readyPip;
        public GameObject waitingPip;

        private LobbyMemberData member;

        public override bool Interactable 
        { 
            get
            {
                if (inviteButton != null)
                    return inviteButton.interactable;
                else if (removeButton != null)
                    return removeButton.interactable;
                else
                    return false;
            }
            set
            {
                if (inviteButton != null)
                    inviteButton.interactable = value;
                
                if (removeButton != null)
                    removeButton.interactable = value;
            }
        }

        public override void ClearUser()
        {
            member = default;
            if (avatar != null)
                avatar.gameObject.SetActive(false);
            if (inviteButton != null)
                inviteButton.gameObject.SetActive(true);
            if (removeButton != null)
                removeButton.gameObject.SetActive(false);
            if (ownerPip != null)
                ownerPip.SetActive(false);
            if (readyPip != null)
                readyPip.SetActive(false);
            if (waitingPip != null)
                waitingPip.SetActive(false);
        }

        public override LobbyMemberData GetUser() => member;

        public override void SetUser(LobbyMemberData member)
        {
            this.member = member;
            if (avatar != null)
            {
                avatar.UserData = member.user;
                avatar.gameObject.SetActive(true);
            }
            if (inviteButton != null)
                inviteButton.gameObject.SetActive(false);
            if (removeButton != null)
                removeButton.gameObject.SetActive(true);
            if (ownerPip != null)
                ownerPip.SetActive(member.IsOwner);
            if (readyPip != null)
                readyPip.SetActive(member.IsReady);
            if (waitingPip != null)
                waitingPip.SetActive(!member.IsReady);
        }
    }
}
#endif