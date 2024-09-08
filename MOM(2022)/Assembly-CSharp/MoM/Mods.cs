// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Mods
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Mods : ScreenBase
{
    public Button btMoveUp;

    public Button btMoveDown;

    public Button btPublish;

    public Button btClose;

    public Button btCancel;

    public Button btConfirm;

    public TextMeshProUGUI modName;

    public TextMeshProUGUI modDesc;

    public TextMeshProUGUI publishModName;

    public TextMeshProUGUI publishModDesc;

    public TextMeshProUGUI publishAuthor;

    public TextMeshProUGUI publishVisibility;

    public TextMeshProUGUI currentUpload;

    public RawImage modIcon;

    public RawImage publishModIcon;

    public GridItemManager modsGrid;

    public Slider sliderTotalUpload;

    public GameObject publishPanel;

    public GameObject uploadProgress;

    public DropDownFilters dropdownPublishMode;

    public bool closePopup;

    public bool publish;

    private bool dirty;

    private List<ModSettings> list;

    private Dictionary<ModSettings, bool> switches = new Dictionary<ModSettings, bool>();

    public override IEnumerator PreStart()
    {
        if (!SteamManager.Initialized)
        {
            this.btPublish.interactable = false;
        }
        this.modsGrid.CustomDynamicItem(ModItem, ModGridUpdate);
        this.modsGrid.onSelectionChange = SelectionUpdate;
        this.PublishSelectionChanged(null);
        UIComponentFill.LinkDropdownCustom(this.dropdownPublishMode, ListPublishOptions, null, (object o) => 0, PublishSelectionChanged);
        this.PrepareList();
        this.SelectionUpdate(null);
        this.PublishSelectionChanged(null);
        yield return base.PreStart();
    }

    public override void OnStart()
    {
        base.OnStart();
        this.ModGridUpdate();
        this.modsGrid.SelectItem(0);
        this.SelectionUpdate(this.modsGrid.GetSelectedObject<ModSettings>());
    }

    public override IEnumerator PreClose()
    {
        this.UpdateManagerModOrder();
        ModManager.Get().SaveModOrderList();
        if (this.dirty)
        {
            PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_MODS_RESTART_REQUIRED", "UI_OK");
        }
        yield return base.PreClose();
    }

    private void UpdateManagerModOrder()
    {
        List<ModOrder> list = new List<ModOrder>();
        for (int i = 0; i < this.list.Count; i++)
        {
            ModSettings modSettings = this.list[i];
            ModOrder modOrder = new ModOrder();
            modOrder.name = modSettings.name;
            modOrder.order = i;
            if (this.switches.ContainsKey(modSettings))
            {
                modOrder.active = this.switches[modSettings];
            }
            else
            {
                modOrder.active = false;
            }
            list.Add(modOrder);
        }
        ModManager.Get().UpdateOrder(list);
    }

    private void PrepareList()
    {
        this.list = new List<ModSettings>(ModManager.Get().GetModList().Values);
        ModOrderList order = ModManager.Get().GetModOrderList();
        if (order == null || order.order == null)
        {
            return;
        }
        this.list.SortInPlace(delegate(ModSettings a, ModSettings b)
        {
            ModOrder modOrder2 = order.order.Find((ModOrder x) => x.name == a.name);
            ModOrder modOrder3 = order.order.Find((ModOrder x) => x.name == b.name);
            if (modOrder2 == null && modOrder3 == null)
            {
                return 0;
            }
            if (modOrder2 == null)
            {
                return 1;
            }
            return (modOrder3 == null) ? (-1) : modOrder2.order.CompareTo(modOrder3.order);
        });
        foreach (ModSettings v in this.list)
        {
            ModOrder modOrder = order.order.Find((ModOrder x) => x.name == v.name);
            this.switches[v] = modOrder?.active ?? false;
        }
    }

    private void ModGridUpdate()
    {
        this.ModGridUpdate(autoSelect: true);
    }

    private void ModGridUpdate(bool autoSelect)
    {
        this.modsGrid.UpdateGrid(this.list, this.list);
        if (autoSelect)
        {
            ModSettings selectedObject = this.modsGrid.GetSelectedObject<ModSettings>();
            if (this.modsGrid.GetGameObjectForData(selectedObject) == null)
            {
                this.modsGrid.SelectItem(0);
            }
        }
    }

    private void SelectionUpdate(object o)
    {
        o = ((!(o is Toggle t)) ? this.modsGrid.GetSelectedObject<ModSettings>() : this.modsGrid.GetObjectForSelectable<ModSettings>(t));
        if (o is ModSettings modSettings)
        {
            this.modName.text = modSettings.name;
            this.modDesc.text = modSettings.description.Replace("\\n", "\n");
            this.modIcon.texture = AssetManager.Get<Texture2D>(modSettings.icon, passModValidation: false);
            this.btMoveUp.interactable = this.list.IndexOf(modSettings) > 0;
            this.btMoveDown.interactable = this.list.IndexOf(modSettings) < this.list.Count - 1;
        }
        else
        {
            this.modName.text = string.Empty;
            this.modDesc.text = string.Empty;
            this.modIcon.texture = UIReferences.GetTransparent();
            this.btMoveUp.interactable = false;
            this.btMoveDown.interactable = false;
        }
    }

    private void ModItem(GameObject itemSource, object source, object data, int index)
    {
        ModListItem item = itemSource.GetComponent<ModListItem>();
        ModSettings mod = source as ModSettings;
        item.text.text = mod.title;
        item.icon.texture = AssetManager.Get<Texture2D>(mod.icon, passModValidation: false);
        itemSource.GetComponent<Toggle>().isOn = this.modsGrid.GetSelectedObject<ModSettings>() == source;
        item.toggle.onValueChanged.RemoveAllListeners();
        item.toggle.isOn = this.switches.ContainsKey(mod) && this.switches[mod];
        item.modOn.SetActive(item.toggle.isOn);
        item.modOff.SetActive(!item.toggle.isOn);
        string text = ModManager.Get().ValidateMod(mod);
        item.warning.SetActive(!string.IsNullOrEmpty(text));
        if (text != null)
        {
            item.tooltip.title = Localization.Get("UI_WARNING", true);
            item.tooltip.description = text;
        }
        item.toggle.onValueChanged.AddListener(delegate(bool b)
        {
            this.dirty = true;
            this.switches[mod] = b;
            item.modOn.SetActive(b);
            item.modOff.SetActive(!b);
        });
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btClose)
        {
            UIManager.Close(this);
        }
        else if (s == this.btMoveUp)
        {
            ModSettings selectedObject = this.modsGrid.GetSelectedObject<ModSettings>();
            int num = this.list.IndexOf(selectedObject);
            if (num > 0)
            {
                this.dirty = true;
                this.list.Remove(selectedObject);
                this.list.Insert(num - 1, selectedObject);
                this.UpdateManagerModOrder();
                this.ModGridUpdate(autoSelect: false);
                if (this.modsGrid.GetGameObjectForData(selectedObject) == null)
                {
                    this.modsGrid.PrevPage();
                }
                this.modsGrid.Select(selectedObject);
            }
        }
        else if (s == this.btMoveDown)
        {
            ModSettings selectedObject2 = this.modsGrid.GetSelectedObject<ModSettings>();
            int num2 = this.list.IndexOf(selectedObject2);
            if (num2 >= 0 && num2 < this.list.Count)
            {
                this.dirty = true;
                this.list.Remove(selectedObject2);
                this.list.Insert(num2 + 1, selectedObject2);
                this.UpdateManagerModOrder();
                this.ModGridUpdate(autoSelect: false);
                if (this.modsGrid.GetGameObjectForData(selectedObject2) == null)
                {
                    this.modsGrid.NextPage();
                }
                this.modsGrid.Select(selectedObject2);
            }
        }
        else if (s == this.btPublish)
        {
            ModSettings selectedObject3 = this.modsGrid.GetSelectedObject<ModSettings>();
            if (selectedObject3 != null)
            {
                this.publishPanel.SetActive(value: true);
                base.StartCoroutine(this.PopulatePublish(selectedObject3));
            }
        }
        else if (s == this.btConfirm)
        {
            this.publish = true;
        }
        else if (s == this.btCancel)
        {
            this.closePopup = true;
        }
    }

    private object ListPublishOptions(object o)
    {
        return new List<string>
        {
            Localization.Get("UI_SELECT_VISIBILITY", true),
            Localization.Get("UI_PUBLIC", true),
            Localization.Get("UI_FRIENDS_ONLY", true),
            Localization.Get("UI_PRIVATE", true)
        };
    }

    private void PublishSelectionChanged(object o)
    {
        if (this.dropdownPublishMode.GetSelectionNR() == 0)
        {
            this.btPublish.interactable = false;
        }
        else
        {
            this.btPublish.interactable = true;
        }
    }

    private IEnumerator PopulatePublish(ModSettings info)
    {
        this.closePopup = false;
        this.publish = false;
        this.uploadProgress.SetActive(value: false);
        this.publishModIcon.texture = AssetManager.Get<Texture2D>(info.icon, passModValidation: false);
        this.publishModName.text = info.name;
        this.publishModDesc.text = info.description;
        this.publishVisibility.text = this.dropdownPublishMode.GetSelection();
        while (!this.publish)
        {
            if (this.closePopup)
            {
                this.publishPanel.SetActive(value: false);
                yield break;
            }
            yield return null;
        }
        this.uploadProgress.SetActive(value: true);
        SteamWorkshop sw = new SteamWorkshop();
        switch (this.dropdownPublishMode.GetSelectionNR())
        {
        case 1:
            base.StartCoroutine(sw.PublishMod(info, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic));
            break;
        case 2:
            base.StartCoroutine(sw.PublishMod(info, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly));
            break;
        case 3:
            base.StartCoroutine(sw.PublishMod(info, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate));
            break;
        }
        this.uploadProgress.SetActive(value: true);
        while (true)
        {
            SteamWorkshop.WorkshopUpdateStatus status = sw.GetStatus();
            if (status.finished)
            {
                break;
            }
            int num = 5;
            float num2 = (float)status.status / (float)num;
            float num3 = status.curentBytes / (1 + status.totalBytes);
            num2 += num3 * (1f / (float)num);
            this.sliderTotalUpload.value = Mathf.RoundToInt(num2 * 100f);
            TextMeshProUGUI textMeshProUGUI = this.currentUpload;
            int status2 = (int)status.status;
            textMeshProUGUI.text = status2 + "/" + num;
            yield return null;
        }
        this.uploadProgress.SetActive(value: false);
        this.publishPanel.SetActive(value: false);
    }
}
