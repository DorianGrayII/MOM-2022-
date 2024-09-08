// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PreBattle
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class PreBattle : ScreenBase
{
    public TextMeshProUGUI attackers;

    public TextMeshProUGUI defenders;

    public TextMeshProUGUI outcomeSummary;

    public Button btDontEngage;

    public Button btAutoCombat;

    public Button btManualCombat;

    public GridItemManager gridAttackers;

    public GridItemManager gridDefenders;

    public GameObject outcome1;

    public GameObject outcome2;

    public GameObject outcome3;

    public GameObject outcome4;

    public GameObject outcome5;

    public GameObject banishedCannotCast;

    public GameObject attackerFlagRed;

    public GameObject attackerFlagBlue;

    public GameObject attackerFlagGreen;

    public GameObject attackerFlagPurple;

    public GameObject attackerFlagYellow;

    public GameObject attackerFlagBrown;

    public GameObject defenderFlagRed;

    public GameObject defenderFlagBlue;

    public GameObject defenderFlagGreen;

    public GameObject defenderFlagPurple;

    public GameObject defenderFlagYellow;

    public GameObject defenderFlagBrown;

    public Toggle tgUseMana;

    public GameObjectEnabler<PlayerWizard.Familiar> familiar;

    private BattleResult battleResult;

    private BattleResult battleResultWithMana;

    private BattleResult battleResultNoMana;

    private bool autofightFinished;

    private Coroutine updateChances;

    public bool debugBattles;

    public override IEnumerator PreStart()
    {
        Battle battle = Battle.GetBattle();
        this.gridAttackers.CustomDynamicItem(Item);
        this.gridDefenders.CustomDynamicItem(Item);
        this.gridAttackers.UpdateGrid(battle.attackerUnits);
        this.gridDefenders.UpdateGrid(battle.defenderUnits);
        this.defenders.text = global::DBUtils.Localization.Get("UI_DEFENDER", true) + " - " + battle.defenderName;
        this.attackers.text = global::DBUtils.Localization.Get("UI_ATTACKER", true) + " - " + battle.attackerName;
        PlayerWizard.Color color = battle.attacker.GetWizardOwner()?.color ?? PlayerWizard.Color.None;
        this.attackerFlagBrown.SetActive(color == PlayerWizard.Color.None);
        this.attackerFlagBlue.SetActive(color == PlayerWizard.Color.Blue);
        this.attackerFlagGreen.SetActive(color == PlayerWizard.Color.Green);
        this.attackerFlagPurple.SetActive(color == PlayerWizard.Color.Purple);
        this.attackerFlagYellow.SetActive(color == PlayerWizard.Color.Yellow);
        this.attackerFlagRed.SetActive(color == PlayerWizard.Color.Red);
        color = battle.defender.GetWizardOwner()?.color ?? PlayerWizard.Color.None;
        this.defenderFlagRed.SetActive(color == PlayerWizard.Color.Red);
        this.defenderFlagBlue.SetActive(color == PlayerWizard.Color.Blue);
        this.defenderFlagGreen.SetActive(color == PlayerWizard.Color.Green);
        this.defenderFlagPurple.SetActive(color == PlayerWizard.Color.Purple);
        this.defenderFlagYellow.SetActive(color == PlayerWizard.Color.Yellow);
        this.defenderFlagBrown.SetActive(color == PlayerWizard.Color.None);
        this.familiar.Set(GameManager.GetHumanWizard().familiar);
        if (!battle.playerIsAttacker)
        {
            this.btDontEngage.name = "ButtonAutoCombat";
            TextMeshProUGUI componentInChildren = this.btDontEngage.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (componentInChildren != null)
            {
                componentInChildren.text = global::DBUtils.Localization.Get("UI_FLEE", true);
            }
        }
        this.btDontEngage.interactable = false;
        this.btAutoCombat.interactable = false;
        this.btManualCombat.interactable = false;
        this.LogGroups(Battle.GetBattle());
        this.updateChances = base.StartCoroutine(this.UpdateWinChances(Battle.GetBattle()));
        yield return base.PreStart();
    }

    public override IEnumerator PostStart()
    {
        yield return base.PostStart();
        this.banishedCannotCast.SetActive(value: false);
        if (GameManager.GetHumanWizard().GetWizardStatus(updateTracker: false) == PlayerWizard.WizardStatus.Banished)
        {
            GameManager.Get().useManaInAutoresolves = false;
            this.tgUseMana.gameObject.SetActive(value: false);
            this.banishedCannotCast.SetActive(value: true);
        }
        this.tgUseMana.interactable = this.updateChances == null;
        this.tgUseMana.isOn = GameManager.Get().useManaInAutoresolves;
        this.tgUseMana.onValueChanged.AddListener(delegate(bool b)
        {
            GameManager.Get().useManaInAutoresolves = b;
            this.updateChances = base.StartCoroutine(this.UpdateWinChances(Battle.GetBattle()));
        });
    }

    private void LogGroups(Battle b)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Power Comparison: " + b.GetStrategicValue(attacker: true) + " vs " + b.GetStrategicValue(attacker: false));
        if (b.attackerUnits != null)
        {
            stringBuilder.AppendLine("Attackers:");
            foreach (BattleUnit attackerUnit in b.attackerUnits)
            {
                stringBuilder.AppendLine(attackerUnit.GetDBName() + " HP " + (int)(attackerUnit.GetTotalHpPercent() * 100f) + "%, value: " + attackerUnit.GetBattleUnitValue());
            }
            stringBuilder.AppendLine("Defenders:");
            foreach (BattleUnit defenderUnit in b.defenderUnits)
            {
                stringBuilder.AppendLine(defenderUnit.GetDBName() + " HP " + (int)(defenderUnit.GetTotalHpPercent() * 100f) + "%, value: " + defenderUnit.GetBattleUnitValue());
            }
        }
        Debug.Log(stringBuilder);
    }

    public override void OnStart()
    {
        base.OnStart();
        Battle battle = Battle.GetBattle();
        global::MOM.Group group = ((battle.gAttacker != null) ? battle.gAttacker : battle.gDefender);
        if (group != null)
        {
            World.ActivatePlane(group.GetPlane());
            CameraController.CenterAt(group.GetPosition(), instant: true, -1f);
            if (battle.gDefender != null && battle.gAttacker != null)
            {
                Vector3 vector = HexCoordinates.HexToWorld3D(battle.gDefender.GetPosition());
                Vector3 vector2 = HexCoordinates.HexToWorld3D(battle.gAttacker.GetPosition());
                Formation mapFormation = battle.gDefender.GetMapFormation(createIfMissing: false);
                if (mapFormation != null && !mapFormation.IsAnimating())
                {
                    Vector3 direction = vector2 - vector;
                    if (direction.magnitude > 10f)
                    {
                        direction *= -1f;
                    }
                    direction.y = 0f;
                    direction.Normalize();
                    mapFormation.direction = direction;
                    mapFormation.InitializeModelPositions();
                }
                mapFormation = battle.gAttacker.GetMapFormation(createIfMissing: false);
                if (mapFormation != null && !mapFormation.IsAnimating())
                {
                    Vector3 direction2 = vector - vector2;
                    if (direction2.magnitude > 10f)
                    {
                        direction2 *= -1f;
                    }
                    direction2.y = 0f;
                    direction2.Normalize();
                    mapFormation.direction = direction2;
                    mapFormation.InitializeModelPositions();
                }
            }
        }
        AudioLibrary.RequestSFX("PreBattle");
    }

    protected override void ButtonClick(Selectable s)
    {
        if (s == this.btManualCombat)
        {
            Battle battle = Battle.GetBattle();
            PlayerWizard wizardOwner = battle.attacker.GetWizardOwner();
            PlayerWizard wizardOwner2 = battle.defender.GetWizardOwner();
            if (wizardOwner != null && wizardOwner2 != null)
            {
                wizardOwner.GetDiplomacy().Attacked(wizardOwner2, battle.gDefender);
            }
        }
        else if (s == this.btAutoCombat)
        {
            Battle battle2 = Battle.GetBattle();
            battle2.ApplyResultsToUnits(this.battleResult);
            battle2.ApplyManaUses(this.battleResult);
            PlayerWizard wizardOwner3 = battle2.attacker.GetWizardOwner();
            PlayerWizard wizardOwner4 = battle2.defender.GetWizardOwner();
            if (wizardOwner3 != null && wizardOwner4 != null)
            {
                wizardOwner3.GetDiplomacy().Attacked(wizardOwner4, battle2.gDefender);
            }
        }
        else if (s == this.btDontEngage)
        {
            Battle battle3 = Battle.GetBattle();
            battle3.attacker.surrendered = true;
            if (!battle3.playerIsAttacker)
            {
                battle3.ApplyFleeDamages(attacker: false);
            }
            else
            {
                global::MOM.Group playerGroup = battle3.gAttacker;
                global::MOM.Group group = GameManager.GetGroupsOfWizard(playerGroup.GetOwnerID()).Find((global::MOM.Group o) => o.SharePositionWith(playerGroup));
                if (group != null && group.GetOwnerID() == playerGroup.GetOwnerID() && group != playerGroup)
                {
                    playerGroup.TransferUnits(group);
                }
            }
            PlayerWizard wizardOwner5 = battle3.attacker.GetWizardOwner();
            PlayerWizard wizardOwner6 = battle3.defender.GetWizardOwner();
            if (wizardOwner5 != null && wizardOwner6 == GameManager.GetHumanWizard())
            {
                wizardOwner5.GetDiplomacy().Attacked(wizardOwner6, battle3.gDefender);
            }
        }
        base.ButtonClick(s);
    }

    private void Item(GameObject itemSource, object source, object data, int index)
    {
        CharacterListItem component = itemSource.GetComponent<CharacterListItem>();
        BattleUnit unit = source as BattleUnit;
        component.portrait.texture = AssetManager.GetTexture(unit.dbSource.Get());
        component.deadMarker.SetActive(value: false);
        component.hp.value = unit.GetTotalHpPercent();
        component.unitLevel.texture = null;
        if (component.race != null)
        {
            if (unit.GetAttFinal((Tag)TAG.REANIMATED) > 0)
            {
                component.race.gameObject.SetActive(value: true);
                component.race.texture = ((Race)RACE.REALM_DEATH).GetDescriptionInfo().GetTexture();
            }
            else if (unit.GetAttFinal((Tag)TAG.FANTASTIC_CLASS) > 0 || unit.GetAttFinal((Tag)TAG.HERO_CLASS) > 0)
            {
                component.race.gameObject.SetActive(value: true);
                component.race.texture = unit.race.Get().GetDescriptionInfo().GetTexture();
            }
            else
            {
                component.race.gameObject.SetActive(value: false);
            }
        }
        if (component.goodEnchantment != null && component.badEnchantment != null && component.goodBadEnchantment != null)
        {
            List<EnchantmentInstance> enchantmentsWithRemotes = unit.GetEnchantmentManager().GetEnchantmentsWithRemotes();
            bool flag = false;
            bool flag2 = false;
            foreach (EnchantmentInstance item in enchantmentsWithRemotes)
            {
                if (item.GetEnchantmentType() == EEnchantmentCategory.Positive)
                {
                    flag = true;
                }
                else if (item.GetEnchantmentType() == EEnchantmentCategory.Negative)
                {
                    flag2 = true;
                }
            }
            component.goodEnchantment.SetActive(value: false);
            component.badEnchantment.SetActive(value: false);
            component.goodBadEnchantment.SetActive(value: false);
            if (flag && flag2)
            {
                component.goodBadEnchantment.SetActive(value: true);
            }
            else if (flag)
            {
                component.goodEnchantment.SetActive(value: true);
            }
            else if (flag2)
            {
                component.badEnchantment.SetActive(value: true);
            }
        }
        Tag uType = ((unit.dbSource.Get() is Hero) ? ((Tag)TAG.HERO_CLASS) : ((Tag)TAG.NORMAL_CLASS));
        UnitLvl unitLvl = DataBase.GetType<UnitLvl>().Find((UnitLvl o) => o.unitClass == uType && o.level == unit.GetLevel());
        if (unitLvl != null)
        {
            component.unitLevel.gameObject.SetActive(value: true);
            component.unitLevel.texture = unitLvl.GetDescriptionInfo().GetTexture();
        }
        else
        {
            component.unitLevel.gameObject.SetActive(value: false);
        }
        component.hp.gameObject.SetActive(unit.GetTotalHpPercent() != 1f);
        itemSource.GetOrAddComponent<RolloverUnitTooltip>().sourceAsUnit = unit;
        itemSource.GetOrAddComponent<MouseClickEvent>().mouseRightClick = delegate
        {
            ScreenBase componentInParent = base.GetComponentInParent<ScreenBase>();
            UnitInfo unitInfo = UIManager.Open<UnitInfo>(UIManager.Layer.Popup, componentInParent);
            Battle battle = Battle.GetBattle();
            List<BattleUnit> sources = ((unit.ownerID == battle.attacker.GetID()) ? battle.attackerUnits : battle.defenderUnits);
            unitInfo.SetData(sources, unit);
        };
    }

    private IEnumerator UpdateWinChances(Battle b)
    {
        if (this.tgUseMana != null)
        {
            this.tgUseMana.interactable = false;
        }
        bool useMana = GameManager.Get().useManaInAutoresolves;
        Func<BattleResult, int, GameObject, object> display = delegate(BattleResult br, int index, GameObject go)
        {
            int num = 0;
            if (br != null)
            {
                num = (b.playerIsAttacker ? br.aWinPerRank[index] : br.dWinPerRank[index]);
            }
            Image image = GameObjectUtils.FindByNameGetComponentInChildren<Image>(go, "Fill", ignoreRoot: false);
            TextMeshProUGUI textMeshProUGUI = GameObjectUtils.FindByNameGetComponentInChildren<TextMeshProUGUI>(go, "Label", ignoreRoot: false);
            image.fillAmount = (float)num * 0.1f;
            textMeshProUGUI.text = num * 10 + " %";
            return (object)null;
        };
        display(null, 0, this.outcome1);
        display(null, 1, this.outcome2);
        display(null, 2, this.outcome3);
        display(null, 3, this.outcome4);
        display(null, 4, this.outcome5);
        if (!this.debugBattles && useMana && this.battleResultWithMana != null)
        {
            this.battleResult = this.battleResultWithMana;
        }
        else if (!this.debugBattles && !useMana && this.battleResultNoMana != null)
        {
            this.battleResult = this.battleResultNoMana;
        }
        else
        {
            int random = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            global::UnityEngine.Random.InitState(0);
            this.battleResult = new BattleResult();
            yield return PowerEstimate.SimulatedBattle(b, 10, this.battleResult, 0, !b.playerIsAttacker || useMana, b.playerIsAttacker || useMana);
            global::UnityEngine.Random.InitState(random);
            if (useMana)
            {
                this.battleResultWithMana = this.battleResult;
            }
            else
            {
                this.battleResultNoMana = this.battleResult;
            }
        }
        display(this.battleResult, 0, this.outcome1);
        display(this.battleResult, 1, this.outcome2);
        display(this.battleResult, 2, this.outcome3);
        display(this.battleResult, 3, this.outcome4);
        display(this.battleResult, 4, this.outcome5);
        this.autofightFinished = true;
        this.btDontEngage.interactable = b.playerIsAttacker || !(b.gDefender?.IsHosted() ?? false);
        this.btAutoCombat.interactable = true;
        this.btManualCombat.interactable = true;
        if (this.tgUseMana != null)
        {
            this.tgUseMana.interactable = true;
        }
        this.updateChances = null;
    }
}
