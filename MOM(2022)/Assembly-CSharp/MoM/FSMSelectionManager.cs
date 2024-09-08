namespace MOM
{
    using HutongGames.PlayMaker;
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using WorldCode;

    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMSelectionManager : FSMStateBase
    {
        private static FSMSelectionManager instance;
        private IPlanePosition selectedGroup;
        public List<Unit> selectedUnits = new List<Unit>();
        private IPlanePosition lastOwnSelection;
        public bool roadBuildingMode;

        public static FSMSelectionManager Get()
        {
            return instance;
        }

        public IPlanePosition GetSelectedGroup()
        {
            return this.selectedGroup;
        }

        private void NewTurn(object sender, object e)
        {
            if ((e as string) == "Turn")
            {
                IGroup selectedGroup = this.selectedGroup as IGroup;
                if ((selectedGroup == null) || (selectedGroup.GetOwnerID() != PlayerWizard.HumanID()))
                {
                    if ((this.lastOwnSelection != null) && GameManager.GroupAlive(this.lastOwnSelection))
                    {
                        World.ActivatePlane(this.lastOwnSelection.GetPlane(), false);
                        if (!HUD.Get().NextPlayersArmyWithoutOrders())
                        {
                            CameraController.CenterAt(this.lastOwnSelection.GetPosition(), false, 0f);
                        }
                    }
                    else
                    {
                        NewTurnCameraCenter(false);
                    }
                }
            }
        }

        public static void NewTurnCameraCenter(bool forceInstant)
        {
            using (List<Location>.Enumerator enumerator = GameManager.Get().registeredLocations.GetEnumerator())
            {
                while (true)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    Location current = enumerator.Current;
                    if (current.GetOwnerID() == PlayerWizard.HumanID())
                    {
                        World.ActivatePlane(current.GetPlane(), false);
                        CameraController.CenterAt(current.GetPosition(), forceInstant, 0f);
                        return;
                    }
                }
            }
            foreach (Group group in GameManager.Get().registeredGroups)
            {
                if (group.GetOwnerID() == PlayerWizard.HumanID())
                {
                    World.ActivatePlane(group.GetPlane(), false);
                    CameraController.CenterAt(group.GetPosition(), forceInstant, 0f);
                    break;
                }
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            instance = this;
            this.selectedGroup = null;
            this.selectedUnits.Clear();
            this.lastOwnSelection = null;
            NewTurnCameraCenter(true);
            MHEventSystem.RegisterListener<TurnManager>(new EventFunction(this.NewTurn), this);
        }

        public override void OnExit()
        {
            base.OnExit();
            instance = null;
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if ((this.selectedUnits != null) && (this.selectedUnits.Count > 0))
            {
                bool flag = false;
                using (List<Unit>.Enumerator enumerator = this.selectedUnits.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.group != null)
                        {
                            continue;
                        }
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.Unselect(false);
                }
            }
        }

        public void Select(IPlanePosition g, bool focus)
        {
            if (!TurnManager.Get(false).playerTurn)
            {
                g = null;
            }
            SetRoadPathMode(false);
            Group group = g as Group;
            if ((group == null) || ((group.GetUnits() == null) || (!group.IsGroupInvisible() || (group.GetOwnerID() == PlayerWizard.HumanID()))))
            {
                if (ReferenceEquals(g, this.selectedGroup))
                {
                    if (focus && (g != null))
                    {
                        World.ActivatePlane(g.GetPlane(), false);
                        CameraController.CenterAt(g.GetPosition(), false, 0f);
                    }
                }
                else if (g == null)
                {
                    this.Unselect(false);
                    MHEventSystem.TriggerEvent<FSMSelectionManager>(this, null);
                }
                else
                {
                    this.Unselect(true);
                    if ((group == null) || ((group.locationHost == null) || (group.GetLocationHost() == null)))
                    {
                        this.selectedGroup = g;
                    }
                    else if ((group.GetUnits() == null) || (group.GetUnits().Count <= 0))
                    {
                        this.selectedGroup = g;
                    }
                    else
                    {
                        Group group2 = new Group(group.GetLocationHost().GetPlane(), group.GetOwnerID(), false) {
                            Position = group.GetPosition()
                        };
                        group.TransferUnits(group2);
                        group2.ChangeBeforeMovingAway(group.GetLocationHost());
                        this.selectedGroup = group2;
                        group2.GetMapFormation(true);
                    }
                    if (this.selectedGroup != null)
                    {
                        if ((this.selectedGroup is Group) && ((this.selectedGroup as Group).GetOwnerID() == PlayerWizard.HumanID()))
                        {
                            Group selectedGroup = this.selectedGroup as Group;
                            this.lastOwnSelection = (selectedGroup.locationHost == null) ? ((selectedGroup.beforeMovingAway == null) ? this.selectedGroup : ((IPlanePosition) selectedGroup.beforeMovingAway.Get())) : ((IPlanePosition) selectedGroup.locationHost.Get());
                        }
                        if ((this.selectedGroup is Location) && ((this.selectedGroup as Location).GetOwnerID() == PlayerWizard.HumanID()))
                        {
                            this.lastOwnSelection = this.selectedGroup;
                        }
                    }
                    if (HUD.Get() != null)
                    {
                        HUD.Get().UpdateSelectedUnit();
                    }
                    if (focus)
                    {
                        World.ActivatePlane(g.GetPlane(), false);
                        CameraController.CenterAt(g.GetPosition(), false, 0f);
                    }
                    MHEventSystem.TriggerEvent<FSMSelectionManager>(this, null);
                }
            }
        }

        public static void SetRoadPathMode(bool b)
        {
            if ((Get() != null) && (Get().roadBuildingMode != b))
            {
                Get().roadBuildingMode = b;
                HUD hud1 = HUD.Get();
                if (hud1 == null)
                {
                    HUD local1 = hud1;
                }
                else if (hud1.goRoadPlanningPrompt == null)
                {
                    GameObject goRoadPlanningPrompt = hud1.goRoadPlanningPrompt;
                }
                else
                {
                    hud1.goRoadPlanningPrompt.SetActive(b);
                }
                using (List<WorldCode.Plane>.Enumerator enumerator = World.GetPlanes().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ShowBuildRoad(b);
                    }
                }
            }
        }

        private void Unselect(bool silent)
        {
            Group g = this.selectedGroup as Group;
            if ((g != null) && (g.locationHost == null))
            {
                bool flag = false;
                List<Location> locationsOfWizard = GameManager.GetLocationsOfWizard(g.GetOwnerID());
                if (locationsOfWizard != null)
                {
                    Location location = locationsOfWizard.Find(o => (o.GetPosition() == g.GetPosition()) && ReferenceEquals(o.GetPlane(), g.GetPlane()));
                    if (location != null)
                    {
                        g.TransferUnits(location.GetLocalGroup());
                        g.Destroy();
                        flag = true;
                    }
                }
                if (!flag)
                {
                    List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(g.GetPlane());
                    if (groupsOfPlane != null)
                    {
                        Group group = groupsOfPlane.Find(o => !ReferenceEquals(o, g) && ((o.GetOwnerID() == g.GetOwnerID()) && (PlanePositionExtension.GetDistanceTo(o, g) == 0)));
                        if (group != null)
                        {
                            g.TransferUnits(group);
                            g.Destroy();
                            flag = true;
                        }
                    }
                }
            }
            this.selectedGroup = null;
            this.selectedUnits.Clear();
            if (!silent)
            {
                MHEventSystem.TriggerEvent<FSMSelectionManager>(this, null);
            }
        }
    }
}

