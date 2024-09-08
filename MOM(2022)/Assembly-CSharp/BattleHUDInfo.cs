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

public class BattleHUDInfo : MonoBehaviour
{
    public TextMeshProUGUI labelUnitName;

    public TextMeshProUGUI labelMelee;

    public TextMeshProUGUI labelMeleeChanceToHit;

    public TextMeshProUGUI labelRanged;

    public TextMeshProUGUI labelRangedChanceToHit;

    public TextMeshProUGUI labelAmmo;

    public TextMeshProUGUI labelDefence;

    public TextMeshProUGUI labelResistance;

    public TextMeshProUGUI labelFigures;

    public TextMeshProUGUI labelHP;

    public TextMeshProUGUI labelActions;

    public TextMeshProUGUI labelMana;

    public TextMeshProUGUI attackerName;

    public GridItemManager armyGrid;

    public GridItemManager enchantmentsGrid;

    public GridItemManager unitSkillsGrid;

    public RawImage unitImage;

    public GameObject goFlagPurple;

    public GameObject goFlagRed;

    public GameObject goFlagBlue;

    public GameObject goFlagGreen;

    public GameObject goFlagYellow;

    public GameObject goFlagBrown;

    public GameObject goUnit;

    public GameObject goUnitMovementGround;

    public GameObject goUnitMovementSwimming;

    public GameObject goUnitMovementFlying;

    public Slider sliderHP;

    public Button buttonNextUnit;

    public Button buttonWait;

    private bool attackerSide;

    private BattleUnit unitDirty;

    private BattleUnit selectedBattleUnit;

    private void Awake()
    {
        this.armyGrid.CustomDynamicItem(UnitGridItem, BaseUpdate);
        this.enchantmentsGrid.CustomDynamicItem(EnchantmentItem);
        this.unitSkillsGrid.CustomDynamicItem(SkillItem);
        MHEventSystem.RegisterListener<BattleUnit>(DeathAnimationUI, this);
        MHEventSystem.RegisterListener("BattleHUDInfoChange", UnitChanged, this);
    }

    private void UnitChanged(object sender, object e)
    {
        if (e is BattleUnit battleUnit && battleUnit == this.selectedBattleUnit)
        {
            this.unitDirty = battleUnit;
        }
    }

    private void OnDestroy()
    {
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
    }

    public void SetFromBattlePlayer(BattlePlayer bp)
    {
        PlayerWizard wizard = bp.wizard;
        if (wizard == null)
        {
            this.attackerName.text = global::DBUtils.Localization.Get("UI_NEUTRAL_UNITS", true);
        }
        else
        {
            this.attackerName.text = wizard.name;
        }
        Battle battle = Battle.GetBattle();
        this.attackerSide = battle.attacker == bp;
        PlayerWizard.Color color = wizard?.color ?? PlayerWizard.Color.None;
        this.goFlagBrown.SetActive(color == PlayerWizard.Color.None);
        this.goFlagBlue.SetActive(color == PlayerWizard.Color.Blue);
        this.goFlagGreen.SetActive(color == PlayerWizard.Color.Green);
        this.goFlagPurple.SetActive(color == PlayerWizard.Color.Purple);
        this.goFlagRed.SetActive(color == PlayerWizard.Color.Red);
        this.goFlagYellow.SetActive(color == PlayerWizard.Color.Yellow);
    }

    public void BaseUpdate()
    {
        Battle battle = Battle.GetBattle();
        this.armyGrid.UpdateGrid(this.attackerSide ? battle.attackerUnits : battle.defenderUnits);
        if (this.attackerSide)
        {
            List<EnchantmentInstance> items = battle.attacker.GetEnchantments().FindAll((EnchantmentInstance o) => !o.source.Get().nonCombatDisplay);
            this.enchantmentsGrid.UpdateGrid(items);
        }
        else
        {
            List<EnchantmentInstance> items2 = battle.defender.GetEnchantments().FindAll((EnchantmentInstance o) => !o.source.Get().nonCombatDisplay);
            this.enchantmentsGrid.UpdateGrid(items2);
        }
    }

    private void UnitGridItem(GameObject itemSource, object source, object data, int index)
    {
        ArmyListItem component = itemSource.GetComponent<ArmyListItem>();
        BattleUnit battleUnit = source as BattleUnit;
        component.GetComponent<Animator>().SetBool("UnitDead", !battleUnit.IsAlive());
        component.Unit = battleUnit;
    }

    private void EnchantmentItem(GameObject itemSource, object source, object data, int index)
    {
        EnchantmentInstance enchantmentInstance = source as EnchantmentInstance;
        RawImage rawImage = GameObjectUtils.FindByNameGetComponentInChildren<RawImage>(itemSource, "E1Image");
        Image image = GameObjectUtils.FindByNameGetComponentInChildren<Image>(itemSource, "Frame");
        rawImage.texture = enchantmentInstance.source.Get().GetDescriptionInfo().GetTexture();
        Color color = WizardColors.GetColor((enchantmentInstance.owner?.GetEntity() is PlayerWizard playerWizard) ? playerWizard.color : PlayerWizard.Color.None);
        image.color = color;
        itemSource.GetOrAddComponent<RolloverSimpleTooltip>().sourceAsDbName = enchantmentInstance.source.Get().dbName;
    }

    private void SkillItem(GameObject itemSource, object source, object data, int index)
    {
        DBReference<Skill> dBReference = source as DBReference<Skill>;
        EnchantmentInstance enchantmentInstance = source as EnchantmentInstance;
        SimpleListItem component = itemSource.GetComponent<SimpleListItem>();
        if (dBReference != null && !dBReference.Get().nonCombatDisplay && !dBReference.Get().hideSkill)
        {
            this.unitSkillsGrid.gameObject.SetActive(value: true);
            component.icon.texture = dBReference.Get().GetDescriptionInfo().GetTexture();
            RolloverSimpleTooltip orAddComponent = itemSource.GetOrAddComponent<RolloverSimpleTooltip>();
            orAddComponent.sourceAsDbName = dBReference.dbName;
            orAddComponent.title = dBReference.Get().GetDescriptionInfo().GetLocalizedName();
            if (!string.IsNullOrEmpty(dBReference.Get().descriptionScript))
            {
                orAddComponent.title = (string)ScriptLibrary.Call(dBReference.Get().descriptionScript, this.selectedBattleUnit, dBReference.Get(), null);
            }
            orAddComponent.useMouseLocation = false;
            orAddComponent.anchor.x = 0.4f;
            orAddComponent.offset.y = 50f;
        }
        else if (enchantmentInstance != null && !enchantmentInstance.source.Get().hideEnch && !enchantmentInstance.source.Get().nonCombatDisplay)
        {
            this.unitSkillsGrid.gameObject.SetActive(value: true);
            component.icon.texture = enchantmentInstance.source.Get().GetDescriptionInfo().GetTexture();
            RolloverSimpleTooltip orAddComponent2 = itemSource.GetOrAddComponent<RolloverSimpleTooltip>();
            orAddComponent2.title = enchantmentInstance.source.Get().GetDescriptionInfo().GetLocalizedName();
            orAddComponent2.sourceAsDbName = enchantmentInstance.source.Get().dbName;
            orAddComponent2.useMouseLocation = false;
            orAddComponent2.anchor.x = 0.4f;
            orAddComponent2.offset.y = 50f;
        }
        else
        {
            itemSource.gameObject.SetActive(value: false);
        }
    }

    public void Reselect()
    {
        if (this.selectedBattleUnit != null)
        {
            this.UpdateUnitInfoDisplay(this.selectedBattleUnit, this.buttonNextUnit.gameObject.activeSelf);
        }
    }

    public void UpdateUnitInfoDisplay(BattleUnit bu, bool showButtons)
    {
        this.goUnit.SetActive(bu != null);
        this.selectedBattleUnit = bu;
        if (bu != null)
        {
            if (bu.ownerID != PlayerWizard.HumanID())
            {
                showButtons = false;
            }
            this.buttonNextUnit.gameObject.SetActive(showButtons);
            this.buttonWait.gameObject.SetActive(showButtons);
            DescriptionInfo descriptionInfo = bu.dbSource.Get().GetDescriptionInfo();
            this.unitImage.texture = descriptionInfo.GetTextureLarge();
            this.labelUnitName.text = bu.GetName();
            this.labelMelee.text = bu.GetCurentFigure().attack.ToString();
            this.labelMeleeChanceToHit.text = bu.GetCurentFigure().attackChance * 100f + "%";
            this.labelMana.text = bu.mana.ToString();
            this.labelDefence.text = bu.GetCurentFigure().defence.ToString();
            this.labelResistance.text = bu.GetCurentFigure().resist.ToString();
            this.labelFigures.text = bu.FigureCount().ToString();
            this.labelActions.text = bu.Mp.ToString(1);
            if (bu.IsRangedUnit())
            {
                this.labelRanged.text = bu.GetCurentFigure().rangedAttack.ToString();
                this.labelRangedChanceToHit.text = bu.GetCurentFigure().rangedAttackChance * 100f + "%";
                this.labelAmmo.text = bu.GetCurentFigure().rangedAmmo.ToString();
            }
            else
            {
                this.labelRanged.text = "0";
                this.labelRangedChanceToHit.text = "0%";
                this.labelAmmo.text = "0";
            }
            this.labelHP.text = bu.currentFigureHP + "/" + bu.GetCurentFigure().maxHitPoints;
            float totalHpPercent = bu.GetTotalHpPercent();
            this.sliderHP.value = (this.sliderHP.maxValue - this.sliderHP.minValue) * totalHpPercent + this.sliderHP.minValue;
            this.sliderHP.gameObject.SetActive(totalHpPercent != 1f || totalHpPercent <= 0f);
            int num = bu.GetAttFinal(TAG.CAN_WALK).ToInt();
            int num2 = bu.GetAttFinal(TAG.CAN_SWIM).ToInt();
            int num3 = bu.GetAttFinal(TAG.CAN_FLY).ToInt();
            this.goUnitMovementGround.SetActive(num > 0);
            this.goUnitMovementSwimming.SetActive(num2 > 0);
            this.goUnitMovementFlying.SetActive(num3 > 0);
            this.UpdateUnitSkillsGrid(bu);
        }
    }

    public void HighlightSelectedUnit(BattleUnit bu)
    {
        Battle battle = Battle.GetBattle();
        List<BattleUnit> list = (this.attackerSide ? battle.attackerUnits : battle.defenderUnits);
        if (list.Contains(bu) && list.IndexOf(bu) + 1 > this.armyGrid.GetPageSize() + this.armyGrid.GetPageIndexOffset())
        {
            this.armyGrid.NextPage();
        }
        else if (list.Contains(bu) && list.IndexOf(bu) + 1 <= this.armyGrid.GetPageIndexOffset())
        {
            this.armyGrid.PrevPage();
        }
        GameObject[] itemInstances = this.armyGrid.itemInstances;
        foreach (GameObject obj in itemInstances)
        {
            ArmyListItem component = obj.GetComponent<ArmyListItem>();
            Toggle component2 = obj.GetComponent<Toggle>();
            if (component != null && component2 != null)
            {
                component2.isOn = component.Unit == bu;
            }
        }
    }

    public void DisableAllHighlights()
    {
        if (this.armyGrid.GetPageNr() > 0)
        {
            this.armyGrid.ResetPage();
        }
        GameObject[] itemInstances = this.armyGrid.itemInstances;
        foreach (GameObject obj in itemInstances)
        {
            ArmyListItem component = obj.GetComponent<ArmyListItem>();
            Toggle component2 = obj.GetComponent<Toggle>();
            if (component != null && component2 != null)
            {
                component2.isOn = false;
            }
        }
    }

    public void SetUnitDirty(BattleUnit bu)
    {
        this.unitDirty = bu;
    }

    private void Update()
    {
        if (this.unitDirty == null)
        {
            return;
        }
        if (this.unitDirty != this.selectedBattleUnit)
        {
            this.unitDirty = null;
            return;
        }
        DescriptionInfo descriptionInfo = this.unitDirty.dbSource.Get().GetDescriptionInfo();
        this.unitImage.texture = descriptionInfo.GetTextureLarge();
        this.labelUnitName.text = this.unitDirty.GetName();
        this.labelMelee.text = this.unitDirty.GetCurentFigure().attack.ToString();
        this.labelMeleeChanceToHit.text = this.unitDirty.GetCurentFigure().attackChance * 100f + "%";
        this.labelMana.text = this.unitDirty.mana.ToString();
        this.labelDefence.text = this.unitDirty.GetCurentFigure().defence.ToString();
        this.labelResistance.text = this.unitDirty.GetCurentFigure().resist.ToString();
        this.labelFigures.text = this.unitDirty.FigureCount().ToString();
        this.labelActions.text = this.unitDirty.Mp.ToString(1);
        if (this.unitDirty.IsRangedUnit())
        {
            this.labelRanged.text = this.unitDirty.GetCurentFigure().rangedAttack.ToString();
            this.labelRangedChanceToHit.text = this.unitDirty.GetCurentFigure().rangedAttackChance * 100f + "%";
            this.labelAmmo.text = this.unitDirty.GetCurentFigure().rangedAmmo.ToString();
        }
        else
        {
            this.labelRanged.text = "0";
            this.labelRangedChanceToHit.text = "0%";
            this.labelAmmo.text = "0";
        }
        this.labelHP.text = this.unitDirty.currentFigureHP + "/" + this.unitDirty.GetCurentFigure().maxHitPoints;
        float totalHpPercent = this.unitDirty.GetTotalHpPercent();
        this.sliderHP.value = (this.sliderHP.maxValue - this.sliderHP.minValue) * totalHpPercent + this.sliderHP.minValue;
        this.sliderHP.gameObject.SetActive(totalHpPercent != 1f || totalHpPercent <= 0f);
        int num = this.unitDirty.GetAttFinal(TAG.CAN_WALK).ToInt();
        int num2 = this.unitDirty.GetAttFinal(TAG.CAN_SWIM).ToInt();
        int num3 = this.unitDirty.GetAttFinal(TAG.CAN_FLY).ToInt();
        this.goUnitMovementGround.SetActive(num > 0);
        this.goUnitMovementSwimming.SetActive(num2 > 0);
        this.goUnitMovementFlying.SetActive(num3 > 0);
        this.UpdateUnitSkillsGrid(this.unitDirty);
        this.unitDirty = null;
    }

    private void DeathAnimationUI(object sender, object e)
    {
        BattleUnit battleUnit = sender as BattleUnit;
        if (battleUnit.attackingSide != this.attackerSide)
        {
            return;
        }
        foreach (KeyValuePair<object, GameObject> item in this.armyGrid.dataToGameObjectDictionary)
        {
            GameObject gameObject = null;
            if (item.Key is BattleUnit && item.Key == battleUnit)
            {
                gameObject = item.Value;
            }
            if (gameObject != null)
            {
                gameObject.GetComponent<Animator>().SetTrigger("Dying");
                break;
            }
        }
    }

    private void UpdateUnitSkillsGrid(BattleUnit bu)
    {
        List<object> list = new List<object>();
        list.AddRange(bu.GetSkills());
        list.AddRange(bu.GetEnchantmentManager().GetEnchantmentsWithRemotes());
        this.unitSkillsGrid.UpdateGrid(list);
    }
}
