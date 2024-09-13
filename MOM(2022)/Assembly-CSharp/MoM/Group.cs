using System.Collections;
using System.Collections.Generic;
using System.Text;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM.Adventures;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class Group : Entity, IGroup, IPlanePosition, IHashableGroup, IVisibilityChange
    {
        public enum GroupActions
        {
            None = 0,
            Guard = 1,
            Skip = 2
        }

        [ProtoIgnore]
        private bool dirty;

        [ProtoIgnore]
        private Formation mapFormation;

        [ProtoIgnore]
        public global::WorldCode.Plane plane;

        [ProtoMember(1)]
        public int ID;

        [ProtoMember(2)]
        private Vector3i position;

        [ProtoMember(3)]
        public bool alive;

        [ProtoMember(4)]
        private int wizardOwner;

        [ProtoMember(5)]
        public int hash;

        [ProtoMember(6)]
        public List<Reference<Unit>> groupUnits;

        [ProtoMember(7)]
        public AdventureTrigger advTrigger;

        [ProtoMember(8)]
        public DBReference<global::DBDef.Plane> planeSerializableReference;

        [ProtoMember(12)]
        public Reference<Location> locationHost;

        [ProtoMember(13)]
        public Reference<Location> beforeMovingAway;

        [ProtoMember(20)]
        public bool landMovement;

        [ProtoMember(21)]
        public bool waterMovement;

        [ProtoMember(22)]
        public bool mountainMovement;

        [ProtoMember(23)]
        public bool forestMovement;

        [ProtoMember(24)]
        public bool pathfinderMovement;

        [ProtoMember(25)]
        public bool nonCorporealMovement;

        [ProtoMember(26)]
        public Reference<Unit> transporter;

        [ProtoMember(27)]
        public AIGroupDesignation aiDesignation;

        [ProtoMember(28)]
        public AINeutralExpedition aiNeturalExpedition;

        [ProtoMember(29)]
        public bool virtualGroup;

        [ProtoMember(30)]
        public int valueCache;

        [ProtoMember(31)]
        public int maxMPCache;

        [ProtoMember(32)]
        public Vector3i destination = Vector3i.invalid;

        [ProtoMember(33)]
        public EngineerManager engineerManager;

        [ProtoMember(34)]
        public PurificationManager purificationManager;

        [ProtoMember(35)]
        public bool isActivelyBuilding;

        [ProtoMember(36)]
        private GroupActions action;

        [ProtoMember(37)]
        public string creationStackRecord;

        [ProtoMember(38)]
        public Vector3i positionOfOrigin;

        [ProtoMember(39)]
        public bool arcanusIsPlaneOfOrigin;

        [ProtoMember(40)]
        public bool earthWalkerMovement;

        [ProtoMember(41)]
        public bool midGameCrisis;

        [ProtoIgnore]
        public bool doomStack;

        [ProtoIgnore]
        public int doomStackPowerMiss;

        [ProtoIgnore]
        public bool blockUnitSort;

        [ProtoIgnore]
        public bool blockFormationUpdates;

        public bool Dirty
        {
            get
            {
                return this.dirty;
            }
            set
            {
                if (value)
                {
                    this.maxMPCache = -1;
                    this.valueCache = 0;
                }
                this.dirty = value;
            }
        }

        [ProtoIgnore]
        public Vector3i Position
        {
            get
            {
                return this.position;
            }
            set
            {
                Vector3i vector3i = this.position;
                this.position = this.GetPlane().area.KeepHorizontalInside(value);
                if (vector3i == Vector3i.invalid)
                {
                    this.positionOfOrigin = this.position;
                }
                if (this.GetPlane() == null)
                {
                    Debug.LogError("setPostion should only happen for unit assigned to a plane");
                    return;
                }
                this.GetPlane().UpdateUnitPosition(vector3i, this.position, this);
                foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
                {
                    if (entity.Value is IPlanePosition planePosition && planePosition != this && (planePosition is Group || planePosition is Location) && planePosition.GetPlane() == this.GetPlane() && planePosition.GetPosition() == this.GetPosition())
                    {
                        World.GetArcanus().ClearSearcherData();
                        World.GetMyrror().ClearSearcherData();
                        break;
                    }
                }
                if (!(this.beforeMovingAway != null) || !(this.position != this.beforeMovingAway.Get().GetPosition()))
                {
                    return;
                }
                if (this.beforeMovingAway.Get().otherPlaneLocation != null)
                {
                    World.GetArcanus().ClearSearcherData();
                    World.GetMyrror().ClearSearcherData();
                }
                else
                {
                    this.beforeMovingAway.Get().GetPlane().ClearSearcherData();
                }
                TownLocation townLocation = this.beforeMovingAway.Get() as TownLocation;
                this.ChangeBeforeMovingAway(null);
                if (townLocation != null)
                {
                    townLocation.SetUnrestDirty();
                    VerticalMarkerManager.Get().UpdateInfoOnMarker(townLocation);
                }
                foreach (Reference<Unit> unit in this.GetUnits())
                {
                    unit.Get().attributes.SetDirty();
                }
            }
        }

        public GroupActions Action
        {
            get
            {
                return this.action;
            }
            set
            {
                this.action = value;
                if (this.GetOwnerID() == PlayerWizard.HumanID())
                {
                    VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
                    HUD.Get()?.UpdateEndTurnButtons();
                }
            }
        }

        public Group()
        {
            MHZombieMemoryDetector.Track(this);
        }

        public Group(global::WorldCode.Plane p, int owner, bool isVirtualGroup = false)
        {
            this.maxMPCache = -1;
            this.wizardOwner = owner;
            this.position = Vector3i.invalid;
            this.plane = p;
            this.planeSerializableReference = this.plane.planeSource;
            this.virtualGroup = isVirtualGroup;
            GameManager.Get().RegisterGroup(this);
        }

        public Group(Location parent)
        {
            this.maxMPCache = -1;
            this.wizardOwner = parent.owner;
            this.position = Vector3i.invalid;
            this.plane = parent.GetPlane();
            this.arcanusIsPlaneOfOrigin = this.plane.arcanusType;
            this.planeSerializableReference = this.plane.planeSource;
            this.locationHost = new Reference<Location>();
            this.locationHost = parent;
            if (parent != null && parent.ID == 0)
            {
                Debug.LogError("invalid location assigned!");
            }
            GameManager.Get().RegisterGroup(this);
        }

        public void PreparationAfterDeserialization()
        {
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get() != null)
                {
                    unit.Get().group = this;
                }
            }
        }

        public void Destroy()
        {
            if (!this.alive)
            {
                return;
            }
            this.alive = false;
            this.DestroyMarkers();
            this.DestroyMapFormation();
            this.GetPlane().ClearUnitPosition(this.position, this.IsHosted());
            if (this.engineerManager != null)
            {
                this.engineerManager.Destroy();
            }
            if (this.purificationManager != null)
            {
                this.purificationManager.Destroy();
            }
            if (this.groupUnits != null)
            {
                for (int num = this.groupUnits.Count - 1; num >= 0; num--)
                {
                    this.groupUnits[num].Get().Destroy();
                }
            }
            if (this.GetLocationHost() != null)
            {
                this.GetLocationHost().localGroup = null;
            }
            if (FSMSelectionManager.Get().GetSelectedGroup() == this)
            {
                FSMSelectionManager.Get().Select(null, focus: false);
            }
            GameManager.Get().Unregister(this);
            MHEventSystem.TriggerEvent<Group>(this, "Destroy");
        }

        public List<Reference<Unit>> GetUnits()
        {
            if (this.groupUnits == null)
            {
                this.groupUnits = new List<Reference<Unit>>();
            }
            return this.groupUnits;
        }

        public void AddUnit(Unit u, bool updateMovementFlags = true)
        {
            if (u.group == this || u.group == this)
            {
                return;
            }
            if (this.alive && this.GetUnits().Count < 9)
            {
                this.Add(u, updateMovementFlags);
                return;
            }
            global::WorldCode.Plane plane = this.GetPlane();
            List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(plane);
            int i = 1;
            bool flag = u.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0 || u.GetAttributes().GetFinal(TAG.CAN_FLY) > 0;
            bool flag2 = u.GetAttributes().GetFinal(TAG.CAN_WALK) > 0 || u.GetAttributes().GetFinal(TAG.CAN_FLY) > 0;
            for (; i < 3; i++)
            {
                foreach (Vector3i v in HexNeighbors.GetRange(this.GetPosition(), i, i))
                {
                    Hex hexAt = plane.GetHexAt(v);
                    if (hexAt != null && (hexAt.IsLand() == flag2 || !hexAt.IsLand() == flag))
                    {
                        Group group = groupsOfPlane.Find((Group o) => o.GetPosition() == v);
                        if (group == null)
                        {
                            Group group2 = new Group(this.GetPlane(), this.GetOwnerID());
                            group2.Position = hexAt.Position;
                            group2.Add(u);
                            group2.GetMapFormation();
                            return;
                        }
                        if (group.alive && group.GetOwnerID() == this.GetOwnerID() && group.GetUnits().Count < 9)
                        {
                            group.AddUnit(u);
                            return;
                        }
                    }
                }
            }
            Debug.LogWarning("Failed to add unit, unit lost");
            u.DestroyNoGroup();
        }

        private void Add(Unit u, bool updateMovementFlags = true)
        {
            if (u.group == this)
            {
                return;
            }
            this.blockFormationUpdates = true;
            if (u.group != null)
            {
                u.group.Get().RemoveUnit(u);
            }
            u.group = this;
            if (!u.everAssignedToGroup)
            {
                if (u.group.Get().GetOwnerID() > 0)
                {
                    PlayerWizard wizard = GameManager.GetWizard(u.group.Get().GetOwnerID());
                    wizard.TriggerScripts(EEnchantmentType.NewUnitModifier, u);
                    if (wizard.unitModificationSkills != null && wizard.unitModificationSkills.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> unitModificationSkill in wizard.unitModificationSkills)
                        {
                            Skill skill = (Skill)DataBase.Get(unitModificationSkill.Key, reportMissing: false);
                            if (skill != null && (bool)ScriptLibrary.Call(unitModificationSkill.Value, u, skill, wizard))
                            {
                                u.AddSkill(skill);
                            }
                        }
                    }
                }
                GameManager.Get().TriggerScripts(EEnchantmentType.NewUnitModifier, u);
            }
            u.everAssignedToGroup = true;
            if (!this.GetUnits().Contains(u))
            {
                this.GetUnits().Add(u);
            }
            this.blockFormationUpdates = false;
            if (u.dbSource.Get() is Hero)
            {
                PlayerWizard playerWizard = u.GetWizardOwner();
                if (playerWizard != null && !playerWizard.heroes.Contains(u))
                {
                    playerWizard.heroes.Add(u);
                    if (u.heroEverAssignedToWizard > 0 && u.heroEverAssignedToWizard != playerWizard.ID)
                    {
                        Debug.LogError("Hero changed their owner! " + u.dbSource?.ToString() + " went from " + u.heroEverAssignedToWizard + " to " + playerWizard.ID);
                    }
                    u.heroEverAssignedToWizard = playerWizard.ID;
                }
            }
            this.Dirty = true;
            u.attributes.SetDirty();
            this.TriggerOnJoinScripts(u);
            if (updateMovementFlags)
            {
                this.UpdateMovementFlags();
            }
            this.UpdateMapFormation(createIfMissing: false);
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
            (this.GetLocationHost() as TownLocation)?.GroupChanged();
            this.GetPlane()?.UpdateUnitPosition(Vector3i.invalid, this.GetPosition(), this);
        }

        public void RemoveUnit(Unit leaver, bool allowGroupDestruction = true, bool updateGroup = true)
        {
            if (leaver == null)
            {
                return;
            }
            if (leaver.group == this)
            {
                leaver.group = null;
            }
            if (!this.GetUnits().Contains(leaver))
            {
                return;
            }
            this.GetUnits().Remove(leaver);
            if (this.transporter?.Get() == leaver)
            {
                this.Dirty = true;
                this.transporter = null;
            }
            if (this.mapFormation?.source == leaver)
            {
                if (this.engineerManager != null)
                {
                    this.engineerManager.Validate();
                }
                if (this.purificationManager != null)
                {
                    this.purificationManager.Validate();
                }
            }
            if (updateGroup)
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
                this.UpdateMovementFlags();
                this.Dirty = true;
            }
            foreach (Reference<Unit> unit2 in this.GetUnits())
            {
                Unit unit = unit2.Get();
                unit.GetEnchantmentManager().OnLeaveTriggers(leaver, null);
                leaver.GetEnchantmentManager().OnLeaveTriggers(unit, this.GetUnits());
                unit.GetSkillManager().OnLeaveTriggers(leaver, null);
                leaver.GetSkillManager().OnLeaveTriggers(unit, this.GetUnits());
            }
            if (allowGroupDestruction && this.GetUnits().Count == 0)
            {
                if (!this.IsHosted())
                {
                    if (this.beforeMovingAway != null && this.beforeMovingAway.Get().GetLocalGroup().GetUnits()
                        .Count == 0)
                    {
                        this.beforeMovingAway.Get().GetLocalGroup().Action = GroupActions.None;
                    }
                    this.Destroy();
                }
                else
                {
                    MHEventSystem.TriggerEvent<Group>(this, "GroupEmpty");
                }
            }
            (this.GetLocationHost() as TownLocation)?.GroupChanged();
            if (this.GetLocationHost() == null)
            {
                this.GetPlane()?.UpdateUnitPosition(Vector3i.invalid, this.GetPosition(), this);
            }
        }

        public void UpdateGroupMarkersAfterSkip()
        {
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
            this.UpdateMovementFlags();
            this.Dirty = true;
        }

        public bool IsGroupDiscoveredAndVisible()
        {
            if (this.GetPlane() != World.GetActivePlane())
            {
                return false;
            }
            return FOW.Get().IsVisible(this.GetPosition(), this.GetPlane());
        }

        public Formation GetMapFormation(bool createIfMissing = true)
        {
            this.UpdateMapFormation(createIfMissing);
            return this.mapFormation;
        }

        public void UpdateMapFormation(bool createIfMissing = true, bool forceDirty = false)
        {
            if (forceDirty)
            {
                this.Dirty = true;
            }
            if (this.locationHost != null || this.blockFormationUpdates)
            {
                return;
            }
            if (this.GetUnits() == null || this.GetUnits().Count < 1)
            {
                this.DestroyMapFormation();
                if (this.locationHost == null)
                {
                    this.Destroy();
                }
            }
            else
            {
                if (!createIfMissing && !this.Dirty)
                {
                    return;
                }
                Reference<Unit> reference = null;
                this.Dirty = false;
                if (this.transporter == null)
                {
                    List<Reference<Unit>> units = this.GetUnits();
                    bool flag = this.GetOwnerID() == PlayerWizard.HumanID();
                    Reference<Unit> reference2 = ((units.Count > 0) ? units[0] : null);
                    for (int i = 0; i < units.Count; i++)
                    {
                        Reference<Unit> reference3 = units[i];
                        if (!flag && reference3.Get().IsInvisibleUnit())
                        {
                            if (reference2 == reference3)
                            {
                                reference2 = null;
                            }
                        }
                        else if (reference2 == null)
                        {
                            reference2 = reference3;
                        }
                        else if (this.engineerManager != null)
                        {
                            if (!reference2.Get().IsEngineer() && reference3.Get().IsEngineer())
                            {
                                reference2 = reference3;
                            }
                        }
                        else if (this.purificationManager != null)
                        {
                            if (!reference2.Get().IsPurifier() && reference3.Get().IsPurifier())
                            {
                                reference2 = reference3;
                            }
                        }
                        else if (reference3.Get().dbSource.Get() is Hero || !(reference2.Get().dbSource.Get() is Hero))
                        {
                            if (!(reference2.Get().dbSource.Get() is Hero) && reference3.Get().dbSource.Get() is Hero)
                            {
                                reference2 = reference3;
                            }
                            else if (reference2.Get().GetSimpleUnitStrength() < reference3.Get().GetSimpleUnitStrength())
                            {
                                reference2 = reference3;
                            }
                        }
                    }
                    reference = reference2;
                    if (!flag)
                    {
                        this.plane.GetSearcherData().UpdateUnitPosition(this.GetPosition(), this);
                    }
                }
                else
                {
                    reference = this.transporter;
                }
                if (reference == null)
                {
                    this.DestroyMapFormation();
                    return;
                }
                if (this.mapFormation != null)
                {
                    if (this.mapFormation.source != reference.Get())
                    {
                        this.mapFormation.Destroy();
                        this.mapFormation = null;
                    }
                }
                else if (!createIfMissing)
                {
                    return;
                }
                if (this.mapFormation == null && reference != null)
                {
                    this.mapFormation = reference.Get().CreateFormation(this);
                    VerticalMarkerManager.Get().Addmarker(this);
                }
            }
        }

        public void TransferUnits(Group g)
        {
            if (g.GetOwnerID() != this.GetOwnerID())
            {
                Debug.LogError("Unit Transfer with changing owner! Between wizard " + g.GetOwnerID() + " and " + this.GetOwnerID());
            }
            List<Reference<Unit>> units = this.GetUnits();
            if (units != null && units.Count != 0)
            {
                List<Reference<Unit>> units2 = g.GetUnits();
                bool flag = units2 != null && units2.Count > 0;
                for (int num = units.Count - 1; num >= 0; num--)
                {
                    Reference<Unit> reference = units[num];
                    g.AddUnit(reference, updateMovementFlags: false);
                }
                if (!flag)
                {
                    g.Action = this.Action;
                }
                if (this.locationHost == null)
                {
                    this.Destroy();
                }
                else
                {
                    this.Action = GroupActions.None;
                }
                g.GetUnits().Reverse();
                g.UpdateMovementFlags();
            }
        }

        public void TransferUnit(Group g, Unit u)
        {
            List<Reference<Unit>> units = this.GetUnits();
            if (units != null || !units.Contains(u))
            {
                if (g.GetOwnerID() != this.GetOwnerID())
                {
                    Debug.LogError("Unit Transfer with changing owner! Between wizard " + g.GetOwnerID() + " and " + this.GetOwnerID());
                }
                this.RemoveUnit(u);
                g.AddUnit(u);
            }
        }

        public void EnsureHasMP(int mp = 1)
        {
            if (this.transporter != null)
            {
                Unit unit = this.transporter.Get();
                if (unit.Mp < mp)
                {
                    unit.Mp = new FInt(mp);
                }
                return;
            }
            foreach (Reference<Unit> unit3 in this.GetUnits())
            {
                Unit unit2 = unit3.Get();
                if (unit2.Mp < mp)
                {
                    unit2.Mp = new FInt(mp);
                }
            }
        }

        public IGroup MoveViaPath(List<Vector3i> path, bool mergeCollidedAlliedGroups, bool enterCollidedAlliedTowns = true, bool aggressive = true, bool costMP = true)
        {
            if (path == null || path.Count < 2)
            {
                return this;
            }
            if (path[0] == path[path.Count - 1])
            {
                return this;
            }
            if (this.IsHosted())
            {
                return this;
            }
            this.Action = GroupActions.None;
            Battle battle = null;
            FInt fInt = this.CurentMP();
            if (costMP && fInt <= 0)
            {
                this.destination = path[path.Count - 1];
                return this;
            }
            bool flag = false;
            if (path == null)
            {
                return this;
            }
            if (path[path.Count - 1] == this.Position)
            {
                path.Invert();
            }
            this.destination = path[path.Count - 1];
            if (costMP)
            {
                path = this.CutPathToMP(fInt, path, useMP: false, mpWastingStop: false);
            }
            if (path.Count < 2)
            {
                this.destination = Vector3i.invalid;
                return this;
            }
            IGroup group = this.CutByCollision(path);
            if (group != null)
            {
                if (group == this)
                {
                    Debug.LogWarning("//KHASH: Self colliding may be the source of many issues! Report please");
                    return this;
                }
                this.destination = Vector3i.invalid;
                if (group is Group)
                {
                    flag = true;
                }
                else if (group is Location location)
                {
                    if (!aggressive && location.GetOwnerID() != this.GetOwnerID() && location.GetGroup().GetUnits().Count > 0)
                    {
                        flag = true;
                    }
                    else if (location.GetAdventureTrigger() != null)
                    {
                        flag = true;
                    }
                    else if (location.GetLocalGroup() != null && location.GetLocalGroup().GetUnits().Count > 0)
                    {
                        flag = true;
                    }
                }
            }
            if (costMP)
            {
                path = this.CutPathToMP(fInt, path, useMP: true, flag);
            }
            if (path.Count > 0 && this.destination == path[path.Count - 1])
            {
                this.destination = Vector3i.invalid;
            }
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this, onlyIfHashChanged: true);
            if (group != null)
            {
                this.destination = Vector3i.invalid;
                if (group.GetOwnerID() == this.GetOwnerID())
                {
                    if ((this.aiDesignation?.destinationPosition?.Reached(group as IPlanePosition)).GetValueOrDefault())
                    {
                        group.ExchangeOptimizedForSelf(this);
                        this.aiDesignation.destinationPosition = null;
                        return this;
                    }
                    if (!mergeCollidedAlliedGroups && group is Group && group is Group group2 && group2.GetLocationHostSmart() == null)
                    {
                        return this;
                    }
                    if (enterCollidedAlliedTowns)
                    {
                        if (group.GetUnits().Count + this.GetUnits().Count <= 9)
                        {
                            Formation formation = this.GetMapFormation(createIfMissing: false);
                            if (formation != null && formation != null)
                            {
                                List<Vector3i> list = new List<Vector3i>(path);
                                if (group is IPlanePosition planePosition)
                                {
                                    list.Add(planePosition.GetPosition());
                                }
                                if (list.Count > 1)
                                {
                                    formation.Move(list, wastefullLastStep: true);
                                    formation.DetachFromGroup();
                                    this.mapFormation = null;
                                }
                            }
                        }
                        if (group.AddUnitsIfPossible(this.GetUnits()))
                        {
                            this.Destroy();
                            return group;
                        }
                        if (path.Count > 1)
                        {
                            if (group is IPlanePosition planePosition2 && path[path.Count - 1] == planePosition2.GetPosition())
                            {
                                path.RemoveAt(path.Count - 1);
                            }
                            this.Position = path[path.Count - 1];
                        }
                    }
                }
                else if (aggressive || (group is Location location2 && location2.GetUnits().Count == 0))
                {
                    if (group is Group)
                    {
                        if (HexCoordinates.HexDistance(this.GetPosition(), (group as Group).GetPosition()) == 1 && this.beforeMovingAway != null)
                        {
                            Location location3 = this.beforeMovingAway.Get();
                            if (FSMSelectionManager.Get().GetSelectedGroup() != this)
                            {
                                this.TransferUnits(location3.GetGroup());
                            }
                            FSMSelectionManager.Get().Select(null, focus: false);
                            battle = new Battle(location3.GetUnits(), group.GetUnits(), location3.GetOwnerID(), group.GetOwnerID());
                        }
                        else
                        {
                            FSMSelectionManager.Get().selectedUnits?.Clear();
                            battle = new Battle(this.GetUnits(), group.GetUnits(), this.GetOwnerID(), group.GetOwnerID());
                        }
                        battle.DebugMode(value: true);
                        bool landBattle = true;
                        if (group is IPlanePosition)
                        {
                            landBattle = (group as IPlanePosition).GetPlane()?.GetHexAt((group as IPlanePosition).GetPosition())?.IsLand() ?? true;
                        }
                        battle.landBattle = landBattle;
                        battle.temperature = 0.5f;
                        battle.humidity = 0.5f;
                        battle.forest = 0.28f;
                        FSMCoreGame.Get().StartBattle(battle);
                    }
                    else if (group is Location location4)
                    {
                        if (this.GetOwnerID() == PlayerWizard.HumanID())
                        {
                            location4.MarkAsExplored();
                        }
                        Group group3 = this;
                        if (HexCoordinates.HexDistance(group3.GetPosition(), location4.GetPosition()) == 1 && this.beforeMovingAway != null)
                        {
                            Location location5 = this.beforeMovingAway.Get();
                            if (FSMSelectionManager.Get().GetSelectedGroup() != this)
                            {
                                this.TransferUnits(location5.GetGroup());
                            }
                            FSMSelectionManager.Get().Select(null, focus: false);
                            group3 = location5.GetGroup();
                        }
                        AdventureTrigger adventureTrigger = location4.GetAdventureTrigger();
                        if (adventureTrigger != null)
                        {
                            FSMSelectionManager.Get().selectedUnits?.Clear();
                            Adventure a = adventureTrigger.Get();
                            AdventureData adventureData = AdventureManager.TryToTriggerAdventure(a, GameManager.GetWizard(this.GetOwnerID()), location4, group3);
                            if (adventureData != null)
                            {
                                AdventureManager.ResolveEvent(adventureData, a);
                            }
                        }
                        else
                        {
                            Location.PreBattle(group as TownLocation, group3);
                            if (location4.GetLocalGroup() != null && location4.GetLocalGroup().GetUnits().Count > 0)
                            {
                                FSMSelectionManager.Get().selectedUnits?.Clear();
                                battle = new Battle(group3.GetUnits(), location4.GetUnits(), group3.GetOwnerID(), location4.GetOwnerID());
                                battle.DebugMode(value: true);
                                bool landBattle2 = true;
                                if (group is IPlanePosition)
                                {
                                    landBattle2 = (group as IPlanePosition).GetPlane()?.GetHexAt((group as IPlanePosition).GetPosition())?.IsLand() ?? true;
                                }
                                battle.landBattle = landBattle2;
                                battle.temperature = 0.5f;
                                battle.humidity = 0.5f;
                                battle.forest = 0.38f;
                                FSMCoreGame.Get().StartBattle(battle);
                            }
                            else
                            {
                                PlayerWizard wizard = GameManager.GetWizard(group3.GetOwnerID());
                                PlayerWizard wizard2 = GameManager.GetWizard(location4.GetOwnerID());
                                if (wizard != null && wizard2 != null && (location4 is TownLocation || (location4.melding?.meldOwner ?? 0) == group3.GetOwnerID()))
                                {
                                    wizard.GetDiplomacy().Attacked(wizard2, location4.GetLocalGroup());
                                }
                                if (group is TownLocation town)
                                {
                                    if (FSMMapGame.Get().PickKeepOrRaze(group3.GetOwnerID(), town, group3))
                                    {
                                        group3.GetMapFormation(createIfMissing: false)?.InstantMove();
                                    }
                                }
                                else
                                {
                                    group.SetOwnerID(group3.GetOwnerID());
                                }
                                if (group3.GetOwnerID() == PlayerWizard.HumanID() && group3.alive && group3.GetLocationHost() == null)
                                {
                                    group3.ChangeBeforeMovingAway(group as Location);
                                }
                            }
                        }
                    }
                }
            }
            if (path.Count < 2)
            {
                MHEventSystem.TriggerEvent<Group>(this, null);
                return this;
            }
            if (this.GetOwnerID() == PlayerWizard.HumanID())
            {
                int sightRange = this.GetSightRange();
                this.GetPlane().Discover(path, sightRange);
                this.GetMapFormation()?.Move(path, flag);
                this.Position = path[path.Count - 1];
            }
            else
            {
                bool flag2 = false;
                foreach (Vector3i item in path)
                {
                    if (FOW.Get().IsVisible(item, this.GetPlane()))
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (flag2 && this.GetUnits().Count > 0)
                {
                    this.GetMapFormation()?.Move(path, flag);
                }
                else
                {
                    this.DestroyMapFormation();
                }
                this.Position = path[path.Count - 1];
            }
            MHEventSystem.TriggerEvent<Group>(this, path);
            return this;
        }

        public FInt CurentMP()
        {
            if (this.GetUnits().Count == 0)
            {
                return FInt.ZERO;
            }
            FInt fInt = FInt.MAX;
            if (this.SubsetSelected())
            {
                this.UpdateMovementFlagsBySelection();
                if (this.transporter != null)
                {
                    fInt = this.transporter.Get().Mp;
                }
                else
                {
                    foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
                    {
                        if (fInt > selectedUnit.Mp)
                        {
                            fInt = selectedUnit.Mp;
                        }
                    }
                }
                this.UpdateMovementFlags();
                return fInt;
            }
            if (this.transporter != null)
            {
                return this.transporter.Get().Mp;
            }
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (fInt > unit.Get().Mp)
                {
                    fInt = unit.Get().Mp;
                }
            }
            return fInt;
        }

        public void UseAllMP()
        {
            if (this.GetUnits() == null)
            {
                return;
            }
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                unit.Get().Mp = FInt.ZERO;
            }
        }

        public bool SubsetSelected()
        {
            if (FSMSelectionManager.Get() != null && FSMSelectionManager.Get().GetSelectedGroup() == this && FSMSelectionManager.Get().selectedUnits.Count > 0 && FSMSelectionManager.Get().selectedUnits.Count < this.GetUnits().Count)
            {
                return true;
            }
            return false;
        }

        public int GetMaxMP()
        {
            if (this.SubsetSelected())
            {
                this.UpdateMovementFlagsBySelection();
                List<Unit> selectedUnits = FSMSelectionManager.Get().selectedUnits;
                List<Reference<Unit>> list = new List<Reference<Unit>>(selectedUnits.Count);
                foreach (Unit item in selectedUnits)
                {
                    list.Add(item);
                }
                int result = ((FInt)ScriptLibrary.Call("CalculateGroupMaxMp", list, this.transporter)).ToInt();
                this.UpdateMovementFlags();
                return result;
            }
            if (this.GetUnits().Count == 0)
            {
                return 0;
            }
            if (this.maxMPCache < 0)
            {
                this.maxMPCache = ((FInt)ScriptLibrary.Call("CalculateGroupMaxMp", this.GetUnits(), this.transporter)).ToInt();
            }
            return this.maxMPCache;
        }

        public IGroup CutByCollision(List<Vector3i> path)
        {
            List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.GetPlane());
            List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.GetPlane());
            int num = 0;
            IGroup group = null;
            int i;
            for (i = 1; i < path.Count; i++)
            {
                num = i;
                Group group2 = groupsOfPlane.Find((Group o) => o.GetPosition() == path[i]);
                if (group2 != null && group2.locationHost == null)
                {
                    group = group2;
                    if (group2.GetOwnerID() != this.GetOwnerID())
                    {
                        break;
                    }
                }
                Location location = locationsOfThePlane.Find((Location o) => o.GetPosition() == path[i]);
                if (location != null)
                {
                    group = location;
                    if (location.GetOwnerID() != this.GetOwnerID())
                    {
                        break;
                    }
                }
                if (group2 == null && location == null)
                {
                    group = null;
                }
            }
            if (group != null && path.Count > num + 1)
            {
                path.RemoveRange(num + 1, path.Count - num - 1);
            }
            return group;
        }

        public List<Vector3i> CutPathToMP(FInt maxMP, List<Vector3i> path, bool useMP, bool mpWastingStop)
        {
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.GetPlane(), this.GetPosition(), this.CurentMP(), this);
            FInt zERO = FInt.ZERO;
            int num = 1;
            for (int i = 1; i < path.Count; i++)
            {
                FInt fInt = requestDataV.GetEntryCost(path[i - 1], path[i]);
                if (fInt < 0)
                {
                    fInt = FInt.ONE;
                }
                zERO += fInt;
                num++;
                if (zERO >= maxMP)
                {
                    break;
                }
            }
            if (useMP)
            {
                if (this.transporter != null)
                {
                    this.transporter.Get().Mp = FInt.Max(this.transporter.Get().Mp - zERO, 0f);
                }
                else
                {
                    foreach (Reference<Unit> unit in this.GetUnits())
                    {
                        unit.Get().Mp = FInt.Max(unit.Get().Mp - zERO, 0f);
                    }
                }
                this.UpdateHash();
            }
            if (mpWastingStop)
            {
                num--;
            }
            if (num <= 0)
            {
                path.Clear();
                return path;
            }
            return path.GetRange(0, num);
        }

        public void NewTurn()
        {
            bool flag = false;
            for (int num = this.GetUnits().Count - 1; num >= 0; num--)
            {
                Unit unit = this.GetUnits()[num].Get();
                if (!(unit.group != this))
                {
                    flag = unit.NewTurn() || flag;
                }
            }
            this.UpdateHash();
            bool onlyIfHashChanged = this.purificationManager == null && this.engineerManager == null;
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this, onlyIfHashChanged);
            if (flag)
            {
                this.Dirty = true;
                this.UpdateMapFormation(createIfMissing: false);
            }
        }

        public void RegainTo1MP()
        {
            for (int num = this.GetUnits().Count - 1; num >= 0; num--)
            {
                this.GetUnits()[num].Get().RegainTo1MP();
            }
        }

        public int GetOwnerID()
        {
            if (this.locationHost != null)
            {
                return this.GetLocationHost().GetOwnerID();
            }
            return this.wizardOwner;
        }

        public Vector3i GetPosition()
        {
            Location location = this.GetLocationHost();
            if (this.locationHost != null && location == null)
            {
                Debug.Log("Null pos");
            }
            if (this.locationHost != null)
            {
                return this.GetLocationHost().GetPosition();
            }
            return this.Position;
        }

        public global::WorldCode.Plane GetPlane()
        {
            if (this.plane == null)
            {
                global::DBDef.Plane p = this.planeSerializableReference.Get();
                this.plane = World.GetPlanes().Find((global::WorldCode.Plane o) => o.planeSource.Get() == p);
            }
            return this.plane;
        }

        public AdventureTrigger GetAdventureTrigger()
        {
            return this.advTrigger;
        }

        public void SetOwnerID(int id, int attackerID = -1, bool additionalLosses = false)
        {
            this.wizardOwner = id;
        }

        public void DestroyMapFormation()
        {
            if (this.mapFormation != null)
            {
                this.mapFormation.Destroy();
                this.mapFormation = null;
            }
        }

        public int GetHash()
        {
            return this.hash;
        }

        public void UpdateHash()
        {
            List<Reference<Unit>> units = this.GetUnits();
            if (units != null)
            {
                this.hash = this.ID + this.CurentMP().storage << 6 + units.Count << 8;
            }
            else
            {
                this.hash = this.ID;
            }
            if (this.locationHost != null)
            {
                this.GetLocationHost().UpdateHash();
            }
        }

        public int GetValue()
        {
            if (this.valueCache == 0)
            {
                List<Reference<Unit>> units = this.GetUnits();
                List<int> list = new List<int>();
                foreach (Reference<Unit> item in units)
                {
                    if (this.GetOwnerID() > 0)
                    {
                        list.Add(item.Get().GetWorldUnitValue());
                    }
                    else
                    {
                        list.Add(item.Get().GetSimpleUnitStrength());
                    }
                }
                list.Sort();
                for (int num = list.Count - 1; num >= 0; num--)
                {
                    int num2 = list[num] + list[num] / (num + 1);
                    this.valueCache += num2 / 2;
                }
            }
            return this.valueCache;
        }

        public int GetSightRange()
        {
            int iSightRange = DifficultySettingsData.GetSettingAsInt("UI_BASE_SIGHT_RANGE");
            if (iSightRange < 1)
                iSightRange = 1;

            return iSightRange + this.GetSightRangeBonus();
        }

        public int GetSightRangeBonus()
        {
            int num = 0;
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                FInt final = unit.Get().attributes.GetFinal(TAG.SIGHT_RANGE_BONUS);
                if (final > num)
                {
                    num = final.ToInt();
                }
            }
            return num;
        }

        public bool AnyUnitHasTag(TAG t)
        {
            if (this.SubsetSelected())
            {
                foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
                {
                    if (selectedUnit.GetAttributes().GetFinal(t) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get().GetAttributes().GetFinal(t) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool AllUnitsHasTag(TAG t)
        {
            if (this.SubsetSelected())
            {
                foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
                {
                    if (selectedUnit.GetAttributes().GetFinal(t) <= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get().GetAttributes().GetFinal(t) <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void UpdateSelectedMovementWater()
        {
            if (this.transporter != null)
            {
                if (this.transporter.Get().CanTravelOverWater())
                {
                    this.waterMovement = true;
                }
                else
                {
                    this.waterMovement = false;
                }
                return;
            }
            foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
            {
                if (!selectedUnit.CanTravelOverWater())
                {
                    this.waterMovement = false;
                    return;
                }
            }
            this.waterMovement = true;
        }

        public void UpdateMovementWater()
        {
            if (this.transporter != null)
            {
                if (this.transporter.Get().CanTravelOverWater())
                {
                    this.waterMovement = true;
                }
                else
                {
                    this.waterMovement = false;
                }
                return;
            }
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (!unit.Get().CanTravelOverWater())
                {
                    this.waterMovement = false;
                    return;
                }
            }
            this.waterMovement = true;
        }

        public bool CanSubsetTravelWater(List<Reference<Unit>> list, Unit exceptThisUnit = null)
        {
            if (list == null)
            {
                return false;
            }
            bool result = true;
            foreach (Reference<Unit> item in list)
            {
                if (item.Get() != exceptThisUnit)
                {
                    if (!item.Get().CanTravelOverWater())
                    {
                        result = false;
                    }
                    else if (item.Get().GetAttributes().GetFinal(TAG.TRANSPORTER) > 0)
                    {
                        return true;
                    }
                }
            }
            return result;
        }

        public void UpdateSelectedMovementLand()
        {
            if (this.transporter != null)
            {
                if (this.transporter.Get().CanTravelOverLand())
                {
                    this.landMovement = true;
                }
                else
                {
                    this.landMovement = false;
                }
                return;
            }
            foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
            {
                if (!selectedUnit.CanTravelOverLand())
                {
                    this.landMovement = false;
                    return;
                }
            }
            this.landMovement = true;
        }

        public void UpdateMovementLand()
        {
            if (this.transporter != null)
            {
                if (this.transporter.Get().CanTravelOverLand())
                {
                    this.landMovement = true;
                }
                else
                {
                    this.landMovement = false;
                }
                return;
            }
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (!unit.Get().CanTravelOverLand())
                {
                    this.landMovement = false;
                    return;
                }
            }
            this.landMovement = true;
        }

        public void UpdateSelectedMovementNonCorporeal()
        {
            if (this.transporter != null)
            {
                if (this.transporter.Get().CanFly())
                {
                    this.nonCorporealMovement = true;
                }
                else
                {
                    this.nonCorporealMovement = false;
                }
                return;
            }
            foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
            {
                if (!selectedUnit.CanFly())
                {
                    this.nonCorporealMovement = false;
                    return;
                }
            }
            this.nonCorporealMovement = true;
        }

        public void UpdateMovementNonCorporeal()
        {
            if (this.transporter != null)
            {
                if (this.transporter.Get().CanFly())
                {
                    this.nonCorporealMovement = true;
                }
                else
                {
                    this.nonCorporealMovement = false;
                }
                return;
            }
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (!unit.Get().CanFly())
                {
                    this.nonCorporealMovement = false;
                    return;
                }
            }
            this.nonCorporealMovement = true;
        }

        public void UpdateSelectedMovementMountain()
        {
            foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
            {
                if (selectedUnit.GetAttributes().GetFinal(TAG.MOUNTAINEER) > 0 || selectedUnit.GetAttributes().GetFinal(TAG.PATHFINDING) > 0)
                {
                    this.mountainMovement = true;
                    return;
                }
            }
            this.mountainMovement = false;
        }

        public void UpdateMovementMountain()
        {
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get().GetAttributes().GetFinal(TAG.MOUNTAINEER) > 0 || unit.Get().GetAttributes().GetFinal(TAG.PATHFINDING) > 0)
                {
                    this.mountainMovement = true;
                    return;
                }
            }
            this.mountainMovement = false;
        }

        public void UpdateSelectedMovementForest()
        {
            foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
            {
                if (selectedUnit.GetAttributes().GetFinal(TAG.FORESTER) > 0)
                {
                    this.forestMovement = true;
                    return;
                }
            }
            this.forestMovement = false;
        }

        public void UpdateMovementForest()
        {
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get().GetAttributes().GetFinal(TAG.FORESTER) > 0)
                {
                    this.forestMovement = true;
                    return;
                }
            }
            this.forestMovement = false;
        }

        public void UpdateSelectedMovementEarthWalker()
        {
            foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
            {
                if (selectedUnit.GetAttributes().GetFinal(TAG.EARTH_WALKER) > 0)
                {
                    this.earthWalkerMovement = true;
                    return;
                }
            }
            this.earthWalkerMovement = false;
        }

        public void UpdateMovementEarthWalker()
        {
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get().GetAttributes().GetFinal(TAG.EARTH_WALKER) > 0)
                {
                    this.earthWalkerMovement = true;
                    return;
                }
            }
            this.earthWalkerMovement = false;
        }

        public void UpdateSelectedMovementPathfinder()
        {
            foreach (Unit selectedUnit in FSMSelectionManager.Get().selectedUnits)
            {
                if (selectedUnit.GetAttributes().GetFinal(TAG.PATHFINDING) > 0)
                {
                    this.pathfinderMovement = true;
                    return;
                }
            }
            this.pathfinderMovement = false;
        }

        public void UpdateMovementPathfinder()
        {
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get().GetAttributes().GetFinal(TAG.PATHFINDING) > 0)
                {
                    this.pathfinderMovement = true;
                    return;
                }
            }
            this.pathfinderMovement = false;
        }

        public void UpdateSelectedHaveTransporter(List<Unit> list = null)
        {
            if (list == null)
            {
                list = FSMSelectionManager.Get().selectedUnits;
            }
            this.transporter = null;
            foreach (Unit item in list)
            {
                if (!item.GetAttributes().Contains(TAG.TRANSPORTER))
                {
                    continue;
                }
                if (this.transporter != null)
                {
                    if (this.transporter.Get().Mp > item.Mp)
                    {
                        this.transporter = item;
                    }
                }
                else
                {
                    this.transporter = item;
                }
            }
        }

        public void UpdateHaveTransporter()
        {
            Reference<Unit> reference = this.transporter;
            this.transporter = null;
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (!unit.Get().GetAttributes().Contains(TAG.TRANSPORTER))
                {
                    continue;
                }
                if (this.transporter != null)
                {
                    if (this.transporter.Get().Mp > unit.Get().Mp)
                    {
                        this.transporter = unit;
                    }
                }
                else
                {
                    this.transporter = unit;
                }
            }
            if (reference != this.transporter)
            {
                this.UpdateMapFormation(createIfMissing: false);
            }
        }

        public void UpdateMovementFlagsBySelection()
        {
            if (!(this.locationHost != null))
            {
                if (FSMSelectionManager.Get() != null && FSMSelectionManager.Get().GetSelectedGroup() == this && FSMSelectionManager.Get().selectedUnits.Count > 0)
                {
                    this.UpdateSelectedHaveTransporter();
                    this.UpdateSelectedMovementPathfinder();
                    this.UpdateSelectedMovementLand();
                    this.UpdateSelectedMovementWater();
                    this.UpdateSelectedMovementNonCorporeal();
                    this.UpdateSelectedMovementMountain();
                    this.UpdateSelectedMovementForest();
                    this.UpdateSelectedMovementEarthWalker();
                }
                else
                {
                    this.UpdateMovementFlags();
                }
            }
        }

        public void UpdateMovementFlags()
        {
            if (!(this.locationHost != null))
            {
                this.UpdateHaveTransporter();
                this.UpdateMovementPathfinder();
                this.UpdateMovementLand();
                this.UpdateMovementWater();
                this.UpdateMovementNonCorporeal();
                this.UpdateMovementMountain();
                this.UpdateMovementForest();
                this.UpdateMovementEarthWalker();
            }
        }

        public void ForceUpdateMovementFlags()
        {
            this.UpdateHaveTransporter();
            this.UpdateMovementPathfinder();
            this.UpdateMovementLand();
            this.UpdateMovementWater();
            this.UpdateMovementNonCorporeal();
            this.UpdateMovementMountain();
            this.UpdateMovementForest();
            this.UpdateMovementEarthWalker();
        }

        public bool IsMarkerVisible()
        {
            return FOW.Get().IsVisible(this.GetPosition(), this.GetPlane());
        }

        public bool IsModelVisible()
        {
            return FOW.Get().IsVisible(this.GetPosition(), this.GetPlane());
        }

        public void UpdateMarkers()
        {
            if (!(this.locationHost != null) && this.alive)
            {
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
                TerrainMarkers markers_ = this.GetPlane().GetMarkers_();
                if (this.GetOwnerID() == PlayerWizard.HumanID())
                {
                    markers_.SetBasicMarker(this.GetPosition(), TerrainMarkers.MarkerType.Friendly, visible: true);
                }
                else
                {
                    markers_.SetBasicMarker(this.GetPosition(), TerrainMarkers.MarkerType.Friendly, visible: false);
                }
            }
        }

        public void DestroyMarkers()
        {
            if (!(this.locationHost != null))
            {
                VerticalMarkerManager.Get().DestroyMarker(this);
                TerrainMarkers markers_ = this.GetPlane().GetMarkers_();
                if (this.GetOwnerID() == PlayerWizard.HumanID())
                {
                    markers_.SetBasicMarker(this.GetPosition(), TerrainMarkers.MarkerType.Friendly, visible: false);
                }
            }
        }

        public void UpdateGroupUnits()
        {
            List<Reference<Unit>> units = this.GetUnits();
            for (int num = units.Count - 1; num >= 0; num--)
            {
                if (units[num].Get().figureCount <= 0)
                {
                    units[num].Get().Destroy();
                }
            }
            if (units.Count == 0 && this.locationHost == null)
            {
                this.Destroy();
            }
        }

        public bool BuildTown()
        {
            if (!this.CanBuildTown())
            {
                return false;
            }
            Unit settler = null;
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                if (unit.Get().IsSettler())
                {
                    settler = unit.Get();
                    break;
                }
            }
            Town town = DataBase.GetType<Town>().Find((Town o) => o.race == settler.dbSource.Get().race);
            if (town == null)
            {
                Debug.LogError("Town location have not been found");
            }
            else
            {
                TownLocation townLocation = TownLocation.CreateLocation(this.GetPosition(), this.GetPlane(), town, 0, this.GetOwnerID());
                townLocation.UpdateVisibility();
                settler.Destroy();
                Unit u = Unit.CreateFrom(PowerEstimate.GetList().Find((Multitype<BattleUnit, int> o) => o.t0.race == settler.dbSource.Get().race && o.t0.GetAttFinal(TAG.TOWN_STARTING_DEFENDER) > 0 && DataBase.GetType<global::DBDef.Unit>().Contains(o.t0.dbSource.Get() as global::DBDef.Unit)).t0.dbSource);
                if (this.GetOwnerID() > 0)
                {
                    PlayerWizard wizard = GameManager.GetWizard(this.GetOwnerID());
                    if (wizard != null && wizard.townExtraBuilding != null && wizard.townExtraBuilding.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> item in wizard.townExtraBuilding)
                        {
                            DBClass dBClass = DataBase.Get(item.Key, reportMissing: false);
                            if (dBClass != null && (bool)ScriptLibrary.Call(item.Value, townLocation, (Building)dBClass, wizard))
                            {
                                townLocation.AddBuilding((Building)dBClass);
                            }
                        }
                    }
                }
                townLocation.AddUnit(u);
            }
            return true;
        }

        public bool CanBuildTown()
        {
            if (this.GetUnits() == null)
            {
                return false;
            }
            if (this.GetUnits().Find((Reference<Unit> o) => o.Get().IsSettler()) != null)
            {
                return Location.CanBuildTownAtLocation(this.GetPlane(), this.GetPosition()) == null;
            }
            return false;
        }

        public Location GetLocationHostSmart()
        {
            if (this.locationHost == null)
            {
                return this.beforeMovingAway;
            }
            return this.locationHost;
        }

        public Location GetLocationHost()
        {
            return this.locationHost;
        }

        public bool IsHosted()
        {
            if (this.locationHost != null)
            {
                return true;
            }
            return false;
        }

        public override int GetID()
        {
            return this.ID;
        }

        public override void SetID(int id)
        {
            this.ID = id;
        }

        public int PathToTurns(List<Vector3i> path, RequestDataV2 rd = null)
        {
            if (path == null || path.Count < 2)
            {
                return 0;
            }
            global::WorldCode.Plane p = this.GetPlane();
            FInt fInt = FInt.ZERO;
            int num = 0;
            if (rd == null)
            {
                rd = RequestDataV2.CreateRequest(p, this.GetPosition(), Vector3i.zero, this);
            }
            for (int i = 1; i < path.Count; i++)
            {
                if (fInt <= 0)
                {
                    fInt = new FInt(this.GetMaxMP());
                    num++;
                }
                FInt fInt2 = rd.GetEntryCost(path[i - 1], path[i]);
                if (fInt2 < 0)
                {
                    fInt2 = FInt.ONE;
                }
                fInt -= fInt2;
            }
            return num;
        }

        public int StepsThisTurn(List<Vector3i> path, RequestDataV2 rd = null)
        {
            if (path == null || path.Count < 2)
            {
                return 0;
            }
            if (rd == null)
            {
                rd = RequestDataV2.CreateRequest(this.plane, this.GetPosition(), Vector3i.zero, this);
            }
            FInt fInt = this.CurentMP();
            int num = 0;
            while (num + 1 < path.Count && !(fInt <= 0))
            {
                num++;
                FInt fInt2 = rd.GetEntryCost(path[num - 1], path[num]);
                if (fInt2 < 0)
                {
                    fInt2 = FInt.ONE;
                }
                fInt -= fInt2;
            }
            return num;
        }

        public AIGroupDesignation GetDesignation(bool createIfMissing = true)
        {
            if (createIfMissing && this.aiDesignation == null && this.GetOwnerID() > PlayerWizard.HumanID() && this.GetLocationHostSmart() == null)
            {
                this.aiDesignation = new AIGroupDesignation(this);
            }
            return this.aiDesignation;
        }

        public Vector3 GetPhysicalPosition()
        {
            if (this.plane != null)
            {
                Chunk chunkFor = this.plane.GetChunkFor(this.GetPosition());
                Vector3 vector = HexCoordinates.HexToWorld3D(this.GetPosition());
                return chunkFor.go.transform.position + vector;
            }
            return Vector3.zero;
        }

        public void UpateSearcherPositionData()
        {
            this.GetPlane().GetSearcherData().UpdateGroupToPosition(this.GetPosition(), this);
        }

        public bool IsGroupInvisible()
        {
            List<Reference<Unit>> units = this.GetUnits();
            if (units.Count == 0)
            {
                return false;
            }
            foreach (Reference<Unit> item in units)
            {
                if (!item.Get().IsInvisibleUnit())
                {
                    return false;
                }
            }
            return true;
        }

        public bool EngineerAtWork()
        {
            if (this.engineerManager != null)
            {
                if (this.engineerManager.pathToBuildRoadsOn != null && this.engineerManager.pathToBuildRoadsOn.Count > 0)
                {
                    return this.engineerManager.pathToBuildRoadsOn[0] == this.GetPosition();
                }
                return false;
            }
            return false;
        }

        public void SetActivelyBuilding(bool value)
        {
            this.isActivelyBuilding = value;
            if (this.mapFormation != null)
            {
                this.mapFormation.SetAcivelyBuilding(value);
            }
            this.UpdateMapFormation();
            VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
        }

        public Group PlaneSwitch(List<Unit> selectedUnits = null)
        {
            if (this.GetLocationHostSmart() != null && this.GetLocationHostSmart().otherPlaneLocation != null)
            {
                if (this.GetOwnerID() == PlayerWizard.HumanID())
                {
                    global::WorldCode.Plane otherPlane = World.GetOtherPlane(World.GetActivePlane());
                    World.ActivatePlane(otherPlane);
                    Location g = ((this.GetLocationHostSmart().GetPlane() == otherPlane) ? this.GetLocationHostSmart() : this.GetLocationHostSmart().otherPlaneLocation.Get());
                    FSMSelectionManager.Get().Select(g, focus: true);
                }
                return this;
            }
            Group group = GameManager.GetGroupsOfPlane(World.GetOtherPlane(this.GetPlane())).Find((Group o) => o.GetPosition() == this.GetPosition() && o.GetOwnerID() == this.GetOwnerID());
            if (group == null)
            {
                group = new Group(World.GetOtherPlane(this.GetPlane()), this.GetOwnerID());
            }
            group.Position = this.GetPosition();
            if (selectedUnits != null && selectedUnits.Count < this.GetUnits().Count)
            {
                for (int num = selectedUnits.Count - 1; num >= 0; num--)
                {
                    this.TransferUnit(group, selectedUnits[num]);
                }
            }
            else
            {
                this.TransferUnits(group);
            }
            if (this.aiDesignation != null)
            {
                group.aiDesignation = this.aiDesignation;
                group.aiDesignation.owner = group;
                this.aiDesignation = null;
            }
            if (this.aiNeturalExpedition != null)
            {
                Debug.Log("Unexpected use for neutral armies!");
                group.aiNeturalExpedition = this.aiNeturalExpedition;
                group.aiNeturalExpedition.owner = group;
                this.aiNeturalExpedition = null;
            }
            if (group.GetOwnerID() == PlayerWizard.HumanID())
            {
                World.ActivatePlane(group.GetPlane());
                FSMSelectionManager.Get().Select(group, focus: true);
            }
            group.GetMapFormation();
            return group;
        }

        public string GetGroupString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Group info");
            stringBuilder.AppendLine("ID " + this.ID + ", pos " + this.GetPosition().ToString() + ", arcanus " + this.GetPlane().arcanusType);
            stringBuilder.AppendLine("-Owner " + this.wizardOwner);
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                stringBuilder.AppendLine("-Unit ID:" + unit.ID + ", " + unit.Get().dbSource.dbName);
            }
            return stringBuilder.ToString();
        }

        public string GetReportForLog()
        {
            return $"Group {this.GetPosition()}({this.GetPlane().arcanusType}) Value: {this.GetUnits().Count}({this.GetValue()}) Designation: {this.GetDesignation(createIfMissing: false)?.designation} with target {this.GetDesignation(createIfMissing: false)?.destinationPosition?.position}";
        }

        public void ForcedToMoveOut()
        {
            if (this.GetUnits() == null || this.GetUnits().Count < 1)
            {
                return;
            }
            Group group = this;
            if (this.IsHosted())
            {
                Group group2 = new Group(World.GetActivePlane(), this.GetOwnerID());
                group2.Position = this.GetPosition();
                this.TransferUnits(group2);
                if (this.GetLocationHostSmart().IsModelVisible())
                {
                    group2.GetMapFormation();
                }
                group = group2;
            }
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.GetPlane(), this.GetPosition(), new FInt(1), group);
            PathfinderV2.FindArea(requestDataV);
            List<Vector3i> area = requestDataV.GetArea();
            if (area != null)
            {
                List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.plane);
                Vector3i vector3i = Vector3i.invalid;
                foreach (Vector3i v in area)
                {
                    if (groupsOfPlane.Find((Group o) => o.GetPosition() == v) == null)
                    {
                        vector3i = v;
                        break;
                    }
                }
                if (vector3i != Vector3i.invalid)
                {
                    group.MoveViaPath(new List<Vector3i>
                    {
                        this.GetPosition(),
                        vector3i
                    }, mergeCollidedAlliedGroups: true, enterCollidedAlliedTowns: true, aggressive: true, costMP: false);
                    return;
                }
            }
            group.Destroy();
        }

        public IEnumerator AIMoveOut(bool guaranteeMP, Unit unit, Destination destination, AIGroupDesignation.Designation designation, List<Vector3i> path)
        {
            Group g = this;
            if (this.IsHosted() || this.GetUnits().Count > 1)
            {
                g = new Group(g.GetPlane(), g.GetOwnerID());
                g.AddUnit(unit);
                if (guaranteeMP)
                {
                    unit.RegainTo1MP();
                }
            }
            g.Position = this.GetPosition();
            if (g.IsModelVisible())
            {
                g.GetMapFormation();
            }
            if (path != null)
            {
                g.MoveViaPath(path, mergeCollidedAlliedGroups: true, enterCollidedAlliedTowns: true, aggressive: false);
                while (!GameManager.Get().IsFocusFree())
                {
                    yield return null;
                }
            }
            if (designation != 0)
            {
                g.GetDesignation().NewDesignation(designation, destination);
                yield return PlayerWizardAI.MoveGroup(g, destination.position, destination.arcanus);
            }
        }

        public void TriggerOnJoinScripts(Unit u)
        {
            this.blockUnitSort = true;
            foreach (Reference<Unit> unit2 in this.GetUnits())
            {
                Unit unit = unit2.Get();
                if (unit == u)
                {
                    unit.GetEnchantmentManager().OnJoinTriggers(unit, this.GetUnits());
                    unit.GetSkillManager().OnJoinTriggers(unit, this.GetUnits());
                    continue;
                }
                unit.GetEnchantmentManager().OnJoinTriggers(u, this.GetUnits());
                u.GetEnchantmentManager().OnJoinTriggers(unit, this.GetUnits());
                unit.GetSkillManager().OnJoinTriggers(u, this.GetUnits());
                u.GetSkillManager().OnJoinTriggers(unit, this.GetUnits());
            }
            this.blockUnitSort = false;
        }

        public Group GetGroup()
        {
            return this;
        }

        public void TakeOverLocation(Location loc)
        {
            TownLocation obj = loc as TownLocation;
            Vector3i vector3i = loc.GetPosition();
            global::WorldCode.Plane p = loc.GetPlane();
            if (obj != null && this.aiNeturalExpedition != null)
            {
                if (this.aiNeturalExpedition.spawnLocation?.Get() == null)
                {
                    if (Random.Range(0, 100) < 50)
                    {
                        loc.SetOwnerID(0);
                        loc.Destroy();
                        List<global::DBDef.Location> type = DataBase.GetType<global::DBDef.Location>();
                        List<global::DBDef.Location> list = null;
                        list = ((TurnManager.GetTurnNumber() < 100) ? type.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.WeakLair) : ((TurnManager.GetTurnNumber() >= 250) ? type.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.StrongLair) : type.FindAll((global::DBDef.Location o) => o.locationType == ELocationType.Ruins)));
                        if (list != null)
                        {
                            int index = Random.Range(0, list.Count);
                            global::DBDef.Location source = list[index];
                            Location.CreateLocation(vector3i, p, source, 0);
                        }
                    }
                    else
                    {
                        PlayerWizard playerWizard = loc.GetWizardOwner();
                        if (playerWizard != null)
                        {
                            playerWizard.TakeFame(loc.LoseFame());
                            playerWizard.money = Mathf.Max(0, playerWizard.money - loc.ConquerGold());
                        }
                    }
                    return;
                }
                List<Reference<Unit>> units = loc.GetUnits();
                if (units.Count > 0)
                {
                    for (int num = units.Count - 1; num >= 0; num--)
                    {
                        units[num].Get().Destroy();
                    }
                }
                this.RegainTo1MP();
            }
            else
            {
                if (this.aiDesignation == null)
                {
                    return;
                }
                List<Reference<Unit>> units2 = loc.GetUnits();
                if (units2.Count > 0)
                {
                    for (int num2 = units2.Count - 1; num2 >= 0; num2--)
                    {
                        units2[num2].Get().Destroy();
                    }
                }
                this.RegainTo1MP();
            }
        }

        public bool SharePositionWith(Group g)
        {
            if (g.GetPlane() == this.GetPlane())
            {
                return g.GetPosition() == this.GetPosition();
            }
            return false;
        }

        public void ChangeBeforeMovingAway(Location g)
        {
            if (g == null || EntityManager.GetEntity(g.ID) != null)
            {
                this.beforeMovingAway = g;
            }
        }

        public int GetSimpleStrength()
        {
            int num = 0;
            foreach (Reference<Unit> unit in this.GetUnits())
            {
                num += unit.Get().GetSimpleUnitStrength();
            }
            return num;
        }

        public void KickOutOneUnit()
        {
            if (this.GetUnits().Count == 1)
            {
                this.SellUnit(this.GetUnits()[0]);
            }
            if (this.GetUnits().Find((Reference<Unit> o) => o.Get().IsSettler()) != null)
            {
                this.KickOutSettler();
            }
            Unit unit = null;
            foreach (Reference<Unit> unit2 in this.GetUnits())
            {
                if ((unit == null || unit.GetWorldUnitValue() > unit2.Get().GetWorldUnitValue()) && !unit2.Get().IsHero())
                {
                    unit = unit2;
                }
            }
            this.SellUnit(unit);
        }

        public Unit KickOutSettler()
        {
            Reference<Unit> reference = this.GetUnits().Find((Reference<Unit> o) => o.Get().IsSettler());
            if (reference == null)
            {
                return null;
            }
            this.KickOutUnit(reference, AIGroupDesignation.Designation.Settler);
            return reference;
        }

        public Unit KickOutEngineer()
        {
            Reference<Unit> reference = this.GetUnits().Find((Reference<Unit> o) => o.Get().IsEngineer());
            if (reference == null)
            {
                return null;
            }
            this.KickOutUnit(reference, AIGroupDesignation.Designation.Engineer);
            return reference;
        }

        public Unit KickOutMelder()
        {
            Reference<Unit> reference = this.GetUnits().Find((Reference<Unit> o) => o.Get().IsMelder());
            if (reference == null)
            {
                return null;
            }
            this.KickOutUnit(reference, AIGroupDesignation.Designation.Melder);
            return reference;
        }

        public void KickOutUnit(Unit u, AIGroupDesignation.Designation d)
        {
            GameManager.GetWizard(this.GetOwnerID());
            bool flag = this.GetUnits().Count > 1 || this.IsHosted();
            Reference<Unit> reference = this.GetUnits().Find((Reference<Unit> o) => o.Get() == u);
            if (!(reference != null))
            {
                return;
            }
            Group group = this;
            if (flag)
            {
                group = new Group(this.GetPlane(), this.GetOwnerID());
                group.Position = this.GetPosition();
                group.AddUnit(reference);
                if (group.IsModelVisible())
                {
                    group.GetMapFormation();
                }
                group.RegainTo1MP();
            }
            group.GetDesignation().NewDesignation(d, null);
        }

        public void SellUnit(Reference<Unit> reference)
        {
            if (reference == null || reference.Get() == null)
            {
                return;
            }
            if (reference.Get().dbSource.Get() is global::DBDef.Unit unit)
            {
                PlayerWizard wizard = GameManager.GetWizard(this.GetOwnerID());
                if (wizard != null)
                {
                    wizard.money += unit.constructionCost;
                }
            }
            reference.Get().Destroy();
        }
    }
}
