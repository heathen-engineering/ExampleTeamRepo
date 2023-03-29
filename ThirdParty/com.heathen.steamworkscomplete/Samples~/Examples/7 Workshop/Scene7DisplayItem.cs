#if HE_SYSCORE && (STEAMWORKSNET || FACEPUNCH) && !DISABLESTEAMWORKS 
using UnityEngine;
using HeathenEngineering.SteamworksIntegration;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene7DisplayItem : MonoBehaviour
    {
        public UnityEngine.UI.RawImage previewImage;
        public UnityEngine.UI.Text title;
        public GameObject subscribeButton;
        public GameObject unsubscribeButton;

        private WorkshopItem record;

        private void OnDestroy()
        {
            if (record != null)
                record.previewImageUpdated.AddListener(HandleImageUpdate);
        }

        public void AssignResult(WorkshopItem item)
        {
            record = item;

            record.previewImageUpdated.AddListener(HandleImageUpdate);

            title.text = record.Title;

            if (record.previewImage != null)
                previewImage.texture = record.previewImage;

            if(record.IsSubscribed)
            {
                unsubscribeButton.SetActive(true);
                subscribeButton.SetActive(false);
            }
            else
            {
                unsubscribeButton.SetActive(false);
                subscribeButton.SetActive(true);
            }
        }

        private void HandleImageUpdate()
        {
            if (record.previewImage != null)
                previewImage.texture = record.previewImage;
        }

        public void Subscribe()
        {
            HeathenEngineering.SteamworksIntegration.API.UserGeneratedContent.Client.SubscribeItem(record.FileId, (r, e) =>
             {
                 if (!e && r.m_eResult == Steamworks.EResult.k_EResultOK)
                 {
                     unsubscribeButton.SetActive(true);
                     subscribeButton.SetActive(false);
                 }
                 else
                     Debug.Log("Failed to subscribe");
             });
        }

        public void Unsubscribe()
        {
            HeathenEngineering.SteamworksIntegration.API.UserGeneratedContent.Client.UnsubscribeItem(record.FileId, (r, e) =>
            {
                if (!e && r.m_eResult == Steamworks.EResult.k_EResultOK)
                {
                    unsubscribeButton.SetActive(false);
                    subscribeButton.SetActive(true);
                 }
                else
                    Debug.Log("Failed to unsubscribe");
            });
        }
    }
}
#endif