// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.TurnManager
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using MOM.Adventures;
using UnityEngine;
using WorldCode;

public class TurnManager : MonoBehaviour
{
    private enum State
    {
        Undefined = 0,
        Loading = 1,
        Normal = 2,
        Exiting = 3
    }

    private static TurnManager instance;

    public int turnNumber;

    public bool playerTurn;

    public bool aiTurn;

    public bool endTurn;

    private Coroutine turnLoop;

    private State managerState;

    private void Awake()
    {
        TurnManager.instance = this;
    }

    public static TurnManager Get(bool allowNull = false)
    {
        if (TurnManager.instance == null && !allowNull)
        {
            TurnManager.instance = new GameObject("TurnManager").AddComponent<TurnManager>();
            TurnManager.instance.turnNumber = 0;
            TurnManager.instance.turnLoop = TurnManager.instance.StartCoroutine(TurnManager.instance.Manager());
        }
        return TurnManager.instance;
    }

    public static int GetTurnNumber()
    {
        return TurnManager.Get().turnNumber;
    }

    private IEnumerator Manager()
    {
        while (HUD.Get() == null)
        {
            yield return null;
        }
        if (World.GetActivePlane() == null)
        {
            PlayerWizard humanWizard = GameManager.GetHumanWizard();
            if (humanWizard.wizardTower != null)
            {
                World.ActivatePlane(humanWizard.wizardTower.Get().GetPlane());
            }
            else
            {
                World.ActivatePlane(World.GetArcanus());
            }
        }
        BaseUnit.EnsureUnitStr();
        bool postLoad = TurnManager.instance.turnNumber > 0;
        SaveManager.UpdateLastSave();
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            wizard.controlRegion = new WizardControlRegion(wizard.ID);
            wizard.controlRegion.regionsDirty = true;
        }
        while (true)
        {
            if (!postLoad)
            {
                this.StartTurnUpdate();
                yield return this.AITurn();
                yield return this.FameEvents();
                yield return SaveManager.AutoSave();
            }
            else
            {
                postLoad = false;
                foreach (PlayerWizardAI item in EntityManager.GetEntitiesType<PlayerWizardAI>())
                {
                    item.PreparationAfterDeserialization();
                }
                this.RestackGroups(World.GetArcanus());
                this.RestackGroups(World.GetMyrror());
                GameManager.Get().AllowPlaneSwitch(GameManager.Get().allowPlaneSwitch);
            }
            if (GameManager.GetHumanWizard().GetWizardStatus() != PlayerWizard.WizardStatus.Killed)
            {
                yield return this.PlayerTurn();
                if (GameManager.Get()?.timeStopMaster == null)
                {
                    yield return this.NeutralTurn();
                }
                yield return this.EventsTurn();
                yield return this.DiplomaticActions();
            }
            yield return this.EndTurnUpdate();
            yield return null;
        }
    }

    private void StartTurnUpdate()
    {
        TurnManager.instance.turnNumber++;
        Debug.Log("Staring Turn number " + TurnManager.instance.turnNumber);
        List<global::MOM.Group> registeredGroups = GameManager.Get().registeredGroups;
        for (int num = registeredGroups.Count - 1; num >= 0; num--)
        {
            registeredGroups[num].NewTurn();
        }
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            wizard.turnSkillLeft = wizard.GetTotalCastingSkill();
            wizard.GetMagicAndResearch().ProgressCast();
        }
    }

    private IEnumerator EndTurnUpdate()
    {
        if (GameManager.Get()?.timeStopMaster == null)
        {
            this.ProgressCrafting();
            yield return this.ProgressResearchSpell();
        }
        this.EndTurnEffects();
        PlayerWizard humanWizard = GameManager.GetHumanWizard();
        if (GameManager.Get().wizards == null)
        {
            yield break;
        }
        foreach (PlayerWizard v in GameManager.Get().wizards)
        {
            if (v.GetID() != PlayerWizard.HumanID() && v.banishedTurn > 0 && v.isAlive && v.banishedBy != PlayerWizard.HumanID() && v.banishedTurn == TurnManager.GetTurnNumber())
            {
                PopupWizardBanished.OpenPopup(null, v);
                while (PopupWizardBanished.IsOpen())
                {
                    yield return null;
                }
            }
            if (v.isAlive && v.banishedTurn > 0 && v.GetWizardStatus() == PlayerWizard.WizardStatus.Killed)
            {
                if (v == humanWizard)
                {
                    TheRoom theRoom = TheRoom.Open(humanWizard, (humanWizard.banishedBy == 0) ? TheRoom.RoomEvents.DefeatByNeutrals : TheRoom.RoomEvents.DefeatByWizard, GameManager.GetWizard(humanWizard.banishedBy));
                    if (theRoom != null)
                    {
                        PlayMusic.Play((humanWizard.banishedBy == 0) ? "SOUND_LIST-CUTSCENE_DEFEAT_BY_NEUTRALS" : "SOUND_LIST-CUTSCENE_DEFEAT_BY_WIZARD", theRoom);
                        yield return theRoom.WaitWhileOpen();
                    }
                    humanWizard.SetGameScore(isVictory: false, masterOfMagicCasted: false);
                    TurnManager.StopTurnLoop();
                    HallOfFame hallOfFame = HallOfFame.Popup(endGame: false);
                    while ((bool)hallOfFame)
                    {
                        yield return null;
                    }
                    FSMGameplay.Get().HandleEvent("ExitGameplay");
                    yield break;
                }
                v.isAlive = false;
                List<global::MOM.Group> list = new List<global::MOM.Group>();
                List<global::MOM.Unit> list2 = new List<global::MOM.Unit>();
                foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
                {
                    if (!(entity.Value is global::MOM.Location location) || location.GetOwnerID() != v.ID || location.GetUnits().Count <= 0)
                    {
                        continue;
                    }
                    foreach (Reference<global::MOM.Unit> unit in location.GetUnits())
                    {
                        if (unit.Get().IsHero())
                        {
                            list2.Add(unit.Get());
                        }
                    }
                }
                foreach (global::MOM.Unit item in list2)
                {
                    item.Destroy();
                }
                foreach (KeyValuePair<int, Entity> entity2 in EntityManager.Get().entities)
                {
                    if (entity2.Value is global::MOM.Location location2 && location2.GetOwnerID() == v.ID)
                    {
                        if (location2.melding != null && location2.melding.meldOwner == v.ID)
                        {
                            location2.melding = null;
                        }
                        location2.SetOwnerID(0, -1, collateralDamage: true);
                    }
                    if (entity2.Value is global::MOM.Group group && group.GetOwnerID() == v.ID && !group.IsHosted())
                    {
                        list.Add(group);
                    }
                }
                foreach (global::MOM.Group item2 in list)
                {
                    item2.Destroy();
                }
                v.mana = 0;
                for (int num = EnchantmentRegister.GetByWizard(v).Count - 1; num >= 0; num--)
                {
                    EnchantmentInstance enchantmentInstance = EnchantmentRegister.GetByWizard(v)[num];
                    if (enchantmentInstance.manager == null)
                    {
                        Debug.LogError("missing manager");
                    }
                    else
                    {
                        enchantmentInstance.manager.owner.RemoveEnchantment(enchantmentInstance);
                    }
                }
                if (v.banishedBy == humanWizard.ID)
                {
                    TheRoom theRoom2 = TheRoom.Open(humanWizard, TheRoom.RoomEvents.EnemyDefeated, v);
                    if (theRoom2 != null)
                    {
                        yield return theRoom2.WaitWhileOpen();
                    }
                }
                else
                {
                    PopupWizardDefeated.OpenPopup(null, v);
                    while (PopupWizardDefeated.IsOpen())
                    {
                        yield return null;
                    }
                }
            }
            v.EndTurnUpdate();
        }
        bool flag = true;
        foreach (PlayerWizard wizard in GameManager.Get().wizards)
        {
            flag = flag && wizard.IsHuman == wizard.isAlive;
        }
        if (flag)
        {
            GameManager.GetHumanWizard().SetGameScore(isVictory: true, masterOfMagicCasted: false);
            UIManager.Open<Victory>(UIManager.Layer.Popup).SetMessage("UI_YOU_ARE_MASTER_OF_MAGIC2");
            while (UIManager.IsOpen<Victory>(UIManager.Layer.Popup))
            {
                yield return null;
            }
        }
    }

    public static void StopTurnLoop()
    {
        if ((bool)TurnManager.instance && TurnManager.instance.turnLoop != null)
        {
            TurnManager.instance.StopCoroutine(TurnManager.instance.turnLoop);
        }
    }

    private void IncomeAndUpkeep(PlayerWizard v)
    {
        if (v == null)
        {
            return;
        }
        bool num = GameManager.Get()?.timeStopMaster == v;
        int num2 = v.CalculateFoodIncome();
        int num3 = v.CalculateManaIncome();
        if (!num)
        {
            v.mana += num3;
        }
        int num4 = v.CalculateMoneyIncome();
        if (!num)
        {
            v.money += num4;
        }
        if (!num)
        {
            v.AdvanceCastingSkill();
        }
        if (num)
        {
            Enchantment enchantment = (Enchantment)ENCH.TIME_STOP;
            int upkeepCost = enchantment.upkeepCost;
            FInt lowerEnchantmentPercentUpkeepCost = v.lowerEnchantmentPercentUpkeepCost;
            FInt fInt = upkeepCost - upkeepCost * lowerEnchantmentPercentUpkeepCost;
            if (v.mana >= fInt)
            {
                v.mana -= fInt.ToInt();
                return;
            }
            List<EnchantmentInstance> enchantments = GameManager.Get().GetEnchantmentManager().GetEnchantments();
            for (int i = 0; i < enchantments.Count; i++)
            {
                if (enchantments[i].source.Get() == enchantment)
                {
                    GameManager.Get().RemoveEnchantment(enchantments[i]);
                    break;
                }
            }
            return;
        }
        foreach (global::MOM.Location registeredLocation in GameManager.Get().registeredLocations)
        {
            if (registeredLocation.owner != v.ID || !(registeredLocation is TownLocation))
            {
                continue;
            }
            TownLocation townLocation = registeredLocation as TownLocation;
            List<DBReference<Building>> buildings = townLocation.buildings;
            if (buildings != null && buildings.Count > 0)
            {
                for (int num5 = buildings.Count - 1; num5 >= 0; num5--)
                {
                    DBReference<Building> dBReference = buildings[num5];
                    int upkeepCost2 = dBReference.Get().upkeepCost;
                    int upkeepManaCost = dBReference.Get().upkeepManaCost;
                    if (v.money >= upkeepCost2 && v.mana >= upkeepManaCost)
                    {
                        v.money -= upkeepCost2;
                        v.mana -= upkeepManaCost;
                    }
                    else
                    {
                        buildings.RemoveAt(num5);
                    }
                }
            }
            if (townLocation != null)
            {
                v.money += townLocation.roadBonus;
            }
        }
        int money = v.money;
        money += v.GetFame();
        FInt zERO = FInt.ZERO;
        FInt zERO2 = FInt.ZERO;
        _ = FInt.ZERO;
        FInt zERO3 = FInt.ZERO;
        FInt zERO4 = FInt.ZERO;
        FInt zERO5 = FInt.ZERO;
        int num6 = 0;
        for (int num7 = GameManager.Get().registeredGroups.Count - 1; num7 >= 0; num7--)
        {
            global::MOM.Group group = GameManager.Get().registeredGroups[num7];
            if (group.GetOwnerID() == v.ID && (group.GetLocationHost()?.otherPlaneLocation?.Get() == null || !group.plane.arcanusType))
            {
                List<Reference<global::MOM.Unit>> units = group.GetUnits();
                if (units != null && units.Count > 0)
                {
                    List<Reference<global::MOM.Unit>> list = null;
                    for (int num8 = units.Count - 1; num8 >= 0; num8--)
                    {
                        Reference<global::MOM.Unit> reference = units[num8];
                        FInt final = reference.Get().GetAttributes().GetFinal(TAG.UPKEEP_GOLD);
                        FInt final2 = reference.Get().GetAttributes().GetFinal(TAG.UPKEEP_MANA);
                        FInt final3 = reference.Get().GetAttributes().GetFinal(TAG.UPKEEP_FOOD);
                        zERO3 += reference.Get().GetUpkeepChannelerManaDiscount();
                        zERO4 += reference.Get().GetUpkeepConjuerManaDiscount();
                        zERO5 += reference.Get().GetUpkeepNatureSummonerManaDiscount();
                        num6 = zERO3.ToInt() + zERO4.ToInt() + zERO5.ToInt();
                        if (num6 > 0)
                        {
                            zERO3 -= num6;
                            v.mana += num6;
                        }
                        List<SkillScript> skillsByType = reference.Get().GetSkillsByType(ESkillType.IncomeChange);
                        if (skillsByType != null && skillsByType.Count > 0)
                        {
                            foreach (SkillScript item in skillsByType)
                            {
                                int num9 = (int)ScriptLibrary.Call(item.activatorMain, reference.Get(), null, null, item, null, null, null, null);
                                final -= num9;
                                v.money -= final.ToInt();
                            }
                        }
                        if (money >= final && v.mana >= final2 && num2 >= final3)
                        {
                            zERO += final;
                            zERO2 += final2;
                            money -= final.ToInt();
                            v.mana -= final2.ToInt();
                            num2 -= final3.ToInt();
                        }
                        else
                        {
                            Debug.Log("[A]" + reference.Get().dbSource?.ToString() + "Unit destruction due to upkeep, \n money " + money + " cost " + final.ToInt() + "\n mana " + v.mana + " cost " + final2.ToInt() + "\n food " + num2 + " cost " + final3.ToInt());
                            if (list == null)
                            {
                                list = new List<Reference<global::MOM.Unit>>();
                            }
                            list.Add(units[num8]);
                        }
                    }
                    if (list != null && list.Count > 0)
                    {
                        for (int num10 = list.Count - 1; num10 >= 0; num10--)
                        {
                            list[num10].Get().Destroy();
                        }
                        group.UpdateMapFormation();
                        if (v == GameManager.GetHumanWizard())
                        {
                            PopupUnitsDisbanded.OpenPopup(HUD.Get(), list);
                        }
                    }
                }
            }
        }
        v.money = Mathf.Min(money, v.money);
        FInt fInt2 = new FInt(v.mana);
        for (int num11 = EnchantmentRegister.GetByWizard(v).Count - 1; num11 >= 0; num11--)
        {
            EnchantmentInstance enchantmentInstance = EnchantmentRegister.GetByWizard(v)[num11];
            int enchantmentCost = enchantmentInstance.GetEnchantmentCost(v);
            FInt lowerEnchantmentPercentUpkeepCost2 = v.lowerEnchantmentPercentUpkeepCost;
            FInt fInt3 = enchantmentCost - enchantmentCost * lowerEnchantmentPercentUpkeepCost2;
            zERO2 += fInt3;
            if (fInt2 >= fInt3)
            {
                fInt2 -= fInt3;
            }
            else
            {
                string[] obj = new string[6]
                {
                    "[A]",
                    enchantmentInstance?.ToString(),
                    "Enchantment destruction due to upkeep, \n mana ",
                    v.mana.ToString(),
                    " cost ",
                    null
                };
                FInt fInt4 = fInt3;
                obj[5] = fInt4.ToString();
                Debug.Log(string.Concat(obj));
                if (enchantmentInstance.manager == null)
                {
                    Debug.LogError("missing manager");
                }
                else
                {
                    enchantmentInstance.manager.owner.RemoveEnchantment(enchantmentInstance);
                }
            }
        }
        v.mana = fInt2.ToInt();
    }

    private void ProgressCrafting()
    {
        foreach (global::MOM.Location registeredLocation in GameManager.Get().registeredLocations)
        {
            if (registeredLocation is TownLocation)
            {
                TownLocation townLocation = registeredLocation as TownLocation;
                if (townLocation.craftingQueue != null)
                {
                    townLocation.craftingQueue.AdvanceQueue();
                }
            }
        }
    }

    private IEnumerator ProgressResearchSpell()
    {
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            yield return wizard.GetMagicAndResearch().ProgressResearchSpell();
        }
    }

    private IEnumerator EventsTurn()
    {
        this.playerTurn = false;
        this.aiTurn = false;
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_MID_GAME_AWAKE");
        if ((settingAsInt == 1 && TurnManager.GetTurnNumber() == 300) || (settingAsInt == 2 && TurnManager.GetTurnNumber() == 250) || (settingAsInt == 3 && TurnManager.GetTurnNumber() == 150) || (settingAsInt == 4 && TurnManager.GetTurnNumber() == 100))
        {
            AdventureLibrary.currentLibrary.GetGenericEvents();
            Adventure midgameCrisis = AdventureLibrary.currentLibrary.GetMidgameCrisis();
            if (midgameCrisis != null)
            {
                AdventureData adventureData = AdventureManager.TryToTriggerAdventure(midgameCrisis, GameManager.GetHumanWizard(), null, null);
                if (adventureData != null)
                {
                    AdventureManager.ResolveEvent(adventureData, midgameCrisis);
                }
            }
        }
        while (AdventureManager.IsAdventureRunning())
        {
            yield return null;
        }
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            if (!FSMCoreGame.GetAdvManager().IsNextEvent(wizard))
            {
                yield return FSMCoreGame.GetAdvManager().TriggerGenericEvents(wizard);
            }
            else
            {
                yield return FSMCoreGame.GetAdvManager().TrigerTurnEvents(wizard);
            }
        }
    }

    private IEnumerator NeutralTurn()
    {
        MHRandom random = new MHRandom(global::UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        yield return this.MidGameThreat(random);
        DifficultyOption setting = DifficultySettingsData.GetSetting("UI_DIFF_NEUTRAL_ARMIES");
        float scale = 1f;
        int num = 1;
        int num2 = 50;
        if (setting.value == "1")
        {
            scale = 0.5f;
            num = 1;
            num2 = 50;
        }
        if (setting.value == "2")
        {
            scale = 0.75f;
            num = 2;
            num2 = 45;
        }
        if (setting.value == "3")
        {
            scale = 1f;
            num = 4;
            num2 = 40;
        }
        if (setting.value == "4")
        {
            scale = 1.5f;
            num = 5;
            num2 = 30;
        }
        int rampagingMonsterAccumulator = GameManager.Get().rampagingMonsterAccumulator;
        rampagingMonsterAccumulator += random.GetInt(1, num);
        MHTimer t = MHTimer.StartNew();
        if (rampagingMonsterAccumulator > num2)
        {
            rampagingMonsterAccumulator = AINeutralManager.ResolveRuins(random, rampagingMonsterAccumulator, scale, num2, num);
        }
        GameManager.Get().rampagingMonsterAccumulator = rampagingMonsterAccumulator;
        yield return AINeutralManager.ResolveTowns();
        yield return AINeutralManager.ResolveExpeditions();
        Debug.Log("Total Neutral Time: " + t.GetTime());
        this.RestackGroups(World.GetArcanus(), (int o) => o == 0);
        this.RestackGroups(World.GetMyrror(), (int o) => o == 0);
    }

    private IEnumerator MidGameThreat(MHRandom random)
    {
        if ((GameManager.Get().dlcSettings & 1) == 0)
        {
            yield break;
        }
        float midgameMonsterAccumulator = GameManager.Get().midgameMonsterAccumulator;
        midgameMonsterAccumulator = AINeutralManager.CreateMidgameThreat(random, midgameMonsterAccumulator);
        GameManager.Get().midgameMonsterAccumulator = midgameMonsterAccumulator;
        int num = PlayerWizard.HumanID();
        int targetPlayerB = num;
        int num2 = 0;
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            int wizardTownCount = GameManager.GetWizardTownCount(wizard.GetID());
            if (wizardTownCount > num2)
            {
                targetPlayerB = wizard.GetID();
                num2 = wizardTownCount;
            }
        }
        yield return AINeutralManager.UpdateMidGameGroups(num, targetPlayerB);
    }

    private IEnumerator AITurn()
    {
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Adventure) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
        {
            yield return null;
        }
        this.playerTurn = false;
        this.aiTurn = true;
        GameManager.InitializePathfindingObstacles();
        yield return GameManager.DoAITurn();
        List<PlayerWizard> wizards = GameManager.GetWizards();
        for (int i = 0; i < wizards.Count; i++)
        {
            if (wizards[i] is PlayerWizardAI && wizards[i].isAlive && (!(GameManager.Get()?.timeStopMaster != null) || GameManager.Get()?.timeStopMaster.Get() == wizards[i]))
            {
                this.IncomeAndUpkeep(wizards[i]);
            }
        }
        this.RestackGroups(World.GetArcanus(), (int o) => o > PlayerWizard.HumanID());
        this.RestackGroups(World.GetMyrror(), (int o) => o > PlayerWizard.HumanID());
        this.aiTurn = false;
    }

    private IEnumerator PlayerTurn()
    {
        while (GameManager.Get() != null && (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Adventure)))
        {
            yield return null;
        }
        if (GameManager.Get().timeStopMaster != null && GameManager.Get().timeStopMaster.Get() != GameManager.GetHumanWizard())
        {
            yield break;
        }
        this.playerTurn = true;
        this.aiTurn = false;
        this.endTurn = false;
        Settings.GetData().UpdateAnimationSpeed();
        PlayerWizard w = GameManager.GetHumanWizard();
        w.PrepareForCast();
        while (UIManager.IsOpen<UnitInfo>(UIManager.Layer.Standard))
        {
            yield return null;
        }
        TheRoom screen = UIManager.GetScreen<TheRoom>(UIManager.Layer.Popup);
        if (screen != null)
        {
            yield return screen.WaitWhileOpen();
        }
        foreach (global::MOM.Location item in GameManager.GetLocationsOfWizard(w.GetID()))
        {
            if (item is TownLocation)
            {
                TownLocation townLocation = item as TownLocation;
                if (townLocation.autoManaged)
                {
                    townLocation.AutoManageUpdate();
                }
            }
        }
        MHEventSystem.TriggerEvent<TurnManager>(TurnManager.Get(), "Turn");
        while (HUD.Get() == null)
        {
            yield return null;
        }
        HUD.Get()?.UpdateEndTurnButtons();
        if (w.showInfoForBanish)
        {
            w.showInfoForBanish = false;
            UIManager.Open<PopupCapitalLost>(UIManager.Layer.Popup);
        }
        AssetManager.ClearUnused();
        while (!this.endTurn)
        {
            yield return null;
        }
        this.IncomeAndUpkeep(w);
        while (UIManager.AnyBlockingScreen())
        {
            yield return null;
        }
        this.RestackGroups(World.GetArcanus(), (int o) => o == PlayerWizard.HumanID());
        this.RestackGroups(World.GetMyrror(), (int o) => o == PlayerWizard.HumanID());
        FSMSelectionManager.Get().Select(null, focus: false);
        World.GetArcanus().temporaryVisibleArea?.Clear();
        World.GetMyrror().temporaryVisibleArea?.Clear();
        FOW.Get()?.ForceFogToOutdated();
        this.playerTurn = false;
        Settings.GetData().UpdateAnimationSpeed();
        HUD.Get()?.UpdateEndTurnButtons();
    }

    private IEnumerator FameEvents()
    {
        while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Adventure) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
        {
            yield return null;
        }
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            if (wizard.isAlive)
            {
                yield return wizard.FameOffers();
            }
        }
        yield return null;
    }

    private void RestackGroups(global::WorldCode.Plane p, Predicate<int> wizzardFilter = null)
    {
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(p);
        for (int i = 0; i < groupsOfPlane.Count; i++)
        {
            global::MOM.Group group = groupsOfPlane[i];
            if (!group.alive)
            {
                continue;
            }
            if (group.locationHost == null && group.GetUnits().Count == 0)
            {
                group.Destroy();
                i--;
                continue;
            }
            if (wizzardFilter == null || wizzardFilter(group.GetOwnerID()))
            {
                for (int j = i + 1; j < groupsOfPlane.Count; j++)
                {
                    global::MOM.Group group2 = groupsOfPlane[j];
                    if (group2.alive && group2.GetOwnerID() == group.GetOwnerID() && group2.GetPosition() == group.GetPosition())
                    {
                        if (!(group.locationHost != null))
                        {
                            group.TransferUnits(group2);
                            group.Destroy();
                            i--;
                            break;
                        }
                        group2.TransferUnits(group);
                        group2.Destroy();
                        j--;
                    }
                }
            }
            if (group == null || !group.alive || group.locationHost != null)
            {
                continue;
            }
            Hex hexAt = group.GetPlane().GetHexAt(group.GetPosition());
            if (hexAt == null)
            {
                group.Destroy();
                i--;
                continue;
            }
            if ((hexAt.IsLand() && !group.landMovement) || (!hexAt.IsLand() && !group.waterMovement))
            {
                if (group.GetOwnerID() == PlayerWizard.HumanID())
                {
                    SummaryInfo summaryInfo = new SummaryInfo();
                    summaryInfo.summaryType = SummaryInfo.SummaryType.eGroupDrown;
                    summaryInfo.isArcanus = group.GetPlane().arcanusType;
                    summaryInfo.position = group.GetPosition();
                    GameManager.GetHumanWizard().AddNotification(summaryInfo);
                }
                for (int num = group.GetUnits().Count - 1; num >= 0; num--)
                {
                    global::MOM.Unit unit = group.GetUnits()[num].Get();
                    if ((hexAt.IsLand() && !unit.CanTravelOverLand()) || (!hexAt.IsLand() && !unit.CanTravelOverWater()))
                    {
                        unit.Destroy();
                    }
                }
            }
            if (!group.alive)
            {
                i--;
            }
        }
    }

    private IEnumerator DiplomaticActions()
    {
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            if (wizard is PlayerWizardAI playerWizardAI && playerWizardAI.isAlive && (!(GameManager.Get()?.timeStopMaster != null) || GameManager.Get()?.timeStopMaster.Get() == playerWizardAI))
            {
                DiplomacyManager diplomacy = playerWizardAI.GetDiplomacy();
                yield return diplomacy.DiplomaticActivity();
            }
        }
    }

    private void EndTurnEffects()
    {
        GameManager.Get().TriggerScripts(EEnchantmentType.EndTurnEffect);
        GameManager.Get().CountdownUpdate();
        if (GameManager.Get().wizards != null)
        {
            foreach (PlayerWizard wizard in GameManager.Get().wizards)
            {
                wizard.TriggerScripts(EEnchantmentType.EndTurnEffect);
                wizard.CountdownUpdate();
                this.UpdateRisedVolcanosList(wizard);
            }
        }
        if (GameManager.Get().registeredGroups != null)
        {
            foreach (global::MOM.Group registeredGroup in GameManager.Get().registeredGroups)
            {
                if (registeredGroup.GetLocationHost()?.otherPlaneLocation?.Get() != null && registeredGroup.plane.arcanusType)
                {
                    continue;
                }
                foreach (Reference<global::MOM.Unit> unit in registeredGroup.GetUnits())
                {
                    unit.Get().TriggerScripts(EEnchantmentType.EndTurnEffect);
                    unit.Get().CountdownUpdate();
                    unit.Get().TriggerSkillScripts(ESkillType.EndTurnEffect);
                    if (unit.Get().GetAttFinal(TAG.REGENERATION) > 0)
                    {
                        unit.Get().Heal(1f, ignoreCanNaturalHeal: true);
                    }
                }
            }
        }
        if (GameManager.Get().registeredLocations == null)
        {
            return;
        }
        List<global::MOM.Location> registeredLocations = GameManager.Get().registeredLocations;
        for (int num = registeredLocations.Count - 1; num >= 0; num--)
        {
            global::MOM.Location location = registeredLocations[num];
            if (location.owner > 0)
            {
                location.TriggerScripts(EEnchantmentType.EndTurnPositiveDispelEffect);
                location.TriggerScripts(EEnchantmentType.EndTurnNegativeDispelEffect);
                location.TriggerScripts(EEnchantmentType.EndTurnEffect);
                if (location is TownLocation townLocation)
                {
                    townLocation.AttemptToDestroyAwaitingBuildings();
                }
                location.CountdownUpdate();
            }
            else
            {
                location.TriggerScripts(EEnchantmentType.EndTurnNeutralTownIncludedEffect);
                if (location is TownLocation townLocation2)
                {
                    townLocation2.AttemptToDestroyAwaitingBuildings();
                }
                location.CountdownUpdate();
            }
            if (location is TownLocation)
            {
                (location as TownLocation).craftingQueue.ClearTempProgres();
            }
        }
    }

    public static void EndTurn()
    {
        if (TurnManager.Get().playerTurn)
        {
            TurnManager.Get().endTurn = true;
            HUD.Get()?.animEnemyTurnNotification.SetBool("Show", value: true);
            HUD.Get()?.goNotifications.SetActive(value: false);
        }
    }

    public static void CleanupSequence()
    {
        if (TurnManager.instance != null)
        {
            global::UnityEngine.Object.Destroy(TurnManager.instance.gameObject);
        }
    }

    private void UpdateRisedVolcanosList(PlayerWizard w)
    {
        List<Multitype<Vector3i, bool>> volcanoList = w.GetVolcanoList();
        MHRandom mHRandom = new MHRandom();
        for (int num = volcanoList.Count - 1; num >= 0; num--)
        {
            if (mHRandom.GetFloat(0f, 1f) <= 0.02f)
            {
                global::WorldCode.Plane plane = (volcanoList[num].t1 ? World.GetArcanus() : World.GetMyrror());
                Hex hexAt = plane.GetHexAt(volcanoList[num].t0);
                if (hexAt.GetTerrain() == (Terrain)TERRAIN.VOLCANO || hexAt.GetTerrain() == (Terrain)TERRAIN.MYR_VOLCANO)
                {
                    Terrain transmuteTo = hexAt.GetTerrain().transmuteTo;
                    hexAt.SetTerrain(transmuteTo, plane);
                    HashSet<Vector3i> hashSet = new HashSet<Vector3i>();
                    hashSet.Add(hexAt.Position);
                    plane.RebuildUpdatedTerrains(hashSet);
                    plane.UpdateHeightsAfterTerrainChange(hexAt.Position);
                    if ((double)mHRandom.GetFloat(0f, 1f) <= 0.05)
                    {
                        List<RESOURCE> obj = new List<RESOURCE>
                        {
                            RESOURCE.ADAMANTINE_ORE,
                            RESOURCE.COAL,
                            RESOURCE.CRYSX_CRYSTALS,
                            RESOURCE.GEMS,
                            RESOURCE.GOLD_ORE,
                            RESOURCE.IRON_ORE,
                            RESOURCE.MITHRIL_ORE,
                            RESOURCE.QUORK_CRYSTALS,
                            RESOURCE.SILVER_ORE
                        };
                        obj.RandomSort();
                        Resource resource = (Resource)obj[0];
                        GameObject gameObject = AssetManager.Get<GameObject>(resource.GetModel3dName());
                        if (gameObject == null)
                        {
                            Debug.Log("Spawn resource :" + resource.dbName + " model " + resource.descriptionInfo.graphic + " at " + hexAt.Position.ToString() + " graphic " + gameObject?.ToString() + " failed");
                            break;
                        }
                        if (hexAt.resourceInstance != null)
                        {
                            global::UnityEngine.Object.Destroy(hexAt.resourceInstance);
                            hexAt.resourceInstance = null;
                        }
                        hexAt.Resource = resource;
                        Vector3 position = HexCoordinates.HexToWorld3D(hexAt.Position);
                        Chunk chunkFor = plane.GetChunkFor(hexAt.Position);
                        GameObject gameObject2 = GameObjectUtils.Instantiate(gameObject, chunkFor.go.transform);
                        gameObject2.transform.localRotation = Quaternion.Euler(Vector3.up * mHRandom.GetFloat(0f, 360f));
                        gameObject2.transform.position = position;
                        hexAt.resourceInstance = gameObject2;
                        List<GameObject> list = null;
                        foreach (Transform item in gameObject2.transform)
                        {
                            Vector3 position2 = item.position;
                            position2.y = plane.GetHeightAt(position2, allowUnderwater: true);
                            GroundOffset component = item.gameObject.GetComponent<GroundOffset>();
                            if (component != null)
                            {
                                if (position2.y < 0f)
                                {
                                    if (list == null)
                                    {
                                        list = new List<GameObject>();
                                    }
                                    list.Add(item.gameObject);
                                    continue;
                                }
                                position2.y += component.heightOffset;
                            }
                            item.position = position2;
                        }
                        if (list != null)
                        {
                            foreach (GameObject item2 in list)
                            {
                                global::UnityEngine.Object.Destroy(item2);
                            }
                        }
                    }
                }
                w.RemoveVolcanoFromList(volcanoList[num]);
            }
        }
    }
}
