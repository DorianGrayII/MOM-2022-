using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class AITarget
    {
        [ProtoMember(1)]
        public Reference<Entity> target;

        [ProtoMember(2)]
        public int priority;

        [ProtoMember(3)]
        public int owner;

        [ProtoMember(4)]
        public int strength;

        [ProtoMember(5)]
        public bool targetIsGroup;

        [ProtoMember(6)]
        public bool preparationOnly;

        [ProtoMember(10)]
        public bool requiresSummoningCircle;

        [ProtoMember(11)]
        public Reference<TownLocation> circleTarget;

        [ProtoMember(13)]
        public bool inRangeForSummon;

        [ProtoMember(20)]
        public DBReference<Spell> topEfficiencySpell;

        [ProtoMember(21)]
        public Reference<Group> targetAssignee;

        [ProtoMember(22)]
        public int topUnitStrength;

        [ProtoMember(23)]
        public bool targetAchievable;

        [ProtoMember(24)]
        public List<Reference<TownLocation>> supplyTowns;

        [ProtoMember(30)]
        public int ignoredUntil;

        [ProtoMember(31)]
        public int nextUpdate;

        [ProtoMember(32)]
        public bool warTarget;

        [ProtoIgnore]
        private PlayerWizard _owner;

        public AITarget()
        {
        }

        public AITarget(int owner, Entity target)
        {
            this.owner = owner;
            this.target = target;
            this.strength = this.GetAsGroup().GetValue();
            this.targetIsGroup = !this.GetAsGroup().IsHosted();
            if (target is Location location && location.GetWizardOwner() != null)
            {
                DiplomaticStatus statusToward = this.GetAIOwner().GetDiplomacy().GetStatusToward(location.GetWizardOwner());
                if (statusToward == null || !statusToward.openWar)
                {
                    this.preparationOnly = true;
                }
            }
            this.Inizialization();
        }

        public static bool IsTargetStrengthInteresting(Location location)
        {
            if (location == null)
            {
                return false;
            }
            if ((location.GetLocalGroup()?.GetValue() ?? 0) < TurnManager.GetTurnNumber() * TurnManager.GetTurnNumber() / 4)
            {
                return true;
            }
            return false;
        }

        public void Inizialization()
        {
            this.topUnitStrength = 0;
            foreach (Reference<Unit> unit in this.GetAsGroup().GetUnits())
            {
                this.topUnitStrength = Mathf.Max(this.topUnitStrength, unit.Get().GetWorldUnitValue());
            }
        }

        public bool ValidateTarget()
        {
            if (this.target == null || this.target.Get() == null)
            {
                return false;
            }
            Entity entity = this.target.Get();
            PlayerWizardAI aIOwner = this.GetAIOwner();
            if (EntityManager.GetEntity(this.target.ID) == null)
            {
                return false;
            }
            if (this.GetAssignee() != null && (!this.GetAssignee().alive || this.GetAssignee().GetUnits().Count < 1 || this.GetAIOwner().GetWareffortForGroup(this.GetAssignee()) != null))
            {
                this.targetAssignee = null;
                return false;
            }
            if (entity is Location location)
            {
                if (location.GetOwnerID() == aIOwner.GetID())
                {
                    return false;
                }
                if (location.GetOwnerID() <= 0)
                {
                    return AITarget.IsTargetStrengthInteresting(location);
                }
                DiplomaticStatus statusToward = aIOwner.GetDiplomacy().GetStatusToward(location.GetOwnerID());
                if (statusToward == null || !statusToward.openWar)
                {
                    return false;
                }
            }
            else if (entity is Group group)
            {
                if (!group.alive)
                {
                    return false;
                }
                if (group.GetOwnerID() > 0)
                {
                    DiplomaticStatus statusToward2 = aIOwner.GetDiplomacy().GetStatusToward(group.GetOwnerID());
                    if (statusToward2 == null || !statusToward2.openWar)
                    {
                        if (this.preparationOnly)
                        {
                            return true;
                        }
                        return false;
                    }
                    this.preparationOnly = false;
                }
            }
            return true;
        }

        public IEnumerator Update(bool allowForSimulation = true)
        {
            yield return this.UpdateIsTargetAchievable(allowForSimulation);
            if (this.GetAIOwner().GetMagicAndResearch().curentlyCastSpell == null)
            {
                yield return this.UpdateSpellRequest();
            }
            yield return this.UpdateSummoningCircleNeeds();
            this.UpdateSupplyTowns();
        }

        public IEnumerator UpdateIsTargetAchievable(bool allowForSimulation = true)
        {
            Group assignee = this.GetAssignee();
            if (!this.ValidateTarget() || assignee == null)
            {
                this.targetAchievable = false;
                yield break;
            }
            Group asGroup = this.GetAsGroup();
            int value = asGroup.GetValue();
            int value2 = assignee.GetValue();
            if (value2 < 1 || (float)value2 < (float)value / 2.5f || assignee.GetUnits().Count < 1)
            {
                this.targetAchievable = false;
                yield break;
            }
            if (value < 1 || (float)value2 > (float)value * 2.5f || asGroup.GetUnits().Count < 1)
            {
                this.targetAchievable = true;
                yield break;
            }
            if (!allowForSimulation)
            {
                this.targetAchievable = false;
                yield break;
            }
            Battle b = Battle.Create(assignee, asGroup);
            BattleResult battleResult = new BattleResult();
            yield return PowerEstimate.SimulatedBattle(b, 3, battleResult);
            this.targetAchievable = battleResult.aWinAveraged >= 4;
            b.Destroy();
        }

        public IEnumerator UpdateSpellRequest()
        {
            Group assignee = this.GetAssignee();
            Group asGroup = this.GetAsGroup();
            PlayerWizardAI aIOwner = this.GetAIOwner();
            this.topEfficiencySpell = null;
            if (aIOwner == null || assignee == null || asGroup == null || asGroup.GetUnits().Count < 1)
            {
                yield break;
            }
            TargetType allyUnit = (TargetType)TARGET_TYPE.UNIT_FRIENDLY;
            List<DBReference<Spell>> list = aIOwner.GetSpells().FindAll((DBReference<Spell> o) => !string.IsNullOrEmpty(o.Get().battleScript) && !string.IsNullOrEmpty(o.Get().worldScript) && o.Get().targetType == allyUnit);
            if (list.Count < 1)
            {
                yield break;
            }
            Battle battle = Battle.Create(assignee, asGroup);
            int num = 0;
            Spell spell = null;
            SpellCastData spellCastData = new SpellCastData(aIOwner, battle);
            foreach (DBReference<Spell> item in list)
            {
                foreach (BattleUnit attackerUnit in battle.attackerUnits)
                {
                    int num2;
                    if (!string.IsNullOrEmpty(item.Get().aiBattleEvaluationScript))
                    {
                        num2 = (int)ScriptLibrary.Call(item.Get().aiBattleEvaluationScript, spellCastData, attackerUnit, item.Get());
                    }
                    else
                    {
                        int battleUnitValue = attackerUnit.GetBattleUnitValue();
                        num2 = attackerUnit.GetStrategicValueForSpell(aIOwner, battle, item) - battleUnitValue;
                    }
                    if (num2 > num)
                    {
                        spell = item;
                        num = num2;
                    }
                }
            }
            this.topEfficiencySpell = spell;
            battle.Destroy();
        }

        public IEnumerator UpdateSummoningCircleNeeds()
        {
            if (this.inRangeForSummon)
            {
                yield break;
            }
            this.requiresSummoningCircle = false;
            PlayerWizardAI aIOwner = this.GetAIOwner();
            if (aIOwner == null)
            {
                yield break;
            }
            Group asGroup = this.GetAsGroup();
            global::WorldCode.Plane plane = asGroup.GetPlane();
            if (aIOwner.summoningCircle != null && aIOwner.summoningCircle.Get().GetPlane() == plane && aIOwner.summoningCircle.Get().GetDistanceTo(asGroup) < 15)
            {
                this.inRangeForSummon = true;
            }
            else
            {
                if (aIOwner.GetSpells().Find((DBReference<Spell> o) => o.Get() == (Spell)SPELL.SUMMONING_CIRCLE) == null)
                {
                    yield break;
                }
                List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(plane);
                int num = int.MaxValue;
                TownLocation townLocation = null;
                foreach (Location item in locationsOfThePlane)
                {
                    if (item is TownLocation && item.GetOwnerID() == this.owner)
                    {
                        int distanceTo = asGroup.GetDistanceTo(item);
                        if (distanceTo < 10 && distanceTo < num)
                        {
                            num = distanceTo;
                            townLocation = item as TownLocation;
                        }
                    }
                }
                if (townLocation != null)
                {
                    this.requiresSummoningCircle = true;
                    this.circleTarget = townLocation;
                }
            }
        }

        public void UpdateSupplyTowns()
        {
            if (this.targetAssignee?.Get() == null)
            {
                return;
            }
            List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.targetAssignee.Get().GetPlane());
            this.supplyTowns?.Clear();
            foreach (Location item in locationsOfThePlane)
            {
                if (item.GetOwnerID() == this.owner && item is TownLocation townLocation && townLocation.GetDistanceTo(this.targetAssignee.Get()) < 12 && townLocation.GetPopUnits() >= 3)
                {
                    if (this.supplyTowns == null)
                    {
                        this.supplyTowns = new List<Reference<TownLocation>>();
                    }
                    this.supplyTowns.Add(townLocation);
                }
            }
        }

        public void RequestAchievabilityUpdate()
        {
            if (this.nextUpdate < TurnManager.GetTurnNumber())
            {
                this.nextUpdate = TurnManager.GetTurnNumber() + Random.Range(2, 6);
            }
        }

        public bool TownCapableOfUsefullUnits(TownLocation tl)
        {
            int num = 0;
            Subrace obj = null;
            foreach (global::DBDef.Unit item in tl.PossibleUnits())
            {
                int unitStrength = BaseUnit.GetUnitStrength(item);
                if (unitStrength > num)
                {
                    obj = item;
                    num = unitStrength;
                }
            }
            if ((float)num > 0.4f * (float)this.topUnitStrength)
            {
                if (obj.GetTag(TAG.CONSTRUCTION_UNIT) > 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public void EnsureAssingee()
        {
            if (this.ignoredUntil <= TurnManager.GetTurnNumber() && this.targetAssignee?.Get() == null)
            {
                this.FindNewAssignee();
                if (this.targetAssignee?.Get() == null)
                {
                    this.ignoredUntil = TurnManager.GetTurnNumber() + Random.Range(1, 5);
                }
            }
        }

        private void FindNewAssignee()
        {
            IPlanePosition asIPlanePosition = this.GetAsIPlanePosition();
            if (asIPlanePosition == null)
            {
                return;
            }
            List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(asIPlanePosition.GetPlane());
            int num = 0;
            foreach (Group item in groupsOfPlane)
            {
                if (item.GetOwnerID() != this.owner || item.GetUnits().Count <= 0 || asIPlanePosition.GetDistanceTo(item) >= 10 || this.GetAIOwner().GetWareffortForGroup(item) != null || (item.IsHosted() && item.GetLocationHost().locationTactic != null && item.GetLocationHost().locationTactic.dangerRank > 0))
                {
                    continue;
                }
                int value = item.GetValue();
                if (value <= num)
                {
                    continue;
                }
                if (item.doomStack)
                {
                    if (this.CanGroupReachTarget(item))
                    {
                        this.targetAssignee = item;
                        num = value;
                    }
                }
                else if (num == 0 && item.IsHosted() && (!(item.GetLocationHost() is TownLocation) || (item.GetLocationHost().locationTactic != null && item.GetGroup().GetUnits().Count >= item.GetLocationHost().locationTactic.MinimumExpectedUnits())))
                {
                    num = item.GetLocationHost().SimpleStrengthOfPossibleExpedition();
                    if (num > 0)
                    {
                        this.targetAssignee = item;
                    }
                }
                else if (item.GetDesignation() != null && (item.GetDesignation().designation == AIGroupDesignation.Designation.AggressionShort || item.GetDesignation().designation == AIGroupDesignation.Designation.AggressionMedium || item.GetDesignation().designation == AIGroupDesignation.Designation.AggressionLong) && this.CanGroupReachTarget(item))
                {
                    this.targetAssignee = item;
                    num = value;
                }
            }
            if (this.targetAssignee != null && this.targetAssignee.Get().IsHosted())
            {
                Group group = this.targetAssignee.Get().GetLocationHost().FormExpedition();
                this.targetAssignee = null;
                if (group != null && group.GetUnits().Count > 0)
                {
                    this.targetAssignee = group;
                }
            }
        }

        public bool HasAssignee()
        {
            return this.targetAssignee != null;
        }

        private IPlanePosition GetAsIPlanePosition()
        {
            IPlanePosition obj = this.target?.Get() as IPlanePosition;
            if (obj == null)
            {
                Debug.LogWarning("AITarget is not IPlanePosition type!");
            }
            return obj;
        }

        private Location GetAsLocation()
        {
            return this.target?.Get() as Location;
        }

        public Group GetAsGroup()
        {
            if (this.target?.Get() is Group result)
            {
                return result;
            }
            return (this.target?.Get() as Location)?.GetLocalGroup();
        }

        private Group GetAssignee()
        {
            return this.targetAssignee?.Get();
        }

        private PlayerWizard GetOwner()
        {
            if (this._owner == null)
            {
                this._owner = GameManager.GetWizard(this.owner);
            }
            return this._owner;
        }

        private PlayerWizardAI GetAIOwner()
        {
            return this.GetOwner() as PlayerWizardAI;
        }

        private bool CanGroupReachTarget(Group g)
        {
            if (!g.alive || g.GetUnits().Count < 1)
            {
                return false;
            }
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(g.GetPlane(), g.GetPosition(), new FInt(25), g);
            if (!requestDataV.water)
            {
                requestDataV.AllowEmbarkAtCost(1);
            }
            PathfinderV2.FindPath(requestDataV);
            List<Vector3i> path = requestDataV.GetPath();
            if (path != null && path.Count > 1)
            {
                return true;
            }
            return false;
        }
    }
}
