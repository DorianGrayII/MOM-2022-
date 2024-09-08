// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupCreateArtefact
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCreateArtefact : ScreenBase
{
    private class ItemGraphic
    {
        public EEquipmentType etype;

        public string graphic;
    }

    private class PowerEntry
    {
        public ArtefactPower power;

        public ArtefactPowerSet set;

        public bool showDivider;

        public PowerEntry(ArtefactPower power = null)
        {
            this.power = power;
            if (power != null)
            {
                this.set = global::MOM.Artefact.GetSet(power);
            }
        }
    }

    public Toggle btSword;

    public Toggle btShield;

    public Toggle btWand;

    public Toggle btBow;

    public Toggle btMace;

    public Toggle btChain;

    public Toggle btMisc;

    public Toggle btStaff;

    public Toggle btAxe;

    public Toggle btPlate;

    public Button btCancel;

    public Button btConfirm;

    public Button btNextImage;

    public Button btPrevImage;

    public RawImage riItem;

    public TextMeshProUGUI labelCost;

    public TMP_InputField inputItemName;

    public GridItemManager gridBonuses;

    public GridItemManager gridSpells;

    public GridItemManager gridEnchantments;

    public GameObject goHeadingCreate;

    public GameObject goHeadingEnchant;

    public GameObject goMaxEnchantments;

    private readonly Dictionary<Toggle, int> btEquipmentTypes = new Dictionary<Toggle, int>();

    private List<PowerEntry> bonuses = new List<PowerEntry>();

    private List<PowerEntry> spells = new List<PowerEntry>();

    private readonly PowerEntry blankPowerEntry = new PowerEntry();

    private readonly List<ArtefactPower> selectedPowers = new List<ArtefactPower>();

    private List<ItemGraphic> graphics;

    private int graphicIndex;

    private string lastDefaultItemName;

    private EEquipmentType lastDefaultType;

    private Dictionary<int, List<ItemGraphic>> graphicsByType;

    private int cost;

    private static Spell spell;

    private static Toggle selectedTypeToggle;

    private new static CastSpells parent;

    public static void Popup(Spell spell, CastSpells parent)
    {
        PopupCreateArtefact.parent = parent;
        PopupCreateArtefact.spell = spell;
        if (UIManager.GetScreen<PopupCreateArtefact>(UIManager.Layer.Popup) == null)
        {
            UIManager.Open<PopupCreateArtefact>(UIManager.Layer.Popup, parent);
        }
    }

    public override IEnumerator PreStart()
    {
        this.btEquipmentTypes[this.btSword] = 1;
        this.btEquipmentTypes[this.btShield] = 256;
        this.btEquipmentTypes[this.btWand] = 32;
        this.btEquipmentTypes[this.btBow] = 8;
        this.btEquipmentTypes[this.btMace] = 2;
        this.btEquipmentTypes[this.btStaff] = 16;
        this.btEquipmentTypes[this.btAxe] = 4;
        this.btEquipmentTypes[this.btChain] = 128;
        this.btEquipmentTypes[this.btPlate] = 64;
        this.btEquipmentTypes[this.btMisc] = 15872;
        PopupCreateArtefact.selectedTypeToggle = this.btSword;
        PopupCreateArtefact.selectedTypeToggle.isOn = true;
        this.inputItemName.text = "";
        foreach (Toggle toggle in this.btEquipmentTypes.Keys)
        {
            toggle.onValueChanged.AddListener(delegate
            {
                if (toggle.isOn)
                {
                    this.TypeSelected(toggle);
                }
            });
        }
        this.gridSpells.CustomDynamicItem(PowerItem, delegate
        {
            this.gridSpells.UpdateGrid(this.spells);
        });
        this.gridBonuses.CustomDynamicItem(PowerItem, delegate
        {
            this.gridBonuses.UpdateGrid(this.bonuses);
        });
        this.gridEnchantments.CustomDynamicItem(EnchantmentsItem, delegate
        {
            this.gridEnchantments.UpdateGrid(this.selectedPowers);
        });
        this.btConfirm.onClick.AddListener(Cast);
        this.btNextImage.onClick.AddListener(delegate
        {
            this.CycleImage(1);
        });
        this.btPrevImage.onClick.AddListener(delegate
        {
            this.CycleImage(-1);
        });
        this.UpdateTypeDependencies();
        yield return base.PreStart();
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btCancel)
        {
            UIManager.Close(this);
        }
    }

    private void TypeSelected(Toggle toggle)
    {
        if (toggle != PopupCreateArtefact.selectedTypeToggle)
        {
            PopupCreateArtefact.selectedTypeToggle = toggle;
            this.UpdateTypeDependencies();
        }
    }

    protected void EnchantmentsItem(GameObject itemSource, object source, object data, int index)
    {
        ArtefactPower artefactPower = source as ArtefactPower;
        TextMeshProUGUI componentInChildren = itemSource.GetComponentInChildren<TextMeshProUGUI>();
        componentInChildren.text = this.GetPowerName(artefactPower);
        if (componentInChildren.text == "DES_PLACEHOLDER")
        {
            componentInChildren.text = artefactPower.skill.dbName;
        }
        GameObjectUtils.FindByNameGetComponent<RolloverSimpleTooltip>(base.gameObject, itemSource.name).sourceAsDbName = artefactPower.skill.dbName;
    }

    protected void PowerItem(GameObject itemSource, object source, object data, int index)
    {
        PowerEntry powerEntry = source as PowerEntry;
        if (powerEntry.power == null)
        {
            itemSource.SetActive(value: false);
            return;
        }
        PowerListItem field = itemSource.GetComponent<PowerListItem>();
        int curPage = field.owner.GetPageNr();
        field.toggle.onValueChanged.RemoveAllListeners();
        if ((bool)field.divider)
        {
            field.divider.SetActive(powerEntry.showDivider);
        }
        field.toggle.isOn = this.selectedPowers.Contains(powerEntry.power);
        field.text.text = this.GetPowerName(powerEntry.power);
        field.power = powerEntry.power;
        field.tooltip.sourceAsDbName = powerEntry.power.skill.dbName;
        if (field.text.text == "DES_PLACEHOLDER")
        {
            field.text.text = powerEntry.power.skill.dbName;
        }
        field.toggle.onValueChanged.AddListener(delegate(bool selected)
        {
            if (curPage == field.owner.GetPageNr())
            {
                if (selected)
                {
                    if (!this.selectedPowers.Contains(powerEntry.power))
                    {
                        ArtefactPower[] power = global::MOM.Artefact.GetSet(powerEntry.power).power;
                        foreach (ArtefactPower artefactPower in power)
                        {
                            if (artefactPower != powerEntry.power && this.selectedPowers.Contains(artefactPower))
                            {
                                this.selectedPowers.Remove(artefactPower);
                                PowerListItem[] componentsInChildren = this.gridBonuses.GetComponentsInChildren<PowerListItem>();
                                foreach (PowerListItem powerListItem in componentsInChildren)
                                {
                                    if (powerListItem.power == artefactPower)
                                    {
                                        powerListItem.toggle.isOn = false;
                                    }
                                }
                            }
                        }
                        if (this.selectedPowers.FindAll((ArtefactPower o) => o.cost != -1).Count < 4)
                        {
                            this.selectedPowers.Add(powerEntry.power);
                        }
                        else
                        {
                            this.goMaxEnchantments.SetActive(value: true);
                            field.toggle.isOn = false;
                        }
                    }
                }
                else
                {
                    this.selectedPowers.Remove(powerEntry.power);
                    if (this.selectedPowers.FindAll((ArtefactPower o) => o.cost != -1).Count < 4)
                    {
                        this.goMaxEnchantments.SetActive(value: false);
                    }
                }
                this.UpdateEnchantments();
            }
        });
    }

    private void UpdateEnchantments()
    {
        this.gridEnchantments.UpdateGrid(this.selectedPowers);
        this.UpdateCost();
    }

    private void InitGraphicsByType()
    {
        this.graphicsByType = new Dictionary<int, List<ItemGraphic>>();
        foreach (ArtefactPrefab item2 in DataBase.GetType<ArtefactPrefab>())
        {
            int key = ConvertType(item2.eType);
            if (!this.graphicsByType.TryGetValue(key, out var value))
            {
                value = new List<ItemGraphic>();
                this.graphicsByType[key] = value;
            }
            AddGraphicIfUnique(value, item2.eType, item2.descriptionInfo.graphic);
            ArtefactGraphic[] alternativeGraphic = item2.alternativeGraphic;
            if (alternativeGraphic != null)
            {
                ArtefactGraphic[] array = alternativeGraphic;
                foreach (ArtefactGraphic artefactGraphic in array)
                {
                    AddGraphicIfUnique(value, item2.eType, artefactGraphic.descriptionInfo.graphic);
                }
            }
        }
        foreach (global::DBDef.Artefact item3 in DataBase.GetType<global::DBDef.Artefact>())
        {
            int key2 = ConvertType(item3.eType);
            if (!this.graphicsByType.TryGetValue(key2, out var value2))
            {
                value2 = new List<ItemGraphic>();
                this.graphicsByType[key2] = value2;
            }
            AddGraphicIfUnique(value2, item3.eType, item3.descriptionInfo.graphic);
        }
        void AddGraphicIfUnique(List<ItemGraphic> list, EEquipmentType type, string graphic)
        {
            if (!string.IsNullOrEmpty(graphic) && !(AssetManager.Get<Texture2D>(graphic) == null) && !list.Exists((ItemGraphic x) => x.graphic == graphic && x.etype == type))
            {
                ItemGraphic item = new ItemGraphic
                {
                    graphic = graphic,
                    etype = type
                };
                list.Add(item);
            }
        }
        int ConvertType(EEquipmentType eType)
        {
            if ((eType & (EEquipmentType)15872) != 0)
            {
                return 15872;
            }
            return (int)eType;
        }
    }

    private void CycleImage(int dir)
    {
        this.graphicIndex = (this.graphicIndex + dir + this.graphics.Count) % this.graphics.Count;
        this.UpdateGraphicAndName();
        this.UpdateCost();
    }

    private void UpdateGraphicAndName()
    {
        ItemGraphic itemGraphic = this.graphics[this.graphicIndex];
        this.riItem.texture = AssetManager.Get<Texture2D>(itemGraphic.graphic);
        if (this.lastDefaultType != itemGraphic.etype)
        {
            this.lastDefaultType = itemGraphic.etype;
            string localizedName = DataBase.GetType<ArtefactPrefab>().Find((ArtefactPrefab o) => o.eType == this.lastDefaultType).descriptionInfo.GetLocalizedName();
            string text = this.inputItemName.text;
            if (text == null || text.Length == 0 || text == this.lastDefaultItemName)
            {
                this.inputItemName.text = localizedName;
            }
            this.lastDefaultItemName = localizedName;
        }
    }

    private string GetPowerName(ArtefactPower power)
    {
        int num = 0;
        if (power.skill.script != null && power.skill.script.Length != 0)
        {
            num = power.skill.script[0].fIntParam.ToInt();
        }
        return string.Format(power.skill.GetDescriptionInfo().GetLocalizedName(), num);
    }

    private void UpdateTypeDependencies()
    {
        int num = this.btEquipmentTypes[PopupCreateArtefact.selectedTypeToggle];
        List<ArtefactPower> list = global::MOM.Artefact.GetBonuses(num);
        List<ArtefactPower> list2 = global::MOM.Artefact.GetSpells(num);
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        this.spells.Clear();
        foreach (ArtefactPower item in list2)
        {
            ArtefactPowerSet set = global::MOM.Artefact.GetSet(item);
            bool flag = true;
            CountedTag[] requiredTag = set.requiredTag;
            foreach (CountedTag countedTag in requiredTag)
            {
                flag = humanWizard.attributes.Contains(countedTag.tag, countedTag.amount);
            }
            if ((flag && item.cost <= 200 && PopupCreateArtefact.spell == (Spell)SPELL.ENCHANT_ITEM) || (flag && PopupCreateArtefact.spell == (Spell)SPELL.CREATE_ARTEFACT))
            {
                this.spells.Add(new PowerEntry(item));
            }
        }
        this.goMaxEnchantments.SetActive(value: false);
        this.bonuses.Clear();
        int num2 = 10;
        foreach (ArtefactPower item2 in list)
        {
            if ((item2.cost > 200 && PopupCreateArtefact.spell == (Spell)SPELL.ENCHANT_ITEM) || item2.cost <= -1)
            {
                continue;
            }
            PowerEntry powerEntry = new PowerEntry(item2);
            int count = this.bonuses.Count;
            if (count % num2 == 0)
            {
                if (count != 0)
                {
                    int num3 = 0;
                    count--;
                    while (this.bonuses[count].set == powerEntry.set && num3 < num2)
                    {
                        this.bonuses[count].showDivider = false;
                        num3++;
                        count--;
                    }
                    count++;
                    if (num3 < num2)
                    {
                        while (num3-- > 0)
                        {
                            this.bonuses.Insert(count, this.blankPowerEntry);
                        }
                    }
                }
            }
            else if (this.bonuses[count - 1].set != powerEntry.set)
            {
                powerEntry.showDivider = true;
            }
            this.bonuses.Add(powerEntry);
        }
        if (this.graphicsByType == null)
        {
            this.InitGraphicsByType();
        }
        this.graphics = this.graphicsByType[num];
        this.graphicIndex = 0;
        this.UpdateGraphicAndName();
        this.ValidateSelected();
        this.AddGuarantedPowers(this.selectedPowers, this.lastDefaultType);
        this.gridSpells.UpdateGrid(this.spells);
        this.gridBonuses.UpdateGrid(this.bonuses);
        this.gridEnchantments.UpdateGrid(this.selectedPowers);
        this.UpdateCost();
    }

    private void ValidateSelected()
    {
        for (int num = this.selectedPowers.Count - 1; num >= 0; num--)
        {
            ArtefactPower sp = this.selectedPowers[num];
            if (!this.spells.Exists((PowerEntry p) => p.power == sp) && !this.bonuses.Exists((PowerEntry p) => p.power == sp))
            {
                this.selectedPowers.RemoveAt(num);
            }
        }
    }

    private void UpdateCost()
    {
        this.cost = 0;
        if (this.selectedPowers.Count > 0)
        {
            EEquipmentType type = this.graphics[this.graphicIndex].etype;
            ArtefactPrefab artefactPrefab = DataBase.GetType<ArtefactPrefab>().Find((ArtefactPrefab o) => o.eType == type);
            this.cost += artefactPrefab.cost;
            foreach (ArtefactPower selectedPower in this.selectedPowers)
            {
                this.cost += selectedPower.cost;
            }
        }
        this.btConfirm.interactable = this.cost > 0;
        float num = GameManager.GetHumanWizard().GetCastCostPercent(PopupCreateArtefact.spell).ToFloat();
        this.cost = Mathf.RoundToInt((float)this.cost * (1f - num));
        this.labelCost.text = global::DBUtils.Localization.Get("UI_COST", true) + this.cost;
    }

    private void Cast()
    {
        MagicAndResearch magicAndResearch = GameManager.GetHumanWizard().GetMagicAndResearch();
        CraftItemSpell craftItemSpell = new CraftItemSpell
        {
            artefact = new global::MOM.Artefact
            {
                name = this.inputItemName.text,
                graphic = this.graphics[this.graphicIndex].graphic,
                equipmentType = this.graphics[this.graphicIndex].etype,
                artefactPowers = new List<DBReference<ArtefactPower>>()
            },
            cost = this.cost
        };
        foreach (ArtefactPower selectedPower in this.selectedPowers)
        {
            craftItemSpell.artefact.artefactPowers.Add(selectedPower);
        }
        magicAndResearch.curentlyCastSpell = PopupCreateArtefact.spell;
        magicAndResearch.craftItemSpell = craftItemSpell;
        UIManager.Close(this);
        PopupCreateArtefact.parent.btClose.onClick.Invoke();
        HUD.Get().UpdateCastingButton();
    }

    private void AddGuarantedPowers(List<ArtefactPower> a, EEquipmentType e)
    {
        foreach (ArtefactPower item in DataBase.GetType<ArtefactPower>().FindAll((ArtefactPower o) => o.cost == -1))
        {
            EEquipmentType[] eTypes = item.eTypes;
            for (int i = 0; i < eTypes.Length; i++)
            {
                if (eTypes[i] == e)
                {
                    a.Add(item);
                }
            }
        }
    }
}
