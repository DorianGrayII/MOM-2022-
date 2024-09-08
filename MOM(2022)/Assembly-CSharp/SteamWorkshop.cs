using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MOM;
using Steamworks;
using UnityEngine;

public class SteamWorkshop
{
    public struct WorkshopUpdateStatus
    {
        public EItemUpdateStatus status;

        public ulong curentBytes;

        public ulong totalBytes;

        public bool finished;
    }

    private PublishedFileId_t m_PublishedFileId;

    private CallResult<CreateItemResult_t> OnCreateItemResultCallResult;

    private CallResult<SteamUGCQueryCompleted_t> OnQueryCompletedCallResult;

    private CallResult<RemoteStorageSubscribePublishedFileResult_t> OnRemoteStorageSubscribePublishedFileResultCallResult;

    private CallResult<RemoteStorageUnsubscribePublishedFileResult_t> OnRemoteStorageUnsubscribePublishedFileResultCallResult;

    private CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResultCallResult;

    private CreateItemResult_t createItem;

    private SteamUGCQueryCompleted_t queryCompleted;

    private SubmitItemUpdateResult_t submitItem;

    private WorkshopUpdateStatus updateStatus;

    private bool waitingResponse;

    public SteamWorkshop()
    {
        this.OnCreateItemResultCallResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
        this.OnQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnQueryCompleted);
        this.OnSubmitItemUpdateResultCallResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
        this.OnRemoteStorageSubscribePublishedFileResultCallResult = CallResult<RemoteStorageSubscribePublishedFileResult_t>.Create(OnRemoteStorageSubscribePublishedFileResult);
        this.OnRemoteStorageUnsubscribePublishedFileResultCallResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(OnRemoteStorageUnsubscribePublishedFileResult);
    }

    public IEnumerator PublishMod(ModSettings modSettings, ERemoteStoragePublishedFileVisibility visibility)
    {
        if (!SteamManager.Initialized)
        {
            yield break;
        }
        string path = ModManager.Get().GetPath(modSettings.name);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("Publish not possible, path not found");
            yield break;
        }
        AssetTracker asset = AssetManager.Get().GetAsset(modSettings.icon);
        string image = asset.assetPath;
        ulong id = ((ulong)Mathf.Abs(modSettings.name.GetHashCode()) * 2uL) ^ (32uL + (ulong)Integration.GetName().GetHashCode());
        uint page = 0u;
        int totalItemsFound = 0;
        SteamAPICall_t hAPICall;
        while (!Input.GetKey(KeyCode.Backspace))
        {
            page++;
            UGCQueryHandle_t qHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, SteamUtils.GetAppID(), SteamUtils.GetAppID(), page);
            if (!SteamUGC.AddRequiredTag(qHandle, id.ToString()))
            {
                Debug.LogWarning("SetCloudFileNameFilter failed");
            }
            this.waitingResponse = true;
            hAPICall = SteamUGC.SendQueryUGCRequest(qHandle);
            this.OnQueryCompletedCallResult.Set(hAPICall);
            while (this.waitingResponse)
            {
                SteamAPI.RunCallbacks();
                yield return null;
            }
            Debug.Log("Query completed with: " + this.queryCompleted.m_eResult);
            uint num = 0u;
            if (this.queryCompleted.m_eResult == EResult.k_EResultOK)
            {
                num = 0u;
                while (true)
                {
                    SteamUGC.GetQueryUGCResult(qHandle, num, out var pDetails);
                    if (pDetails.m_eResult != EResult.k_EResultOK)
                    {
                        break;
                    }
                    Debug.Log("details " + pDetails.m_pchFileName);
                    totalItemsFound++;
                    this.m_PublishedFileId = pDetails.m_nPublishedFileId;
                    num++;
                }
                Debug.Log("totalItemsQuerried = " + num);
            }
            if (num == 0)
            {
                break;
            }
        }
        if (totalItemsFound != 1)
        {
            if (totalItemsFound > 1)
            {
                PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_MULTIPLE_IDENTICAL_MODS_FOUND_CANT_UPDATE", "UI_OK");
            }
            this.waitingResponse = true;
            hAPICall = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst);
            this.OnCreateItemResultCallResult.Set(hAPICall);
            while (this.waitingResponse)
            {
                SteamAPI.RunCallbacks();
                yield return null;
            }
            this.m_PublishedFileId = this.createItem.m_nPublishedFileId;
            if (this.createItem.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Saving file did get " + this.createItem.m_eResult);
                yield break;
            }
            Debug.Log("Updating mod impossible, creating new!");
        }
        else
        {
            Debug.Log("Mod already published, Updating it!");
        }
        string text = modSettings.description;
        if (text.Length > 7500)
        {
            text = text.Substring(0, 7500);
            PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_DESCRIPTION_TOO_LONG", "UI_OK");
        }
        UGCUpdateHandle_t updateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), this.m_PublishedFileId);
        if (!SteamUGC.SetItemTags(updateHandle, new List<string> { id.ToString() }))
        {
            Debug.LogWarning("SetID Tag failed");
        }
        if (!string.IsNullOrEmpty(modSettings.name) && !SteamUGC.SetItemTitle(updateHandle, modSettings.title))
        {
            Debug.LogWarning("SetItemTitle failed");
        }
        if (!string.IsNullOrEmpty(text) && !SteamUGC.SetItemDescription(updateHandle, text))
        {
            Debug.LogWarning("SetItemDescription failed");
        }
        if (!SteamUGC.SetItemVisibility(updateHandle, visibility))
        {
            Debug.LogWarning("SetItemVisibility failed");
        }
        if (!SteamUGC.SetItemContent(updateHandle, path))
        {
            Debug.LogWarning("SetItemContent failed");
        }
        if (!string.IsNullOrEmpty(image) && !SteamUGC.SetItemPreview(updateHandle, image))
        {
            Debug.LogWarning("SetItemPreview failed");
        }
        hAPICall = SteamUGC.SubmitItemUpdate(updateHandle, "no change notes");
        this.OnSubmitItemUpdateResultCallResult.Set(hAPICall);
        this.waitingResponse = true;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(0.1f);
        while (this.waitingResponse)
        {
            SteamAPI.RunCallbacks();
            ulong punBytesProcessed;
            ulong punBytesTotal;
            EItemUpdateStatus itemUpdateProgress = SteamUGC.GetItemUpdateProgress(updateHandle, out punBytesProcessed, out punBytesTotal);
            Debug.Log(itemUpdateProgress.ToString() + ", progress: " + punBytesProcessed + " of " + punBytesTotal + " bytes");
            this.updateStatus.status = itemUpdateProgress;
            this.updateStatus.curentBytes = punBytesProcessed;
            this.updateStatus.totalBytes = punBytesTotal;
            yield return wait;
        }
        this.updateStatus.finished = true;
    }

    public WorkshopUpdateStatus GetStatus()
    {
        return this.updateStatus;
    }

    private void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
    {
        string[] obj = new string[8]
        {
            "[",
            3403.ToString(),
            " - CreateItemResult] - ",
            pCallback.m_eResult.ToString(),
            " -- ",
            null,
            null,
            null
        };
        PublishedFileId_t nPublishedFileId = pCallback.m_nPublishedFileId;
        obj[5] = nPublishedFileId.ToString();
        obj[6] = " -- ";
        obj[7] = pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement.ToString();
        Debug.Log(string.Concat(obj));
        this.createItem = pCallback;
        this.waitingResponse = false;
        if (bIOFailure)
        {
            Debug.LogWarning("OnCreateItemResult Call failure: " + pCallback.m_eResult);
        }
    }

    private void OnQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
    {
        this.queryCompleted = param;
        this.waitingResponse = false;
        if (bIOFailure)
        {
            Debug.LogWarning("OnCreateItemResult Call failure: " + param.m_eResult);
        }
    }

    private void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
    {
        string[] obj = new string[8]
        {
            "[",
            3404.ToString(),
            " - SubmitItemUpdateResult] - ",
            pCallback.m_eResult.ToString(),
            " -- ",
            pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement.ToString(),
            " -- ",
            null
        };
        PublishedFileId_t nPublishedFileId = pCallback.m_nPublishedFileId;
        obj[7] = nPublishedFileId.ToString();
        Debug.Log(string.Concat(obj));
        this.submitItem = pCallback;
        if (pCallback.m_eResult == EResult.k_EResultLimitExceeded)
        {
            PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_NOT_ENOUGH_SPACE_OR_ICON_TOO_BIG", "UI_OK");
        }
        this.waitingResponse = false;
        if (bIOFailure)
        {
            Debug.LogWarning("OnSubmitItemUpdateResult Call failure: " + pCallback.m_eResult);
        }
    }

    private void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t pCallback, bool bIOFailure)
    {
        string[] obj = new string[6]
        {
            "[",
            1313.ToString(),
            " - RemoteStorageSubscribePublishedFileResult] - ",
            pCallback.m_eResult.ToString(),
            " -- ",
            null
        };
        PublishedFileId_t nPublishedFileId = pCallback.m_nPublishedFileId;
        obj[5] = nPublishedFileId.ToString();
        Debug.Log(string.Concat(obj));
        this.m_PublishedFileId = pCallback.m_nPublishedFileId;
    }

    private void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t pCallback, bool bIOFailure)
    {
        string[] obj = new string[6]
        {
            "[",
            1315.ToString(),
            " - RemoteStorageUnsubscribePublishedFileResult] - ",
            pCallback.m_eResult.ToString(),
            " -- ",
            null
        };
        PublishedFileId_t nPublishedFileId = pCallback.m_nPublishedFileId;
        obj[5] = nPublishedFileId.ToString();
        Debug.Log(string.Concat(obj));
        this.m_PublishedFileId = pCallback.m_nPublishedFileId;
    }

    public static List<string> GetAllInstalledMods()
    {
        List<string> list = new List<string>();
        if (!SteamManager.Initialized)
        {
            return list;
        }
        uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
        if (numSubscribedItems == 0)
        {
            return list;
        }
        PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
        SteamUGC.GetSubscribedItems(array, numSubscribedItems);
        PublishedFileId_t[] array2 = array;
        foreach (PublishedFileId_t nPublishedFileID in array2)
        {
            EItemState itemState = (EItemState)SteamUGC.GetItemState(nPublishedFileID);
            Debug.Log("GetAllMods " + itemState);
            if ((itemState & EItemState.k_EItemStateInstalled) > EItemState.k_EItemStateNone)
            {
                uint cchFolderSize = 256u;
                SteamUGC.GetItemInstallInfo(nPublishedFileID, out var punSizeOnDisk, out var pchFolder, cchFolderSize, out var punTimeStamp);
                Debug.Log("a " + punSizeOnDisk + " text " + pchFolder + " size " + cchFolderSize + " ts " + punTimeStamp);
                list.Add(pchFolder);
            }
        }
        return list;
    }
}
