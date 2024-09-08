// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.Unit
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;

[ClassPrototype("UNIT", "")]
public class Unit : Subrace
{
    public new static string abbreviation = "";

    [Prototype("Figures", true)]
    public int figures;

    [Prototype("RequiredBuildings", false)]
    public Building[] requiredBuildings;

    [Prototype("ConstructionCost", false)]
    public int constructionCost;

    [Prototype("PopulationCost", false)]
    public int populationCost;

    public static explicit operator Unit(Enum e)
    {
        return DataBase.Get<Unit>(e);
    }

    public static explicit operator Unit(string e)
    {
        return DataBase.Get<Unit>(e, reportMissing: true);
    }

    public void Set_requiredBuildings(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.requiredBuildings = new Building[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is Building))
            {
                Debug.LogError("requiredBuildings of type Building received invalid type from array! " + list[i]);
            }
            this.requiredBuildings[i] = list[i] as Building;
        }
    }
}
