#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using Friends = HeathenEngineering.SteamworksIntegration.API.Friends;

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public class ClanProfile : MonoBehaviour
    {
        [SerializeField]
        private RawImage icon;
        [SerializeField]
        private TMPro.TextMeshProUGUI displayName;
        [SerializeField]
        private TMPro.TextMeshProUGUI clanTag;

        public ClanData Clan
        {
            get
            {
                return currentClan;
            }
            set
            {
                Apply(value);
            }
        }

        private ClanData currentClan;

        private void OnEnable()
        {
            Friends.Client.EventPersonaStateChange.AddListener(HandlePersonaStateChange);
        }

        private void OnDisable()
        {
            Friends.Client.EventPersonaStateChange.RemoveListener(HandlePersonaStateChange);
        }

        private void HandlePersonaStateChange(PersonaStateChange arg)
        {
            if (Friends.Client.PersonaChangeHasFlag(arg.Flags, EPersonaChange.k_EPersonaChangeAvatar)
                && arg.SubjectId == currentClan)
            {
                Apply(currentClan);
            }
        }

        public void Apply(ClanData clan)
        {
            currentClan = clan;

            if (displayName != null)
                displayName.text = clan.Name;

            if (clanTag != null)
                clanTag.text = clan.Tag;

            if (icon != null)
                clan.LoadIcon(r => icon.texture = r);
        }
    }
}
#endif