namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("TERRAIN", "")]
    public class Terrain : DBClass, IDescriptionInfoType
    {
        public static string abbreviation = "";
        [Prototype("DescriptionInfo", false)]
        public DescriptionInfo descriptionInfo;
        [Prototype("TerrainGraphic", true)]
        public TerrainGraphic terrainGraphic;
        [Prototype("TerrainType", true)]
        public ETerrainType terrainType;
        [Prototype("Plane", false)]
        public DBDef.Plane plane;
        [Prototype("SpawnLimitation", false)]
        public FInt spawnLimitation;
        [Prototype("MovementCost", false)]
        public int movementCost;
        [Prototype("RoadCost", false)]
        public int roadCost;
        [Prototype("Production", false)]
        public FInt production;
        [Prototype("FoodProduction", false)]
        public FInt foodProduction;
        [Prototype("GoldProduction", false)]
        public FInt goldProduction;
        [Prototype("ResourcesSpawnChance", false)]
        public ResourceChance[] resourcesSpawnChance;
        [Prototype("DecorSpawnChance", false)]
        public Decor[] decorSpawnChance;
        [Prototype("Tag", false)]
        public Tag[] tags;
        [Prototype("Foliage", false)]
        public FoliageSet[] foliage;
        [Prototype("TransmuteTo", false)]
        public Terrain transmuteTo;
        [Prototype("MinimapColor", false)]
        public string minimapColor;

        public DescriptionInfo GetDescriptionInfo()
        {
            return this.descriptionInfo;
        }

        public static explicit operator Terrain(Enum e)
        {
            return DataBase.Get<Terrain>(e, false);
        }

        public static explicit operator Terrain(string e)
        {
            return DataBase.Get<Terrain>(e, true);
        }

        public void Set_decorSpawnChance(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.decorSpawnChance = new Decor[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Decor))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("decorSpawnChance of type Decor received invalid type from array! " + text1);
                    }
                    this.decorSpawnChance[i] = list[i] as Decor;
                }
            }
        }

        public void Set_foliage(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.foliage = new FoliageSet[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is FoliageSet))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("foliage of type FoliageSet received invalid type from array! " + text1);
                    }
                    this.foliage[i] = list[i] as FoliageSet;
                }
            }
        }

        public void Set_resourcesSpawnChance(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.resourcesSpawnChance = new ResourceChance[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is ResourceChance))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("resourcesSpawnChance of type ResourceChance received invalid type from array! " + text1);
                    }
                    this.resourcesSpawnChance[i] = list[i] as ResourceChance;
                }
            }
        }

        public void Set_tags(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.tags = new Tag[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Tag))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("tags of type Tag received invalid type from array! " + text1);
                    }
                    this.tags[i] = list[i] as Tag;
                }
            }
        }
    }
}

