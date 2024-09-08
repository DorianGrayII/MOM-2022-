using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;
using WorldCode;

public class AINeutralTown
{
    private TownLocation owner;

    private HashSet<Vector3i> areaOfInterest;

    private int buildDelay;

    private int armySpawn;

    public AINeutralTown(TownLocation tl)
    {
        this.owner = tl;
        this.buildDelay = Random.Range(25, 150);
        this.armySpawn = Random.Range(10, 40);
    }

    private HashSet<Vector3i> GetAOE()
    {
        if (this.areaOfInterest == null)
        {
            RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.owner.GetPlane(), this.owner.GetPosition(), new FInt(20), null);
            PathfinderV2.FindArea(requestDataV);
            List<Vector3i> area = requestDataV.GetArea();
            this.areaOfInterest = new HashSet<Vector3i>();
            foreach (Vector3i item in area)
            {
                this.areaOfInterest.Add(this.owner.GetPlane().area.KeepHorizontalInside(item));
            }
        }
        return this.areaOfInterest;
    }

    public void ResolveTurn()
    {
        int settingAsInt = DifficultySettingsData.GetSettingAsInt("UI_DIFF_NEUTRAL_ARMIES");
        int num = (int)((float)this.owner.CalculateProductionIncome() * 0.5f + (float)settingAsInt * 0.5f);
        this.owner.EndTurnUpdate();
        List<Reference<global::MOM.Unit>> units = this.owner.GetLocalGroup().GetUnits();
        int count = units.Count;
        this.buildDelay -= num;
        this.armySpawn--;
        if (count > this.owner.GetPopUnits())
        {
            global::MOM.Unit unit = units[0];
            for (int i = 1; i < units.Count; i++)
            {
                if (unit.GetWorldUnitValue() > units[i].Get().GetWorldUnitValue())
                {
                    unit = units[i];
                }
            }
            unit.Destroy();
            return;
        }
        if (count < this.owner.GetPopUnits() && count < 9 && this.buildDelay <= 0 && 18 - count * 2 - Random.Range(0, 100) > 0)
        {
            List<global::DBDef.Unit> list = this.owner.PossibleUnits().FindAll((global::DBDef.Unit o) => o.constructionCost <= 100);
            if (list.Count > 0)
            {
                global::DBDef.Unit unit2 = list[0];
                for (int j = 1; j < list.Count; j++)
                {
                    if (BaseUnit.GetUnitStrength(unit2) < BaseUnit.GetUnitStrength(list[j]))
                    {
                        unit2 = list[j];
                    }
                }
                global::MOM.Unit u = global::MOM.Unit.CreateFrom(unit2);
                this.owner.GetLocalGroup().AddUnit(u, updateMovementFlags: false);
                this.buildDelay = unit2.constructionCost;
            }
        }
        int num2 = Random.Range(0, 100);
        if (this.armySpawn > 0 || TurnManager.GetTurnNumber() <= 15 || !((float)num2 < 3f + (float)settingAsInt * 1.5f))
        {
            return;
        }
        global::MOM.Group attackTarget = this.GetAttackTarget();
        if (attackTarget == null)
        {
            return;
        }
        int num3 = this.owner.GetPopUnits() * (15 + settingAsInt * 5);
        int a = 100 + settingAsInt * num3 * Random.Range(1, 1 + settingAsInt);
        a = ((TurnManager.GetTurnNumber() >= 50) ? Mathf.Min(a, (int)(100f + (float)((30 + TurnManager.GetTurnNumber()) * settingAsInt) * 1.5f)) : Mathf.Min(a, 100 + (30 + TurnManager.GetTurnNumber() * settingAsInt)));
        if (!(ScriptLibrary.Call("DEF_GeneralDefenders", this.owner, a, true) is global::MOM.Group group))
        {
            return;
        }
        if (group.GetUnits().Count == 0)
        {
            group.Destroy();
            return;
        }
        RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this.owner.GetPlane(), this.owner.GetPosition(), attackTarget.GetPosition(), group);
        PathfinderV2.FindPath(requestDataV);
        List<Vector3i> path = requestDataV.GetPath();
        if (path == null || path.Count < 3)
        {
            group.Destroy();
            return;
        }
        group.Position = path[1];
        group.aiNeturalExpedition = new AINeutralExpedition(group, this.owner, attackTarget);
        if (group.GetPlane() == World.GetActivePlane() && group.IsModelVisible())
        {
            group.GetMapFormation();
        }
        int num4 = 40 + Random.Range(0, 60);
        float num5 = 1f - 0.15f * (float)settingAsInt;
        this.armySpawn = (int)((float)num4 * num5);
        group.RegainTo1MP();
        Debug.Log(group.GetGroupString());
    }

    public global::MOM.Group GetAttackTarget()
    {
        List<global::MOM.Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.owner.GetPlane());
        global::MOM.Group group = null;
        foreach (global::MOM.Group item in groupsOfPlane)
        {
            if (item.GetOwnerID() == PlayerWizard.HumanID() && this.GetAOE().Contains(item.GetPosition()) && (!item.IsHosted() || item.GetLocationHost() is TownLocation))
            {
                if (group == null)
                {
                    group = item;
                }
                else if (item.GetValue() < group.GetValue())
                {
                    group = item;
                }
            }
        }
        return group;
    }
}
