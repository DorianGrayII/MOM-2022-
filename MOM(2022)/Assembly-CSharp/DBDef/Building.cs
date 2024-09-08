// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.Building
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;

[ClassPrototype("BUILDING", "")]
public class Building : DBClass, IDescriptionInfoType
{
    public static string abbreviation = "";

    [Prototype("DescriptionInfo", false)]
    public DescriptionInfo descriptionInfo;

    [Prototype("ParentBuildingRequired", false)]
    public Building[] parentBuildingRequired;

    [Prototype("BuildingResourceBonus", false)]
    public BuildingResourceBonus[] buildingResourceBonus;

    [Prototype("BuildCost", true)]
    public int buildCost;

    [Prototype("UpkeepCost", true)]
    public int upkeepCost;

    [Prototype("UpkeepManaCost", false)]
    public int upkeepManaCost;

    [Prototype("Enchantments", false)]
    public Enchantment[] enchantments;

    [Prototype("Tags", false)]
    public Tag[] tags;

    [Prototype("MarineBuilding", false)]
    public bool marineBuilding;

    public DescriptionInfo GetDescriptionInfo()
    {
        return this.descriptionInfo;
    }

    public static explicit operator Building(Enum e)
    {
        return DataBase.Get<Building>(e);
    }

    public static explicit operator Building(string e)
    {
        return DataBase.Get<Building>(e, reportMissing: true);
    }

    public void Set_parentBuildingRequired(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.parentBuildingRequired = new Building[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is Building))
            {
                Debug.LogError("parentBuildingRequired of type Building received invalid type from array! " + list[i]);
            }
            this.parentBuildingRequired[i] = list[i] as Building;
        }
    }

    public void Set_buildingResourceBonus(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.buildingResourceBonus = new BuildingResourceBonus[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is BuildingResourceBonus))
            {
                Debug.LogError("buildingResourceBonus of type BuildingResourceBonus received invalid type from array! " + list[i]);
            }
            this.buildingResourceBonus[i] = list[i] as BuildingResourceBonus;
        }
    }

    public void Set_enchantments(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.enchantments = new Enchantment[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is Enchantment))
            {
                Debug.LogError("enchantments of type Enchantment received invalid type from array! " + list[i]);
            }
            this.enchantments[i] = list[i] as Enchantment;
        }
    }

    public void Set_tags(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.tags = new Tag[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is Tag))
            {
                Debug.LogError("tags of type Tag received invalid type from array! " + list[i]);
            }
            this.tags[i] = list[i] as Tag;
        }
    }
}
