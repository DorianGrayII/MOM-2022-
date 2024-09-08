// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupDisjunction
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDisjunction : ScreenBase
{
    public Button btCancel;

    public Button btCloseMoreEnchantments;

    public GameObject goGlobal;

    public GameObject goWizard1;

    public GameObject goWizard2;

    public GameObject goWizard3;

    public GameObject goWizard4;

    public GameObject goWizard5;

    public GameObject goMoreEnchantments;

    public GameObject goMoreEnchantmentsGreen;

    public GameObject goMoreEnchantmentsBlue;

    public GameObject goMoreEnchantmentsRed;

    public GameObject goMoreEnchantmentsPurple;

    public GameObject goMoreEnchantmentsYellow;

    public GameObject goMoreEnchantmentsWizard;

    public GameObject goMoreEnchantmentsUnknownWizard;

    public RawImage riMoreEnchantmentsWizard;

    public GridItemManager gridMoreEnchantments;

    public TextMeshProUGUI labelMoreEnchantmentsWizardName;

    private Callback cancel;

    private Callback confirm;

    private static PopupDisjunction instance;

    public static void OpenPopup(ScreenBase parent, Spell spell, List<PlayerWizard> wizards, PlayerWizard caster, Callback confirm = null, Callback cancel = null)
    {
        PopupDisjunction.instance = UIManager.Open<PopupDisjunction>(UIManager.Layer.Popup, parent);
        PopupDisjunction.instance.cancel = cancel;
        PopupDisjunction.instance.confirm = confirm;
        PopupDisjunction.instance.goMoreEnchantments.SetActive(value: false);
        for (int i = 0; i < wizards.Count; i++)
        {
            string text = "Wizard" + (1 + i);
            PopupDisjunction.instance.WizardItem(wizards[i], GameObject.Find(text), caster);
        }
        for (int j = wizards.Count; j < 5; j++)
        {
            string text = "Wizard" + (1 + j);
            GameObject.Find(text).SetActive(value: false);
        }
        PopupDisjunction.instance.ActiveGlobalEnchantmentBt(PopupDisjunction.instance.goGlobal, caster);
    }

    private void WizardItem(PlayerWizard wizardTarget, GameObject wizardGO = null, PlayerWizard caster = null)
    {
        if (wizardTarget == null || wizardGO == null || caster == null)
        {
            Debug.LogError("WizardItem() in class PopupDisjunction miss critical informations.");
        }
        if (!wizardTarget.isAlive)
        {
            wizardGO.SetActive(value: false);
            return;
        }
        WizardListItem2 component = wizardGO.GetComponent<WizardListItem2>();
        component.gemBlue.SetActive(wizardTarget.color == PlayerWizard.Color.Blue);
        component.gemGreen.SetActive(wizardTarget.color == PlayerWizard.Color.Green);
        component.gemPurple.SetActive(wizardTarget.color == PlayerWizard.Color.Purple);
        component.gemRed.SetActive(wizardTarget.color == PlayerWizard.Color.Red);
        component.gemYellow.SetActive(wizardTarget.color == PlayerWizard.Color.Yellow);
        if (caster.GetDiscoveredWizards().Find((Reference<PlayerWizard> o) => o.ID == wizardTarget.ID) != null || wizardTarget.ID == caster.ID)
        {
            component.icon.texture = wizardTarget.Graphic;
            component.labelName.text = wizardTarget.name;
        }
        else
        {
            component.icon.gameObject.SetActive(value: false);
            component.labelName.text = global::DBUtils.Localization.Get("UI_UNKNOWN_WIZARD", true);
        }
        List<EnchantmentInstance> enchList = wizardTarget.GetEnchantments().FindAll((EnchantmentInstance o) => o.owner?.ID != caster.ID && o.source.Get().allowDispel);
        this.ActiveEnchantmentBt(wizardTarget, component, enchList);
    }

    private void ActiveEnchantmentBt(PlayerWizard wizardTarget, WizardListItem2 wizardItem, List<EnchantmentInstance> enchList)
    {
        if (wizardItem == null || enchList == null)
        {
            Debug.LogError("ActiveEnchantmentBt() miss critical informations.");
        }
        List<Button> list = new List<Button>();
        if (enchList.Count < wizardItem.btEnchantment.Length)
        {
            wizardItem.btMoreEnchantments.gameObject.SetActive(value: false);
            for (int i = enchList.Count; i < wizardItem.btEnchantment.Length; i++)
            {
                wizardItem.btEnchantment[i].gameObject.SetActive(value: false);
            }
            for (int j = 0; j < enchList.Count; j++)
            {
                list.Add(wizardItem.btEnchantment[j]);
                wizardItem.riEnchantment[j].texture = enchList[j].source.Get().descriptionInfo.GetTexture();
            }
        }
        else if (enchList.Count == wizardItem.btEnchantment.Length)
        {
            wizardItem.btMoreEnchantments.gameObject.SetActive(value: false);
            for (int k = 0; k < enchList.Count; k++)
            {
                list.Add(wizardItem.btEnchantment[k]);
                wizardItem.riEnchantment[k].texture = enchList[k].source.Get().descriptionInfo.GetTexture();
            }
        }
        else
        {
            wizardItem.btEnchantment[6].gameObject.SetActive(value: false);
            for (int l = 0; l < Mathf.Min(enchList.Count, 6); l++)
            {
                list.Add(wizardItem.btEnchantment[l]);
                wizardItem.riEnchantment[l].texture = enchList[l].source.Get().descriptionInfo.GetTexture();
            }
            this.MoreEnchantmentsActive(wizardTarget, wizardItem, enchList);
        }
        foreach (Button item in list)
        {
            EnchantmentInstance actualEnchantment = enchList[list.IndexOf(item)];
            this.EnchantmentButtonItem(item.gameObject, actualEnchantment, item);
            item.onClick.RemoveAllListeners();
            item.onClick.AddListener(delegate
            {
                UIManager.Close(this);
                this.confirm(actualEnchantment);
                TooltipBase.Close();
            });
        }
    }

    private void ActiveGlobalEnchantmentBt(GameObject globalGO = null, PlayerWizard caster = null)
    {
        if (globalGO == null || caster == null)
        {
            Debug.LogError("ActiveGlobalEnchantmentBt() miss critical informations.");
        }
        List<Button> list = new List<Button>();
        List<EnchantmentInstance> list2 = GameManager.Get().GetEnchantments().FindAll((EnchantmentInstance o) => o.owner.ID != caster.ID);
        WizardListItem2 component = globalGO.GetComponent<WizardListItem2>();
        if (list2.Count < component.btEnchantment.Length)
        {
            component.btMoreEnchantments.gameObject.SetActive(value: false);
            for (int i = list2.Count; i < component.btEnchantment.Length; i++)
            {
                component.btEnchantment[i].gameObject.SetActive(value: false);
            }
            for (int j = 0; j < list2.Count; j++)
            {
                list.Add(component.btEnchantment[j]);
                component.riEnchantment[j].texture = list2[j].source.Get().descriptionInfo.GetTexture();
            }
        }
        else if (list2.Count == component.btEnchantment.Length)
        {
            component.btMoreEnchantments.gameObject.SetActive(value: false);
            for (int k = 0; k < list2.Count; k++)
            {
                list.Add(component.btEnchantment[k]);
                component.riEnchantment[k].texture = list2[k].source.Get().descriptionInfo.GetTexture();
            }
        }
        else
        {
            component.btEnchantment[6].gameObject.SetActive(value: false);
            for (int l = 0; l < Mathf.Min(list2.Count, 6); l++)
            {
                list.Add(component.btEnchantment[l]);
                component.riEnchantment[l].texture = list2[l].source.Get().descriptionInfo.GetTexture();
            }
            this.MoreEnchantmentsActive(null, component, list2);
        }
        foreach (Button item in list)
        {
            EnchantmentInstance actualEnchantment = list2[list.IndexOf(item)];
            this.EnchantmentButtonItem(item.gameObject, actualEnchantment, item);
            item.onClick.RemoveAllListeners();
            item.onClick.AddListener(delegate
            {
                UIManager.Close(this);
                this.confirm(actualEnchantment);
            });
        }
    }

    private void EnchantmentButtonItem(GameObject itemSource, object source, Button bt)
    {
        EnchantmentInstance e = source as EnchantmentInstance;
        itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = e.source.Get().dbName;
        bt.gameObject.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
        {
            if (e.source != null)
            {
                ScreenBase componentInParent = base.GetComponentInParent<ScreenBase>();
                UIManager.Open<PopupEnchantmentInfo>(UIManager.Layer.Popup, componentInParent).Set(e);
            }
        };
    }

    private void EnchantmentGridItem(GameObject itemSource, object source, object data, int index)
    {
        EnchantmentInstance e = source as EnchantmentInstance;
        GameObjectUtils.FindByNameGetComponentInChildren<RawImage>(itemSource, "EnchantmentIcon").texture = e.source.Get().GetDescriptionInfo().GetTexture();
        itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = e.source.Get().dbName;
        itemSource.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
        {
            if (e.source != null)
            {
                ScreenBase componentInParent = base.GetComponentInParent<ScreenBase>();
                UIManager.Open<PopupEnchantmentInfo>(UIManager.Layer.Popup, componentInParent).Set(e);
            }
        };
        Button component = itemSource.GetComponent<Button>();
        if (!(component != null))
        {
            return;
        }
        component.onClick.RemoveAllListeners();
        component.onClick.AddListener(delegate
        {
            UIManager.Close(this);
            if (this.confirm != null)
            {
                this.confirm(source);
            }
            TooltipBase.Close();
        });
    }

    private void MoreEnchantmentsActive(PlayerWizard wizardTarget, WizardListItem2 wizardItem, List<EnchantmentInstance> enchList)
    {
        if (wizardTarget != null)
        {
            this.goMoreEnchantmentsWizard.SetActive(value: true);
            this.goMoreEnchantmentsUnknownWizard.SetActive(value: true);
            this.goMoreEnchantmentsBlue.SetActive(wizardTarget.color == PlayerWizard.Color.Blue);
            this.goMoreEnchantmentsGreen.SetActive(wizardTarget.color == PlayerWizard.Color.Green);
            this.goMoreEnchantmentsPurple.SetActive(wizardTarget.color == PlayerWizard.Color.Purple);
            this.goMoreEnchantmentsRed.SetActive(wizardTarget.color == PlayerWizard.Color.Red);
            this.goMoreEnchantmentsYellow.SetActive(wizardTarget.color == PlayerWizard.Color.Yellow);
            this.goMoreEnchantmentsWizard.GetComponent<RawImage>().texture = wizardTarget.Graphic;
            this.labelMoreEnchantmentsWizardName.text = wizardTarget.name;
        }
        else
        {
            this.goMoreEnchantmentsWizard.SetActive(value: false);
            this.goMoreEnchantmentsUnknownWizard.SetActive(value: false);
            this.labelMoreEnchantmentsWizardName.text = "UI_GLOBAL_ENCHANTMENTS";
        }
        List<EnchantmentInstance> enchMoreList = new List<EnchantmentInstance>(enchList);
        enchMoreList.RemoveRange(0, 6);
        wizardItem.btMoreEnchantments.onClick.RemoveAllListeners();
        wizardItem.btMoreEnchantments.onClick.AddListener(delegate
        {
            this.goMoreEnchantments.gameObject.SetActive(value: true);
            this.gridMoreEnchantments.CustomDynamicItem(EnchantmentGridItem);
            this.gridMoreEnchantments.UpdateGrid(enchMoreList);
        });
        this.btCloseMoreEnchantments.onClick.RemoveAllListeners();
        this.btCloseMoreEnchantments.onClick.AddListener(delegate
        {
            this.goMoreEnchantments.gameObject.SetActive(value: false);
        });
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btCancel)
        {
            UIManager.Close(this);
            if (this.cancel != null)
            {
                this.cancel(null);
            }
            TooltipBase.Close();
        }
    }

    public static bool IsOpen()
    {
        return PopupDisjunction.instance != null;
    }

    public override IEnumerator Closing()
    {
        PopupDisjunction.instance = null;
        yield return base.Closing();
    }
}
