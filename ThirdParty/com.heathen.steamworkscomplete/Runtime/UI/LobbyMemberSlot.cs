#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
#endif

namespace HeathenEngineering.SteamworksIntegration.UI
{
    public abstract class LobbyMemberSlot : MonoBehaviour
    {
        public UnityEvent InviteUserRequest;
        public UnityEvent RemoveUserRequest;

        public abstract bool Interactable { get; set; }

        public abstract void SetUser(LobbyMemberData user);
        public abstract LobbyMemberData GetUser();
        public abstract void ClearUser();
    }

    public class WorkshopBrowserControl : MonoBehaviour
    {
        public GameObject itemTemplate;
    }

    public interface IWorkshopBrowserItemTemplate
    {
        void Load(WorkshopItem item);
    }

    public class WorkshopBrowserSimpleItemRecord : MonoBehaviour, IWorkshopBrowserItemTemplate
    {
        public RawImage previewImage;
        public TMPro.TextMeshProUGUI titleLabel;
        public TMPro.TextMeshProUGUI authorLabel;
        public Image voteFillImage;
        [Header("Tooltip Elements")]
        public TMPro.TextMeshProUGUI tipTitleLabel;
        public TMPro.TextMeshProUGUI tipDescriptionLabel;

        private WorkshopItem _item;
        public WorkshopItem Item 
        {
            get => _item;
            set => Load(value);
        }

        public void Load(WorkshopItem item)
        {
            _item = item;

            if(item.previewImage != null)
                previewImage.texture = item.previewImage;
            else
            {
                item.DownloadPreviewImage();
            }
        }
    }
}
#endif