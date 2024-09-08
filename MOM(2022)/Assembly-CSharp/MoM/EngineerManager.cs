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
    public class EngineerManager
    {
        public enum WorkMode
        {
            TravelToWork = 0,
            Work = 1,
            Finished = 2,
            MAX = 3
        }

        [ProtoMember(1)]
        public List<Vector3i> pathToBuildRoadsOn;

        [ProtoMember(2)]
        public float progress;

        [ProtoMember(3)]
        public float curentWorkDifficulty;

        [ProtoMember(4)]
        public WorkMode workMode;

        [ProtoMember(5)]
        public Reference<Group> owner;

        private bool buildActionActive;

        private bool virtualVariant;

        private EngineerManager()
        {
            MHEventSystem.RegisterListener<Group>(GroupMoved, this);
        }

        public EngineerManager(Group g, bool virtualVariant = false)
        {
            this.owner = g;
            this.virtualVariant = virtualVariant;
            if (!virtualVariant)
            {
                MHEventSystem.RegisterListener<Group>(GroupMoved, this);
            }
        }

        private void GroupMoved(object sender, object e)
        {
            Group group = sender as Group;
            List<Vector3i> list = e as List<Vector3i>;
            if (this.owner.Get() == group && list != null && !this.buildActionActive)
            {
                this.Destroy();
            }
        }

        public void AddRoadNode(Vector3i roadNode)
        {
            if (roadNode == Vector3i.invalid)
            {
                return;
            }
            if (this.pathToBuildRoadsOn != null)
            {
                if (!this.pathToBuildRoadsOn.Contains(roadNode))
                {
                    this.pathToBuildRoadsOn.Add(roadNode);
                }
            }
            else
            {
                if (this.pathToBuildRoadsOn == null)
                {
                    this.pathToBuildRoadsOn = new List<Vector3i>();
                }
                this.pathToBuildRoadsOn.Add(roadNode);
            }
            if (this.pathToBuildRoadsOn[0] == this.owner.Get().GetPosition() && !this.virtualVariant)
            {
                this.owner.Get().SetActivelyBuilding(value: true);
            }
        }

        public void AddRoadPath(List<Vector3i> path)
        {
            this.pathToBuildRoadsOn = path;
            if (path != null && this.pathToBuildRoadsOn.Count > 0 && this.pathToBuildRoadsOn[0] == this.owner.Get().GetPosition() && !this.virtualVariant)
            {
                this.owner.Get().SetActivelyBuilding(value: true);
            }
        }

        public void Destroy()
        {
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
            if (this.owner != null)
            {
                this.owner.Get().engineerManager = null;
                if (!this.virtualVariant)
                {
                    this.owner.Get().SetActivelyBuilding(value: false);
                }
            }
        }

        public IEnumerator TurnUpdate()
        {
            Vector3i pos = this.owner.Get().GetPosition();
            global::WorldCode.Plane plane = this.owner.Get().GetPlane();
            this.buildActionActive = true;
            while (true)
            {
                if (this.owner == null || !this.owner.Get().alive)
                {
                    this.Destroy();
                    yield break;
                }
                while (this.owner.Get().CurentMP() > 0)
                {
                    if (this.pathToBuildRoadsOn == null || this.pathToBuildRoadsOn.Count < 1)
                    {
                        break;
                    }
                    Vector3i destination = this.pathToBuildRoadsOn[0];
                    pos = this.owner.Get().GetPosition();
                    if (!(pos != destination))
                    {
                        break;
                    }
                    if (!plane.GetHexAt(destination).IsLand())
                    {
                        this.pathToBuildRoadsOn.RemoveAt(0);
                        continue;
                    }
                    if (GameManager.GetLocationsOfThePlane(plane).Find((Location o) => o.GetPosition() == destination) != null)
                    {
                        this.pathToBuildRoadsOn.RemoveAt(0);
                        this.Destroy();
                        yield break;
                    }
                    if (GameManager.GetGroupsOfPlane(plane).Find((Group o) => o.GetPosition() == destination) != null)
                    {
                        this.pathToBuildRoadsOn.RemoveAt(0);
                        this.Destroy();
                        yield break;
                    }
                    this.progress = 0f;
                    this.curentWorkDifficulty = 0f;
                    RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, pos, destination, this.owner.Get());
                    requestDataV.water = false;
                    requestDataV.nonCorporeal = false;
                    PathfinderV2.FindPath(requestDataV);
                    List<Vector3i> path = requestDataV.GetPath();
                    if (this.owner.Get().GetOwnerID() == PlayerWizard.HumanID())
                    {
                        bool num = World.GetActivePlane() != plane;
                        FSMSelectionManager.Get().Select(this.owner.Get(), focus: true);
                        if (num)
                        {
                            yield return null;
                            yield return null;
                        }
                    }
                    this.owner.Get().MoveViaPath(path, mergeCollidedAlliedGroups: false);
                    if (this.owner.Get().GetOwnerID() == PlayerWizard.HumanID())
                    {
                        Formation mapFormation = this.owner.Get().GetMapFormation();
                        if (mapFormation != null)
                        {
                            yield return mapFormation.WaitToEndOfMovement();
                        }
                    }
                }
                if (!(this.owner.Get().CurentMP() > 0))
                {
                    break;
                }
                if (plane.GetRoadManagers().GetRoadAt(this.owner.Get().Position) > 0)
                {
                    if (this.pathToBuildRoadsOn == null || this.pathToBuildRoadsOn.Count <= 0)
                    {
                        break;
                    }
                    this.pathToBuildRoadsOn.RemoveAt(0);
                    continue;
                }
                FInt zERO = FInt.ZERO;
                foreach (Reference<Unit> unit in this.owner.Get().GetUnits())
                {
                    if (unit.Get().IsEngineer())
                    {
                        unit.Get().Mp = FInt.ZERO;
                        zERO += unit.Get().GetAttFinal((Tag)TAG.ENGINEER_UNIT);
                    }
                }
                if (HUD.Get() != null)
                {
                    HUD.Get().UpdateSelectedUnit();
                }
                if (this.curentWorkDifficulty * zERO == 0)
                {
                    this.CalculateWorkDiff();
                }
                this.progress += this.curentWorkDifficulty * zERO.ToFloat();
                if (this.progress >= 1f)
                {
                    plane.GetRoadManagers().SetRoadMode(pos, plane);
                    this.pathToBuildRoadsOn.RemoveAt(0);
                }
                break;
            }
            this.buildActionActive = false;
            if (this.pathToBuildRoadsOn == null || this.pathToBuildRoadsOn.Count < 1)
            {
                this.Destroy();
            }
        }

        public void CalculateWorkDiff()
        {
            global::WorldCode.Plane plane = this.owner.Get().GetPlane();
            Vector3i position = this.owner.Get().GetPosition();
            int num = plane.GetHexAt(position).MovementCost();
            this.curentWorkDifficulty = 1f / ((float)num * 2.4f);
        }

        public int TurnsOfWorkLeft()
        {
            if (this.pathToBuildRoadsOn == null || this.pathToBuildRoadsOn.Count == 0 || this.owner == null)
            {
                return 0;
            }
            if (this.pathToBuildRoadsOn[0] == this.owner.Get().GetPosition())
            {
                FInt zERO = FInt.ZERO;
                FInt zERO2 = FInt.ZERO;
                foreach (Reference<Unit> unit in this.owner.Get().GetUnits())
                {
                    if (unit.Get().IsEngineer())
                    {
                        FInt attFinal = unit.Get().GetAttFinal((Tag)TAG.ENGINEER_UNIT);
                        zERO += attFinal;
                        if (unit.Get().Mp > 0)
                        {
                            zERO2 += attFinal;
                        }
                    }
                }
                if (this.curentWorkDifficulty * zERO == 0)
                {
                    this.CalculateWorkDiff();
                }
                float num = 1f - this.progress;
                float num2 = this.curentWorkDifficulty * zERO2.ToFloat();
                if (num2 >= num)
                {
                    return 1;
                }
                num -= num2;
                return Mathf.CeilToInt(num / (this.curentWorkDifficulty * zERO.ToFloat())) + 1;
            }
            return 0;
        }

        public void Validate()
        {
            if (!(this.owner.Get().GetUnits().Find((Reference<Unit> o) => o.Get().IsEngineer()) != null))
            {
                this.Destroy();
            }
        }
    }
}
