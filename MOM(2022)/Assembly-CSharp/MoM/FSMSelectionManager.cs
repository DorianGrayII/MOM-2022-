using System.Collections.Generic;
using HutongGames.PlayMaker;
using MHUtils;
using WorldCode;

namespace MOM
{
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
            return FSMSelectionManager.instance;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            FSMSelectionManager.instance = this;
            this.selectedGroup = null;
            this.selectedUnits.Clear();
            this.lastOwnSelection = null;
            FSMSelectionManager.NewTurnCameraCenter(forceInstant: true);
            MHEventSystem.RegisterListener<TurnManager>(NewTurn, this);
        }

        public static void SetRoadPathMode(bool b)
        {
            if (FSMSelectionManager.Get() == null || FSMSelectionManager.Get().roadBuildingMode == b)
            {
                return;
            }
            FSMSelectionManager.Get().roadBuildingMode = b;
            HUD.Get()?.goRoadPlanningPrompt?.SetActive(b);
            foreach (Plane plane in World.GetPlanes())
            {
                plane.ShowBuildRoad(b);
            }
        }

        private void NewTurn(object sender, object e)
        {
            if (!(e as string == "Turn") || (this.selectedGroup is IGroup group && group.GetOwnerID() == PlayerWizard.HumanID()))
            {
                return;
            }
            if (this.lastOwnSelection != null && GameManager.GroupAlive(this.lastOwnSelection))
            {
                World.ActivatePlane(this.lastOwnSelection.GetPlane());
                if (!HUD.Get().NextPlayersArmyWithoutOrders())
                {
                    CameraController.CenterAt(this.lastOwnSelection.GetPosition());
                }
            }
            else
            {
                FSMSelectionManager.NewTurnCameraCenter();
            }
        }

        public static void NewTurnCameraCenter(bool forceInstant = false)
        {
            foreach (Location registeredLocation in GameManager.Get().registeredLocations)
            {
                if (registeredLocation.GetOwnerID() == PlayerWizard.HumanID())
                {
                    World.ActivatePlane(registeredLocation.GetPlane());
                    CameraController.CenterAt(registeredLocation.GetPosition(), forceInstant);
                    return;
                }
            }
            foreach (Group registeredGroup in GameManager.Get().registeredGroups)
            {
                if (registeredGroup.GetOwnerID() == PlayerWizard.HumanID())
                {
                    World.ActivatePlane(registeredGroup.GetPlane());
                    CameraController.CenterAt(registeredGroup.GetPosition(), forceInstant);
                    break;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            FSMSelectionManager.instance = null;
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
        }

        public IPlanePosition GetSelectedGroup()
        {
            return this.selectedGroup;
        }

        public void Select(IPlanePosition g, bool focus)
        {
            if (!TurnManager.Get().playerTurn)
            {
                g = null;
            }
            FSMSelectionManager.SetRoadPathMode(b: false);
            Group group = g as Group;
            if (group != null && group.GetUnits() != null && group.IsGroupInvisible() && group.GetOwnerID() != PlayerWizard.HumanID())
            {
                return;
            }
            if (g == this.selectedGroup)
            {
                if (focus && g != null)
                {
                    World.ActivatePlane(g.GetPlane());
                    CameraController.CenterAt(g.GetPosition());
                }
                return;
            }
            if (g == null)
            {
                this.Unselect();
                MHEventSystem.TriggerEvent<FSMSelectionManager>(this, null);
                return;
            }
            this.Unselect(silent: true);
            if (group != null && group.locationHost != null && group.GetLocationHost() != null)
            {
                if (group.GetUnits() != null && group.GetUnits().Count > 0)
                {
                    Group group2 = new Group(group.GetLocationHost().GetPlane(), group.GetOwnerID());
                    group2.Position = group.GetPosition();
                    group.TransferUnits(group2);
                    group2.ChangeBeforeMovingAway(group.GetLocationHost());
                    this.selectedGroup = group2;
                    group2.GetMapFormation();
                }
                else
                {
                    this.selectedGroup = g;
                }
            }
            else
            {
                this.selectedGroup = g;
            }
            if (this.selectedGroup != null)
            {
                if (this.selectedGroup is Group && (this.selectedGroup as Group).GetOwnerID() == PlayerWizard.HumanID())
                {
                    Group group3 = this.selectedGroup as Group;
                    if (group3.locationHost != null)
                    {
                        this.lastOwnSelection = group3.locationHost.Get();
                    }
                    else if (group3.beforeMovingAway != null)
                    {
                        this.lastOwnSelection = group3.beforeMovingAway.Get();
                    }
                    else
                    {
                        this.lastOwnSelection = this.selectedGroup;
                    }
                }
                if (this.selectedGroup is Location && (this.selectedGroup as Location).GetOwnerID() == PlayerWizard.HumanID())
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
                World.ActivatePlane(g.GetPlane());
                CameraController.CenterAt(g.GetPosition());
            }
            MHEventSystem.TriggerEvent<FSMSelectionManager>(this, null);
        }

        private void Unselect(bool silent = false)
        {
            Group g = this.selectedGroup as Group;
            if (g != null && g.locationHost == null)
            {
                bool flag = false;
                List<Location> locationsOfWizard = GameManager.GetLocationsOfWizard(g.GetOwnerID());
                if (locationsOfWizard != null)
                {
                    Location location = locationsOfWizard.Find((Location o) => o.GetPosition() == g.GetPosition() && o.GetPlane() == g.GetPlane());
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
                        Group group = groupsOfPlane.Find((Group o) => o != g && o.GetOwnerID() == g.GetOwnerID() && o.GetDistanceTo(g) == 0);
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

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (this.selectedUnits == null || this.selectedUnits.Count <= 0)
            {
                return;
            }
            bool flag = false;
            foreach (Unit selectedUnit in this.selectedUnits)
            {
                if (selectedUnit.group == null)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                this.Unselect();
            }
        }
    }
}
