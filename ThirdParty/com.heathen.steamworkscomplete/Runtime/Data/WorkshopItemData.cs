#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using Steamworks;
using System;
using System.IO;
using System.Linq;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct WorkshopItemData
    {
        public PublishedFileId_t? publishedFileId;
        public AppData appId;
        public string title;
        public string description;
        public DirectoryInfo content;
        public FileInfo preview;
        public string metadata;
        public string[] tags;
        public ERemoteStoragePublishedFileVisibility visibility;

        /// <summary>
        /// To be valid the following must be true
        /// <list type="bullet">
        /// <item><see cref="appId"/> must be valid</item>
        /// <item><see cref="title"/> must be populated with a value whoes length is less than <see cref="Constants.k_cchPublishedDocumentTitleMax"/></item>
        /// <item><see cref="description"/> must be populated with a value whoes length is less than <see cref="Constants.k_cchPublishedDocumentDescriptionMax"/></item>
        /// <item><see cref="metadata"/> is option and can be an empty string, if populated its length must be less than <see cref="Constants.k_cchDeveloperMetadataMax"/></item>
        /// <item><see cref="content"/> must be the full path of a valid directory (aka folder path)</item>
        /// <item><see cref="preview"/> must be the full path of a valid JPG, PNG or GIF file whoes total size is less than 1mb</item>
        /// <item><see cref="imageFiles"/> is optional and can be null, if populated each path must be a valid JPG, PNG or GIF and each image must have a size less than 1mb</item>
        /// <item><see cref="youTubeVideoIds"/> is optional and can be null, if populated each must be a valid YouTube video ID</item>
        /// <item><see cref="tags"/> is optional and can be null, if populated each tag must have a length less than 255</item>
        /// <item><see cref="keyValueTags"/> is optional and can be null, if populated the length of the key + the length of the value for each entry must be less than 255</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// It is not required to call this.
        /// If you have have assured that the input is correct you can avoid calling this feature.
        /// This feature can be processor heavy and should only be called if you are unshure if the input values are valid.
        /// This does not catch every possible invalid case but catches the most common
        /// </remarks>
        public bool IsValid
        {
            get
            {
                return appId != AppId_t.Invalid
                    && !string.IsNullOrEmpty(title)
                    && title.Length < Constants.k_cchPublishedDocumentTitleMax
                    && !string.IsNullOrEmpty(description)
                    && description.Length < Constants.k_cchPublishedDocumentDescriptionMax
                    && (string.IsNullOrEmpty(metadata) || metadata.Length < Constants.k_cchDeveloperMetadataMax)
                    && preview != null
                    && content != null
                    && preview.Exists
                    && content.Exists
                    && !tags.Any(p => p.Length > 255);
            }
        }

        public bool Create(Action<WorkshopItemDataCreateStatus> completedCallback = null, Action<UGCUpdateHandle_t> uploadStartedCallback = null, Action<CreateItemResult_t> fileCreatedCallback = null) => API.UserGeneratedContent.Client.CreateItem(this, null, null, null, completedCallback, uploadStartedCallback, fileCreatedCallback);
        public bool Create(WorkshopItemPreviewFile[] additionalPreviews, string[] additionalYouTubeIds, WorkshopItemKeyValueTag[] additionalKeyValueTags, Action<WorkshopItemDataCreateStatus> completedCallback = null, Action<UGCUpdateHandle_t> uploadStartedCallback = null, Action<CreateItemResult_t> fileCreatedCallback = null) => API.UserGeneratedContent.Client.CreateItem(this, additionalPreviews, additionalYouTubeIds, additionalKeyValueTags, completedCallback, uploadStartedCallback, fileCreatedCallback);
        public bool Update(Action<WorkshopItemDataUpdateStatus> completedCallback = null, Action<UGCUpdateHandle_t> uploadStartedCallback = null) => API.UserGeneratedContent.Client.UpdateItem(this, null, null, null, completedCallback, uploadStartedCallback);
        public bool Update(WorkshopItemPreviewFile[] additionalPreviews, string[] additionalYouTubeIds, WorkshopItemKeyValueTag[] additionalKeyValueTags, Action<WorkshopItemDataUpdateStatus> completedCallback = null, Action<UGCUpdateHandle_t> uploadStartedCallback = null) => API.UserGeneratedContent.Client.UpdateItem(this, additionalPreviews, additionalYouTubeIds, additionalKeyValueTags, completedCallback, uploadStartedCallback);

        public static void Get(PublishedFileId_t file, Action<WorkshopItem> callback)
        {
            var query = UgcQuery.Get(file);
            query.SetReturnLongDescription(true);
            query.SetReturnMetadata(true);
            query.Execute(r =>
            {
                callback?.Invoke(r.ResultsList != null && r.ResultsList.Count > 0 ? r.ResultsList[0] : null);
                query.Dispose();
            });
        }
    }

    public class WorkshopUploadWorker
    {
        public PublishedFileId_t? FileId => itemData.publishedFileId;
        public AppData AppId => itemData.appId;
        public string Title => itemData.title;
        public string Description => itemData.description;
        public DirectoryInfo Content => itemData.content;
        public FileInfo Preview => itemData.preview;
        public string Metadata => itemData.metadata;
        public string[] Tags => itemData.tags;
        public ERemoteStoragePublishedFileVisibility Visibility => itemData.visibility;

        public event EventHandler<WorkshopItemDataCreateStatus> Completed;
        public event EventHandler<UGCUpdateHandle_t> UpdateStarted;
        public event EventHandler<CreateItemResult_t> FileCreated;

        private WorkshopItemData itemData;
        private UGCUpdateHandle_t? updateHandle;

        public static WorkshopUploadWorker Get(WorkshopItemData data) => new WorkshopUploadWorker { itemData = data };

        public bool RunCreate()
        {
            if (itemData.IsValid)
                return itemData.Create(CompletedHandler, UploadStartedHandler, FileCreatedHandler);
            else
                return false;
        }
        public bool RunCreate(WorkshopItemPreviewFile[] additionalPreviews, string[] additionalYouTubeIds, WorkshopItemKeyValueTag[] additionalKeyValueTags)
        {
            if (itemData.IsValid)
                return itemData.Create(additionalPreviews, additionalYouTubeIds, additionalKeyValueTags, CompletedHandler, UploadStartedHandler, FileCreatedHandler);
            else
                return false;
        }
        public EItemUpdateStatus GetUpdateProgress(out float progress)
        {
            progress = 0;
            if (updateHandle.HasValue)
                return API.UserGeneratedContent.Client.GetItemUpdateProgress(updateHandle.Value, out progress);
            else
                return EItemUpdateStatus.k_EItemUpdateStatusInvalid;
        }

        private void CompletedHandler(WorkshopItemDataCreateStatus arg)
        {
            updateHandle = null;
            Completed?.Invoke(this, arg);
        }
        private void UploadStartedHandler(UGCUpdateHandle_t arg)
        {
            updateHandle = arg;
            UpdateStarted?.Invoke(this, arg);
        }
        private void FileCreatedHandler(CreateItemResult_t arg) => FileCreated?.Invoke(this, arg);
    }
}
#endif