// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.SaveGame
using System;
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using HutongGames.PlayMaker;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class SaveGame : ScreenBase
{
    public Button btCancel;

    public Button btSave;

    public TextMeshProUGUI labelHeading;

    public TMP_InputField inputSaveName;

    public GridItemManager gridSaves;

    public List<SaveMeta> saveMeta;

    private bool isSaveMode;

    public static SaveMeta queuedLoad;

    private static string lastUsedSave;

    private int dlc;

    public static void Popup(bool isSave, State parent)
    {
        UIManager.Open<SaveGame>(UIManager.Layer.Popup, parent).isSaveMode = isSave;
    }

    public override IEnumerator PreStart()
    {
        this.gridSaves.CustomDynamicItem(CustomSaveItem, UpdateList);
        this.gridSaves.onSelectionChange = UpdateName;
        this.dlc = DLCManager.GetActiveDLC();
        yield return this.GetSavesMeta();
        this.saveMeta.SortInPlace(delegate(SaveMeta a, SaveMeta b)
        {
            if (b.saveName == SaveGame.lastUsedSave)
            {
                return 1;
            }
            return (a.saveName == SaveGame.lastUsedSave) ? (-1) : (-a.saveDate.CompareTo(b.saveDate));
        });
        this.UpdateList();
        this.gridSaves.SelectItem(0);
        if (!this.isSaveMode && this.gridSaves.itemInstances[0] != null && this.gridSaves.itemInstances[0].GetComponent<SaveListItem>().warning.activeInHierarchy)
        {
            this.btSave.interactable = false;
        }
        else
        {
            this.btSave.interactable = true;
        }
        yield return base.PreStart();
    }

    private void UpdateName(object o)
    {
        SaveMeta selectedObject = this.gridSaves.GetSelectedObject<SaveMeta>();
        if (selectedObject != null)
        {
            this.inputSaveName.text = selectedObject.saveName;
        }
    }

    public override IEnumerator Starting()
    {
        this.inputSaveName.gameObject.SetActive(this.isSaveMode);
        this.btSave.GetComponentInChildren<TextMeshProUGUI>().text = Localization.Get(this.isSaveMode ? "UI_SAVE" : "UI_LOAD", true);
        if ((bool)this.labelHeading)
        {
            this.labelHeading.text = Localization.Get(this.isSaveMode ? "UI_SAVE_GAME" : "UI_LOAD_GAME", true);
        }
        yield return base.Starting();
    }

    private IEnumerator GetSavesMeta()
    {
        this.saveMeta = SaveManager.GetAvaliableSaves();
        ProtoLibrary.SanitizeSaveCache(this.saveMeta);
        yield return null;
    }

    private void UpdateList()
    {
        this.gridSaves.UpdateGrid(this.saveMeta);
    }

    private void CustomSaveItem(GameObject itemSource, object source, object data, int index)
    {
        SaveMeta saveMeta = source as SaveMeta;
        SaveListItem component = itemSource.GetComponent<SaveListItem>();
        component.SetSaveMode(this.isSaveMode);
        component.SetSaveMeta(saveMeta);
        component.labelTurn.text = saveMeta.turn.ToString();
        component.labelWizardName.text = saveMeta.wizardName;
        component.labelSaveName.text = saveMeta.saveName;
        component.labelSaveDate.text = saveMeta.GetTimeStamp();
        component.btDeleteSave.onClick.RemoveAllListeners();
        component.btDeleteSave.onClick.AddListener(delegate
        {
            PopupGeneral.OpenPopup(this, "UI_DELETE_SAVE", "UI_DELETE_SAVE_DESCRIPTION", "UI_DELETE", delegate
            {
                SaveManager.DeleteSaveGame(saveMeta);
                this.saveMeta.Remove(saveMeta);
                this.UpdateList();
                Toggle firstActiveToggle = this.gridSaves.GetFirstActiveToggle();
                if (!this.isSaveMode && firstActiveToggle != null && firstActiveToggle.GetComponent<SaveListItem>().warning.activeInHierarchy && SaveManager.IsAnySaveAvailable())
                {
                    this.btSave.interactable = false;
                }
                else
                {
                    this.btSave.interactable = true;
                }
            }, "UI_CANCEL");
        });
        int num = GameVersion.FirstDifference(saveMeta.gameVersion, GameVersion.GetGameVersionFull());
        bool canSaveLoad = true;
        component.dlc0.SetActive(this.IsDlcInSave(saveMeta.dlc, DLCManager.DLCs.Dlc0, ref canSaveLoad));
        component.dlc1.SetActive(this.IsDlcInSave(saveMeta.dlc, DLCManager.DLCs.Dlc1, ref canSaveLoad));
        component.dlc2.SetActive(this.IsDlcInSave(saveMeta.dlc, DLCManager.DLCs.Dlc2, ref canSaveLoad));
        component.dlc3.SetActive(this.IsDlcInSave(saveMeta.dlc, DLCManager.DLCs.Dlc3, ref canSaveLoad));
        bool flag = false;
        if ((saveMeta == null || saveMeta.dlc != this.dlc) && saveMeta.dlc != this.dlc && !canSaveLoad)
        {
            flag = true;
        }
        component.warning.SetActive(value: false);
        component.info.SetActive(value: false);
        if (flag || num <= 2)
        {
            component.warning.SetActive(value: true);
        }
        else if (num > 2 && num <= 5)
        {
            component.info.SetActive(value: true);
        }
        RolloverSimpleTooltip component2 = component.warning.GetComponent<RolloverSimpleTooltip>();
        if (component2 != null && component.warning.activeInHierarchy)
        {
            component2.descriptionParams = this.SetParametrs(saveMeta, saveMeta.dlc, this.dlc);
        }
        component2 = component.info.GetComponent<RolloverSimpleTooltip>();
        if (component2 != null && component.info.activeInHierarchy)
        {
            component2.descriptionParams = new object[2]
            {
                saveMeta.gameVersion,
                GameVersion.GetGameVersionFull()
            };
        }
    }

    private bool IsDlcInSave(int save, DLCManager.DLCs dlc, ref bool canSaveLoad)
    {
        bool flag = (int)((uint)save & (uint)dlc) > 0;
        if (flag)
        {
            canSaveLoad = canSaveLoad && flag && DLCManager.IsDlcOwned(dlc);
        }
        return flag;
    }

    public static void LoadSavedGame(SaveMeta sm)
    {
        if (SaveGame.queuedLoad != sm)
        {
            SaveGame.queuedLoad = sm;
        }
        SaveGame.lastUsedSave = SaveGame.queuedLoad?.saveName;
        if (SaveGame.queuedLoad != null && SaveGame.queuedLoad.gameVersion != GameVersion.GetGameVersionFull())
        {
            Debug.LogWarning("[WARNING] Loading save for game version: " + SaveGame.queuedLoad.gameVersion + " in game version: " + GameVersion.GetGameVersionFull());
        }
        if (World.Get() != null && World.Get().seed == SaveGame.queuedLoad.worldSeed && World.Get().gameID == SaveGame.queuedLoad.gameID)
        {
            FSMGameplay.Get().HandleEvent("LoadSelfInGame");
        }
        else if (FSMGameplay.Get() != null)
        {
            FSMGameplay.Get().HandleEvent("LoadGame");
        }
        else
        {
            List<Fsm> fsmList = Fsm.FsmList;
            if (fsmList.Count > 0)
            {
                fsmList[0].Event("LoadGame");
            }
        }
        PauseMenu screen = UIManager.GetScreen<PauseMenu>(UIManager.Layer.Standard);
        if (screen != null)
        {
            UIManager.Close(screen);
        }
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btSave)
        {
            if (!this.isSaveMode)
            {
                SaveGame.queuedLoad = this.gridSaves.GetSelectedObject<SaveMeta>();
                if (SaveGame.queuedLoad == null)
                {
                    SaveGame.queuedLoad = this.saveMeta[0];
                }
                DataBase.UpdateUse(SaveGame.queuedLoad.dlc);
                SaveGame.LoadSavedGame(SaveGame.queuedLoad);
            }
            else if (string.IsNullOrEmpty(this.inputSaveName.text))
            {
                PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_SET_SAVE_NAME", "UI_OKAY");
            }
            else
            {
                this.Saving();
            }
        }
        else if (s == this.btCancel)
        {
            PauseMenu screen = UIManager.GetScreen<PauseMenu>(UIManager.Layer.Standard);
            if (screen != null)
            {
                UIManager.Close(screen);
            }
        }
    }

    private void Saving()
    {
        this.btCancel.interactable = false;
        this.btSave.interactable = false;
        Exception ex = SaveManager.SaveGame(World.Get().seed, this.inputSaveName.text);
        if (!UIManager.IsOpen<PopupGeneral>(UIManager.Layer.Popup))
        {
            if (ex == null)
            {
                SaveGame.lastUsedSave = this.inputSaveName.text;
                PopupGeneral.OpenPopup(this, "UI_GAME_SAVED", "UI_SAVE_SUCCESSFUL", "UI_OKAY", CloseMe);
            }
            else
            {
                PopupGeneral.OpenPopup(this, "UI_ERROR", ex.ToString(), "UI_OKAY", CloseMe);
            }
        }
        else
        {
            this.CloseMe(null);
        }
    }

    private void CloseMe(object o)
    {
        UIManager.Close(this);
    }

    public static bool DoQueuedLoad()
    {
        if (SaveGame.queuedLoad == null)
        {
            FSMGameplay.Get()?.Finish();
            FSMGameplay.Clear();
            return false;
        }
        SaveManager.loadingMode = true;
        EntityManager.Reset();
        SaveBlock saveBlock = SaveManager.Load(SaveGame.queuedLoad);
        SaveManager.loadingMode = false;
        FOW.Get()?.SetArcanusData(saveBlock.arcanusData);
        FOW.Get()?.SetMyrrorData(saveBlock.myrrorData);
        World.Get().seed = SaveGame.queuedLoad.worldSeed;
        World.Get().gameID = SaveGame.queuedLoad.gameID;
        DifficultySettingsData.current = saveBlock.settings;
        foreach (Group item in EntityManager.GetEntitiesType<Group>())
        {
            item.PreparationAfterDeserialization();
        }
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (entity.Value is IEnchantable)
            {
                EnchantmentRegister.EnchantmentAdded((entity.Value as IEnchantable).GetEnchantments());
            }
        }
        TurnManager.Get().turnNumber = saveBlock.turnNumber;
        World.Get().worldSizeSetting = saveBlock.worldSizeSetting;
        GameManager.Get().dlcSettings = SaveGame.queuedLoad.dlc;
        SaveGame.queuedLoad = null;
        FSMGameplay.Get()?.Finish();
        FSMGameplay.Clear();
        return true;
    }

    private object[] SetParametrs(SaveMeta saveMeta, int dlcActiveInSave, int usingDlc)
    {
        List<object> list = new List<object>
        {
            saveMeta.gameVersion,
            GameVersion.GetGameVersionFull()
        };
        if (dlcActiveInSave != usingDlc)
        {
            list.Add(Localization.Get("UI_DLC_DIFFERENCES_DETECTED", true));
        }
        else
        {
            list.Add("");
        }
        return list.ToArray();
    }
}
