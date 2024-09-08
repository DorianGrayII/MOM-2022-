// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.UnitInfo
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : ScreenBase
{
    public Button buttonClose;

    public Button buttonHire;

    public Button buttonReject;

    public Button buttonJoin;

    public Button buttonDismiss;

    public Button buttonEquip;

    public Button buttonSpellbook;

    public Button buttonPrevUnit;

    public Button buttonNextUnit;

    public TextMeshProUGUI labelUnitName;

    public TextMeshProUGUI labelMelee;

    public TextMeshProUGUI labelMeleeChanceToHit;

    public TextMeshProUGUI labelRange;

    public TextMeshProUGUI labelRangeChanceToHit;

    public TextMeshProUGUI labelArmour;

    public TextMeshProUGUI labelChanceToDefend;

    public TextMeshProUGUI labelResist;

    public TextMeshProUGUI labelChanceToResist;

    public TextMeshProUGUI labelFigures;

    public TextMeshProUGUI labelHits;

    public TextMeshProUGUI labelHP;

    public TextMeshProUGUI labelMana;

    public TextMeshProUGUI labelAmmo;

    public TextMeshProUGUI labelRecruit;

    public TextMeshProUGUI labelExp;

    public TextMeshProUGUI labelMP;

    public TextMeshProUGUI labelUpkeepMoney;

    public TextMeshProUGUI labelUpkeepFood;

    public TextMeshProUGUI labelUpkeepMana;

    public TextMeshProUGUI resGold;

    public TextMeshProUGUI resMana;

    public TextMeshProUGUI resFood;

    public TextMeshProUGUI resFame;

    public GameObject itemMeleeIcons;

    public GameObject itemRangeIcons;

    public GameObject itemArmourIcons;

    public GameObject itemResistIcons;

    public GameObject itemFiguresIcons;

    public GameObject itemHitsIcons;

    public GameObject movement;

    public GameObject upkeep;

    public GameObject mana;

    public GameObject heroRecruit;

    public GameObject heroJoin;

    public GameObject leftPanel;

    public GameObject jail;

    public GameObject movementWalking;

    public GameObject movementSwimming;

    public GameObject movementFlying;

    public GameObject itemUpkeepMoney;

    public GameObject itemUpkeepFood;

    public GameObject itemUpkeepMana;

    public GameObject normalMelee;

    public GameObject enchantedMelee;

    public GameObject chaosRanged;

    public GameObject natureRanged;

    public GameObject sorceryRanged;

    public GameObject deathRanged;

    public GameObject lifeRanged;

    public GameObject techRanged;

    public GameObject missileRanged;

    public GameObject boulderRanged;

    public GameObject enchantedRanged;

    public GameObject fantasticUnit;

    public GameObject construct;

    public GameObject story;

    public GameObject storyTooltipPosition;

    public GameObject resources;

    public GameObject offeredCharacters;

    public GridItemManager gridSkillsEnchantments;

    public GridItemManager gridOfferedCharacters;

    public Slider sliderHealth;

    public Toggle tgFilterAll;

    public Toggle tgFilterSkills;

    public Toggle tgFilterEnchantments;

    public RawImage jailHeroPortrait;

    public Tutorial_Generic tutorialUnitInfo;

    private bool multipleUnitsOffer;

    private object _unit;

    private global::MOM.Group _group;

    private BattleUnit unitBaseRef;

    public List<object> sources;

    public RawImage largeGraphic;

    public RawImage largeGraphicMultiply;

    public UnitExperience experience;

    public RawImage race;

    private UnitOffer buyInfo;

    private List<UnitOffer> buySetInfo;

    private SimpleCallback callback;

    private Callback hireCallback;

    public bool heroJoinMode;

    public object unit
    {
        get
        {
            return this._unit;
        }
        set
        {
            if (value == this._unit)
            {
                return;
            }
            global::MOM.Unit u = value as global::MOM.Unit;
            if (u != null && u.group != null)
            {
                this._group = u.group.Get();
                if (this._group.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get() == u) == null)
                {
                    this._group = null;
                }
            }
            else
            {
                this._group = null;
            }
            this._unit = value;
            this.unitBaseRef = BaseUnit.UnitBaseRef(value as BaseUnit);
            bool interactable = this._group != null && this._group.GetOwnerID() == PlayerWizard.HumanID();
            this.buttonDismiss.interactable = interactable;
            this.buttonEquip.interactable = interactable;
            if (this.buttonDismiss.interactable && !this.heroJoinMode)
            {
                this.tutorialUnitInfo.OpenIfNotSeen(this);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.buttonClose.gameObject.SetActive(value: true);
        this.buttonPrevUnit.gameObject.SetActive(value: true);
        this.buttonNextUnit.gameObject.SetActive(value: true);
        this.buttonSpellbook.onClick.AddListener(OnSpellBook);
        this.resources.SetActive(value: false);
    }

    public override IEnumerator PreStart()
    {
        yield return base.PreStart();
        this.gridSkillsEnchantments.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            if (source is Skill skill)
            {
                DescriptionInfo descriptionInfo = skill.GetDescriptionInfo();
                SkillEnchantmentListItem component = itemSource.GetComponent<SkillEnchantmentListItem>();
                component.skill.SetActive(value: true);
                component.enchantment.SetActive(value: false);
                component.btCancelEnchantment.gameObject.SetActive(value: false);
                component.skillImage.texture = descriptionInfo.GetTexture();
                if (!string.IsNullOrEmpty(skill.descriptionScript))
                {
                    component.label.text = (string)ScriptLibrary.Call(skill.descriptionScript, this.unit, skill, null);
                }
                else
                {
                    component.label.text = descriptionInfo.GetLocalizedName();
                }
                RolloverSimpleTooltip orAddComponent = component.skillTooltip.GetOrAddComponent<RolloverSimpleTooltip>();
                orAddComponent.image = descriptionInfo.GetTexture();
                orAddComponent.description = descriptionInfo.GetLocalizedDescription();
                orAddComponent.title = component.label.text;
                orAddComponent.sourceAsDbName = null;
            }
            else
            {
                EnchantmentInstance ench = source as EnchantmentInstance;
                if (ench != null)
                {
                    DescriptionInfo descriptionInfo2 = ench.source.Get().GetDescriptionInfo();
                    SkillEnchantmentListItem component2 = itemSource.GetComponent<SkillEnchantmentListItem>();
                    component2.skill.SetActive(value: false);
                    component2.enchantment.SetActive(value: true);
                    component2.enchantmentImage.texture = descriptionInfo2.GetTexture();
                    component2.label.text = descriptionInfo2.GetLocalizedName();
                    RolloverSimpleTooltip orAddComponent2 = component2.enchantmentTooltip.GetOrAddComponent<RolloverSimpleTooltip>();
                    orAddComponent2.sourceAsDbName = ench.source.Get().dbName;
                    orAddComponent2.image = null;
                    orAddComponent2.description = null;
                    orAddComponent2.title = null;
                    bool flag = ench.owner == GameManager.GetHumanWizard() && ench.source.Get().enchCategory == EEnchantmentCategory.Positive && ench.source.Get().allowDispel;
                    if (ench.source.Get().scripts != null)
                    {
                        flag = flag && Array.Find(ench.source.Get().scripts, (EnchantmentScript o) => o.triggerType == EEnchantmentType.RemoteUnitAttributeChange) == null && Array.Find(ench.source.Get().scripts, (EnchantmentScript o) => o.triggerType == EEnchantmentType.RemoteUnitAttributeChangeMP) == null;
                    }
                    component2.btCancelEnchantment.gameObject.SetActive(flag);
                    component2.btCancelEnchantment.onClick.RemoveAllListeners();
                    component2.btCancelEnchantment.onClick.AddListener(delegate
                    {
                        if (ench.manager == null)
                        {
                            Debug.LogError("missing manager");
                        }
                        else
                        {
                            ench.manager.owner.RemoveEnchantment(ench);
                            this.UpdateScreen();
                        }
                    });
                }
            }
        }, UpdateScreen);
        if (this.multipleUnitsOffer)
        {
            this.gridOfferedCharacters.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
            {
                global::MOM.Unit unit = source as global::MOM.Unit;
                itemSource.GetComponent<ToggleAndIconListItem>().riIcon.texture = unit.GetDescriptionInfo().GetTexture();
            });
            this.gridOfferedCharacters.onSelectionChange = delegate
            {
                global::MOM.Unit selectedObject = this.gridOfferedCharacters.GetSelectedObject<global::MOM.Unit>();
                if (selectedObject != null)
                {
                    this.UpdateBuyInfo(selectedObject);
                }
            };
            List<global::MOM.Unit> i = new List<global::MOM.Unit>();
            this.buySetInfo.ForEach(delegate(UnitOffer o)
            {
                i.Add(o.unit);
            });
            this.gridOfferedCharacters.UpdateGrid(i);
            this.gridOfferedCharacters.SelectItem(0);
        }
        MHEventSystem.RegisterListener<HeroEquip>(HeroEquipClose, this);
        AudioLibrary.RequestSFX("OpenUnitInfo");
        this.UpdateScreen();
    }

    public void SetBuyInfo(UnitOffer uo, SimpleCallback callback)
    {
        this.SetData(null, uo.unit);
        this.buyInfo = new UnitOffer(uo);
        this.callback = callback;
        this.buttonClose.gameObject.SetActive(value: false);
        this.buttonPrevUnit.gameObject.SetActive(value: false);
        this.buttonNextUnit.gameObject.SetActive(value: false);
        this.buttonJoin.gameObject.SetActive(value: false);
        this.buttonHire.gameObject.SetActive(value: true);
        this.buttonReject.gameObject.SetActive(value: true);
        this.buttonEquip.gameObject.SetActive(value: false);
        this.heroRecruit.SetActive(value: true);
        if (uo.unit.dbSource.Get() is Hero)
        {
            this.labelRecruit.text = global::DBUtils.Localization.Get("DES_RECRUIT_HERO", true, uo.cost);
        }
        else if (uo.quantity == 1)
        {
            this.labelRecruit.text = global::DBUtils.Localization.Get("DES_RECRUIT_UNIT", true, uo.cost, uo.quantity);
        }
        else
        {
            this.labelRecruit.text = global::DBUtils.Localization.Get("DES_RECRUIT_UNITS", true, uo.cost, uo.quantity);
        }
        this.resources.SetActive(value: true);
        this.UpdateResources();
    }

    public void SetBuyInfo(List<UnitOffer> buySet, Callback hireCallback, bool spellSummon = false)
    {
        this.multipleUnitsOffer = true;
        this.buySetInfo = buySet;
        this.hireCallback = hireCallback;
        this.buttonClose.gameObject.SetActive(value: false);
        this.buttonPrevUnit.gameObject.SetActive(value: false);
        this.buttonNextUnit.gameObject.SetActive(value: false);
        this.buttonJoin.gameObject.SetActive(value: false);
        this.buttonHire.gameObject.SetActive(value: true);
        this.buttonReject.gameObject.SetActive(!spellSummon);
        this.buttonEquip.gameObject.SetActive(value: false);
        this.heroRecruit.SetActive(!spellSummon);
        this.gridOfferedCharacters.gameObject.SetActive(value: true);
        this.resources.SetActive(value: true);
        this.UpdateResources();
    }

    private void UpdateBuyInfo(BaseUnit hero)
    {
        if (hero != null)
        {
            this.unit = hero;
            int index = this.buySetInfo.FindIndex((UnitOffer o) => o.unit == hero);
            this.buyInfo = new UnitOffer
            {
                baseUnit = this.buySetInfo[index].baseUnit,
                unit = this.buySetInfo[index].unit,
                quantity = this.buySetInfo[index].quantity,
                exp = this.buySetInfo[index].exp,
                cost = this.buySetInfo[index].cost
            };
            if ((this.unit as BaseUnit)?.dbSource.Get() is Hero)
            {
                this.labelRecruit.text = global::DBUtils.Localization.Get("DES_RECRUIT_HERO", true, this.buyInfo.cost);
            }
            else if (this.buyInfo.quantity == 1)
            {
                this.labelRecruit.text = global::DBUtils.Localization.Get("DES_RECRUIT_UNIT", true, this.buyInfo.cost, this.buyInfo.baseUnit);
            }
            else
            {
                this.labelRecruit.text = global::DBUtils.Localization.Get("DES_RECRUIT_UNITS", true, this.buyInfo.cost, this.buyInfo.baseUnit);
            }
            this.SetData(null, hero);
            this.UpdateScreen();
        }
    }

    public void SetHeroJoin(global::MOM.Unit hero, SimpleCallback callback)
    {
        this.heroJoinMode = true;
        this.leftPanel.SetActive(!this.heroJoinMode);
        this.jail.SetActive(this.heroJoinMode);
        this.heroJoin.SetActive(this.heroJoinMode);
        this.buttonJoin.gameObject.SetActive(this.heroJoinMode);
        this.buttonReject.gameObject.SetActive(this.heroJoinMode);
        this.buttonClose.gameObject.SetActive(!this.heroJoinMode);
        this.buttonPrevUnit.gameObject.SetActive(!this.heroJoinMode);
        this.buttonNextUnit.gameObject.SetActive(!this.heroJoinMode);
        this.buttonHire.gameObject.SetActive(value: false);
        this.resources.SetActive(this.heroJoinMode);
        this.SetData(null, hero);
        this.callback = callback;
        this.UpdateResources();
    }

    public void SetData(IEnumerable sources, object unit)
    {
        this.unit = unit;
        this.sources = sources?.Cast<object>().ToList();
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.buttonClose)
        {
            UIManager.Close(this);
        }
        else if (s == this.buttonHire)
        {
            if (this.callback != null)
            {
                this.callback();
            }
            else if (this.hireCallback != null)
            {
                this.hireCallback(this.buyInfo);
            }
            UIManager.Close(this);
        }
        else if (s == this.buttonJoin)
        {
            this.callback();
            UIManager.Close(this);
        }
        else if (s == this.buttonReject)
        {
            if (this.heroJoinMode)
            {
                (this.unit as global::MOM.Unit).Destroy();
                if (this._group != null)
                {
                    this._group.UpdateMapFormation();
                }
            }
            UIManager.Close(this);
        }
        else if (s == this.buttonPrevUnit)
        {
            object obj = this.PreviousItem();
            if (obj != null)
            {
                this.unit = obj;
                this.UpdateScreen();
            }
        }
        else if (s == this.buttonNextUnit)
        {
            object obj2 = this.NextItem();
            if (obj2 != null)
            {
                this.unit = obj2;
                this.UpdateScreen();
            }
        }
        else if (s == this.buttonEquip)
        {
            UIManager.Open<HeroEquip>(UIManager.Layer.Popup, this);
        }
        else if (s == this.buttonDismiss)
        {
            PopupGeneral.OpenPopup(this, "UI_WARNING", "UI_CONFIRM_DISBAND", "UI_DISBAND", delegate
            {
                this.unit = this.RemoveItem();
                if (this.unit == null)
                {
                    UIManager.Close(this);
                }
                else
                {
                    this.UpdateScreen();
                }
                HUD.Get().UpdateHUD();
                TownScreen townScreen = TownScreen.Get();
                if ((bool)townScreen)
                {
                    townScreen.UpdateRightPanel();
                }
            }, "UI_CANCEL");
        }
        else if (s == this.tgFilterAll || s == this.tgFilterSkills || s == this.tgFilterEnchantments)
        {
            this.UpdateEnchantmentList();
        }
    }

    private object RemoveItem()
    {
        if (this.sources != null)
        {
            int num = this.sources.FindIndex((object o) => o == this.unit);
            if (num >= 0)
            {
                global::MOM.Unit unit = this.unit as global::MOM.Unit;
                this.sources.RemoveAt(num);
                unit.Destroy(dismissed: true);
                this._group.UpdateMapFormation();
                if (FSMSelectionManager.Get().GetSelectedGroup() == this._group)
                {
                    List<global::MOM.Unit> selectedUnits = FSMSelectionManager.Get().selectedUnits;
                    if (selectedUnits.Contains(unit))
                    {
                        selectedUnits.Remove(unit);
                    }
                    HUD.Get().UpdateSelectedUnit();
                }
                if (num > this.sources.Count)
                {
                    num = 0;
                }
                if (num < this.sources.Count)
                {
                    this.unit = this.sources[num];
                }
                else
                {
                    this.unit = null;
                }
            }
        }
        return this.unit;
    }

    private object PreviousItem()
    {
        if (this.sources != null)
        {
            object obj = null;
            bool flag = false;
            foreach (object source in this.sources)
            {
                if (source != this.unit)
                {
                    obj = source;
                }
                else
                {
                    if (obj != null)
                    {
                        break;
                    }
                    flag = true;
                }
                if (flag)
                {
                    obj = source;
                }
            }
            if (obj != null)
            {
                return obj;
            }
        }
        return null;
    }

    private object NextItem()
    {
        if (this.sources != null)
        {
            object obj = null;
            bool flag = false;
            foreach (object source in this.sources)
            {
                if (obj == null && source != this.unit)
                {
                    obj = source;
                }
                if (flag && source != this.unit)
                {
                    obj = source;
                    break;
                }
                if (source == this.unit)
                {
                    flag = true;
                }
            }
            if (obj != null)
            {
                return obj;
            }
        }
        return null;
    }

    private void UpdateScreen()
    {
        int num = this.Get(TAG.HIT_POINTS);
        int num2 = this.Get2(TAG.HIT_POINTS);
        if (num < 1)
        {
            num = 1;
        }
        if (num2 < 1)
        {
            num2 = 1;
        }
        this.labelUnitName.text = this.GetName();
        int num3 = this.Get(TAG.MELEE_ATTACK);
        int num4 = this.Get2(TAG.MELEE_ATTACK);
        this.labelMelee.text = Mathf.Max(0, num3).ToString();
        this.DisplayIcons(this.itemMeleeIcons, num4, Mathf.Max(num4, num3), num3);
        if (this.Get(TAG.NORMAL_RANGE) > 0 || this.Get(TAG.MAGIC_RANGE) > 0 || this.Get(TAG.BOULDER_RANGE) > 0)
        {
            num3 = this.Get(TAG.RANGE_ATTACK);
            num4 = this.Get2(TAG.RANGE_ATTACK);
            this.labelRange.text = Mathf.Max(0, num3).ToString();
            this.DisplayIcons(this.itemRangeIcons, num4, Mathf.Max(num4, num3), num3);
            FInt fInt = this.GetFInt(TAG.RANGE_ATTACK_CHANCE);
            fInt = FInt.Max(0f, fInt);
            this.labelRangeChanceToHit.text = (fInt * 100).ToInt() + "%";
        }
        else
        {
            int num5 = 0;
            this.labelRange.text = 0.ToString();
            this.DisplayIcons(this.itemRangeIcons, num5, num5, num5);
            this.labelRangeChanceToHit.text = "0%";
        }
        num3 = this.Get(TAG.DEFENCE);
        num4 = this.Get2(TAG.DEFENCE);
        this.labelArmour.text = Mathf.Max(0, num3).ToString();
        this.DisplayIcons(this.itemArmourIcons, num4, Mathf.Max(num4, num3), num3);
        num3 = this.Get(TAG.RESIST);
        num4 = this.Get2(TAG.RESIST);
        this.labelResist.text = Mathf.Max(0, num3).ToString();
        this.DisplayIcons(this.itemResistIcons, num4, Mathf.Max(num4, num3), num3);
        num3 = this.GetFigures();
        int maxFigures = this.GetMaxFigures();
        this.labelFigures.text = Mathf.Max(0, num3).ToString();
        this.DisplayIcons(this.itemFiguresIcons, maxFigures, Mathf.Max(maxFigures, num3), num3);
        num3 = this.GetHP();
        this.labelHits.text = Mathf.Max(0, num3) + "/" + Mathf.Max(0, num);
        this.DisplayIcons(this.itemHitsIcons, num2, Mathf.Max(num2, num), num3);
        float num6 = this.TotalHP();
        this.labelHP.text = Mathf.RoundToInt(num6 * 100f) + "%";
        this.sliderHealth.value = Mathf.RoundToInt(num6 * 100f);
        num3 = this.Get(TAG.MOVEMENT_POINTS);
        if (this.unit is BattleUnit && (this.unit as BattleUnit).haste)
        {
            num3 *= 2;
        }
        this.labelMP.text = num3.ToString();
        if (this.unit is BattleUnit battleUnit)
        {
            int maxMana = battleUnit.maxMana;
            if (maxMana <= 0)
            {
                this.labelMana.text = "-/-";
            }
            else
            {
                this.labelMana.text = battleUnit.mana + "/" + maxMana;
            }
            maxMana = this.Get(TAG.AMMUNITION);
            if (maxMana <= 0)
            {
                this.labelAmmo.text = "-/-";
            }
            else
            {
                this.labelAmmo.text = battleUnit.GetCurentFigure().rangedAmmo + "/" + maxMana;
            }
        }
        else
        {
            num3 = this.Get(TAG.MANA_POINTS);
            if (num3 <= 0)
            {
                this.labelMana.text = "-";
            }
            else
            {
                this.labelMana.text = num3.ToString();
            }
            num3 = this.Get(TAG.AMMUNITION);
            if (num3 <= 0)
            {
                this.labelAmmo.text = "-";
            }
            else
            {
                this.labelAmmo.text = num3.ToString();
            }
        }
        FInt fInt2 = this.GetFInt(TAG.MELEE_ATTACK_CHANCE);
        fInt2 = FInt.Max(0f, fInt2);
        this.labelMeleeChanceToHit.text = (fInt2 * 100).ToInt() + "%";
        fInt2 = this.GetFInt(TAG.DEFENCE_CHANCE);
        fInt2 = FInt.Max(0f, fInt2);
        this.labelChanceToDefend.text = (fInt2 * 100).ToInt() + "%";
        fInt2 = this.GetFInt(TAG.RESIST);
        fInt2 = FInt.Max(0f, fInt2);
        this.labelChanceToResist.text = (fInt2 * 10).ToInt() + "%";
        BaseUnit baseUnit = this.unit as BaseUnit;
        if (baseUnit != null)
        {
            this.buttonEquip.gameObject.SetActive(baseUnit.dbSource.Get() is Hero && !(this.unit is BattleUnit));
        }
        this.experience.Set(baseUnit, this.labelExp);
        ISpellCaster spellCaster = this.unit as ISpellCaster;
        this.buttonSpellbook.gameObject.SetActive(spellCaster != null && spellCaster.GetSpellManager().GetSpells().Count > 0);
        int num7 = this.Get(TAG.CAN_WALK);
        int num8 = this.Get(TAG.CAN_SWIM);
        int num9 = Mathf.Max(this.Get(TAG.CAN_FLY), this.Get(TAG.WIND_WALKING));
        this.movementWalking.SetActive(num7 > 0);
        this.movementSwimming.SetActive(num8 > 0);
        this.movementFlying.SetActive(num9 > 0);
        num3 = this.Get(TAG.UPKEEP_GOLD);
        this.labelUpkeepMoney.text = num3.ToString();
        this.itemUpkeepMoney.SetActive(num3 > 0);
        num3 = this.Get(TAG.UPKEEP_FOOD);
        this.labelUpkeepFood.text = num3.ToString();
        this.itemUpkeepFood.SetActive(num3 > 0);
        FInt manaUpkeep = (this.unit as BaseUnit).GetManaUpkeep();
        this.labelUpkeepMana.text = manaUpkeep.ToString();
        this.itemUpkeepMana.SetActive(manaUpkeep > 0);
        this.enchantedMelee.SetActive(value: false);
        this.enchantedRanged.SetActive(value: false);
        this.chaosRanged.SetActive(value: false);
        this.natureRanged.SetActive(value: false);
        this.lifeRanged.SetActive(value: false);
        this.deathRanged.SetActive(value: false);
        this.techRanged.SetActive(value: false);
        this.boulderRanged.SetActive(value: false);
        this.sorceryRanged.SetActive(value: false);
        if (this.Get(TAG.ENCHANTED_WEAPON) > 0)
        {
            this.normalMelee.SetActive(value: false);
            this.enchantedMelee.SetActive(value: true);
            this.missileRanged.SetActive(value: false);
            this.enchantedRanged.SetActive(value: true);
        }
        this.SetAttackIco();
        this.UpdateEnchantmentList();
        this.largeGraphic.texture = this.GetImage();
        this.jailHeroPortrait.texture = this.GetImage();
        this.largeGraphicMultiply.texture = this.GetImage();
        this.race.texture = this.GetRace().GetDescriptionInfo().GetTexture();
        RolloverSimpleTooltip orAddComponent = this.race.gameObject.GetOrAddComponent<RolloverSimpleTooltip>();
        orAddComponent.sourceAsDbName = this.GetRace().dbName;
        orAddComponent.useMouseLocation = false;
        orAddComponent.anchor.x = 0.4f;
        orAddComponent.anchor.y = 1f;
        this.fantasticUnit.SetActive(this.Get(TAG.FANTASTIC_CLASS) > 0);
        this.construct.SetActive(this.Get(TAG.MECHANICAL_UNIT) > 0);
        this.story.SetActive(this.Get(TAG.FANTASTIC_CLASS) > 0 || this.Get(TAG.CHAMPION_CLASS) > 0 || this.Get(TAG.HERO_CLASS) > 0);
        RolloverSimpleTooltip orAddComponent2 = this.story.GetOrAddComponent<RolloverSimpleTooltip>();
        orAddComponent2.title = this.GetName();
        orAddComponent2.description = global::DBUtils.Localization.Get(this.GetDI().GetDescriptionKey(), true, this.GetName());
        orAddComponent2.useMouseLocation = false;
        orAddComponent2.optionalPosition = this.storyTooltipPosition;
    }

    private void OnSpellBook()
    {
        ISpellCaster spellCaster = this.unit as ISpellCaster;
        if (spellCaster.GetSpellManager().GetSpells().Count > 0)
        {
            UIManager.Open<CastSpells>(UIManager.Layer.Popup).SetSpellCaster(spellCaster);
        }
    }

    private void SetAttackIco()
    {
        int num = this.Get(TAG.NORMAL_RANGE);
        int num2 = this.Get(TAG.MAGIC_RANGE);
        int num3 = this.Get(TAG.BOULDER_RANGE);
        if (num == 0)
        {
            this.missileRanged.SetActive(value: false);
            this.enchantedRanged.SetActive(value: false);
        }
        if (num2 <= 0 && num3 <= 0)
        {
            return;
        }
        if (num2 > 0)
        {
            if (this.Get(TAG.MAGIC_SORCERY_RANGE) > 0)
            {
                this.missileRanged.SetActive(value: false);
                this.sorceryRanged.SetActive(value: true);
                return;
            }
            if (this.Get(TAG.MAGIC_LIFE_RANGE) > 0)
            {
                this.missileRanged.SetActive(value: false);
                this.lifeRanged.SetActive(value: true);
                return;
            }
            if (this.Get(TAG.MAGIC_NATURE_RANGE) > 0)
            {
                this.missileRanged.SetActive(value: false);
                this.natureRanged.SetActive(value: true);
                return;
            }
            if (this.Get(TAG.MAGIC_DEATH_RANGE) > 0)
            {
                this.missileRanged.SetActive(value: false);
                this.deathRanged.SetActive(value: true);
                return;
            }
            if (this.Get(TAG.MAGIC_CHAOS_RANGE) > 0)
            {
                this.missileRanged.SetActive(value: false);
                this.chaosRanged.SetActive(value: true);
                return;
            }
            Tag tag = DataBase.Get<Tag>("TAG-MAGIC_TECH_RANGE", reportMissing: false);
            if (tag != null && this.Get(tag) > 0)
            {
                this.missileRanged.SetActive(value: false);
                this.techRanged.SetActive(value: true);
            }
        }
        else if (num3 > 0)
        {
            this.missileRanged.SetActive(value: false);
            this.boulderRanged.SetActive(value: true);
        }
    }

    private void UpdateEnchantmentList()
    {
        List<object> list = new List<object>();
        if (this.tgFilterAll.isOn || this.tgFilterSkills.isOn)
        {
            foreach (Skill skill in this.GetSkills())
            {
                if (!skill.hideSkill)
                {
                    list.Add(skill);
                }
            }
        }
        if (this.tgFilterAll.isOn || this.tgFilterEnchantments.isOn)
        {
            EnchantmentManager enchantmentManager = (this.unit as IEnchantable)?.GetEnchantmentManager();
            if (enchantmentManager != null)
            {
                foreach (EnchantmentInstance item in enchantmentManager?.GetEnchantmentsWithRemotes())
                {
                    if (!item.source.Get().hideEnch)
                    {
                        list.Add(item);
                    }
                }
            }
        }
        this.gridSkillsEnchantments.UpdateGrid(list);
    }

    private Race GetRace()
    {
        if (this.unit is BaseUnit)
        {
            return (this.unit as BaseUnit).race.Get();
        }
        if (this.unit is global::DBDef.Unit)
        {
            return (this.unit as global::DBDef.Unit).race;
        }
        Debug.LogError("Not implemented");
        return null;
    }

    private int Get(Tag tag)
    {
        if (this.unit is BaseUnit)
        {
            return (this.unit as BaseUnit).GetAttributes().GetFinal(tag).ToInt();
        }
        if (this.unit is global::DBDef.Unit)
        {
            return (this.unit as global::DBDef.Unit).GetTag(tag).ToInt();
        }
        Debug.LogError("Not implemented");
        return 0;
    }

    private int Get(TAG tag)
    {
        if (this.unit is BaseUnit)
        {
            return (this.unit as BaseUnit).GetAttributes().GetFinal(tag).ToInt();
        }
        if (this.unit is global::DBDef.Unit)
        {
            return (this.unit as global::DBDef.Unit).GetTag(tag).ToInt();
        }
        Debug.LogError("Not implemented");
        return 0;
    }

    private int Get2(TAG tag)
    {
        return this.unitBaseRef.GetAttributes().GetFinal(tag).ToInt();
    }

    private FInt GetFInt(TAG tag)
    {
        if (this.unit is BaseUnit)
        {
            return (this.unit as BaseUnit).GetAttFinal(tag);
        }
        if (this.unit is global::DBDef.Unit)
        {
            return (this.unit as global::DBDef.Unit).GetTag(tag);
        }
        Debug.LogError("Not implemented");
        return FInt.ZERO;
    }

    private FInt GetFInt2(TAG tag)
    {
        return this.unitBaseRef.GetAttributes().GetFinal(tag);
    }

    private int GetFigures()
    {
        if (this.unit is global::MOM.Unit)
        {
            return (this.unit as global::MOM.Unit).figureCount;
        }
        if (this.unit is global::DBDef.Unit)
        {
            return (this.unit as global::DBDef.Unit).figures;
        }
        if (this.unit is BattleUnit)
        {
            return (this.unit as BattleUnit).FigureCount();
        }
        Debug.LogError("Not implemented");
        return 0;
    }

    private int GetFigures2()
    {
        return this.unitBaseRef.FigureCount();
    }

    private int GetMaxFigures()
    {
        if (this.unit is global::MOM.Unit)
        {
            return (this.unit as global::MOM.Unit).MaxCount();
        }
        if (this.unit is global::DBDef.Unit)
        {
            return (this.unit as global::DBDef.Unit).figures;
        }
        if (this.unit is BattleUnit)
        {
            if (!((this.unit as BattleUnit).dbSource.Get() is global::DBDef.Unit unit))
            {
                return 1;
            }
            return unit.figures;
        }
        Debug.LogError("Not implemented");
        return 0;
    }

    private int GetHP()
    {
        if (this.unit is BaseUnit)
        {
            return (this.unit as BaseUnit).currentFigureHP;
        }
        if (this.unit is global::DBDef.Unit)
        {
            return this.Get(TAG.HIT_POINTS);
        }
        Debug.LogError("Not implemented");
        return 0;
    }

    private float TotalHP()
    {
        if (this.unit is BaseUnit)
        {
            return (this.unit as BaseUnit).GetTotalHpPercent();
        }
        return 1f;
    }

    private List<Skill> GetSkills()
    {
        if (this.unit is ISkillable)
        {
            return (this.unit as ISkillable).GetSkillsConverted();
        }
        if (this.unit is global::DBDef.Unit)
        {
            return new List<Skill>((this.unit as global::DBDef.Unit).skills);
        }
        Debug.LogError("Not implemented");
        return new List<Skill>();
    }

    private List<Skill> GetSkills2()
    {
        return this.unitBaseRef.GetSkillsConverted();
    }

    private string GetName()
    {
        if (this.unit is BaseUnit baseUnit)
        {
            return baseUnit.GetName();
        }
        if (this.unit is global::DBDef.Unit)
        {
            return this.GetDI().GetLocalizedName();
        }
        Debug.LogError("!! unit type not implemented");
        return "N/A";
    }

    private void DisplayIcons(GameObject instance, int naturalMax, int curentMax, int curent)
    {
        int num = curent - naturalMax;
        int num2 = Mathf.Max(0, num);
        int num3 = naturalMax - Mathf.Max(0, -num);
        int childCount = instance.transform.parent.childCount;
        if (childCount < curentMax)
        {
            for (int i = childCount; i < curentMax; i++)
            {
                GameObjectUtils.Instantiate(instance, instance.transform.parent);
            }
        }
        int num4 = 0;
        int num5 = 0;
        string text = instance.transform.parent.name;
        try
        {
            childCount = instance.transform.parent.childCount;
            for (int j = 0; j < childCount; j++)
            {
                num4 = j;
                Transform child = instance.transform.parent.GetChild(j);
                child.gameObject.SetActive(j < curentMax);
                if (j < curentMax)
                {
                    bool active = j < num3;
                    GameObjectUtils.FindByName(child.gameObject, "Normal").SetActive(active);
                    active = j >= num3 && j < num3 + num2;
                    GameObjectUtils.FindByName(child.gameObject, "Added").SetActive(active);
                    active = j >= num3 + num2;
                    GameObjectUtils.FindByName(child.gameObject, "Missing").SetActive(active);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(text + " \n " + num4 + " of " + num5 + " \n " + ex);
        }
    }

    private Texture2D GetImage()
    {
        if (this.unit is global::MOM.Unit || this.unit is global::DBDef.Unit || this.unit is BattleUnit)
        {
            return this.GetDI().GetTextureLarge();
        }
        Debug.LogError("!! unit type not implemented");
        return null;
    }

    private DescriptionInfo GetDI()
    {
        if (this.unit is global::MOM.Unit)
        {
            return (this.unit as global::MOM.Unit).dbSource.Get().GetDescriptionInfo();
        }
        if (this.unit is global::DBDef.Unit)
        {
            return (this.unit as global::DBDef.Unit).GetDescriptionInfo();
        }
        if (this.unit is BattleUnit)
        {
            return (this.unit as BattleUnit).GetDescriptionInfo();
        }
        Debug.LogError("!! unit type not implemented");
        return null;
    }

    private void HeroEquipClose(object sender, object e)
    {
        this.UpdateScreen();
    }

    private void UpdateResources()
    {
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        this.resGold.text = humanWizard.money + "(" + humanWizard.CalculateMoneyIncome(includeUpkeep: true).SInt() + ")";
        this.resMana.text = humanWizard.GetMana() + "(" + humanWizard.CalculateManaIncome(includeUpkeep: true).SInt() + ")";
        this.resFood.text = humanWizard.CalculateFoodIncome(includeUpkeep: true).SInt();
        this.resFame.text = humanWizard.GetFame().ToString();
    }
}
