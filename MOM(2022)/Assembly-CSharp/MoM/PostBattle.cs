// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PostBattle
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using MOM.Adventures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class PostBattle : ScreenBase
{
    public TextMeshProUGUI attackers;

    public TextMeshProUGUI defenders;

    public TextMeshProUGUI fameLabel;

    public Button btOkay;

    public GridItemManager gridAttackers;

    public GridItemManager gridDefenders;

    public GameObject defeat;

    public GameObject victory;

    public GameObject draw;

    public GameObject retreat;

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

    public GameObject fame;

    private List<BattleUnit> aUnits;

    private List<BattleUnit> dUnits;

    public override IEnumerator PreStart()
    {
        CursorsLibrary.SetMode(CursorsLibrary.Mode.Default);
        Battle battle = Battle.GetBattle();
        global::MOM.Group gDefender = battle.gDefender;
        battle.battleEnd = true;
        battle.ApplyManaUses();
        Battle.PrepareChanges(battle);
        this.aUnits = battle.attackerUnits;
        this.dUnits = battle.defenderUnits;
        this.gridAttackers.CustomDynamicItem(Item, UpdateGridsA);
        this.gridDefenders.CustomDynamicItem(Item, UpdateGridsD);
        this.UpdateGridsA();
        this.UpdateGridsD();
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
        switch (battle.HumanBattleResult())
        {
        case -1:
            this.defeat.SetActive(value: true);
            this.victory.SetActive(value: false);
            this.draw.SetActive(value: false);
            this.retreat.SetActive(value: false);
            AudioLibrary.RequestSFX("BattleDefeat");
            if (this.FameAddedAfterBattle() == 0)
            {
                this.fame.SetActive(value: false);
                break;
            }
            this.fame.SetActive(value: true);
            this.fameLabel.text = global::DBUtils.Localization.Get("UI_FAME_LOST", true) + " " + Math.Abs(this.FameLostAfterBattle());
            break;
        case 1:
            this.defeat.SetActive(value: false);
            this.victory.SetActive(value: true);
            this.draw.SetActive(value: false);
            this.retreat.SetActive(value: false);
            AudioLibrary.RequestSFX("BattleVictory");
            if (this.FameAddedAfterBattle() == 0)
            {
                this.fame.SetActive(value: false);
            }
            else
            {
                this.fame.SetActive(value: true);
                this.fameLabel.text = global::DBUtils.Localization.Get("UI_FAME_AWARDED", true) + " " + this.FameAddedAfterBattle();
            }
            if (gDefender != null && gDefender.IsHosted() && gDefender.locationHost.Get().otherPlaneLocation != null)
            {
                AchievementManager.Progress(AchievementManager.Achievement.PortalMaster, gDefender.locationHost.Get().source.Get().dbName);
            }
            break;
        case 0:
            this.defeat.SetActive(value: false);
            this.victory.SetActive(value: false);
            this.draw.SetActive(!battle.humanSurrendered);
            this.retreat.SetActive(battle.humanSurrendered);
            AudioLibrary.RequestSFX("BattleDraw");
            this.fame.SetActive(value: false);
            break;
        }
        if (FSMSelectionManager.Get().GetSelectedGroup() == battle.gAttacker)
        {
            FSMSelectionManager.Get().Select(null, focus: false);
            if (battle.gAttacker != null && battle.gAttacker.alive)
            {
                FSMSelectionManager.Get().Select(battle.gAttacker, focus: false);
            }
        }
        yield return base.PreStart();
    }

    private void UpdateGridsA()
    {
        this.gridAttackers.UpdateGrid(this.aUnits);
    }

    private void UpdateGridsD()
    {
        this.gridDefenders.UpdateGrid(this.dUnits);
    }

    private void Item(GameObject itemSource, object source, object data, int index)
    {
        CharacterListItem component = itemSource.GetComponent<CharacterListItem>();
        BattleUnit unit = source as BattleUnit;
        component.portrait.texture = AssetManager.GetTexture(unit.dbSource.Get());
        float totalHpPercent = unit.GetTotalHpPercent();
        component.hp.value = totalHpPercent;
        component.hp.gameObject.SetActive(totalHpPercent != 1f || totalHpPercent <= 0f);
        component.deadMarker.SetActive(totalHpPercent <= 0f);
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
        itemSource.GetOrAddComponent<RolloverUnitTooltip>().sourceAsUnit = unit;
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
    }

    public override void OnStart()
    {
        base.OnStart();
        Battle battle = Battle.GetBattle();
        VerticalMarkerManager.Get().ClearBattleMarkers();
        global::MOM.Group group = ((battle.gAttacker != null) ? battle.gAttacker : battle.gDefender);
        if (group != null)
        {
            World.ActivatePlane(group.GetPlane());
            CameraController.CenterAt(group.GetPosition());
            if (!AdventureManager.IsAdventureRunning())
            {
                if (battle.gAttacker != null && battle.gAttacker.alive && battle.gAttacker.GetOwnerID() == PlayerWizard.HumanID())
                {
                    FSMSelectionManager.Get().Select(battle.gAttacker, focus: true);
                }
                else if (battle.gDefender != null && battle.gDefender.alive && battle.gDefender.GetOwnerID() == PlayerWizard.HumanID())
                {
                    FSMSelectionManager.Get().Select(battle.gDefender, focus: true);
                }
            }
        }
        battle.plane?.Destroy();
    }

    public int FameAddedAfterBattle()
    {
        return Battle.GetBattle().winnerFame;
    }

    public int FameLostAfterBattle()
    {
        return Battle.GetBattle().loserFame;
    }
}
