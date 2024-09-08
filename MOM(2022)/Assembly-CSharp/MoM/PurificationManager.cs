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
    public class PurificationManager
    {
        public enum WorkMode
        {
            TravelToWork = 0,
            Work = 1,
            Finished = 2,
            MAX = 3
        }

        [ProtoMember(1)]
        public List<Vector3i> pathToWorkOn;

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

        private PurificationManager()
        {
            MHEventSystem.RegisterListener<Group>(GroupMoved, this);
        }

        public PurificationManager(Group g, bool virtualVariant = false)
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

        public void Destroy()
        {
            MHEventSystem.UnRegisterListenersLinkedToObject(this);
            if (this.owner != null)
            {
                this.owner.Get().purificationManager = null;
                if (!this.virtualVariant)
                {
                    this.owner.Get().SetActivelyBuilding(value: false);
                }
            }
        }

        public void AddNode(Vector3i purifyNode)
        {
            if (purifyNode == Vector3i.invalid)
            {
                return;
            }
            if (this.pathToWorkOn != null)
            {
                if (!this.pathToWorkOn.Contains(purifyNode))
                {
                    this.pathToWorkOn.Add(purifyNode);
                }
            }
            else
            {
                if (this.pathToWorkOn == null)
                {
                    this.pathToWorkOn = new List<Vector3i>();
                }
                this.pathToWorkOn.Add(purifyNode);
            }
            if (this.pathToWorkOn[0] == this.owner.Get().GetPosition() && !this.virtualVariant)
            {
                this.owner.Get().SetActivelyBuilding(value: true);
            }
        }

        public void AddRoadPath(List<Vector3i> path)
        {
            if (path == null)
            {
                return;
            }
            if (this.pathToWorkOn != null)
            {
                foreach (Vector3i item in path)
                {
                    if (!this.pathToWorkOn.Contains(item))
                    {
                        this.pathToWorkOn.Add(item);
                    }
                }
            }
            else
            {
                this.pathToWorkOn = path;
            }
            if (this.pathToWorkOn.Count > 0 && this.pathToWorkOn[0] == this.owner.Get().GetPosition() && !this.virtualVariant)
            {
                this.owner.Get().SetActivelyBuilding(value: true);
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
                    if (this.pathToWorkOn == null || this.pathToWorkOn.Count < 1)
                    {
                        this.Destroy();
                        yield break;
                    }
                    Vector3i destination = this.pathToWorkOn[0];
                    if (!(pos != destination))
                    {
                        break;
                    }
                    if (!plane.GetHexAt(destination).IsLand())
                    {
                        this.pathToWorkOn.RemoveAt(0);
                        continue;
                    }
                    if (GameManager.GetLocationsOfThePlane(plane).Find((Location o) => o.GetPosition() == destination) != null)
                    {
                        this.pathToWorkOn.RemoveAt(0);
                        continue;
                    }
                    if (GameManager.GetGroupsOfPlane(plane).Find((Group o) => o.GetPosition() == destination) != null)
                    {
                        this.pathToWorkOn.RemoveAt(0);
                        continue;
                    }
                    this.progress = 0f;
                    this.curentWorkDifficulty = 0f;
                    RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, pos, destination, this.owner.Get());
                    PathfinderV2.FindPath(requestDataV);
                    List<Vector3i> path = requestDataV.GetPath();
                    if (this.owner.Get().GetOwnerID() == PlayerWizard.HumanID())
                    {
                        FSMSelectionManager.Get().Select(this.owner.Get(), focus: true);
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
                Hex hexAt = plane.GetHexAt(pos);
                if (hexAt.ActiveHex)
                {
                    this.pathToWorkOn.RemoveAt(0);
                    continue;
                }
                FInt zERO = FInt.ZERO;
                foreach (Reference<Unit> unit in this.owner.Get().GetUnits())
                {
                    if (unit.Get().IsPurifier())
                    {
                        unit.Get().Mp = FInt.ZERO;
                        zERO += unit.Get().GetAttFinal((Tag)TAG.PURIFER_UNIT);
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
                    hexAt.ActiveHex = true;
                    this.pathToWorkOn.RemoveAt(0);
                }
                break;
            }
            this.buildActionActive = false;
            if (this.pathToWorkOn == null || this.pathToWorkOn.Count < 1)
            {
                this.Destroy();
            }
        }

        public void CalculateWorkDiff()
        {
            this.curentWorkDifficulty = 0.25f;
        }

        public int TurnsOfWorkLeft()
        {
            if (this.pathToWorkOn == null || this.pathToWorkOn.Count == 0 || this.owner == null)
            {
                return 0;
            }
            if (this.pathToWorkOn[0] == this.owner.Get().GetPosition())
            {
                FInt zERO = FInt.ZERO;
                FInt zERO2 = FInt.ZERO;
                foreach (Reference<Unit> unit in this.owner.Get().GetUnits())
                {
                    if (unit.Get().IsPurifier())
                    {
                        FInt attFinal = unit.Get().GetAttFinal((Tag)TAG.PURIFER_UNIT);
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
                return Mathf.CeilToInt(num / (this.curentWorkDifficulty * zERO.ToFloat()));
            }
            return 0;
        }

        public void Validate()
        {
            if (!(this.owner.Get().GetUnits().Find((Reference<Unit> o) => o.Get().IsPurifier()) != null))
            {
                this.Destroy();
            }
        }
    }
}
