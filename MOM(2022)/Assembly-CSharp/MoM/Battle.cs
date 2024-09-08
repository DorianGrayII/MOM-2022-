using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using UnityEngine;
using WorldCode;


namespace MOM
{
    public class Battle : IEnchantable
    {
        public enum AttackForm
        {
            eNone = 0,
            eMelee = 1,
            eRanged = 2,
            MAX = 3
        }

        public enum BattleWinner
        {
            ATTACKER_WINS = 0,
            ROW = 1,
            DEFENDER_WINS = 2,
            NONE = 3
        }

        public static Battle instance;

        public AIBattleTactics aIBattleTactics = new AIBattleTactics();

        public global::WorldCode.Plane plane;

        public List<Unit> sources = new List<Unit>();

        public List<BattleUnit> attackerUnits = new List<BattleUnit>();

        public List<BattleUnit> defenderUnits = new List<BattleUnit>();

        private List<BattleUnit> cachedAllUnits;

        public Dictionary<BattleUnit, Unit> buToSource = new Dictionary<BattleUnit, Unit>();

        public BattlePlayer attacker = new BattlePlayer();

        public BattlePlayer defender = new BattlePlayer();

        public List<Vector3i> houses = new List<Vector3i>();

        public List<BattleWall> battleWalls = new List<BattleWall>();

        public bool darknessWall;

        public List<GameObject> darnkessWallGo = new List<GameObject>();

        public bool fireWall;

        public List<GameObject> fireWallGo = new List<GameObject>();

        public GameObject wizardTower;

        public List<Vortex> vortexList = new List<Vortex>();

        public List<BattleUnit> confusedList = new List<BattleUnit>();

        public static HashSet<Vector3i> outsideWallLine;

        public static HashSet<Vector3i> fireWalled;

        public static HashSet<Vector3i> insideCity;

        public string attackerName = "TODO.Name";

        public string defenderName = "TODO.Name";

        public int turn;

        public int lastTurn = 50;

        public bool landBattle;

        public float temperature;

        public float humidity;

        public float forest;

        public bool debugMode;

        public bool playerIsAttacker;

        public bool battleEnd;

        public bool humanSurrendered;

        public bool battleOnArcanus;

        public Group gAttacker;

        public Group gDefender;

        public FSMBattleTurn activeTurn;

        private HashSet<Vector3i> lastUnitLocations;

        public MHRandom random;

        public WeakReference<IAttentionController> attentionFocus;
//        public WeakReference attentionFocus;

        public object attentionMessage;

        public EnchantmentManager enchantmentManager;

        public BattleWinner battleWinner = BattleWinner.NONE;

        public int winnerFame;

        public int loserFame;

        public int battleCounterMagic;

        public bool unitBlind;

        public bool simulation;

        public Battle(List<Reference<Unit>> attacker, List<Reference<Unit>> defender, int attackerWizzardID, int defenderWizzardID)
        {
            GameManager.Get().TakeFocus(this, GameManager.FocusFlag.Battle);
            Battle.SetBattle(this);
            this.random = new MHRandom(global::UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            foreach (Reference<Unit> item in attacker)
            {
                if (this.gAttacker == null)
                {
                    this.gAttacker = item.Get().group?.Get();
                    break;
                }
            }
            foreach (Reference<Unit> item2 in defender)
            {
                if (this.gDefender == null)
                {
                    this.gDefender = item2.Get().group?.Get();
                    break;
                }
            }
            this.battleOnArcanus = this.IsArcanus();
            Location location = this.gDefender.GetLocationHost();
            if (location != null && location.otherPlaneLocation != null && this.gAttacker != null && this.gAttacker.GetPlane() == location.otherPlaneLocation.Get().GetPlane())
            {
                location = location.otherPlaneLocation.Get();
            }
            this.playerIsAttacker = attackerWizzardID == PlayerWizard.HumanID();
            this.attacker.playerOwner = attackerWizzardID == PlayerWizard.HumanID();
            this.attacker.autoPlayByAI = !this.attacker.playerOwner;
            PlayerWizard wizard = GameManager.GetWizard(attackerWizzardID);
            if (wizard != null)
            {
                this.attacker.wizard = wizard;
                this.attacker.castingSkill = wizard.GetTotalCastingSkill();
                this.attacker.mana = wizard.GetMana(wizard is PlayerWizardAI);
                if (location != null)
                {
                    this.attacker.towerDistanceCost = wizard.GetDistanceCost(location.GetPlane(), location.GetPosition());
                }
                else if (this.gDefender != null)
                {
                    this.attacker.towerDistanceCost = wizard.GetDistanceCost(this.gDefender.GetPlane(), this.gDefender.GetPosition());
                }
                else if (this.gAttacker != null)
                {
                    this.attacker.towerDistanceCost = wizard.GetDistanceCost(this.gAttacker.GetPlane(), this.gAttacker.GetPosition());
                }
                else
                {
                    this.attacker.towerDistanceCost = 30f;
                }
            }
            this.defender.playerOwner = defenderWizzardID == PlayerWizard.HumanID();
            this.defender.autoPlayByAI = !this.defender.playerOwner;
            wizard = GameManager.GetWizard(defenderWizzardID);
            if (wizard != null)
            {
                this.defender.wizard = wizard;
                this.defender.castingSkill = wizard.GetTotalCastingSkill();
                this.defender.mana = wizard.GetMana(wizard is PlayerWizardAI);
                if (location != null)
                {
                    this.defender.towerDistanceCost = wizard.GetDistanceCost(location.GetPlane(), location.GetPosition());
                }
                else if (this.gDefender != null)
                {
                    this.defender.towerDistanceCost = wizard.GetDistanceCost(this.gDefender.GetPlane(), this.gDefender.GetPosition());
                }
                else if (this.gAttacker != null)
                {
                    this.defender.towerDistanceCost = wizard.GetDistanceCost(this.gAttacker.GetPlane(), this.gAttacker.GetPosition());
                }
                else
                {
                    this.defender.towerDistanceCost = 30f;
                }
            }
            this.attackerName = this.attacker.GetName();
            this.defenderName = this.defender.GetName();
            if (location != null)
            {
                if (location is TownLocation)
                {
                    this.defender.isChaosProtected = (location as TownLocation).isChaosProtected;
                    this.defender.isSorceryProtected = (location as TownLocation).isSorceryProtected;
                    this.defender.isLifeProtected = (location as TownLocation).isLifeProtected;
                    this.defender.isDeathProtected = (location as TownLocation).isDeathProtected;
                    this.defender.isNatureProtected = (location as TownLocation).isNatureProtected;
                }
                foreach (EnchantmentInstance enchantment in location.GetEnchantments())
                {
                    if (enchantment.source.Get().scripts == null)
                    {
                        continue;
                    }
                    EnchantmentScript[] scripts = enchantment.source.Get().scripts;
                    foreach (EnchantmentScript enchantmentScript in scripts)
                    {
                        if (enchantmentScript.triggerType == EEnchantmentType.RemoteUnitAttributeChange || enchantmentScript.triggerType == EEnchantmentType.RemoteUnitAttributeChangeMP || enchantmentScript.triggerType == EEnchantmentType.BattleTurnStartEffect || enchantmentScript.triggerType == EEnchantmentType.BattleTurnEndEffect || enchantmentScript.triggerType == EEnchantmentType.BattleStartEffect || enchantmentScript.triggerType == EEnchantmentType.BattleEndEffect || enchantmentScript.triggerType == EEnchantmentType.BattleWizardTowerEffect)
                        {
                            this.GetEnchantmentManager().Add(enchantment.source.Get(), (enchantment.owner != null) ? enchantment.owner.GetEntity() : null);
                            break;
                        }
                    }
                }
            }
            foreach (EnchantmentInstance enchantment2 in GameManager.Get().GetEnchantments())
            {
                if (enchantment2.source.Get().scripts == null)
                {
                    continue;
                }
                EnchantmentScript[] scripts = enchantment2.source.Get().scripts;
                foreach (EnchantmentScript enchantmentScript2 in scripts)
                {
                    if (enchantmentScript2.triggerType == EEnchantmentType.RemoteUnitAttributeChange || enchantmentScript2.triggerType == EEnchantmentType.RemoteUnitAttributeChangeMP || enchantmentScript2.triggerType == EEnchantmentType.BattleTurnStartEffect || enchantmentScript2.triggerType == EEnchantmentType.BattleTurnEndEffect || enchantmentScript2.triggerType == EEnchantmentType.BattleStartEffect || enchantmentScript2.triggerType == EEnchantmentType.BattleEndEffect)
                    {
                        this.GetEnchantmentManager().Add(enchantment2.source.Get(), (enchantment2.owner != null) ? enchantment2.owner.GetEntity() : null);
                        break;
                    }
                }
            }
            foreach (Reference<Unit> item3 in attacker)
            {
                BattleUnit battleUnit = BattleUnit.Create(item3.Get(), abstractMode: false, -1, attackingSide: true);
                this.sources.Add(item3.Get());
                this.attackerUnits.Add(battleUnit);
                this.buToSource[battleUnit] = item3.Get();
            }
            foreach (Reference<Unit> item4 in defender)
            {
                BattleUnit battleUnit2 = BattleUnit.Create(item4.Get());
                this.sources.Add(item4.Get());
                this.defenderUnits.Add(battleUnit2);
                this.buToSource[battleUnit2] = item4.Get();
            }
            foreach (BattleUnit item5 in ListUtils.MultiEnumerable(this.attackerUnits, this.defenderUnits))
            {
                item5.attributes.SetDirty();
                item5.EnsureFinal();
                item5.FillMovementCost();
            }
            if (this.attacker.GetWizardOwner() != null)
            {
                this.attacker.GetWizardOwner().GetMagicAndResearch().extensionItemSpellBattle.extraMana = 0;
                this.attacker.GetWizardOwner().GetMagicAndResearch().extensionItemSpellBattle.extraSkill = 0;
                this.attacker.GetWizardOwner().GetMagicAndResearch().extensionItemSpellBattle.extraPower = 0;
            }
            if (this.defender.GetWizardOwner() != null)
            {
                this.defender.GetWizardOwner().GetMagicAndResearch().extensionItemSpellBattle.extraMana = 0;
                this.defender.GetWizardOwner().GetMagicAndResearch().extensionItemSpellBattle.extraSkill = 0;
                this.defender.GetWizardOwner().GetMagicAndResearch().extensionItemSpellBattle.extraPower = 0;
            }
            this.UnitListsDirty();
            this.InitialUnitPlacement();
        }

        public static Battle Create(Group attacker, Group defender)
        {
            Battle battle = new Battle(attacker.GetUnits(), defender.GetUnits(), attacker.GetOwnerID(), defender.GetOwnerID());
            battle.DebugMode(value: true);
            bool flag = defender.GetPlane()?.GetHexAt(defender.GetPosition())?.IsLand() ?? true;
            battle.landBattle = flag;
            battle.temperature = 0.5f;
            battle.humidity = 0.5f;
            battle.forest = 0.38f;
            return battle;
        }

        public static Battle GetBattle()
        {
            return Battle.instance;
        }

        public static void SetBattle(Battle battle)
        {
            if (Battle.instance != null)
            {
                Debug.LogError("BattleOverride!");
                throw new Exception("BattleOverride");
            }
            Battle.instance = battle;
        }

        private bool IsArcanus()
        {
            Location location = this.gDefender.GetLocationHost();
            if (location != null)
            {
                if (location.otherPlaneLocation != null && this.gAttacker != null && this.gAttacker.GetPlane() == location.otherPlaneLocation.Get().GetPlane())
                {
                    location = location.otherPlaneLocation.Get();
                }
                return location.GetPlane().arcanusType;
            }
            return (this.gDefender?.GetPlane()?.arcanusType ?? this.gAttacker?.GetPlane()?.arcanusType) ?? true;
        }

        public BattleUnit CreateSummon(int owner, Subrace unit, Vector3i pos)
        {
            if (unit == null)
            {
                Debug.LogError("Unit not found in database");
                return null;
            }
            bool attackingSide = this.attacker.GetID() == owner;
            Unit unit2 = Unit.CreateFrom(unit);
            BattleUnit battleUnit = BattleUnit.Create(unit2, abstractMode: false, owner, attackingSide);
            battleUnit.battlePosition = pos;
            battleUnit.simulated = this.simulation;
            this.sources.Add(unit2);
            this.buToSource[battleUnit] = unit2;
            if (battleUnit.attackingSide)
            {
                this.AttackerAddUnit(battleUnit);
            }
            else
            {
                this.DefenderAddUnit(battleUnit);
            }
            this.UnitListsDirty();
            battleUnit.GetOrCreateFormation();
            if (this.lastUnitLocations != null)
            {
                this.lastUnitLocations.Add(pos);
            }
            if (battleUnit.GetWizardOwner() != null)
            {
                battleUnit.GetWizardOwner().ModifyUnitSkillsByTraits(battleUnit);
            }
            battleUnit.Mp = battleUnit.attributes.GetFinal(TAG.MOVEMENT_POINTS);
            return battleUnit;
        }

        public bool IsLocationEmpty(Vector3i pos)
        {
            if (this.GetUnitAt(pos) != null)
            {
                return false;
            }
            HashSet<Vector3i> exclusionPoints = this.plane.exclusionPoints;
            if (exclusionPoints != null && exclusionPoints.Contains(pos))
            {
                return false;
            }
            return true;
        }

        public void InitialUnitPlacement()
        {
            for (int i = 0; i < this.attackerUnits.Count; i++)
            {
                int num = ((i % 2 != 0) ? 1 : (-1));
                int num3;
                int num4;
                if (i < 5)
                {
                    int num2 = (i + 1) / 2;
                    num3 = 4 + num2 * num;
                    num4 = -4;
                }
                else
                {
                    int num5 = (i - 4) / 2;
                    num3 = 5 + num5 * num;
                    num4 = -5;
                }
                Vector3i battlePosition = new Vector3i(num3, num4, -num3 - num4);
                this.attackerUnits[i].battlePosition = battlePosition;
            }
            for (int j = 0; j < this.defenderUnits.Count; j++)
            {
                int num6 = ((j % 2 != 0) ? 1 : (-1));
                int num3;
                int num4;
                if (j < 5)
                {
                    int num7 = (j + 1) / 2;
                    num3 = -4 + num7 * num6;
                    num4 = 4;
                }
                else
                {
                    int num8 = (j - 4) / 2;
                    num3 = -5 + num8 * num6;
                    num4 = 5;
                }
                Vector3i battlePosition2 = new Vector3i(num3, num4, -num3 - num4);
                this.defenderUnits[j].battlePosition = battlePosition2;
            }
        }

        internal bool IsFinished()
        {
            bool flag = false;
            foreach (BattleUnit attackerUnit in this.attackerUnits)
            {
                if (attackerUnit.IsAlive())
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return true;
            }
            flag = false;
            foreach (BattleUnit defenderUnit in this.defenderUnits)
            {
                if (defenderUnit.IsAlive())
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return true;
            }
            return false;
        }

        internal void Destroy()
        {
            if (this.attackerUnits != null)
            {
                foreach (BattleUnit attackerUnit in this.attackerUnits)
                {
                    if (attackerUnit.battleFormation != null)
                    {
                        attackerUnit.battleFormation.Destroy();
                    }
                }
            }
            if (this.defenderUnits != null)
            {
                foreach (BattleUnit defenderUnit in this.defenderUnits)
                {
                    if (defenderUnit.battleFormation != null)
                    {
                        defenderUnit.battleFormation.Destroy();
                    }
                }
            }
            Battle.instance = null;
            GameManager.Get().FreeFocus(this);
            MHEventSystem.TriggerEvent<Battle>(this, this);
        }

        public void DebugMode(bool value)
        {
            this.debugMode = value;
        }

        public BattleUnit GetUnitAt(Vector3i pos)
        {
            if (this.unitBlind)
            {
                return null;
            }
            foreach (BattleUnit attackerUnit in this.attackerUnits)
            {
                if (attackerUnit.battlePosition == pos && attackerUnit.FigureCount() > 0)
                {
                    if (attackerUnit.FigureCount() == 0)
                    {
                        return null;
                    }
                    return attackerUnit;
                }
            }
            foreach (BattleUnit defenderUnit in this.defenderUnits)
            {
                if (defenderUnit.battlePosition == pos && defenderUnit.FigureCount() > 0)
                {
                    if (defenderUnit.FigureCount() == 0)
                    {
                        return null;
                    }
                    return defenderUnit;
                }
            }
            return null;
        }

        public bool IsAttackerUnit(BattleUnit bu)
        {
            return this.attackerUnits.Contains(bu);
        }

        public bool IsLocationOccupied(Vector3i pos)
        {
            foreach (BattleUnit attackerUnit in this.attackerUnits)
            {
                if (attackerUnit.IsAlive() && attackerUnit.GetPosition() == pos)
                {
                    return true;
                }
            }
            foreach (BattleUnit defenderUnit in this.defenderUnits)
            {
                if (defenderUnit.IsAlive() && defenderUnit.GetPosition() == pos)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsLocationFreeForUnit(Vector3i pos, bool defender)
        {
            if (!this.plane.area.IsInside(pos))
            {
                return false;
            }
            if (!this.IsLocationEmpty(pos))
            {
                return false;
            }
            if (this.battleWalls != null)
            {
                foreach (BattleWall battleWall in this.battleWalls)
                {
                    if (battleWall.position == pos)
                    {
                        if (defender && battleWall.gate)
                        {
                            return true;
                        }
                        return !battleWall.standing;
                    }
                }
            }
            return true;
        }

        public HashSet<Vector3i> GetUnitLocations()
        {
            if (this.lastUnitLocations == null)
            {
                this.lastUnitLocations = new HashSet<Vector3i>();
            }
            else
            {
                this.lastUnitLocations.Clear();
            }
            foreach (BattleUnit attackerUnit in this.attackerUnits)
            {
                if (attackerUnit.FigureCount() != 0)
                {
                    this.lastUnitLocations.Add(attackerUnit.battlePosition);
                }
            }
            foreach (BattleUnit defenderUnit in this.defenderUnits)
            {
                if (defenderUnit.FigureCount() != 0)
                {
                    this.lastUnitLocations.Add(defenderUnit.battlePosition);
                }
            }
            return this.lastUnitLocations;
        }

        private bool SortingNeeded()
        {
            bool flag = true;
            for (int i = 0; i < this.attackerUnits.Count; i++)
            {
                if (this.attackerUnits[i].figureCount > 0)
                {
                    if (!flag)
                    {
                        return true;
                    }
                }
                else
                {
                    flag = false;
                }
            }
            flag = true;
            for (int j = 0; j < this.defenderUnits.Count; j++)
            {
                if (this.defenderUnits[j].figureCount > 0)
                {
                    if (!flag)
                    {
                        return true;
                    }
                }
                else
                {
                    flag = false;
                }
            }
            return false;
        }

        public void SortIfNeeded()
        {
            if (this.SortingNeeded())
            {
                this.attackerUnits.Sort(SortingCallback);
                this.defenderUnits.Sort(SortingCallback);
            }
        }

        private int SortingCallback(BattleUnit A, BattleUnit B)
        {
            if (A.figureCount > 0 && B.figureCount > 0)
            {
                return 0;
            }
            if (A.figureCount == 0 && B.figureCount == 0)
            {
                return 0;
            }
            if (A.figureCount > 0 && B.figureCount == 0)
            {
                return -1;
            }
            return 1;
        }

        public BattlePlayer GetPlayer(bool attacker)
        {
            if (!attacker)
            {
                return this.defender;
            }
            return this.attacker;
        }

        public BattlePlayer GetOtherPlayer(BattlePlayer b)
        {
            if (this.attacker != b)
            {
                return this.attacker;
            }
            return this.defender;
        }

        public List<BattleUnit> GetUnits(bool attacker)
        {
            if (!attacker)
            {
                return this.defenderUnits;
            }
            return this.attackerUnits;
        }

        public BattlePlayer GetBattlePlayerForWizard(PlayerWizard w)
        {
            if (this.attacker.wizard == w)
            {
                return this.attacker;
            }
            if (this.defender.wizard == w)
            {
                return this.defender;
            }
            return null;
        }

        public BattlePlayer GetHumanPlayer()
        {
            if (!this.defender.playerOwner)
            {
                return this.attacker;
            }
            return this.defender;
        }

        public List<BattleUnit> GetHumanPlayerUnits()
        {
            if (!this.defender.playerOwner)
            {
                return this.attackerUnits;
            }
            return this.defenderUnits;
        }

        public void ApplyManaUses(BattleResult br)
        {
            if (this.attacker.wizard != null)
            {
                this.attacker.wizard.SetMana(br.aManaLeft);
            }
            if (this.defender.wizard != null)
            {
                this.defender.wizard.SetMana(br.dManaLeft);
            }
        }

        public void ApplyManaUses()
        {
            if (this.attacker.wizard != null)
            {
                int mana = Mathf.Min(this.attacker.wizard.mana, this.GetBattlePlayerForWizard(this.attacker.wizard).mana);
                this.attacker.wizard.SetMana(mana);
            }
            if (this.defender.wizard != null)
            {
                int mana2 = Mathf.Min(this.defender.wizard.mana, this.GetBattlePlayerForWizard(this.defender.wizard).mana);
                this.defender.wizard.SetMana(mana2);
            }
        }

        public void ApplyResultsToUnits(BattleResult br)
        {
            foreach (BattleUnit v in ListUtils.MultiEnumerable(br.attacker, br.defender))
            {
                if (v.isAnimatedInSimulation)
                {
                    BattleUnit battleUnit = this.attackerUnits.Find((BattleUnit o) => o.ID == v.ID);
                    BattleUnit battleUnit2 = this.defenderUnits.Find((BattleUnit o) => o.ID == v.ID);
                    if (battleUnit != null)
                    {
                        battleUnit.isAnimatedInSimulation = true;
                        battleUnit.attackingSide = v.attackingSide;
                    }
                    else if (battleUnit2 != null)
                    {
                        battleUnit2.isAnimatedInSimulation = true;
                        battleUnit2.attackingSide = v.attackingSide;
                    }
                }
            }
            foreach (Unit unit in this.sources)
            {
                BattleUnit battleUnit3 = br.attacker.Find((BattleUnit o) => o.ID == unit.ID);
                BattleUnit battleUnit4 = this.attackerUnits.Find((BattleUnit o) => o.ID == unit.ID);
                if (battleUnit3 == null)
                {
                    battleUnit3 = br.defender.Find((BattleUnit o) => o.ID == unit.ID);
                }
                if (battleUnit4 == null)
                {
                    battleUnit4 = this.defenderUnits.Find((BattleUnit o) => o.ID == unit.ID);
                }
                if (battleUnit3 == null || battleUnit4 == null)
                {
                    unit.figureCount = 0;
                    if (battleUnit4 != null)
                    {
                        battleUnit4.figureCount = 0;
                    }
                }
                else
                {
                    unit.figureCount = battleUnit3.FigureCount();
                    battleUnit4.figureCount = battleUnit3.FigureCount();
                    unit.currentFigureHP = battleUnit3.currentFigureHP;
                    battleUnit4.currentFigureHP = battleUnit3.currentFigureHP;
                    battleUnit4.normalDamages = battleUnit3.normalDamages;
                    battleUnit4.undeadDamages = battleUnit3.undeadDamages;
                    battleUnit4.irreversibleDamages = battleUnit3.irreversibleDamages;
                }
            }
        }

        public void ApplyFleeDamages(bool attacker)
        {
            int num = 0;
            List<BattleUnit> list;
            List<BattleUnit> list2;
            if (attacker)
            {
                list = this.attackerUnits;
                list2 = this.defenderUnits;
            }
            else
            {
                list = this.defenderUnits;
                list2 = this.attackerUnits;
            }
            if (list2 == null || list2.Count < 1)
            {
                return;
            }
            foreach (BattleUnit item in list2)
            {
                FInt attFinal = item.GetAttFinal(TAG.MOVEMENT_POINTS);
                if (attFinal > num)
                {
                    num = attFinal.ToInt();
                }
            }
            bool flag = list.FindAll((BattleUnit o) => o.IsAlive()).Count > list2.FindAll((BattleUnit o) => o.IsAlive()).Count;
            foreach (BattleUnit item2 in list)
            {
                if (this.buToSource.ContainsKey(item2))
                {
                    int num2 = global::UnityEngine.Random.Range(0, item2.GetAttFinal(TAG.MOVEMENT_POINTS).ToInt());
                    int num3 = global::UnityEngine.Random.Range(0, num);
                    if ((!flag || num2 < num3) && num2 <= num3)
                    {
                        Unit unit = this.buToSource[item2];
                        float num4 = global::UnityEngine.Random.Range(0.2f, 1f);
                        int value = item2.figureCount - Mathf.RoundToInt((float)item2.figureCount * num4);
                        value = (item2.figureCount = Mathf.Clamp(value, 0, item2.figureCount));
                        unit.figureCount = item2.figureCount;
                        int value2 = item2.currentFigureHP - Mathf.RoundToInt((float)item2.currentFigureHP * num4);
                        value2 = ((value != 0) ? Mathf.Clamp(value2, 1, item2.currentFigureHP) : 0);
                        item2.currentFigureHP = value2;
                        unit.currentFigureHP = item2.currentFigureHP;
                    }
                }
            }
        }

        public bool ValidTargetIgnoringDistance(BattleUnit attacker, BattleUnit defender, bool melee)
        {
            AttackForm attackForm = Battle.AttackFormPossible(attacker, defender, melee ? 1 : 2);
            if (melee)
            {
                return attackForm == AttackForm.eMelee;
            }
            return attackForm == AttackForm.eRanged;
        }

        public bool ValidTarget(BattleUnit attacker, BattleUnit defender, bool melee)
        {
            AttackForm attackForm = Battle.AttackFormPossible(attacker, defender);
            if (melee)
            {
                return attackForm == AttackForm.eMelee;
            }
            return attackForm == AttackForm.eRanged;
        }

        public static AttackForm AttackFormPossible(BattleUnit attacker, BattleUnit defender, int forcedDistance = -1)
        {
            int num = forcedDistance;
            if (num < 0)
            {
                num = HexCoordinates.HexDistance(attacker.GetPosition(), defender.GetPosition());
            }
            switch (num)
            {
            case 0:
                return AttackForm.eNone;
            case 1:
                if (defender.GetAttributes().GetFinal(TAG.CAN_FLY) > FInt.ZERO)
                {
                    if (attacker.GetAttributes().GetFinal(TAG.CAN_FLY) > FInt.ZERO)
                    {
                        return AttackForm.eMelee;
                    }
                    List<DBReference<Skill>> skills = attacker.GetSkills();
                    if (skills == null)
                    {
                        break;
                    }
                    foreach (DBReference<Skill> item in skills)
                    {
                        if (item.Get().script == null)
                        {
                            continue;
                        }
                        SkillScript[] script = item.Get().script;
                        foreach (SkillScript skillScript in script)
                        {
                            if (skillScript.allowMeleeVsFly && (skillScript.triggerType == ESkillType.BattleAttack || skillScript.triggerType == ESkillType.BattleAttackAddon || skillScript.triggerType == ESkillType.BattleAttackAddon2))
                            {
                                return AttackForm.eMelee;
                            }
                        }
                    }
                    break;
                }
                return AttackForm.eMelee;
            default:
                if (attacker.GetCurentFigure().rangedAmmo > 0)
                {
                    if (defender.IsInvisibleUnit() && attacker.GetAttributes().GetFinal(TAG.ILLUSIONS_IMMUNITY) < 1)
                    {
                        return AttackForm.eNone;
                    }
                    return AttackForm.eRanged;
                }
                break;
            }
            return AttackForm.eNone;
        }

        internal void UnitBlind()
        {
            this.unitBlind = true;
        }

        public bool IsAttentionAvaliable()
        {
            if (this.attentionMessage != null)
            {
                return false;
            }
            if (this.attentionFocus == null)
            {
                return true;
            }
            if (this.attentionFocus.TryGetTarget(out var target) && target != null && target.RequiresFocus())
            {
                return false;
            }
            return true;
        }

        internal void UnitNormal()
        {
            this.unitBlind = false;
        }

        public IEnumerator WaitForAttention()
        {
            while (!this.IsAttentionAvaliable())
            {
                yield return null;
            }
        }

        public void GainAttention(IAttentionController a)
        {
            this.attentionFocus = new WeakReference<IAttentionController>(a);
            if (!(a is Formation) || !Settings.GetData().GetBattleCameraFollow())
            {
                return;
            }
            if ((a as Formation).owner is BattleUnit battleUnit)
            {
                if (battleUnit.ownerID == PlayerWizard.HumanID() || (battleUnit.ownerID != PlayerWizard.HumanID() && battleUnit.currentlyVisible))
                {
                    CameraController.CenterAt((a as Formation).owner.GetPosition());
                }
            }
            else
            {
                CameraController.CenterAt((a as Formation).owner.GetPosition());
            }
        }

        public void SetMessageAttentionTo(object messageAttention)
        {
            if (this.attentionMessage != null && messageAttention != null)
            {
                Debug.LogWarning("Message attention is overridden!");
            }
            this.attentionMessage = messageAttention;
        }

        public bool MayUseRanged(BattleUnit u)
        {
            if (u.IsAlive() && u.GetCurentFigure().rangedAmmo == 0)
            {
                return false;
            }
            foreach (BattleUnit item in u.attackingSide ? this.defenderUnits : this.attackerUnits)
            {
                if (item.IsAlive() && HexCoordinates.HexDistance(item.GetPosition(), u.GetPosition()) == 1)
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerator AttackWall(BattleUnit attacker, BattleWall bw)
        {
            if (attacker == null || !(attacker.Mp > 0) || !this.attackerUnits.Contains(attacker) || !attacker.attackingSide)
            {
                yield break;
            }
            BattleHUD.CombatLogAdd(global::DBUtils.Localization.Get("UI_COMBAT_LOG_WALL_ATTACK", true, attacker.GetName(), WizardColors.GetHex(attacker.GetWizardOwner())));
            int dist = HexCoordinates.HexDistance(bw.position, attacker.GetPosition());
            attacker.UseHalfOfMaxMP();
            attacker.GetCurentFigure().rangedAmmo--;
            if (dist <= 1)
            {
                attacker.battleFormation.Attack(bw.position);
                if ((dist <= 1 && global::UnityEngine.Random.Range(0f, 1f) < 0.5f) || (dist > 1 && global::UnityEngine.Random.Range(0f, 1f) < 0.25f))
                {
                    bw.standing = false;
                    bw.AnimateDestroy();
                    this.plane.ClearSearcherData();
                }
            }
            else
            {
                attacker.battleFormation.AttackRanged(bw.position, null, delegate
                {
                    if ((dist <= 1 && global::UnityEngine.Random.Range(0f, 1f) < 0.5f) || (dist > 1 && global::UnityEngine.Random.Range(0f, 1f) < 0.25f))
                    {
                        bw.standing = false;
                        bw.AnimateDestroy();
                        this.plane.ClearSearcherData();
                    }
                });
            }
            if (attacker.dbSource.Get().selfdestructing)
            {
                attacker.currentFigureHP = 0;
                attacker.figureCount = 0;
                attacker.GetOrCreateFormation().UpdateFigureCount();
            }
            while (attacker.battleFormation.IsAnimating())
            {
                yield return null;
            }
        }

        public IEnumerator AttackUnit(BattleUnit attacker, BattleUnit defender)
        {
            switch (HexCoordinates.HexDistance(defender.GetPosition(), attacker.GetPosition()))
            {
            case 0:
                yield break;
            case 1:
            {
                if (attacker.canAttack)
                {
                    attacker.UseHalfOfMaxMP();
                }
                BattleAttackStack battleStack2 = new BattleAttackStack(this, attacker, defender, this.random);
                battleStack2.ExecuteSkillStack(populateHUD: true);
                if (!this.battleEnd && attacker.canAttack)
                {
                    attacker.GetOrCreateFormation()?.Attack(defender.GetPosition());
                    yield return new WaitForSeconds(0.2f);
                }
                if (this.battleEnd)
                {
                    BattleHUD.CombatLogFinishStack(battleStack2);
                    yield break;
                }
                if (defender.canContrAttack)
                {
                    defender.GetOrCreateFormation()?.Attack(attacker.GetPosition());
                }
                attacker.GetOrCreateFormation()?.UpdateFigureCount();
                defender.GetOrCreateFormation()?.UpdateFigureCount();
                if (!attacker.IsAlive() || !defender.IsAlive())
                {
                    if (!attacker.IsAlive())
                    {
                        this.UnitDie(attacker);
                    }
                    if (!defender.IsAlive())
                    {
                        this.UnitDie(defender);
                    }
                    BattleHUD.CombatLogFinishStack(battleStack2);
                    this.UpdateInvisibility();
                    yield break;
                }
                if (attacker.haste && attacker.canAttack)
                {
                    yield return new WaitForSeconds(0.2f);
                    if (!this.battleEnd)
                    {
                        attacker.GetOrCreateFormation()?.Attack(defender.GetPosition());
                    }
                }
                if (!this.battleEnd && defender.haste && defender.canContrAttack)
                {
                    yield return new WaitForSeconds(0.2f);
                    if (!this.battleEnd)
                    {
                        defender.GetOrCreateFormation()?.Attack(attacker.GetPosition());
                    }
                }
                if (this.battleEnd)
                {
                    BattleHUD.CombatLogFinishStack(battleStack2);
                    yield break;
                }
                if (attacker.haste && attacker.canAttack)
                {
                    defender.GetOrCreateFormation()?.UpdateFigureCount();
                }
                if (defender.haste && defender.canContrAttack)
                {
                    attacker.GetOrCreateFormation()?.UpdateFigureCount();
                }
                BattleHUD.CombatLogFinishStack(battleStack2);
                break;
            }
            default:
            {
                if (this.darknessWall && this.AttactThroughWall(attacker.GetPosition(), defender.GetPosition(), this.darknessWall) && attacker.GetAttFinal(TAG.ILLUSIONS_IMMUNITY) <= 0)
                {
                    yield break;
                }
                if (!attacker.canAttack)
                {
                    break;
                }
                if (attacker.GetCurentFigure().rangedAmmo == 0 || attacker.GetSkillsByType(ESkillType.BattleRangedAttack, canReturnNull: false).Count == 0)
                {
                    yield break;
                }
                attacker.Mp = FInt.ZERO;
                attacker.GetCurentFigure().rangedAmmo--;
                BattleAttackStack battleStack2 = new BattleAttackStack(this, attacker, defender, this.random);
                battleStack2.ExecuteSkillStack(populateHUD: true);
                bool shooting = true;
                CharacterActor ca = null;
                attacker.GetOrCreateFormation()?.AttackRanged(defender.GetPosition(), defender, delegate(object o)
                {
                    ca = o as CharacterActor;
                    shooting = false;
                });
                float softLockStop2 = 10f;
                while (shooting && softLockStop2 > 0f)
                {
                    softLockStop2 -= Time.deltaTime;
                    yield return null;
                }
                defender.GetOrCreateFormation()?.UpdateFigureCount();
                defender.GetOrCreateFormation()?.GetHit();
                if (!attacker.IsAlive() || !defender.IsAlive())
                {
                    if (!attacker.IsAlive())
                    {
                        this.UnitDie(attacker);
                    }
                    if (!defender.IsAlive())
                    {
                        this.UnitDie(defender);
                    }
                    BattleHUD.CombatLogFinishStack(battleStack2);
                    this.UpdateInvisibility();
                    yield break;
                }
                int additionalAttacks = 0;
                if (attacker.doubleShot && attacker.canAttack)
                {
                    additionalAttacks++;
                }
                if (attacker.haste && attacker.canAttack)
                {
                    additionalAttacks++;
                }
                for (int i = 0; i < additionalAttacks; i++)
                {
                    if (attacker.GetCurentFigure().rangedAmmo == 0)
                    {
                        BattleHUD.CombatLogFinishStack(battleStack2);
                        yield break;
                    }
                    attacker.GetCurentFigure().rangedAmmo--;
                    shooting = true;
                    attacker.GetOrCreateFormation()?.AttackRanged(defender.GetPosition(), defender, delegate(object o)
                    {
                        ca = o as CharacterActor;
                        shooting = false;
                    });
                    softLockStop2 = 10f;
                    while (shooting && softLockStop2 > 0f)
                    {
                        softLockStop2 -= Time.deltaTime;
                        yield return null;
                    }
                    defender.GetOrCreateFormation()?.UpdateFigureCount();
                    defender.GetOrCreateFormation()?.GetHit();
                }
                BattleHUD.CombatLogFinishStack(battleStack2);
                break;
            }
            }
            while (true)
            {
                int num = 0;
                if (defender == null || defender.battleFormation == null || !defender.battleFormation.IsAnimating())
                {
                    num++;
                }
                if (attacker == null || attacker.battleFormation == null || !attacker.battleFormation.IsAnimating())
                {
                    num++;
                }
                if (num != 2)
                {
                    yield return null;
                    continue;
                }
                break;
            }
        }

        public int HumanBattleResult()
        {
            if (this.attacker.playerOwner)
            {
                if (this.battleWinner == BattleWinner.ATTACKER_WINS)
                {
                    return 1;
                }
                if (this.battleWinner == BattleWinner.DEFENDER_WINS)
                {
                    return -1;
                }
                return 0;
            }
            if (this.defender.playerOwner)
            {
                if (this.battleWinner == BattleWinner.ATTACKER_WINS)
                {
                    return -1;
                }
                if (this.battleWinner == BattleWinner.DEFENDER_WINS)
                {
                    return 1;
                }
                return 0;
            }
            return 0;
        }

        public EnchantmentManager GetEnchantmentManager()
        {
            if (this.enchantmentManager == null)
            {
                this.enchantmentManager = new EnchantmentManager(this);
            }
            return this.enchantmentManager;
        }

        public PlayerWizard GetWizardOwner()
        {
            return null;
        }

        public static Battle Get()
        {
            return Battle.GetBattle();
        }

        public static void UpdateUnitPosition(BattleUnit bu, Vector3i from, Vector3i to)
        {
            Battle battle = Battle.Get();
            if (battle != null && battle.buToSource != null && battle.buToSource.ContainsKey(bu))
            {
                battle.plane.UpdateUnitPosition(from, to, bu);
            }
        }

        public int GetStrategicValue(bool attacker)
        {
            List<BattleUnit> list = (attacker ? this.attackerUnits : this.defenderUnits);
            if (list == null)
            {
                return 0;
            }
            int num = 0;
            List<int> list2 = new List<int>();
            foreach (BattleUnit item in list)
            {
                list2.Add(item.GetBattleUnitValue());
            }
            list2.Sort();
            for (int num2 = list2.Count - 1; num2 >= 0; num2--)
            {
                int num3 = list2[num2] + list2[num2] / (num2 + 1);
                num += num3 / 2;
            }
            return num;
        }

        public static void PrepareChanges(Battle b)
        {
            b.WinnerResult();
            b.ApplyChanges();
        }

        public static void ApplyBattleChanges(Battle b)
        {
            if (b.gAttacker != null)
            {
                b.gAttacker.UpdateMapFormation(createIfMissing: false);
            }
            if (b.gDefender != null)
            {
                b.gDefender.UpdateMapFormation(createIfMissing: false);
            }
            if (b.gDefender.GetLocationHostSmart() is TownLocation && b.HumanBattleResult() != 0 && !b.playerIsAttacker)
            {
                Debug.Log("AI decide to capture or raze this town by repeating entry");
            }
            if (b.defender.surrendered && b.gAttacker != null && b.gDefender != null)
            {
                Vector3i position = b.gAttacker.GetPosition();
                Vector3i position2 = b.gDefender.GetPosition();
                if (b.battleWinner == BattleWinner.ATTACKER_WINS && b.gAttacker.alive && HexCoordinates.HexDistance(position, position2) == 1 && b.gDefender.alive && b.gDefender.GetUnits().Count > 0)
                {
                    Group groupD = b.gDefender;
                    GameManager.Get().AddFocusCallback(delegate
                    {
                        if (groupD != null && groupD.alive)
                        {
                            groupD.ForcedToMoveOut();
                        }
                    });
                }
            }
            b.Destroy();
        }

        private void ApplyChanges()
        {
            int num = 0;
            int num2 = 0;
            foreach (KeyValuePair<BattleUnit, Unit> item in this.buToSource)
            {
                BattleUnit key = item.Key;
                if (!key.IsAlive())
                {
                    if (key.attackingSide)
                    {
                        num++;
                    }
                    else
                    {
                        num2++;
                    }
                }
            }
            if (this.buToSource == null)
            {
                return;
            }
            PlayerWizard wizard = this.attacker.wizard;
            PlayerWizard wizard2 = this.defender.wizard;
            bool flag = wizard != null && wizard2 != null;
            DiplomacyManager diplomacyManager = null;
            DiplomacyManager diplomacyManager2 = null;
            if (flag)
            {
                diplomacyManager = wizard.GetDiplomacy();
                diplomacyManager2 = wizard2.GetDiplomacy();
            }
            int num3 = 0;
            int num4 = 0;
            PlayerWizard playerWizard = null;
            PlayerWizard playerWizard2 = null;
            if (this.battleWinner == BattleWinner.ATTACKER_WINS)
            {
                playerWizard2 = this.attacker.wizard;
                playerWizard = this.defender.wizard;
            }
            if (this.battleWinner == BattleWinner.DEFENDER_WINS)
            {
                playerWizard2 = this.defender.wizard;
                playerWizard = this.attacker.wizard;
            }
            Group group = null;
            if (playerWizard2 != null)
            {
                group = ((playerWizard2.ID == this.attacker.GetID()) ? this.gAttacker : this.gDefender);
            }
            HashSet<BattleUnit> hashSet = null;
            foreach (KeyValuePair<BattleUnit, Unit> item2 in this.buToSource)
            {
                if (!item2.Key.summon && (item2.Key.isAnimatedInSimulation || item2.Value.group == null || item2.Key.isHopingToJoin))
                {
                    if (hashSet == null)
                    {
                        hashSet = new HashSet<BattleUnit>();
                    }
                    hashSet.Add(item2.Key);
                }
            }
            for (int i = 0; i < 2 && (i <= 0 || hashSet != null); i++)
            {
                foreach (KeyValuePair<BattleUnit, Unit> item3 in this.buToSource)
                {
                    if (hashSet != null && ((i == 0 && hashSet.Contains(item3.Key)) || (i > 0 && !hashSet.Contains(item3.Key))))
                    {
                        continue;
                    }
                    Battle.TryToRegenerate(this, item3.Key);
                    bool flag2 = false;
                    if (item3.Key.isAnimatedInSimulation)
                    {
                        Group group2 = null;
                        if (item3.Key.attackingSide && this.gAttacker != null && this.gAttacker.alive && this.gAttacker.GetUnits().Count < 9)
                        {
                            group2 = this.gAttacker;
                        }
                        else if (!item3.Key.attackingSide && this.gDefender != null && this.gDefender.alive && this.gDefender.GetUnits().Count < 9)
                        {
                            group2 = this.gDefender;
                        }
                        Group group3 = item3.Value.group;
                        if (group2 != null && (group2 == group3 || group2.GetUnits().Count < 9))
                        {
                            group3?.RemoveUnit(item3.Value);
                            group2?.AddUnit(item3.Value);
                            object obj = ScriptLibrary.Call("AnimateDead", item3.Value, null, null);
                            if (item3.Value != obj)
                            {
                                item3.Value.Destroy();
                                group2.AddUnit(item3.Value);
                                continue;
                            }
                        }
                        else
                        {
                            item3.Key.figureCount = 0;
                        }
                    }
                    if (item3.Key.FigureCount() > 0 && !item3.Key.summon && ((item3.Key.attackingSide && item3.Value.group != this.gAttacker) || (!item3.Key.attackingSide && item3.Value.group != this.gDefender)))
                    {
                        if (item3.Key.attackingSide && this.gAttacker != null && this.gAttacker.alive && this.gAttacker.GetUnits().Count < 9 && item3.Value.group != this.gAttacker)
                        {
                            this.gAttacker.AddUnit(item3.Value);
                        }
                        else if (!item3.Key.attackingSide && this.gDefender != null && this.gDefender.alive && this.gDefender.GetUnits().Count < 9 && item3.Value.group != this.gDefender)
                        {
                            this.gDefender.AddUnit(item3.Value);
                        }
                        else
                        {
                            item3.Key.figureCount = 0;
                        }
                    }
                    PlayerWizard wizardOwner = item3.Value.GetWizardOwner();
                    if (item3.Key.FigureCount() == 0 || item3.Key.summon)
                    {
                        item3.Value.Destroy();
                        flag2 = true;
                        if (group != null && group.alive)
                        {
                            Battle.TryToReanimateAsUndead(this, item3.Key, item3.Value, playerWizard2, group);
                        }
                    }
                    List<EnchantmentInstance> enchantments = item3.Value.GetEnchantments();
                    for (int num5 = enchantments.Count - 1; num5 >= 0; num5--)
                    {
                        if (!EnchantmentRegister.IsStillValid(enchantments[num5]))
                        {
                            item3.Value.RemoveEnchantment(enchantments[num5]);
                        }
                    }
                    if (!flag2)
                    {
                        item3.Value.UpdateFrom(item3.Key);
                        if (item3.Value.GetTotalHpPercent() == 0f)
                        {
                            flag2 = true;
                        }
                    }
                    if (!flag2 && item3.Value.GetAttFinal((Tag)TAG.DEATH_EATER) > 0)
                    {
                        item3.Key.HealUnit(item3.Key.GetMaxFigureCount() * item3.Key.GetCurentFigure().maxHitPoints / 2, ignoreCanNaturalHeal: true);
                        item3.Value.UpdateFrom(item3.Key);
                    }
                    if (flag2)
                    {
                        Unit value = item3.Value;
                        if (wizardOwner != null && wizardOwner == playerWizard)
                        {
                            if (value.dbSource.Get() is Hero)
                            {
                                int num6 = value.GetLevel() / 2;
                                wizardOwner.TakeFame(num6);
                                this.loserFame -= num6;
                            }
                            num3++;
                            if (value.GetAttFinal(TAG.RARE_CLASS) > 0)
                            {
                                num4++;
                            }
                        }
                        if (wizardOwner == null)
                        {
                            num3++;
                            if (value.GetAttFinal(TAG.RARE_CLASS) > 0)
                            {
                                num4++;
                            }
                        }
                        if (flag)
                        {
                            if (wizardOwner == wizard)
                            {
                                diplomacyManager2.KilledUnitOf(value, wizard);
                                diplomacyManager.LostUnitBy(value, wizard2);
                            }
                            else
                            {
                                diplomacyManager.KilledUnitOf(value, wizard2);
                                diplomacyManager2.LostUnitBy(value, wizard);
                            }
                        }
                    }
                    else if (item3.Value.canGainXP)
                    {
                        if (item3.Key.attackingSide)
                        {
                            item3.Value.xp += num2 * 2;
                        }
                        else
                        {
                            item3.Value.xp += num * 2;
                        }
                    }
                }
            }
            if (num3 >= 4)
            {
                playerWizard2?.AddFame(1);
                playerWizard?.TakeFame(1);
                this.loserFame--;
                this.winnerFame++;
            }
            if (num4 >= 1)
            {
                playerWizard2?.AddFame(1);
                playerWizard?.TakeFame(1);
                this.loserFame--;
                this.winnerFame++;
            }
            if (this.gAttacker.virtualGroup)
            {
                this.gAttacker.Destroy();
            }
            if (this.gDefender.virtualGroup)
            {
                this.gDefender.Destroy();
            }
        }

        private static void TryToRegenerate(Battle b, BattleUnit bu)
        {
            if (((b.battleWinner == BattleWinner.ATTACKER_WINS && bu.attackingSide) || (b.battleWinner == BattleWinner.DEFENDER_WINS && !bu.attackingSide)) && bu.GetAttributes().Contains(TAG.REGENERATION) && bu.irreversibleDamages < bu.GetMaxTotalHp())
            {
                bu.Regeneration(postBattle: true);
            }
        }

        private static void TryToReanimateAsUndead(Battle b, BattleUnit bu, Unit deadUnit, PlayerWizard winner, Group group)
        {
            if (bu.dbSource.Get() is Hero)
            {
                return;
            }
            if (((b.battleWinner == BattleWinner.ATTACKER_WINS && b.defenderUnits.Contains(bu)) || (b.battleWinner == BattleWinner.DEFENDER_WINS && b.attackerUnits.Contains(bu))) && bu.CanReanimateAsUndead())
            {
                Unit unit = ((deadUnit.dbSource == (Subrace)UNIT.LIF_ARCH_ANGEL) ? Unit.CreateFrom((Subrace)UNIT.DTH_ARCH_ANGEL) : ((deadUnit.dbSource == (Subrace)UNIT.CHA_EFREET) ? Unit.CreateFrom((Subrace)UNIT.DTH_EFREET) : ((!(deadUnit.dbSource == (Subrace)UNIT.SOR_DJINN)) ? Unit.CreateFrom(deadUnit.dbSource) : Unit.CreateFrom((Subrace)UNIT.DTH_DJINN))));
                deadUnit.CopySkillManager(unit);
                unit.xp = deadUnit.xp;
                unit.AddEnchantment((Enchantment)ENCH.REANIMATE_UNDEAD, null);
                unit.race = (Race)RACE.REALM_DEATH;
                List<DBReference<Skill>> skills = unit.GetSkills();
                if (!skills.Contains((Skill)SKILL.COLD_IMMUNITY))
                {
                    unit.AddSkill((Skill)SKILL.COLD_IMMUNITY);
                }
                if (!skills.Contains((Skill)SKILL.POISON_IMMUNITY))
                {
                    unit.AddSkill((Skill)SKILL.POISON_IMMUNITY);
                }
                if (!skills.Contains((Skill)SKILL.ILLUSIONS_IMMUNITY))
                {
                    unit.AddSkill((Skill)SKILL.ILLUSIONS_IMMUNITY);
                }
                if (!skills.Contains((Skill)SKILL.DEATH_IMMUNITY))
                {
                    unit.AddSkill((Skill)SKILL.DEATH_IMMUNITY);
                }
                if (unit.GetAttributes().GetFinal(TAG.SETTLER_UNIT) > 0)
                {
                    unit.AddSkill((Skill)SKILL.REANIMATED_SETTLER);
                }
                if (unit.GetAttributes().Contains(TAG.NORMAL_CLASS))
                {
                    unit.GetAttributes().SetBaseTo(TAG.NORMAL_CLASS, FInt.ZERO);
                    unit.GetAttributes().SetBaseTo(TAG.FANTASTIC_CLASS, FInt.ONE);
                    unit.GetAttributes().SetBaseTo(TAG.UPKEEP_FOOD, FInt.ZERO);
                    unit.GetAttributes().SetBaseTo(TAG.UPKEEP_GOLD, FInt.ZERO);
                    unit.EnsureEnchantments();
                }
                unit.canGainXP = false;
                group.AddUnit(unit);
                unit.canNaturalHeal = false;
                if (winner == GameManager.GetHumanWizard())
                {
                    winner.AddNotification(new SummaryInfo
                    {
                        summaryType = SummaryInfo.SummaryType.eRaisedUndead,
                        unit = unit,
                        graphic = unit.GetDescriptionInfo().graphic
                    });
                }
            }
            else
            {
                if (winner.GetEnchantments().Find((EnchantmentInstance o) => o.source == (Enchantment)ENCH.ZOMBIE_MASTERY) == null || bu.GetAttFinal((Tag)TAG.FANTASTIC_CLASS) > 0)
                {
                    return;
                }
                Unit unit2 = Unit.CreateFrom((Subrace)UNIT.DTH_ZOMBIES);
                List<DBReference<Skill>> list = deadUnit.GetSkills().FindAll((DBReference<Skill> o) => o.Get() == (Skill)SKILL.ENCHANTED_WEAPON2 || o.Get() == (Skill)SKILL.ENCHANTED_WEAPON3 || o.Get() == (Skill)SKILL.ENCHANTED_WEAPON4);
                if ((list != null) & (list.Count > 0))
                {
                    foreach (DBReference<Skill> item in list)
                    {
                        unit2.AddSkill(item);
                    }
                }
                group.AddUnit(unit2);
            }
        }

        private void WinnerResult()
        {
            int num = 0;
            int num2 = 0;
            foreach (BattleUnit item in ListUtils.MultiEnumerable(this.attackerUnits, this.defenderUnits))
            {
                if (item.attackingSide && item.IsAlive() && !item.summon)
                {
                    num2++;
                }
                else if (!item.attackingSide && item.IsAlive() && !item.summon)
                {
                    num++;
                }
            }
            if ((num2 > 0 && num > 0) || (num2 == 0 && num == 0))
            {
                this.battleWinner = BattleWinner.ROW;
            }
            else if (num2 == 0)
            {
                this.battleWinner = BattleWinner.DEFENDER_WINS;
            }
            else
            {
                this.battleWinner = BattleWinner.ATTACKER_WINS;
            }
        }

        public string GetName()
        {
            return global::DBUtils.Localization.Get("UI_Global", true);
        }

        public void UnitListsDirty()
        {
            this.cachedAllUnits = null;
        }

        public List<BattleUnit> GetAllUnits()
        {
            if (this.cachedAllUnits == null)
            {
                this.cachedAllUnits = new List<BattleUnit>(this.attackerUnits);
                this.cachedAllUnits.AddRange(this.defenderUnits);
            }
            return this.cachedAllUnits;
        }

        public void UpdateInvisibility()
        {
            bool flag = this.attackerUnits.Find((BattleUnit o) => o.IsAlive() && o.GetAttributes().GetFinal(TAG.ILLUSIONS_IMMUNITY) > 0) != null;
            bool flag2 = this.defenderUnits.Find((BattleUnit o) => o.IsAlive() && o.GetAttributes().GetFinal(TAG.ILLUSIONS_IMMUNITY) > 0) != null;
            foreach (BattleUnit bu2 in this.attackerUnits)
            {
                if (bu2.IsInvisibleUnit())
                {
                    if (flag2)
                    {
                        bu2.currentlyVisible = true;
                    }
                    else if (this.defenderUnits.Find((BattleUnit o) => o.IsAlive() && HexCoordinates.HexDistance(o.GetPosition(), bu2.GetPosition()) == 1) != null)
                    {
                        bu2.currentlyVisible = true;
                    }
                    else
                    {
                        bu2.currentlyVisible = false;
                    }
                }
                else
                {
                    bu2.currentlyVisible = true;
                }
            }
            foreach (BattleUnit bu in this.defenderUnits)
            {
                if (bu.IsInvisibleUnit())
                {
                    if (flag)
                    {
                        bu.currentlyVisible = true;
                    }
                    else if (this.attackerUnits.Find((BattleUnit o) => o.IsAlive() && HexCoordinates.HexDistance(o.GetPosition(), bu.GetPosition()) == 1) != null)
                    {
                        bu.currentlyVisible = true;
                    }
                    else
                    {
                        bu.currentlyVisible = false;
                    }
                }
                else
                {
                    bu.currentlyVisible = true;
                }
            }
        }

        public List<GameObject> AddWalls(string postfix)
        {
            List<GameObject> list = new List<GameObject>();
            GameObject gameObject = null;
            if (postfix != "_Fire")
            {
                gameObject = this.AddWall(position: new Vector3i(-3, 3, 0), name: "Gate" + postfix, side: 0, physicalWall: string.IsNullOrEmpty(postfix));
                if (gameObject != null)
                {
                    list.Add(gameObject);
                }
            }
            gameObject = this.AddWall(position: new Vector3i(0, 3, -3), name: "WallTurnR" + postfix, side: 0, physicalWall: string.IsNullOrEmpty(postfix));
            if (gameObject != null)
            {
                list.Add(gameObject);
            }
            gameObject = this.AddWall(position: new Vector3i(-6, 3, 3), name: "WallTurnL" + postfix, side: 0, physicalWall: string.IsNullOrEmpty(postfix));
            if (gameObject != null)
            {
                list.Add(gameObject);
            }
            gameObject = this.AddWall(position: new Vector3i(-1, 3, -2), name: "WallStraightR" + postfix, side: 0, physicalWall: string.IsNullOrEmpty(postfix));
            if (gameObject != null)
            {
                list.Add(gameObject);
            }
            gameObject = this.AddWall(position: new Vector3i(-2, 3, -1), name: "WallStraightTowerR" + postfix, side: 0, physicalWall: string.IsNullOrEmpty(postfix));
            if (gameObject != null)
            {
                list.Add(gameObject);
            }
            gameObject = this.AddWall(position: new Vector3i(-4, 3, 1), name: "WallStraightTowerL" + postfix, side: 0, physicalWall: string.IsNullOrEmpty(postfix));
            if (gameObject != null)
            {
                list.Add(gameObject);
            }
            gameObject = this.AddWall(position: new Vector3i(-5, 3, 2), name: "WallStraightL" + postfix, side: 0, physicalWall: string.IsNullOrEmpty(postfix));
            if (gameObject != null)
            {
                list.Add(gameObject);
            }
            for (int i = 4; i < 13; i++)
            {
                int num = 0;
                Vector3i position8 = new Vector3i(num, i, -num - i);
                if ((i - 4) % 3 == 2)
                {
                    gameObject = this.AddWall("WallStraightTowerR" + postfix, position8, 1, string.IsNullOrEmpty(postfix));
                    if (gameObject != null)
                    {
                        list.Add(gameObject);
                    }
                }
                else
                {
                    gameObject = this.AddWall("WallStraightR" + postfix, position8, 1, string.IsNullOrEmpty(postfix));
                    if (gameObject != null)
                    {
                        list.Add(gameObject);
                    }
                }
            }
            for (int num2 = -7; num2 > -16; num2--)
            {
                int num3 = 3;
                Vector3i position8 = new Vector3i(num2, -num2 - num3, num3);
                if ((Mathf.Abs(num2) - 7) % 3 == 2)
                {
                    gameObject = this.AddWall("WallStraightTowerL" + postfix, position8, -1, string.IsNullOrEmpty(postfix));
                    if (gameObject != null)
                    {
                        list.Add(gameObject);
                    }
                }
                else
                {
                    gameObject = this.AddWall("WallStraightL" + postfix, position8, -1, string.IsNullOrEmpty(postfix));
                    if (gameObject != null)
                    {
                        list.Add(gameObject);
                    }
                }
            }
            return list;
        }

        public void AddMageTower()
        {
            Vector3i vector3i = new Vector3i(-7, 8, -1);
            GameObject gameObject = AssetManager.Get<GameObject>("Fortress");
            if (gameObject == null)
            {
                Debug.LogError("Model 'Fortress' is missing!");
            }
            Chunk chunkFor = this.plane.GetChunkFor(vector3i);
            GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, chunkFor.go.transform);
            if (!(gameObject2 == null))
            {
                MHZombieMemoryDetector.Track(gameObject2);
                this.SetHexModelPosition(gameObject2, vector3i);
                this.wizardTower = gameObject2;
                global::WorldCode.Plane plane = this.plane;
                if (plane.exclusionPoints == null)
                {
                    plane.exclusionPoints = new HashSet<Vector3i>();
                }
                this.plane.exclusionPoints.Add(vector3i);
            }
        }

        private GameObject AddWall(string name, Vector3i position, int side, bool physicalWall)
        {
            if (!this.plane.GetHexes().ContainsKey(position))
            {
                return null;
            }
            BattleWall battleWall = null;
            if (physicalWall)
            {
                battleWall = new BattleWall();
                battleWall.position = position;
                battleWall.standing = true;
                switch (side)
                {
                case -1:
                    battleWall.defenceNormal = HexCoordinates.HexToWorld3D(new Vector3i(-1, 0, 1)).normalized;
                    break;
                case 0:
                    battleWall.defenceNormal = HexCoordinates.HexToWorld3D(new Vector3i(1, -1, 0)).normalized;
                    break;
                case 1:
                    battleWall.defenceNormal = HexCoordinates.HexToWorld3D(new Vector3i(0, 1, -1)).normalized;
                    break;
                default:
                    Debug.LogError("unsupported wall direction");
                    break;
                }
                this.battleWalls.Add(battleWall);
                if (this.battleWalls.Count == 1)
                {
                    battleWall.gate = true;
                }
            }
            Chunk chunkFor = this.plane.GetChunkFor(position);
            GameObject gameObject = AssetManager.Get<GameObject>(name);
            if (gameObject == null)
            {
                Debug.LogError("Model " + name + " is missing!");
                return null;
            }
            GameObject gameObject2 = global::UnityEngine.Object.Instantiate(gameObject, chunkFor.go.transform);
            if (gameObject2 == null)
            {
                return null;
            }
            MHZombieMemoryDetector.Track(gameObject2);
            gameObject2.transform.localRotation = Quaternion.Euler(Vector3.up * (180 - 60 * side));
            if (battleWall != null)
            {
                battleWall.mapModel = gameObject2;
            }
            this.SetHexModelPosition(gameObject2, position);
            return gameObject2;
        }

        public void SetHexModelPosition(GameObject model, Vector3i position)
        {
            Vector3 vector = HexCoordinates.HexToWorld3D(position);
            global::WorldCode.Plane plane = this.plane;
            Chunk chunkFor = plane.GetChunkFor(position);
            model.transform.parent = chunkFor.go.transform;
            if (model.transform.childCount < 1)
            {
                return;
            }
            model.transform.position = chunkFor.go.transform.position + vector;
            List<GameObject> list = null;
            foreach (Transform item in model.transform)
            {
                Vector3 position2 = item.position;
                position2.y = plane.GetHeightAt(position2, allowUnderwater: true);
                GroundOffset component = item.gameObject.GetComponent<GroundOffset>();
                if (component != null)
                {
                    position2.y += component.heightOffset;
                }
                item.position = position2;
            }
            if (list == null)
            {
                return;
            }
            foreach (GameObject item2 in list)
            {
                global::UnityEngine.Object.Destroy(item2);
            }
        }

        public HashSet<Vector3i> GetOutsideWalls()
        {
            if (Battle.outsideWallLine == null)
            {
                Battle.outsideWallLine = new HashSet<Vector3i>();
                for (int num = 0; num >= -5; num--)
                {
                    int num2 = 2;
                    Battle.outsideWallLine.Add(new Vector3i(num, num2, -num2 - num));
                }
                for (int i = 2; i < 10; i++)
                {
                    int num3 = 1;
                    Battle.outsideWallLine.Add(new Vector3i(num3, i, -num3 - i));
                }
                for (int num4 = -6; num4 > -14; num4--)
                {
                    int num5 = 4;
                    Battle.outsideWallLine.Add(new Vector3i(num4, -num4 - num5, num5));
                }
            }
            return Battle.outsideWallLine;
        }

        public HashSet<Vector3i> GetFireWalled()
        {
            if (Battle.fireWalled == null)
            {
                Battle.fireWalled = new HashSet<Vector3i>();
                for (int num = 0; num >= -6; num--)
                {
                    if (num != -3)
                    {
                        int num2 = 3;
                        Battle.fireWalled.Add(new Vector3i(num, num2, -num - num2));
                    }
                }
                for (int i = 4; i < 13; i++)
                {
                    int num3 = 0;
                    Battle.fireWalled.Add(new Vector3i(num3, i, -num3 - i));
                }
                for (int num4 = -7; num4 > -16; num4--)
                {
                    int num5 = 3;
                    Battle.fireWalled.Add(new Vector3i(num4, -num4 - num5, num5));
                }
            }
            return Battle.fireWalled;
        }

        public bool StepThroughFirewall(Vector3i a, Vector3i b)
        {
            if ((this.GetFireWalled().Contains(a) && this.GetOutsideWalls().Contains(b)) || (this.GetFireWalled().Contains(b) && this.GetOutsideWalls().Contains(a)))
            {
                return true;
            }
            return false;
        }

        public bool IsInsideTownArea(Vector3i v)
        {
            if (v.x < 1 && v.y > 2)
            {
                return v.z < 4;
            }
            return false;
        }

        public bool AttactThroughWall(Vector3i a, Vector3i b, bool darkness = false)
        {
            if ((this.battleWalls == null || this.battleWalls.Count == 0) && !darkness)
            {
                return false;
            }
            bool flag = this.IsInsideTownArea(b);
            if (!flag || this.IsInsideTownArea(a) == flag)
            {
                return false;
            }
            if (darkness)
            {
                return true;
            }
            foreach (Vector3i v in PathfinderV2.GetDirectLine(a, b))
            {
                BattleWall battleWall = this.battleWalls.Find((BattleWall o) => o.position == v);
                if (battleWall != null && battleWall.standing)
                {
                    return true;
                }
            }
            return false;
        }

        public void ResistedSpell(Vector3i position, bool counterMagickedSpell)
        {
            FSMBattleTurn.instance?.CastFailedEffect(position, counterMagickedSpell);
        }

        public static void CastBattleSpell(Spell spell, SpellCastData castData, object target, bool simulated = false)
        {
            if (Battle.GetBattle() == null || FSMBattleTurn.instance == null)
            {
                return;
            }
            string text = null;
            string text2 = null;
            if (target != null && target is BattleUnit)
            {
                BattleUnit battleUnit = target as BattleUnit;
                if (!simulated)
                {
                    text = battleUnit.GetFastChangeDetector();
                }
            }
            ScriptLibrary.Call(spell.battleScript, castData, target, spell);
            if (target != null && target is BattleUnit)
            {
                BattleUnit battleUnit2 = target as BattleUnit;
                if (!simulated)
                {
                    text2 = battleUnit2.GetFastChangeDetector();
                }
                if (!simulated && text2 == text)
                {
                    Battle.GetBattle()?.ResistedSpell(battleUnit2.GetPosition(), counterMagickedSpell: false);
                }
            }
        }

        private void UnitDie(BattleUnit bu)
        {
            bu.PlayDeadAnimUI();
            this.TriggerLeaveScripts(bu);
            bu.Mp = FInt.ZERO;
        }

        public void TriggerLeaveScripts(BattleUnit bu)
        {
            List<BattleUnit> list = (bu.attackingSide ? this.attackerUnits : this.defenderUnits);
            list = list.FindAll((BattleUnit o) => o.IsAlive());
            foreach (BattleUnit item in list)
            {
                item.GetEnchantmentManager().OnLeaveTriggers(bu, null);
                bu.GetEnchantmentManager().OnLeaveTriggers(item, list);
                item.GetSkillManager().OnLeaveTriggers(bu, null);
                bu.GetSkillManager().OnLeaveTriggers(item, list);
            }
        }

        public void TriggerJoinScripts(BattleUnit bu)
        {
            List<BattleUnit> list = (bu.attackingSide ? this.attackerUnits : this.defenderUnits);
            list = list.FindAll((BattleUnit o) => o.IsAlive());
            foreach (BattleUnit item in list)
            {
                if (item == bu)
                {
                    item.GetEnchantmentManager().OnJoinTriggers(item, list);
                    item.GetSkillManager().OnJoinTriggers(item, list);
                    continue;
                }
                item.GetEnchantmentManager().OnJoinTriggers(bu, list);
                bu.GetEnchantmentManager().OnJoinTriggers(item, list);
                item.GetSkillManager().OnJoinTriggers(bu, list);
                bu.GetSkillManager().OnJoinTriggers(item, list);
            }
        }

        public void AttackerAddUnit(BattleUnit bu)
        {
            this.attackerUnits.Add(bu);
            this.TriggerJoinScripts(bu);
            this.UpdateInvisibility();
        }

        public void AttackerRemoveUnit(BattleUnit bu)
        {
            if (this.attackerUnits.Contains(bu))
            {
                this.attackerUnits.Remove(bu);
                this.TriggerLeaveScripts(bu);
            }
        }

        public void DefenderAddUnit(BattleUnit bu)
        {
            this.defenderUnits.Add(bu);
            this.TriggerJoinScripts(bu);
            this.UpdateInvisibility();
        }

        public void DefenderRemoveUnit(BattleUnit bu)
        {
            if (this.defenderUnits.Contains(bu))
            {
                this.defenderUnits.Remove(bu);
                this.TriggerLeaveScripts(bu);
            }
        }

        public void FinishedIteratingEnchantments()
        {
        }

        public IEnumerator AdditionalShootingAttack(BattleUnit attacker, BattleUnit defender, BattleAttackStack battleStack)
        {
            if (!attacker.doubleShot || !attacker.canAttack)
            {
                yield break;
            }
            if (attacker.GetCurentFigure().rangedAmmo == 0)
            {
                BattleHUD.CombatLogFinishStack(battleStack);
                yield break;
            }
            attacker.GetCurentFigure().rangedAmmo--;
            bool shooting = true;
            attacker.GetOrCreateFormation()?.AttackRanged(defender.GetPosition(), defender, delegate
            {
                shooting = false;
            });
            float softLockStop = 10f;
            while (shooting && softLockStop > 0f)
            {
                softLockStop -= Time.deltaTime;
                yield return null;
            }
            defender.GetOrCreateFormation()?.UpdateFigureCount();
            defender.GetOrCreateFormation()?.GetHit();
        }
    }
}
