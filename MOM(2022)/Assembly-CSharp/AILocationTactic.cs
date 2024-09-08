// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AILocationTactic
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class AILocationTactic
{
    public enum LocationDesignation
    {
        None = 0,
        Economic = 1,
        Military = 2,
        SeaFaring = 3,
        MAX = 4
    }

    [ProtoMember(1)]
    public Reference<global::MOM.Location> owner;

    [ProtoMember(2)]
    public int strategicValue;

    [ProtoMember(3)]
    public int danger;

    [ProtoMember(4)]
    public int regionalForces;

    [ProtoMember(5)]
    public int dangerRank;

    [ProtoMember(10)]
    public bool wishToExtractSettlers;

    [ProtoMember(11)]
    public bool wishToExtractEngineers;

    [ProtoMember(12)]
    public int nextTurnToConsiderSettler;

    [ProtoMember(13)]
    public int nextTurnToConsiderEngineer;

    [ProtoMember(14)]
    public bool locationExpectsSettler;

    [ProtoMember(15)]
    public bool locationExpectsEngineer;

    [ProtoMember(16)]
    public List<Vector2i> failedHelpRequests;

    [ProtoMember(20)]
    public int needToBuildArmy;

    [ProtoMember(21)]
    public int needToBuildEconomy;

    [ProtoMember(22)]
    public bool needArmyFromUnit;

    [ProtoMember(23)]
    public int needToBuildTransport;

    [ProtoMember(24)]
    public bool militaryAdvancementRequired;

    [ProtoMember(25)]
    public LocationDesignation locationDesignation;

    [ProtoMember(26)]
    public int militaryUpgradeTimer;

    [ProtoIgnore]
    private global::WorldCode.Plane plane;

    [ProtoIgnore]
    private global::MOM.Group group;

    [ProtoIgnore]
    private PlayerWizard wizard;

    public AILocationTactic(global::MOM.Location l)
    {
        this.owner = l;
        this.nextTurnToConsiderSettler = TurnManager.GetTurnNumber() + global::UnityEngine.Random.Range(0, 10);
    }

    public void TurnPreparation()
    {
        this.EstimateStrategicValue();
        this.EstimateDanger();
    }

    private void EstimateStrategicValue()
    {
        this.strategicValue = 0;
        if (this.owner.Get() is TownLocation)
        {
            TownLocation townLocation = this.owner.Get() as TownLocation;
            this.strategicValue = townLocation.Population;
            if (townLocation.buildings != null)
            {
                foreach (DBReference<Building> building in townLocation.buildings)
                {
                    this.strategicValue += building.Get().buildCost * 40;
                }
            }
            List<EnchantmentInstance> enchantments = townLocation.GetEnchantments();
            if (enchantments == null)
            {
                return;
            }
            {
                foreach (EnchantmentInstance item in enchantments)
                {
                    if (!(item.owner != null))
                    {
                        continue;
                    }
                    PlayerWizard playerWizard = item.owner.Get<PlayerWizard>();
                    if (playerWizard != null)
                    {
                        if (playerWizard.ID == townLocation.GetOwnerID())
                        {
                            this.strategicValue += item.upkeepMana * 500;
                        }
                        else
                        {
                            this.strategicValue -= item.upkeepMana * 500;
                        }
                    }
                }
                return;
            }
        }
        int iD = this.GetOwnerWizard().GetID();
        List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.GetPlane());
        int num = 15;
        foreach (global::MOM.Location item2 in locationsOfThePlane)
        {
            if (item2 is TownLocation && item2.GetOwnerID() == iD)
            {
                int num2 = HexCoordinates.HexDistance(item2.GetPosition(), this.GetPosition());
                if (num2 < num)
                {
                    num = num2;
                }
            }
        }
        this.strategicValue = Mathf.RoundToInt(1500f * (1f - (float)num / 15f));
    }

    private void EstimateDanger()
    {
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.owner.Get().GetPlane());
        int ownerID = this.owner.Get().GetOwnerID();
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        PlayerWizardAI ownerWizard = this.GetOwnerWizard();
        foreach (global::MOM.Group item in groupsOfPlane)
        {
            if (item.GetOwnerID() == ownerID || (item.GetLocationHostSmart() != null && item.GetOwnerID() == 0))
            {
                continue;
            }
            int num4 = this.Distance(item.GetPosition(), this.GetPosition());
            if (num4 >= 8)
            {
                continue;
            }
            int num5 = item.GetValue();
            if (!ownerWizard.GetDiplomacy().IsAtWarWith(item.GetOwnerID()))
            {
                num5 = (int)((float)num5 * 0.6f);
                if (ownerWizard.GetDiplomacy().IsAlliedWith(item.GetOwnerID()))
                {
                    num5 = 10;
                }
            }
            if (num4 > 4)
            {
                num5 = num5 * 2 / 3;
            }
            if (num < num5)
            {
                num = num5;
            }
            num2 += num5;
            num3++;
        }
        if (num3 == 0)
        {
            this.danger = 0;
            return;
        }
        this.danger = Mathf.Max(num, (num + num2) / 2);
        int value = this.GetGroup().GetValue();
        float num6 = (float)this.danger / (1f + (float)value);
        if (num6 < 0.2f)
        {
            this.dangerRank = 0;
        }
        else if (num6 < 0.8f)
        {
            this.dangerRank = 1;
        }
        else if (num6 < 1.5f)
        {
            this.dangerRank = 2;
        }
        else
        {
            this.dangerRank = 3;
        }
    }

    public IEnumerator ResolveTurn()
    {
        this.needToBuildArmy = 0;
        this.needToBuildEconomy = 0;
        this.needToBuildTransport = 0;
        this.regionalForces = 0;
        if (this.danger > 0)
        {
            this.ProcessDanger();
        }
        if (this.IsTown() && this.GetAsTown().GetPopUnits() >= 1)
        {
            this.PlanNeeds();
            yield return this.ActOnNeeds();
        }
        yield return this.PowerNodeTurnResolve();
        this.CreateExpeditionsIfPossible();
        this.needArmyFromUnit = false;
    }

    private IEnumerator PowerNodeTurnResolve()
    {
        global::MOM.Location location = this.owner.Get();
        if (location.power <= 0 || (location.melding != null && location.melding.meldOwner == this.GetOwnerWizard()?.GetID()))
        {
            yield break;
        }
        if (location.GetUnits() != null)
        {
            global::MOM.Unit unit = null;
            foreach (Reference<global::MOM.Unit> unit3 in location.GetUnits())
            {
                if (unit3.Get().IsMelder())
                {
                    unit = unit3.Get();
                }
            }
            if (unit != null)
            {
                location.MeldAttempt(unit);
                unit.Destroy();
            }
        }
        if (location.melding != null && location.melding.meldOwner == this.GetOwnerWizard()?.GetID())
        {
            yield break;
        }
        bool melderRequired = true;
        List<global::MOM.Group> groupsOfWizard = GameManager.GetGroupsOfWizard(this.GetOwnerWizard().ID);
        bool flag = false;
        global::MOM.Group group = null;
        global::MOM.Unit unit2 = null;
        foreach (global::MOM.Group v in groupsOfWizard)
        {
            Reference<global::MOM.Unit> reference = v.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsMelder());
            if (reference == null)
            {
                continue;
            }
            if (v.IsHosted())
            {
                if (v.GetLocationHost().power != 0 && (v.GetLocationHost().melding == null || v.GetLocationHost().melding.meldOwner != this.GetOwnerWizard().ID))
                {
                    continue;
                }
                flag = true;
            }
            else if (v.GetDesignation().designation == AIGroupDesignation.Designation.Melder)
            {
                if (v.destination == this.GetPosition())
                {
                    melderRequired = false;
                    flag = false;
                    break;
                }
                global::MOM.Location location2 = GameManager.GetLocationsOfThePlane(this.GetPlane()).Find((global::MOM.Location o) => o.GetPosition() == v.destination);
                if (location2 != null && location2.power != 0 && (location2.melding == null || location2.melding.meldOwner != this.GetOwnerWizard().ID))
                {
                    continue;
                }
                flag = true;
            }
            else
            {
                flag = true;
            }
            if (group == null)
            {
                group = v;
                unit2 = reference;
                break;
            }
            int distanceWrapping = this.GetPlane().GetDistanceWrapping(group.GetPosition(), this.GetPosition());
            if (this.GetPlane().GetDistanceWrapping(v.GetPosition(), this.GetPosition()) < distanceWrapping)
            {
                group = v;
                unit2 = reference;
                break;
            }
        }
        if (flag && group != null)
        {
            melderRequired = false;
            if (group.GetUnits().Count > 1 || group.IsHosted())
            {
                yield return group.AIMoveOut(guaranteeMP: true, unit2, new Destination(this.owner.Get(), aggressive: true), AIGroupDesignation.Designation.Melder, null);
            }
            else
            {
                group.GetDesignation().NewDesignation(AIGroupDesignation.Designation.Melder, new Destination(this.owner.Get(), aggressive: true));
            }
        }
        PlayerWizardAI ownerWizard = this.GetOwnerWizard();
        if (melderRequired && ownerWizard != null && ownerWizard.turnSkillLeft > 0)
        {
            DBReference<Spell> dBReference = this.GetOwnerWizard().GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.MAGIC_SPIRIT);
            if (dBReference != null && this.GetOwnerWizard().summoningCircle != null && this.GetOwnerWizard().banishedTurn == 0 && dBReference.Get().worldCost < this.wizard.mana)
            {
                this.GetOwnerWizard().mana -= dBReference.Get().worldCost;
                ownerWizard.turnSkillLeft = Mathf.Max(0, ownerWizard.turnSkillLeft - dBReference.Get().worldCost);
                global::MOM.Unit u = global::MOM.Unit.CreateFrom((global::DBDef.Unit)(Enum)UNIT.ARC_MAGIC_SPIRIT);
                this.GetOwnerWizard().summoningCircle.Get().GetLocalGroup().AddUnit(u);
            }
        }
    }

    private void ProcessDanger()
    {
        this.wishToExtractSettlers = false;
        this.wishToExtractEngineers = false;
        global::MOM.Group localGroup = this.owner.Get().GetLocalGroup();
        int value = localGroup.GetValue();
        if (value < this.danger)
        {
            List<Reference<global::MOM.Unit>> units = localGroup.GetUnits();
            if (units != null && units.Count > 5)
            {
                foreach (Reference<global::MOM.Unit> item in units)
                {
                    FInt attFinal = item.Get().GetAttFinal(TAG.SETTLER_UNIT);
                    FInt attFinal2 = item.Get().GetAttFinal(TAG.ENGINEER_UNIT);
                    if (attFinal > 0)
                    {
                        this.wishToExtractSettlers = true;
                    }
                    if (attFinal2 > 0)
                    {
                        this.wishToExtractEngineers = true;
                    }
                }
            }
            int num = 0;
            int ownerID = this.owner.Get().GetOwnerID();
            global::MOM.Group group = null;
            float num2 = 0f;
            Vector3i position = this.GetPosition();
            if (this.failedHelpRequests != null)
            {
                for (int num3 = this.failedHelpRequests.Count - 1; num3 > 0; num3--)
                {
                    if (this.failedHelpRequests[num3].y < TurnManager.GetTurnNumber() - 10)
                    {
                        this.failedHelpRequests.RemoveAt(num3);
                    }
                }
                if (this.failedHelpRequests.Count < 1)
                {
                    this.failedHelpRequests = null;
                }
            }
            foreach (global::MOM.Group v in GameManager.GetGroupsOfPlane(this.owner.Get().GetPlane()))
            {
                if (v.GetOwnerID() != ownerID || v.GetLocationHostSmart() != null)
                {
                    continue;
                }
                int num4 = 0;
                switch (v.GetDesignation().designation)
                {
                case AIGroupDesignation.Designation.Defend:
                    if (v.GetDesignation().destinationPosition != null && v.GetDesignation().destinationPosition.position == position)
                    {
                        num += v.GetValue();
                    }
                    break;
                case AIGroupDesignation.Designation.AggressionShort:
                case AIGroupDesignation.Designation.AggressionMedium:
                case AIGroupDesignation.Designation.AggressionLong:
                    num4 = 15;
                    break;
                case AIGroupDesignation.Designation.Retreat:
                    num4 = 6;
                    break;
                }
                if (num4 <= 0 || localGroup.GetUnits().Count >= 9)
                {
                    continue;
                }
                int num5 = HexCoordinates.HexDistance(v.GetPosition(), position);
                if (num5 < num4 && (this.failedHelpRequests == null || this.failedHelpRequests.FindIndex((Vector2i o) => o.x == v.ID) <= -1))
                {
                    float num6 = 30f / ((float)num5 + 29f);
                    int value2 = v.GetValue();
                    if ((float)value2 * num6 > num2)
                    {
                        num2 = (float)value2 * num6;
                        group = v;
                    }
                }
            }
            if (value > this.danger)
            {
                this.needToBuildArmy = 0;
            }
            if (num + value < this.danger)
            {
                this.needToBuildArmy = 2;
                if (group != null)
                {
                    RequestDataV2 requestDataV = RequestDataV2.CreateRequest(group.GetPlane(), group.GetPosition(), position, group);
                    PathfinderV2.FindPath(requestDataV);
                    List<Vector3i> path = requestDataV.GetPath();
                    bool flag = false;
                    if (path == null)
                    {
                        flag = true;
                    }
                    else if (group.PathToTurns(path) >= 10)
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        if (this.failedHelpRequests == null)
                        {
                            this.failedHelpRequests = new List<Vector2i>();
                        }
                        this.failedHelpRequests.Add(new Vector2i(group.ID, TurnManager.GetTurnNumber()));
                    }
                    else
                    {
                        group.GetDesignation().NewDesignation(AIGroupDesignation.Designation.Defend, Vector3i.invalid, position);
                        num += group.GetValue();
                    }
                }
            }
            if (num + value < this.danger)
            {
                this.needToBuildArmy = 3;
            }
        }
        if (this.GetAsTown() != null)
        {
            int num7 = this.TargetUnitCount() - this.GetAsTown().GetUnits().Count;
            if (num7 > 0 && this.GetAsTown().GetUnits().Count < 9)
            {
                this.needToBuildArmy = Mathf.Clamp(this.needToBuildArmy + num7 * 2, 0, 3);
            }
        }
    }

    private void PlanNeeds()
    {
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.GetPlane());
        bool flag = false;
        foreach (global::MOM.Group item in groupsOfPlane)
        {
            if (item.GetOwnerID() == this.GetOwnerWizard().ID && item.doomStack && item.doomStackPowerMiss > 100 && this.GetPlane().area.HexDistance(this.GetPosition(), item.GetPosition()) < 10)
            {
                if (this.needToBuildArmy < 3)
                {
                    this.needToBuildArmy++;
                }
                flag = true;
            }
        }
        int num = 1;
        if (this.owner.Get() is TownLocation)
        {
            int popUnits = (this.owner.Get() as TownLocation).GetPopUnits();
            num = Mathf.Min(9, 1 + popUnits / 5);
            if (popUnits < 8)
            {
                this.needToBuildEconomy++;
            }
        }
        if (this.owner.Get().GetUnits().Count < num)
        {
            this.needToBuildArmy++;
        }
        this.needToBuildEconomy = 0;
        if (this.dangerRank == 0)
        {
            this.needToBuildEconomy++;
            if (flag)
            {
                this.needToBuildEconomy++;
            }
        }
        else if (this.dangerRank > 1)
        {
            this.needToBuildArmy++;
        }
    }

    private int MilitaryNeedsFromTime()
    {
        float num = Mathf.Clamp01((float)TurnManager.GetTurnNumber() / 500f);
        float num2 = Mathf.Pow(num, 1.4f);
        float num3 = Mathf.Pow(num, 1.2f);
        return (int)(400f + Mathf.Lerp(num2 * 6000f, num3 * 3000f, num));
    }

    private IEnumerator ActOnNeeds()
    {
        TownLocation tl = this.GetAsTown();
        if (tl != null && tl.seaside && tl.PossibleUnits().Contains((global::DBDef.Unit)(Enum)UNIT.TRIREME))
        {
            List<Vector3i> surroundingArea = tl.GetSurroundingArea();
            Dictionary<Vector3i, int> waterBodies = tl.GetPlane().waterBodies;
            bool flag = false;
            foreach (Vector3i item in surroundingArea)
            {
                if (waterBodies.ContainsKey(item) && waterBodies[item] > 300)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                bool flag2 = true;
                foreach (global::MOM.Group ownGroup in (tl.GetWizardOwner() as PlayerWizardAI).GetOwnGroups())
                {
                    if (ownGroup.waterMovement && !(ownGroup.transporter == null) && ownGroup.GetDistanceTo(tl) < 12 && ownGroup.GetUnits().Count == 1)
                    {
                        flag2 = false;
                        break;
                    }
                }
                if (flag2)
                {
                    tl.craftingQueue.InsertFirstItem((global::DBDef.Unit)(Enum)UNIT.TRIREME, 1);
                    CraftingItem first = tl.craftingQueue.GetFirst();
                    first.progress = first.requirementValue;
                    yield break;
                }
            }
        }
        if (this.needToBuildArmy <= 2 && this.nextTurnToConsiderSettler < TurnManager.GetTurnNumber() && this.dangerRank < 2 && this.GetAsTown().GetPopUnits() > 3 + this.GetAsTown().GetRebels())
        {
            this.nextTurnToConsiderSettler = TurnManager.GetTurnNumber() + global::UnityEngine.Random.Range(4, 14);
            DataHeatMaps hm = DataHeatMaps.Get(this.owner.Get().GetPlane());
            while (!hm.IsMapReady(DataHeatMaps.HMType.SettlementValue, this.owner.Get().GetOwnerID()))
            {
                yield return null;
            }
            Vector3i position = this.GetPosition();
            HeatMap heatMap = hm.GetHeatMap(DataHeatMaps.HMType.SettlementValue);
            ReadOnlyCollection<Vector3i> rangeSimple = HexNeighbors.GetRangeSimple(10);
            _ = Vector3i.invalid;
            int num = 0;
            foreach (Vector3i item2 in rangeSimple)
            {
                int value = heatMap.GetValue(item2 + position);
                if (value > num)
                {
                    _ = item2 + position;
                    num = value;
                }
            }
            if (num > 7)
            {
                this.locationExpectsSettler = true;
            }
            else
            {
                this.locationExpectsSettler = false;
            }
        }
        else if (this.dangerRank > 1)
        {
            this.locationExpectsSettler = false;
        }
        bool flag3 = false;
        if (this.locationExpectsSettler && this.GetGroup().GetUnits().FindIndex((Reference<global::MOM.Unit> o) => o.Get().IsSettler()) == -1)
        {
            List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.GetPlane());
            int iD = this.GetOwnerWizard().ID;
            foreach (global::MOM.Location item3 in locationsOfThePlane)
            {
                if (item3.GetOwnerID() != iD || item3.locationTactic == null)
                {
                    continue;
                }
                if (item3.locationTactic.wishToExtractSettlers && HexCoordinates.HexDistance(item3.GetPosition(), this.GetPosition()) < 20)
                {
                    flag3 = true;
                    List<Reference<global::MOM.Unit>> list = item3.locationTactic.ProduceSettlerGroup(item3.GetUnits());
                    if (list != null && list.Count < 1)
                    {
                        global::MOM.Group group = new global::MOM.Group(item3.GetPlane(), item3.GetOwnerID());
                        group.Position = item3.GetPosition();
                        for (int i = 0; i <= list.Count; i++)
                        {
                            item3.GetLocalGroup().TransferUnit(group, list[i]);
                        }
                        group.GetDesignation().NewDesignation(AIGroupDesignation.Designation.Transfer, this.GetPosition());
                    }
                }
                else if (item3.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsSettler()) != null)
                {
                    flag3 = true;
                }
            }
        }
        if (tl != null)
        {
            this.UpgradeDefendingArmyQuality();
            this.UpdateTown(tl, !flag3 && this.locationExpectsSettler, this.locationExpectsEngineer);
        }
    }

    internal void OwnerChanged()
    {
        this.wizard = null;
        this.nextTurnToConsiderSettler = TurnManager.GetTurnNumber() + global::UnityEngine.Random.Range(7, 14);
        this.locationExpectsSettler = false;
    }

    private void CreateExpeditionsIfPossible()
    {
        int value = this.GetGroup().GetValue();
        List<Reference<global::MOM.Unit>> units = this.GetGroup().GetUnits();
        if (!this.IsTown())
        {
            bool flag = false;
            bool flag2 = false;
            global::MOM.Location location = this.owner.Get();
            if ((float)this.danger > (float)value * 1.3f)
            {
                flag = true;
            }
            else if (location.power == 0)
            {
                if (location.GetUnits().Count > 1)
                {
                    flag2 = true;
                }
            }
            else if (location.melding == null || location.melding.meldOwner != this.GetOwnerWizard().ID)
            {
                if (location.GetUnits().Count > 1)
                {
                    flag2 = true;
                }
            }
            else
            {
                flag = true;
            }
            if (flag && location.power > 0 && (location.melding?.meldOwner ?? 0) == this.GetOwnerWizard().ID)
            {
                flag = false;
                flag2 = true;
            }
            if (flag)
            {
                List<Reference<global::MOM.Unit>> list = location.GetUnits().FindAll((Reference<global::MOM.Unit> o) => !o.Get().IsSettler() && !o.Get().IsMelder());
                if (list.Count <= 0)
                {
                    return;
                }
                global::MOM.Group group = new global::MOM.Group(this.GetPlane(), this.GetOwnerWizard().ID);
                group.Position = this.GetPosition();
                foreach (Reference<global::MOM.Unit> item in list)
                {
                    group.AddUnit(item, updateMovementFlags: false);
                }
                group.UpdateMovementFlags();
            }
            else
            {
                if (!flag2)
                {
                    return;
                }
                global::MOM.Unit weakest = null;
                foreach (Reference<global::MOM.Unit> unit in location.GetUnits())
                {
                    if (weakest == null)
                    {
                        weakest = unit;
                    }
                    else if (weakest.GetWorldUnitValue() > unit.Get().GetWorldUnitValue())
                    {
                        weakest = unit;
                    }
                }
                List<Reference<global::MOM.Unit>> list2 = location.GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get() != weakest);
                if (list2.Count <= 0)
                {
                    return;
                }
                global::MOM.Group group2 = new global::MOM.Group(this.GetPlane(), this.GetOwnerWizard().ID);
                group2.Position = this.GetPosition();
                foreach (Reference<global::MOM.Unit> item2 in list2)
                {
                    group2.AddUnit(item2, updateMovementFlags: false);
                }
                group2.GetDesignation()?.NewDesignation(AIGroupDesignation.Designation.AggressionShort, null);
                group2.UpdateMovementFlags();
            }
        }
        else
        {
            if (this.danger >= value)
            {
                return;
            }
            DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
            if (this.danger == 0)
            {
                if (units.Count < this.MinimumExpectedUnits())
                {
                    return;
                }
            }
            else if (units.Count < this.TargetUnitCount())
            {
                return;
            }
            List<Reference<global::MOM.Unit>> list3 = this.ProduceSettlerGroup(units);
            if (list3 != null && list3.Count > 0)
            {
                global::MOM.Group group3 = new global::MOM.Group(this.GetPlane(), this.GetOwnerWizard().ID);
                group3.Position = this.GetPosition();
                for (int i = 0; i < list3.Count; i++)
                {
                    this.GetGroup().TransferUnit(group3, list3[i]);
                }
                group3.RegainTo1MP();
            }
            List<Reference<global::MOM.Unit>> list4 = this.ProduceTransportGroup(units);
            if (list4 != null && list4.Count > 0)
            {
                global::MOM.Group group4 = new global::MOM.Group(this.GetPlane(), this.GetOwnerWizard().ID);
                group4.Position = this.GetPosition();
                for (int j = 0; j < list4.Count; j++)
                {
                    this.GetGroup().TransferUnit(group4, list4[j]);
                }
                group4.RegainTo1MP();
            }
            if (this.GetGroup().GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsHero()) != null)
            {
                global::DBDef.Unit bestCraftableDefender = this.GetBestCraftableDefender();
                int num = 2;
                if (this.owner.Get().GetWizardOwner()?.wizardTower == this.owner.Get())
                {
                    num++;
                }
                if (bestCraftableDefender == null || this.GetOwnerWizard() == null)
                {
                    num = 0;
                }
                else
                {
                    num = Mathf.Min(num, this.GetOwnerWizard().money / bestCraftableDefender.constructionCost);
                    if (num * BaseUnit.GetUnitStrength(bestCraftableDefender) < this.danger)
                    {
                        return;
                    }
                }
                global::MOM.Group group5 = new global::MOM.Group(this.GetPlane(), this.GetOwnerWizard().ID);
                group5.Position = this.GetPosition();
                this.GetGroup().TransferUnits(group5);
                group5.GetDesignation().actOrResuplyOthers = true;
                group5.GetDesignation().NewDesignation(AIGroupDesignation.Designation.AggressionShort, null);
                for (int k = 0; k < num; k++)
                {
                    if (this.GetGroup().GetUnits().Count < 9 && bestCraftableDefender != null && this.GetOwnerWizard().money > bestCraftableDefender.constructionCost)
                    {
                        this.GetOwnerWizard().money -= bestCraftableDefender.constructionCost;
                        this.GetGroup().AddUnit(global::MOM.Unit.CreateFrom(bestCraftableDefender));
                    }
                }
            }
            else
            {
                if (this.GetGroup().GetUnits().Count <= this.MinimumExpectedUnits() || (this.GetGroup().GetUnits().Count < 4 && this.regionalForces > this.danger) || (float)this.GetGroup().GetSimpleStrength() < (float)this.danger * 2.5f)
                {
                    return;
                }
                List<Reference<global::MOM.Unit>> list5 = this.ProduceExpeditionGroup(this.GetGroup(), this.MilitaryNeedsFromTime(), this.danger);
                if (list5 != null && list5.Count > 0)
                {
                    global::MOM.Group group6 = new global::MOM.Group(this.GetPlane(), this.GetOwnerWizard().ID);
                    group6.Position = this.GetPosition();
                    for (int l = 0; l < list5.Count; l++)
                    {
                        this.GetGroup().TransferUnit(group6, list5[l]);
                    }
                    group6.GetDesignation().actOrResuplyOthers = true;
                    group6.GetDesignation().NewDesignation(AIGroupDesignation.Designation.AggressionShort, null);
                }
            }
        }
    }

    private global::DBDef.Unit GetBestCraftableDefender()
    {
        List<global::DBDef.Unit> list = this.GetAsTown().PossibleUnits();
        global::DBDef.Unit unit = null;
        int num = int.MaxValue;
        for (int num2 = list.Count - 1; num2 >= 0; num2--)
        {
            global::DBDef.Unit unit2 = list[num2];
            int unitStrength = BaseUnit.GetUnitStrength(unit2);
            if ((unit == null || num <= unitStrength) && (!(unit2.GetTag(TAG.CAN_WALK) == 0) || !(unit2.GetTag(TAG.CAN_SWIM) > 0)))
            {
                num = unitStrength;
                unit = unit2;
            }
        }
        return unit;
    }

    private List<global::DBDef.Unit> GetBestCraftableDefenders(int minValue = 0)
    {
        List<global::DBDef.Unit> list = this.GetAsTown().PossibleUnits();
        List<global::DBDef.Unit> list2 = new List<global::DBDef.Unit>();
        for (int num = list.Count - 1; num >= 0; num--)
        {
            global::DBDef.Unit unit = list[num];
            int unitStrength = BaseUnit.GetUnitStrength(unit);
            if (minValue <= unitStrength && (!(unit.GetTag(TAG.CAN_WALK) == 0) || !(unit.GetTag(TAG.CAN_SWIM) > 0)))
            {
                list2.Add(unit);
            }
        }
        return list2;
    }

    private void UpgradeDefendingArmyQuality()
    {
        TownLocation asTown = this.GetAsTown();
        if (asTown == null || (asTown.GetWizardOwner() == null && asTown.GetUnits().Count > 0))
        {
            return;
        }
        if (this.militaryUpgradeTimer <= 0)
        {
            this.militaryUpgradeTimer = global::UnityEngine.Random.Range(4, 20);
            return;
        }
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
        this.militaryUpgradeTimer -= settingAsInt;
        if (this.militaryUpgradeTimer > 0)
        {
            return;
        }
        global::DBDef.Unit bestCraftableDefender = this.GetBestCraftableDefender();
        if (bestCraftableDefender != null && asTown.GetUnits().Count < this.MinimumExpectedUnits())
        {
            if (bestCraftableDefender != null && this.GetOwnerWizard().money > bestCraftableDefender.constructionCost)
            {
                this.GetOwnerWizard().money -= bestCraftableDefender.constructionCost;
                this.GetGroup().AddUnit(global::MOM.Unit.CreateFrom(bestCraftableDefender));
            }
            return;
        }
        if (bestCraftableDefender == null || global::UnityEngine.Random.Range(0f, 1f) < 0.1f)
        {
            List<Building> militaryBuildings = GameplayHelper.Get().GetMilitaryBuildings(asTown.GetSourceTown());
            List<Building> list = this.FilterByRequirements(militaryBuildings);
            if (list != null && list.Count > 0)
            {
                int index = global::UnityEngine.Random.Range(0, list.Count);
                Building building = list[index];
                if (this.GetOwnerWizard().money > building.buildCost / settingAsInt + 200)
                {
                    this.GetOwnerWizard().money -= building.buildCost / settingAsInt;
                    asTown.AddBuilding(building);
                    asTown.craftingQueue.SanitizeQueue();
                }
            }
            return;
        }
        global::MOM.Unit unit = null;
        int num = int.MaxValue;
        int unitStrength = BaseUnit.GetUnitStrength(bestCraftableDefender);
        foreach (Reference<global::MOM.Unit> unit3 in asTown.GetUnits())
        {
            if (!unit3.Get().IsHero())
            {
                int unitStrength2 = BaseUnit.GetUnitStrength(unit3.Get().dbSource);
                unitStrength2 += unit3.Get().xp / 5;
                if ((unit != null || !((float)unitStrength2 > (float)unitStrength * 0.6f)) && (unit == null || num >= unitStrength2))
                {
                    unit = unit3.Get();
                    num = unitStrength2;
                }
            }
        }
        if (unit == null)
        {
            return;
        }
        List<global::DBDef.Unit> bestCraftableDefenders = this.GetBestCraftableDefenders((int)((float)num * 1.3f));
        if (bestCraftableDefenders.Count != 0)
        {
            if (bestCraftableDefenders.Count > 1)
            {
                bestCraftableDefenders.RandomSort();
            }
            global::DBDef.Unit unit2 = bestCraftableDefenders[0];
            if (this.GetOwnerWizard().money > unit2.constructionCost / settingAsInt + 300)
            {
                unit.Destroy();
                this.GetOwnerWizard().money -= unit2.constructionCost / settingAsInt;
                this.GetGroup().AddUnit(global::MOM.Unit.CreateFrom(bestCraftableDefender));
            }
        }
    }

    private void UpdateTown(TownLocation tl, bool needSettler, bool needEngineer)
    {
        bool flag = false;
        CraftingItem curentCraft = ((tl.craftingQueue != null) ? tl.craftingQueue.GetFirst() : null);
        if (this.GetOwnerWizard().money > 350)
        {
            AITarget aITarget = this.GetOwnerWizard().priorityTargets?.GetSupportedTarget(tl);
            if (aITarget != null && !aITarget.targetAchievable && global::UnityEngine.Random.Range(1, 10) < tl.GetPopUnits())
            {
                flag = true;
                this.BuildArmy(tl, curentCraft, aITarget);
            }
        }
        if (!flag)
        {
            if (this.needToBuildEconomy > this.needToBuildArmy)
            {
                this.BuildEconomy(tl, curentCraft, this.needToBuildEconomy == 3, needSettler, needEngineer);
            }
            else
            {
                this.BuildArmy(tl, curentCraft);
            }
        }
        int money = this.GetOwnerWizard().money;
        if (money < 200)
        {
            return;
        }
        CraftingItem first = tl.craftingQueue.GetFirst();
        if (first == null || first.requirementValue <= 0)
        {
            return;
        }
        AITarget aITarget2 = this.GetOwnerWizard().priorityTargets?.GetSupportedTarget(tl);
        if ((this.wizard.wizardTower == tl || aITarget2 != null || this.dangerRank > 1 || tl.GetUnits().Count < this.TargetUnitCount()) && first.craftedUnit != null)
        {
            int num = first.BuyCost();
            if (money <= num + 100)
            {
                return;
            }
            int num2 = 0;
            if (aITarget2 != null && aITarget2.targetIsGroup)
            {
                num2++;
                if (money > num + 500)
                {
                    num2++;
                }
            }
            if (this.wizard.wizardTower == tl)
            {
                num2++;
            }
            if (money > num + 750)
            {
                num2++;
            }
            if (money > num + 1500)
            {
                num2++;
            }
            if (this.dangerRank + num2 >= 3)
            {
                this.GetOwnerWizard().money -= first.BuyCost();
                first.progress = first.requirementValue;
            }
            else if (this.dangerRank + num2 == 2 && first.progress > first.requirementValue / 2)
            {
                this.GetOwnerWizard().money -= first.BuyCost();
                first.progress = first.requirementValue;
            }
        }
        else
        {
            if (!(first.craftedBuilding != null))
            {
                return;
            }
            Building building = first.craftedBuilding.Get();
            int popUnits = tl.GetPopUnits();
            int num3 = tl.MaxPopulation();
            int num4 = tl.CalculateProductionIncome();
            float num5 = (float)first.progress / (float)first.requirementValue;
            int num6 = ((num4 < 1) ? 100 : ((first.requirementValue - first.progress) / num4));
            if (building == (Building)BUILDING.BUILDERS_HALL)
            {
                float num7 = 0.5f;
                if (num3 > 20)
                {
                    num7 = 0.2f;
                }
                else if (num3 > 10)
                {
                    num7 = 0.5f;
                }
                if (num5 > num7 && num6 > 10 && first.BuyCost() < money - 100)
                {
                    this.GetOwnerWizard().money -= first.BuyCost();
                    first.progress = first.requirementValue;
                }
            }
            else
            {
                if (building.tags == null)
                {
                    return;
                }
                if (Array.Find(building.tags, (Tag o) => o == (Tag)TAG.FOOD_PRODUCTION) != null)
                {
                    float num8 = 0.5f;
                    if (num3 > 20)
                    {
                        num8 = 0.3f;
                    }
                    else if (num3 > 10)
                    {
                        num8 = 0.4f;
                    }
                    if (num5 > num8 && num6 > 10 && first.BuyCost() < money - 100)
                    {
                        this.GetOwnerWizard().money -= first.BuyCost();
                        first.progress = first.requirementValue;
                    }
                }
                else if (popUnits > 3 && Array.Find(building.tags, (Tag o) => o == (Tag)TAG.GOLD_PRODUCTION) != null)
                {
                    if (((popUnits > 20 && num6 > 10) || (popUnits > 10 && num6 > 15) || num6 > 25) && first.BuyCost() < money - 100)
                    {
                        this.GetOwnerWizard().money -= first.BuyCost();
                        first.progress = first.requirementValue;
                    }
                }
                else if (popUnits > 8 && Array.Find(building.tags, (Tag o) => o == (Tag)TAG.MILITARY) != null && aITarget2 != null && first.BuyCost() < money - 100)
                {
                    this.GetOwnerWizard().money -= first.BuyCost();
                    first.progress = first.requirementValue;
                }
            }
        }
    }

    private void BuildEconomy(TownLocation tl, CraftingItem curentCraft, bool forced, bool needSettler, bool needEngineer)
    {
        if (curentCraft == null)
        {
            return;
        }
        if (curentCraft.craftedBuilding == null && curentCraft.craftedUnit == null)
        {
            Debug.LogError("both craft forms are turned off!");
        }
        if (!forced && (curentCraft.craftedUnit != null || curentCraft.craftedBuilding.Get().buildCost > 0))
        {
            return;
        }
        if (curentCraft.craftedBuilding != null && curentCraft.craftedBuilding.Get().buildCost > 0)
        {
            HashSet<Building> economyBuildings = GameplayHelper.Get().GetEconomyBuildings(tl.GetSourceTown());
            tl.PossibleBuildings(atThisMoment: true);
            if (economyBuildings.Contains(curentCraft.craftedBuilding))
            {
                return;
            }
        }
        if (curentCraft.craftedUnit != null && (curentCraft.craftedUnit.Get().GetTag((Tag)TAG.SETTLER_UNIT) > 0 || curentCraft.craftedUnit.Get().GetTag((Tag)TAG.ENGINEER_UNIT) > 0 || curentCraft.craftedUnit.Get().GetTag((Tag)TAG.TRANSPORTER) > 0))
        {
            return;
        }
        if (needSettler && global::UnityEngine.Random.Range(0f, 1f) < 0.5f)
        {
            global::DBDef.Unit townProducedSettler = GameplayHelper.GetTownProducedSettler(tl.GetSourceTown());
            if (townProducedSettler != null)
            {
                global::MOM.Unit u = global::MOM.Unit.CreateFrom(townProducedSettler);
                tl.GetLocalGroup().AddUnit(u);
                this.nextTurnToConsiderSettler = TurnManager.GetTurnNumber() + global::UnityEngine.Random.Range(8, 20);
                this.locationExpectsSettler = false;
                return;
            }
            Debug.LogError("Missing Settler for town " + tl.GetSourceTown().dbName);
        }
        if (needEngineer && global::UnityEngine.Random.Range(0f, 1f) < 0.5f)
        {
            global::DBDef.Unit townProducedEngineer = GameplayHelper.GetTownProducedEngineer(tl.GetSourceTown());
            if (townProducedEngineer != null)
            {
                if (!tl.craftingQueue.ReturnWorkToEngineerUnit())
                {
                    tl.craftingQueue.InsertFirstItem(townProducedEngineer, 1);
                }
            }
            else
            {
                Debug.LogError("Missing Engineer for town " + tl.GetSourceTown().dbName);
            }
            return;
        }
        List<Building> list = tl.PossibleBuildings(atThisMoment: true);
        if (list.Contains((Building)BUILDING.BUILDERS_HALL))
        {
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem((Building)BUILDING.BUILDERS_HALL);
            }
            return;
        }
        if (list.Contains((Building)BUILDING.GRANARY))
        {
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem((Building)BUILDING.GRANARY);
            }
            return;
        }
        if (list.Contains((Building)BUILDING.MARKETPLACE))
        {
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem((Building)BUILDING.MARKETPLACE);
            }
            return;
        }
        if (list.Contains((Building)BUILDING.FARMERS_MARKET))
        {
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem((Building)BUILDING.FARMERS_MARKET);
            }
            return;
        }
        if (list.Contains((Building)BUILDING.SHRINE))
        {
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem((Building)BUILDING.SHRINE);
            }
            return;
        }
        List<Building> foodBuildings;
        if (global::UnityEngine.Random.Range(0f, 1f) < 0.5f)
        {
            if (global::UnityEngine.Random.Range(0f, 1f) < 0.5f)
            {
                foodBuildings = GameplayHelper.Get().GetFoodBuildings(tl.GetSourceTown());
                foodBuildings = this.FilterByRequirements(foodBuildings);
                if (foodBuildings != null && foodBuildings.Count > 0)
                {
                    int index = global::UnityEngine.Random.Range(0, foodBuildings.Count);
                    if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
                    {
                        tl.craftingQueue.InsertFirstItem(foodBuildings[index]);
                    }
                    return;
                }
            }
            else
            {
                foodBuildings = GameplayHelper.Get().GetMoneyBuildings(tl.GetSourceTown());
                foodBuildings = this.FilterByRequirements(foodBuildings);
                if (foodBuildings != null && foodBuildings.Count > 0)
                {
                    int index2 = global::UnityEngine.Random.Range(0, foodBuildings.Count);
                    if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
                    {
                        tl.craftingQueue.InsertFirstItem(foodBuildings[index2]);
                    }
                    return;
                }
            }
        }
        foodBuildings = GameplayHelper.Get().GetOtherEconomyBuildings(tl.GetSourceTown());
        foodBuildings = this.FilterByRequirements(foodBuildings);
        if (foodBuildings != null && foodBuildings.Count > 0)
        {
            int index3 = global::UnityEngine.Random.Range(0, foodBuildings.Count);
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem(foodBuildings[index3]);
            }
            return;
        }
        HashSet<Building> economyBuildings2 = GameplayHelper.Get().GetEconomyBuildings(tl.GetSourceTown());
        foodBuildings = this.FilterByRequirements(economyBuildings2);
        if (foodBuildings != null && foodBuildings.Count > 0)
        {
            int index4 = global::UnityEngine.Random.Range(0, foodBuildings.Count);
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem(foodBuildings[index4]);
            }
            return;
        }
        foodBuildings = tl.PossibleBuildings(atThisMoment: true).FindAll((Building o) => o.buildCost < 1);
        foodBuildings = this.FilterByRequirements(foodBuildings);
        if (foodBuildings != null && foodBuildings.Count > 0)
        {
            int index5 = global::UnityEngine.Random.Range(0, foodBuildings.Count);
            if (!tl.craftingQueue.ReturnWorkToEconomyBuilding())
            {
                tl.craftingQueue.InsertFirstItem(foodBuildings[index5]);
            }
        }
    }

    private List<Building> FilterByRequirements(IEnumerable buildings)
    {
        List<Building> list = new List<Building>();
        TownLocation asTown = this.GetAsTown();
        List<Building> list2 = asTown.PossibleBuildings(atThisMoment: true);
        foreach (object building2 in buildings)
        {
            Building building = building2 as Building;
            if (!list2.Contains(building))
            {
                continue;
            }
            if (building.parentBuildingRequired != null)
            {
                bool flag = true;
                Building[] parentBuildingRequired = building.parentBuildingRequired;
                foreach (Building b in parentBuildingRequired)
                {
                    if (!asTown.HaveBuilding(b))
                    {
                        flag = false;
                        break;
                    }
                }
                if (!flag)
                {
                    continue;
                }
            }
            list.Add(building);
        }
        return list;
    }

    public int MinimumExpectedUnits()
    {
        if (!(this.owner.Get() is TownLocation))
        {
            return 0;
        }
        int turnNumber = TurnManager.GetTurnNumber();
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_AI_SKILL");
        float num = 1f - (float)(settingAsInt - 1) * 0.15f;
        PlayerWizard playerWizard = this.owner?.Get()?.GetWizardOwner();
        if (playerWizard == null || playerWizard.GetTowerLocation() != this.GetAsTown())
        {
            int popUnits = this.GetAsTown().GetPopUnits();
            return Mathf.Min(7, 1 + popUnits / 3);
        }
        if ((float)turnNumber < 5f * num)
        {
            return 0;
        }
        if ((float)turnNumber < 20f * num)
        {
            return 1;
        }
        if ((float)turnNumber < 50f * num)
        {
            return 2;
        }
        if ((float)turnNumber < 80f * num)
        {
            return 3;
        }
        if ((float)turnNumber < 120f * num)
        {
            return 4;
        }
        if ((float)turnNumber < 150f * num)
        {
            return 5;
        }
        if ((float)turnNumber < 220f * num)
        {
            return 6;
        }
        return 7;
    }

    public int TargetUnitCount()
    {
        int num = this.MinimumExpectedUnits();
        return Mathf.Min(9, Mathf.Min((int)((float)num * 1.51f), num + 2));
    }

    private void BuildArmy(TownLocation tl, CraftingItem curentCraft, AITarget tg = null)
    {
        if (curentCraft != null && (curentCraft.craftedUnit != null || (curentCraft.craftedBuilding != null && GameplayHelper.Get().GetMilitaryBuildings(tl.GetSourceTown()).Contains(curentCraft.craftedBuilding))))
        {
            return;
        }
        List<Reference<global::MOM.Unit>> units = tl.GetLocalGroup().GetUnits();
        int count = units.Count;
        Town sourceTown = tl.GetSourceTown();
        global::DBDef.Unit settler = GameplayHelper.GetTownProducedSettler(sourceTown);
        global::DBDef.Unit engineer = GameplayHelper.GetTownProducedEngineer(sourceTown);
        List<global::DBDef.Unit> list = tl.PossibleUnits().FindAll((global::DBDef.Unit o) => o != settler && o != engineer);
        if (list.Count < (int)((float)tl.GetPopUnits() * 0.5f) || units.Count >= this.MinimumExpectedUnits())
        {
            this.militaryAdvancementRequired = true;
        }
        else
        {
            this.militaryAdvancementRequired = false;
        }
        if (list.Count > 0 && (!this.militaryAdvancementRequired || count < 2))
        {
            int minInclusive = list.Count / 2;
            if (count < 1 || (double)global::UnityEngine.Random.Range(0f, 1f) < 0.4 - (double)(0.1f * (float)count))
            {
                if (!tl.craftingQueue.ReturnWorkToMilitaryUnit())
                {
                    global::DBDef.Unit unit = list[global::UnityEngine.Random.Range(minInclusive, list.Count)];
                    global::DBDef.Unit unit2 = list[global::UnityEngine.Random.Range(minInclusive, list.Count)];
                    global::DBDef.Unit unit3 = list[global::UnityEngine.Random.Range(minInclusive, list.Count)];
                    unit = ((unit.constructionCost < unit2.constructionCost) ? unit : unit2);
                    unit = ((unit.constructionCost < unit3.constructionCost) ? unit : unit3);
                    tl.craftingQueue.InsertFirstItem(unit, global::UnityEngine.Random.Range(1, 2));
                }
                return;
            }
            if (count < 9)
            {
                bool num;
                if (tg == null)
                {
                    if (this.dangerRank <= 0)
                    {
                        goto IL_0212;
                    }
                    num = global::UnityEngine.Random.Range(0f, 1f) < 0.4f;
                }
                else
                {
                    num = tg.TownCapableOfUsefullUnits(tl);
                }
                if (num)
                {
                    global::DBDef.Unit unit4 = list[global::UnityEngine.Random.Range(minInclusive, list.Count)];
                    tl.craftingQueue.InsertFirstItem(unit4, 1);
                    return;
                }
            }
            goto IL_0212;
        }
        if (!tl.craftingQueue.ReturnWorkToMilitaryBuilding())
        {
            List<Building> militaryBuildings = GameplayHelper.Get().GetMilitaryBuildings(tl.GetSourceTown());
            List<Building> list2 = this.FilterByRequirements(militaryBuildings);
            if (list2 != null && list2.Count > 0)
            {
                int index = global::UnityEngine.Random.Range(0, list2.Count);
                tl.craftingQueue.InsertFirstItem(list2[index]);
            }
        }
        return;
        IL_0212:
        if (this.GetAsTown().seaside)
        {
            Tag tag = (Tag)TAG.TRANSPORTER;
            global::DBDef.Unit unit5 = this.GetAsTown().PossibleUnits().Find((global::DBDef.Unit o) => Array.Find(o.tags, (CountedTag k) => k.tag == tag) != null);
            if (unit5 == null)
            {
                return;
            }
            bool flag = true;
            foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
            {
                if (!flag)
                {
                    break;
                }
                if (!(entity.Value is global::MOM.Group group) || !group.alive || group.GetUnits().Count <= 0 || group.GetOwnerID() != this.GetOwnerWizard().ID || group.GetDistanceTo(tl) >= 15)
                {
                    continue;
                }
                foreach (Reference<global::MOM.Unit> unit7 in group.GetUnits())
                {
                    if (unit7.Get().GetAttFinal(tag) > 0)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag && !tl.craftingQueue.ReturnWorkToTransportUnit())
            {
                tl.craftingQueue.InsertFirstItem(unit5, 1);
            }
            return;
        }
        if (global::UnityEngine.Random.Range(0f, 1f) < 0.5f || ((this.wizard.GetDiplomacy()?.GetStatusToward(PlayerWizard.HumanID())?.openWar).GetValueOrDefault() && global::UnityEngine.Random.Range(0f, 1f) < 0.5f))
        {
            global::DBDef.Unit unit6 = null;
            {
                foreach (global::DBDef.Unit item in list)
                {
                    if (unit6 == null)
                    {
                        unit6 = item;
                    }
                    else if (unit6.constructionCost < item.constructionCost)
                    {
                        unit6 = item;
                    }
                }
                return;
            }
        }
        if (tl.craftingQueue.ReturnWorkToMilitaryBuilding())
        {
            return;
        }
        List<Building> militaryBuildings2 = GameplayHelper.Get().GetMilitaryBuildings(tl.GetSourceTown());
        List<Building> list3 = this.FilterByRequirements(militaryBuildings2);
        if (list3 != null && list3.Count > 0)
        {
            int index2 = global::UnityEngine.Random.Range(0, list3.Count);
            tl.craftingQueue.InsertFirstItem(list3[index2]);
        }
        else
        {
            if (tl.craftingQueue.ReturnWorkToMilitaryUnit())
            {
                return;
            }
            global::DBDef.Unit ship = (global::DBDef.Unit)(Enum)UNIT.TRIREME;
            global::DBDef.Unit ship2 = (global::DBDef.Unit)(Enum)UNIT.GALLEY;
            List<global::DBDef.Unit> list4 = list.FindAll((global::DBDef.Unit o) => o != ship && o != ship2);
            if (list4.Count > 0)
            {
                list4.Sort(delegate(global::DBDef.Unit a, global::DBDef.Unit b)
                {
                    int unitStrength = BaseUnit.GetUnitStrength(a);
                    int unitStrength2 = BaseUnit.GetUnitStrength(b);
                    return -unitStrength.CompareTo(unitStrength2);
                });
                tl.craftingQueue.InsertFirstItem(list4[0], 1);
            }
        }
    }

    private List<Reference<global::MOM.Unit>> ProduceSettlerGroup(List<Reference<global::MOM.Unit>> units)
    {
        if (units == null || units.Count < 1)
        {
            return null;
        }
        List<Reference<global::MOM.Unit>> list = null;
        Reference<global::MOM.Unit> reference = units.Find((Reference<global::MOM.Unit> o) => o.Get().GetAttFinal(TAG.SETTLER_UNIT) > 0);
        if (reference != null)
        {
            list = new List<Reference<global::MOM.Unit>>();
            list.Add(reference);
            int num = reference.Get().GetSimpleUnitStrength();
            int num2 = 0;
            {
                foreach (Reference<global::MOM.Unit> unit in units)
                {
                    if (unit != reference)
                    {
                        if (num < num2)
                        {
                            num += unit.Get().GetSimpleUnitStrength();
                            list.Add(unit);
                        }
                        else
                        {
                            num2 += unit.Get().GetSimpleUnitStrength();
                        }
                    }
                }
                return list;
            }
        }
        return null;
    }

    private List<Reference<global::MOM.Unit>> ProduceTransportGroup(List<Reference<global::MOM.Unit>> units)
    {
        if (units == null || units.Count < 1)
        {
            return null;
        }
        List<Reference<global::MOM.Unit>> list = null;
        Reference<global::MOM.Unit> reference = units.Find((Reference<global::MOM.Unit> o) => o.Get().GetAttFinal(TAG.TRANSPORTER) > 0);
        if (reference != null)
        {
            list = new List<Reference<global::MOM.Unit>>();
            list.Add(reference);
        }
        return list;
    }

    private List<Reference<global::MOM.Unit>> ProduceExpeditionGroup(global::MOM.Group group, int maxLeftPower, int minLeftPower)
    {
        List<Reference<global::MOM.Unit>> units = group.GetUnits();
        if (units == null || units.Count < 1)
        {
            return null;
        }
        int num = group.GetValue();
        List<Reference<global::MOM.Unit>> list = new List<Reference<global::MOM.Unit>>();
        foreach (Reference<global::MOM.Unit> item in units)
        {
            if (!(item.Get().dbSource.Get() is Hero))
            {
                continue;
            }
            int simpleUnitStrength = item.Get().GetSimpleUnitStrength();
            num -= simpleUnitStrength;
            if (minLeftPower > num)
            {
                num += simpleUnitStrength;
                continue;
            }
            if (list == null)
            {
                list = new List<Reference<global::MOM.Unit>>();
            }
            list.Add(item);
        }
        foreach (Reference<global::MOM.Unit> item2 in units)
        {
            if (num <= maxLeftPower || item2.Get().dbSource.Get() is Hero)
            {
                continue;
            }
            int simpleUnitStrength2 = item2.Get().GetSimpleUnitStrength();
            num -= simpleUnitStrength2;
            if (minLeftPower > num)
            {
                num += simpleUnitStrength2;
                continue;
            }
            if (list == null)
            {
                list = new List<Reference<global::MOM.Unit>>();
            }
            list.Add(item2);
        }
        return list;
    }

    private global::MOM.Group GetGroup()
    {
        if (this.group == null)
        {
            this.group = this.owner.Get().GetLocalGroup();
        }
        return this.group;
    }

    private global::WorldCode.Plane GetPlane()
    {
        if (this.plane == null)
        {
            this.plane = this.owner.Get().GetPlane();
        }
        return this.plane;
    }

    private PlayerWizardAI GetOwnerWizard()
    {
        if (this.wizard == null)
        {
            this.wizard = this.owner.Get().GetWizardOwner();
        }
        return this.wizard as PlayerWizardAI;
    }

    private Vector3i GetPosition()
    {
        return this.owner.Get().GetPosition();
    }

    private bool IsTown()
    {
        return this.owner.Get() is TownLocation;
    }

    private TownLocation GetAsTown()
    {
        return this.owner.Get() as TownLocation;
    }

    private int Distance(Vector3i a, Vector3i b)
    {
        return this.GetPlane().area.HexDistance(a, b);
    }
}
