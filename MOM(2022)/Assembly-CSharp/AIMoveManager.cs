// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AIMoveManager
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using UnityEngine;
using WorldCode;

public class AIMoveManager
{
    private Dictionary<global::MOM.Group, global::MOM.Group> passengerAndTransport = new Dictionary<global::MOM.Group, global::MOM.Group>();

    private List<Spell> waterAllowingSpells;

    public void Validate()
    {
        if (this.passengerAndTransport == null)
        {
            this.passengerAndTransport = new Dictionary<global::MOM.Group, global::MOM.Group>();
        }
        foreach (global::MOM.Group item in new List<global::MOM.Group>(this.passengerAndTransport.Keys))
        {
            if (!item.alive || !this.passengerAndTransport[item].alive)
            {
                this.passengerAndTransport.Remove(item);
            }
        }
    }

    public void EndTurnValidation()
    {
        if (this.passengerAndTransport == null)
        {
            this.passengerAndTransport = new Dictionary<global::MOM.Group, global::MOM.Group>();
        }
        foreach (global::MOM.Group item in new List<global::MOM.Group>(this.passengerAndTransport.Keys))
        {
            global::MOM.Group group = this.passengerAndTransport[item];
            if (!item.alive || !group.alive)
            {
                this.passengerAndTransport.Remove(item);
            }
            if (item.CurentMP() == item.GetMaxMP() || group.CurentMP() == group.GetMaxMP())
            {
                this.passengerAndTransport.Remove(item);
            }
        }
    }

    public bool IsAssignedAsTransport(global::MOM.Group g)
    {
        if (this.passengerAndTransport == null)
        {
            this.passengerAndTransport = new Dictionary<global::MOM.Group, global::MOM.Group>();
        }
        foreach (KeyValuePair<global::MOM.Group, global::MOM.Group> item in this.passengerAndTransport)
        {
            if (item.Value == g)
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator Move(global::MOM.Group group, Vector3i destination, global::WorldCode.Plane destinationPlane)
    {
        AIGroupDesignation designation = group.GetDesignation(createIfMissing: false);
        if (designation != null)
        {
            if (group.waterMovement)
            {
                designation.inNeedOfWaterTransport = false;
            }
            if (group.GetPlane() == destinationPlane)
            {
                yield return this.Move(designation, group, destination);
            }
            else
            {
                yield return this.MovePlanar(designation, group, destination, destinationPlane);
            }
        }
    }

    private IEnumerator MovePlanar(AIGroupDesignation d, global::MOM.Group group, Vector3i destination, global::WorldCode.Plane destinationPlane)
    {
        MultiPlanePath multiPlanePath = new MultiPlanePath(new Destination(destination, destinationPlane.arcanusType, d.designation == AIGroupDesignation.Designation.Defend || d.designation == AIGroupDesignation.Designation.AggressionShort || d.designation == AIGroupDesignation.Designation.AggressionMedium || d.designation == AIGroupDesignation.Designation.AggressionLong));
        int a = group.GetDistanceTo(destination) * 3;
        a = Mathf.Max(a, 20);
        if (multiPlanePath.PlanStaging(d, new FInt(a), group.GetPosition(), group.GetPlane()))
        {
            yield return multiPlanePath.MoveViaMPath(group, d);
        }
    }

    private IEnumerator Move(AIGroupDesignation d, global::MOM.Group group, Vector3i destination)
    {
        int distanceTo = group.GetDistanceTo(destination);
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(group.GetPlane(), group.GetPosition(), destination, group);
        PathfinderV2.FindPath(requestDataV);
        List<Vector3i> path = requestDataV.GetPath();
        if (path == null || ((!group.waterMovement || !group.landMovement) && distanceTo < path.Count + 5 + group.GetMaxMP() * 5 && this.GetEmbarkSpot(group.GetPlane(), path) == -1 && this.TransportOnPlanePossible(group)))
        {
            d.inNeedOfWaterTransport = !group.waterMovement;
            requestDataV = RequestDataV2.CreateRequest(group.GetPlane(), group.GetPosition(), destination, group, allTerrain: true);
            requestDataV.allowAllyPassMode = false;
            requestDataV.avoidLandingOnTarget = !group.waterMovement || !group.landMovement;
            PathfinderV2.FindPath(requestDataV);
            List<Vector3i> path2 = requestDataV.GetPath();
            if (path != null && path2 != null)
            {
                if ((float)path2.Count < (float)path.Count * 0.5f)
                {
                    path = path2;
                }
            }
            else if (path2 != null)
            {
                path = path2;
            }
        }
        if (path == null || path.Count < 2)
        {
            yield break;
        }
        int embarkSpot = this.GetEmbarkSpot(group.GetPlane(), path);
        if (embarkSpot > -1 && (!group.waterMovement || !group.landMovement))
        {
            if (group.GetPlane().GetHexAt(group.GetPosition()).IsLand())
            {
                yield return this.MoveLandToWater(group, embarkSpot, path, d);
            }
            else
            {
                yield return this.MoveWaterToLand(group, embarkSpot, path, d);
            }
        }
        else
        {
            this.MoveViaPath(group, path, d);
        }
        bool cameraFollow = Settings.GetData().GetFollowAIMovement();
        if (group.alive && group.IsModelVisible() && World.GetActivePlane() != group.GetPlane() && cameraFollow)
        {
            World.ActivatePlane(group.GetPlane());
            yield return null;
        }
        while ((group?.alive ?? false) && cameraFollow)
        {
            bool flag = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement);
            bool flag2 = !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle);
            if (!flag && !flag2)
            {
                break;
            }
            if (flag && !flag2 && group != null && group.alive && !group.IsGroupInvisible() && group.GetOwnerID() != GameManager.GetHumanWizard().GetID() && path.Count > 1 && FOW.Get().IsVisible(group.GetPosition(), group.GetPlane()))
            {
                if (World.GetActivePlane() != group.GetPlane())
                {
                    World.ActivatePlane(group.GetPlane());
                }
                CharacterActor characterActor = group.GetMapFormation().GetCharacterActors().Find((CharacterActor o) => o != null);
                if (characterActor != null)
                {
                    CameraController.CenterAt(characterActor.transform.localPosition);
                }
            }
            yield return null;
        }
        yield return this.WaitForFocus();
    }

    private IEnumerator MoveLandToWater(global::MOM.Group group, int switchIndex, List<Vector3i> path, AIGroupDesignation d)
    {
        int index = switchIndex + 1;
        Vector3i vector3i = path[index];
        global::MOM.Group g = this.GetTransport(group, findNewIfNeeded: true, vector3i);
        if (g == null)
        {
            yield break;
        }
        if (g.GetPosition() != vector3i)
        {
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(g.GetPlane(), g.GetPosition(), vector3i, g);
            PathfinderV2.FindPath(requestDataV);
            List<Vector3i> path2 = requestDataV.GetPath();
            if (path2 != null && path2.Count > 1)
            {
                this.MoveViaPath(g, path2, g.GetDesignation());
                yield return this.WaitForFocus();
            }
            else if (this.passengerAndTransport.ContainsKey(group))
            {
                this.passengerAndTransport.Remove(group);
                g = null;
            }
        }
        if (group.CurentMP() == 0)
        {
            yield break;
        }
        if (switchIndex > 0)
        {
            List<Vector3i> path3 = path.GetRange(0, switchIndex + 1);
            if (g != null && g.GetDistanceTo(path[switchIndex]) < 2 && switchIndex <= group.CurentMP())
            {
                RequestDataV2 requestDataV2 = RequestDataV2.CreateRequest(group.GetPlane(), group.GetPosition(), group.CurentMP(), group);
                PathfinderV2.FindArea(requestDataV2);
                List<Vector3i> area = requestDataV2.GetArea();
                Vector3i vector3i2 = group.GetPosition();
                int distanceTo = g.GetDistanceTo(vector3i2);
                for (int i = 0; i < area.Count; i++)
                {
                    if (g.GetDistanceTo(area[i]) < distanceTo)
                    {
                        vector3i2 = area[i];
                    }
                }
                List<Vector3i> pathTo = requestDataV2.GetPathTo(vector3i2);
                if (pathTo != null && pathTo.Count > 1)
                {
                    path3 = pathTo;
                }
            }
            this.MoveViaPath(group, path3, d);
            yield return this.WaitForFocus();
        }
        if (group.CurentMP() == 0 || !this.ValidateTransport(group, g) || group.GetDistanceTo(g) != 1)
        {
            yield break;
        }
        if (group.GetUnits().Count >= 9)
        {
            List<Reference<global::MOM.Unit>> list = new List<Reference<global::MOM.Unit>>(group.GetUnits());
            global::MOM.Unit u;
            switch (d.designation)
            {
            case AIGroupDesignation.Designation.Settler:
                list.Sort(delegate(Reference<global::MOM.Unit> a, Reference<global::MOM.Unit> b)
                {
                    global::MOM.Unit unit3 = a.Get();
                    global::MOM.Unit unit4 = b.Get();
                    int num2 = -unit3.IsSettler().CompareTo(unit4.IsSettler());
                    return (num2 != 0) ? num2 : (-unit3.GetSimpleUnitStrength().CompareTo(unit4.GetSimpleUnitStrength()));
                });
                u = list[list.Count - 1];
                break;
            case AIGroupDesignation.Designation.Engineer:
                list.Sort(delegate(Reference<global::MOM.Unit> a, Reference<global::MOM.Unit> b)
                {
                    global::MOM.Unit unit = a.Get();
                    global::MOM.Unit unit2 = b.Get();
                    int num = -unit.IsEngineer().CompareTo(unit2.IsEngineer());
                    return (num != 0) ? num : (-unit.GetSimpleUnitStrength().CompareTo(unit2.GetSimpleUnitStrength()));
                });
                u = list[list.Count - 1];
                break;
            default:
                list.Sort(delegate(Reference<global::MOM.Unit> a, Reference<global::MOM.Unit> b)
                {
                    global::MOM.Unit unit5 = a.Get();
                    global::MOM.Unit unit6 = b.Get();
                    int num3 = -unit5.IsHero().CompareTo(unit6.IsHero());
                    return (num3 != 0) ? num3 : (-unit5.GetSimpleUnitStrength().CompareTo(unit6.GetSimpleUnitStrength()));
                });
                u = list[list.Count - 1];
                break;
            }
            group.Position = g.GetPosition();
            global::MOM.Group group2 = new global::MOM.Group(group.GetPlane(), group.GetOwnerID());
            group2.Position = group.GetPosition();
            group2.AddUnit(u);
            group2.GetMapFormation(group.GetMapFormation(createIfMissing: false) != null);
            g.TransferUnits(group);
            group.GetPlane().ClearSearcherData();
            if (this.passengerAndTransport.ContainsKey(group))
            {
                this.passengerAndTransport.Remove(group);
            }
        }
        else
        {
            Vector3i position = g.GetPosition();
            g.TransferUnits(group);
            group.Position = position;
            group.GetPlane().ClearSearcherData();
        }
    }

    private IEnumerator MoveWaterToLand(global::MOM.Group group, int switchIndex, List<Vector3i> path, AIGroupDesignation d)
    {
        int firstLandIndex = switchIndex + 1;
        if (group.CurentMP() == 0)
        {
            yield break;
        }
        if (switchIndex > 0)
        {
            List<Vector3i> range = path.GetRange(0, switchIndex + 1);
            this.MoveViaPath(group, range, d);
            yield return this.WaitForFocus();
        }
        if (group.CurentMP() == 0)
        {
            yield break;
        }
        foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
        {
            if (entity.Value is IPlanePosition planePosition && planePosition.GetPlane() == group.GetPlane() && planePosition.GetPosition() == path[firstLandIndex])
            {
                yield break;
            }
        }
        group.Position = path[firstLandIndex];
        if (group.landMovement)
        {
            yield break;
        }
        global::MOM.Group group2 = new global::MOM.Group(group.GetPlane(), group.GetOwnerID());
        group2.Position = path[switchIndex];
        List<Reference<global::MOM.Unit>> units = group.GetUnits();
        for (int num = units.Count - 1; num >= 0; num--)
        {
            Reference<global::MOM.Unit> reference = units[num];
            if (!reference.Get().CanTravelOverLand())
            {
                group2.AddUnit(reference.Get());
                group.RemoveUnit(reference.Get());
            }
        }
        group2.GetMapFormation(group.GetMapFormation(createIfMissing: false) != null);
    }

    private IEnumerator WaitForFocus()
    {
        while (!GameManager.Get().IsFocusFree())
        {
            yield return null;
        }
    }

    private global::MOM.Group GetTransport(global::MOM.Group passenger, bool findNewIfNeeded, Vector3i placeOfNeed)
    {
        if (this.passengerAndTransport == null)
        {
            this.passengerAndTransport = new Dictionary<global::MOM.Group, global::MOM.Group>();
        }
        if (this.passengerAndTransport.ContainsKey(passenger))
        {
            global::MOM.Group group = this.passengerAndTransport[passenger];
            if (this.ValidateTransport(passenger, group))
            {
                return group;
            }
            this.passengerAndTransport.Remove(passenger);
        }
        if (this.passengerAndTransport.ContainsKey(passenger))
        {
            return this.passengerAndTransport[passenger];
        }
        if (findNewIfNeeded)
        {
            PlayerWizard wizard = GameManager.GetWizard(passenger.GetOwnerID());
            global::MOM.Group group2 = this.FindAndFormTransport(wizard as PlayerWizardAI, placeOfNeed, passenger.GetPlane());
            if (group2 != null)
            {
                this.passengerAndTransport.Add(passenger, group2);
            }
            return group2;
        }
        return null;
    }

    private bool ValidateTransport(global::MOM.Group passenger, global::MOM.Group transport)
    {
        if (passenger == null || transport == null || !passenger.alive || !transport.alive)
        {
            return false;
        }
        if (passenger.GetPlane() != transport.GetPlane())
        {
            return false;
        }
        if (transport.GetUnits().Count > 1)
        {
            return false;
        }
        return true;
    }

    public bool ValidateTransportFor(global::MOM.Group passenger)
    {
        if (this.passengerAndTransport == null || !this.passengerAndTransport.ContainsKey(passenger))
        {
            return false;
        }
        global::MOM.Group transport = this.passengerAndTransport[passenger];
        return this.ValidateTransport(passenger, transport);
    }

    private global::MOM.Group FindAndFormTransport(PlayerWizardAI ai, Vector3i placeOfNeed, global::WorldCode.Plane plane)
    {
        AIPlaneVisibility planeVisibility = ai.GetPlaneVisibility(plane);
        if (planeVisibility.ownGroups == null)
        {
            return null;
        }
        int num = 10000;
        global::MOM.Group group = null;
        foreach (global::MOM.Group ownGroup in planeVisibility.ownGroups)
        {
            int distanceTo = ownGroup.GetDistanceTo(placeOfNeed);
            if (distanceTo >= 25 || distanceTo >= num || ownGroup.transporter?.Get() == null || (ownGroup.CurentMP() == 0 && !ownGroup.IsHosted()))
            {
                continue;
            }
            bool flag = false;
            foreach (global::MOM.Group value in this.passengerAndTransport.Values)
            {
                if (value == ownGroup)
                {
                    flag = true;
                    break;
                }
            }
            if (flag || (ownGroup.GetUnits().Count > 1 && !ownGroup.CanSubsetTravelWater(ownGroup.GetUnits(), ownGroup.transporter)))
            {
                continue;
            }
            if (ownGroup.GetPosition() == placeOfNeed)
            {
                if (ownGroup.GetUnits().Count == 1)
                {
                    group = ownGroup;
                    break;
                }
                continue;
            }
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(plane, ownGroup.GetPosition(), placeOfNeed, ownGroup);
            PathfinderV2.FindPath(requestDataV);
            int num2 = requestDataV.GetPath()?.Count ?? int.MaxValue;
            if (num2 >= 2 && num2 < num)
            {
                num = num2;
                group = ownGroup;
            }
        }
        if (group != null && (group.GetUnits().Count > 1 || group.IsHosted()))
        {
            global::MOM.Group group2 = new global::MOM.Group(group.GetPlane(), group.GetOwnerID());
            group2.Position = group.GetPosition();
            group2.AddUnit(group.transporter);
            group2.GetMapFormation(group.GetMapFormation(createIfMissing: false) != null);
            group2.GetUnits().ForEach(delegate(Reference<global::MOM.Unit> o)
            {
                o.Get().UpdateMP();
            });
            group = group2;
        }
        return group;
    }

    private bool TransportOnPlanePossible(global::MOM.Group g)
    {
        if (this.ValidateTransportFor(g))
        {
            return true;
        }
        PlayerWizard wizard = GameManager.GetWizard(g.GetOwnerID());
        if (wizard.banishedTurn == 0 && wizard.GetSpells().Find((DBReference<Spell> o) => this.GetWaterAllowingSpells().Contains(o.Get())) != null)
        {
            return true;
        }
        return GameManager.GetGroupsOfPlane(g.GetPlane().arcanusType).Find((global::MOM.Group o) => o.GetOwnerID() == g.GetOwnerID() && o.transporter != null && o.waterMovement) != null;
    }

    private int GetEmbarkSpot(global::WorldCode.Plane p, List<Vector3i> path, int startIndex = 0)
    {
        if (path == null || path.Count <= startIndex)
        {
            return -1;
        }
        for (int i = startIndex; i < path.Count - 1; i++)
        {
            Hex hexAt = p.GetHexAt(path[i]);
            Hex hexAt2 = p.GetHexAt(path[i + 1]);
            if (hexAt != null && hexAt2 != null && hexAt.IsLand() != hexAt2.IsLand())
            {
                return i;
            }
        }
        return -1;
    }

    private List<Spell> GetWaterAllowingSpells()
    {
        if (this.waterAllowingSpells == null)
        {
            if (this.waterAllowingSpells == null)
            {
                this.waterAllowingSpells = new List<Spell>();
            }
            this.waterAllowingSpells.Add((Spell)SPELL.WIND_WALKING);
            this.waterAllowingSpells.Add((Spell)SPELL.WRAITH_FORM);
            this.waterAllowingSpells.Add((Spell)SPELL.FLIGHT);
            this.waterAllowingSpells.Add((Spell)SPELL.WATER_WALKING);
        }
        return this.waterAllowingSpells;
    }

    private void MoveViaPath(global::MOM.Group g, List<Vector3i> path, AIGroupDesignation d)
    {
        bool flag = d.designation == AIGroupDesignation.Designation.Transfer;
        bool aggressive = d.designation == AIGroupDesignation.Designation.Defend || d.designation == AIGroupDesignation.Designation.AggressionShort || d.designation == AIGroupDesignation.Designation.AggressionMedium || d.designation == AIGroupDesignation.Designation.AggressionLong;
        g.MoveViaPath(path, flag, flag, aggressive);
    }
}
