using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;

namespace WorldCode
{
    public class PlaneBattle : Plane
    {
        private int terrainMargin = 2;

        public override void UpdateChunksPositionForWrap()
        {
        }

        protected override void I1_PlanHeight(object o)
        {
            base.pathfindingArea = new V3iRect(base.size.x, base.size.y, Vector3i.zero, !base.battlePlane);
            base.size.x += this.terrainMargin;
            base.size.y += this.terrainMargin;
            base.area = new V3iRect(base.size.x, base.size.y, Vector3i.zero, !base.battlePlane);
            List<Vector3i> areaHex = base.area.GetAreaHex();
            base.perlin = new PerlinMap();
            base.perlin.Initialize(base.random, 130f);
            _ = (float)base.size.y / (float)base.size.x;
            base.height = new Dictionary<Vector3i, float>();
            new List<Vector3i>();
            new List<Vector3i>();
            float waterLevel = base.settings.waterLevel;
            foreach (Vector3i item in areaHex)
            {
                if (waterLevel > 0.99f)
                {
                    base.height[item] = 0f;
                }
                else
                {
                    base.height[item] = base.random.GetFloat(0f, 1f);
                }
            }
            base.heightSorted = new float[base.height.Values.Count];
            if (waterLevel > 0.99f)
            {
                return;
            }
            base.height.Values.CopyTo(base.heightSorted, 0);
            Array.Sort(base.heightSorted);
            base.settings.hillsAboveLevel = 0.7f;
            base.settings.mountainAboveLevel = 1f;
            if (base.settings.sourceHex != null)
            {
                if (base.settings.sourceHex.HaveFlag(ETerrainType.Mountain))
                {
                    base.settings.hillsAboveLevel = base.heightSorted[(int)((float)base.heightSorted.Length * 0.7f)];
                }
                else if (base.settings.sourceHex.HaveFlag(ETerrainType.Hill))
                {
                    base.settings.hillsAboveLevel = base.heightSorted[(int)((float)base.heightSorted.Length * 0.8f)];
                }
                else
                {
                    base.settings.hillsAboveLevel = base.heightSorted[(int)((float)base.heightSorted.Length * 0.9f)];
                }
            }
        }

        protected override void I2_InitializeHexData(object o)
        {
            base.height.Values.CopyTo(base.heightSorted, 0);
            Array.Sort(base.heightSorted);
            Battle battle = Battle.GetBattle();
            float yAspectRatio = (float)base.size.y / (float)base.size.x;
            base.hexes = new Dictionary<Vector3i, Hex>();
            foreach (KeyValuePair<Vector3i, float> item in base.height)
            {
                Hex hex = new Hex();
                hex.Position = item.Key;
                if (!battle.landBattle)
                {
                    hex.SetFlag(ETerrainType.Sea);
                }
                else
                {
                    float num = 1f;
                    if (battle.gDefender != null && battle.gDefender.IsHosted())
                    {
                        if (battle.gDefender.GetLocationHost() is TownLocation)
                        {
                            num = Mathf.Clamp01((float)Mathf.Abs(item.Key.z) * 0.2f);
                        }
                        else if (HexCoordinates.HexDistance(new Vector3i(-10, 10), hex.Position) <= 2)
                        {
                            num *= 0f;
                        }
                    }
                    if (item.Value * num > base.settings.hillsAboveLevel)
                    {
                        hex.SetFlag(ETerrainType.Hill);
                    }
                    else
                    {
                        this.SetHexBio(hex, base.perlin, base.area, yAspectRatio, base.settings);
                    }
                }
                base.hexes[item.Key] = hex;
                hex.GetTerrain(base.random, this);
            }
            new HashSet<Vector3i>();
        }

        protected override void I4_PlanRivers(object o)
        {
        }

        protected override void FinalizePlaneFormation(object o)
        {
            foreach (Vector3i item in new List<Vector3i>(base.area.GetAreaHex()))
            {
                if (!base.pathfindingArea.IsInside(item))
                {
                    if (base.height != null && base.height.ContainsKey(item))
                    {
                        base.height.Remove(item);
                    }
                    if (base.hexes != null && base.hexes.ContainsKey(item))
                    {
                        base.hexes.Remove(item);
                    }
                    if (base.landHexes != null && base.landHexes.Contains(item))
                    {
                        base.landHexes.Remove(item);
                    }
                }
            }
        }

        private void SetHexBio(Hex hex, PerlinMap perlinMap, V3iRect rect, float yAspectRatio, PlaneSettings settings)
        {
            Vector2 uV = rect.GetUV(hex.Position);
            Vector2 pos = uV;
            pos.y *= yAspectRatio;
            float num = perlinMap.ProduceValueAtLayer(PerlinMap.Layer.Humidity3, pos);
            float num2 = perlinMap.ProduceValueAtLayer(PerlinMap.Layer.Forest1, pos);
            float num3 = perlinMap.ProduceValueAtLayer(PerlinMap.Layer.Forest2, pos);
            if (settings.overrideTemperature > -1f)
            {
                hex.temperature = settings.overrideTemperature;
            }
            else
            {
                float f = uV.y * 2f * (float)Math.PI;
                f = (Mathf.Cos(f) + 1f) * 0.5f;
                hex.temperature = 1f - f;
            }
            Battle battle = Battle.GetBattle();
            num = battle.humidity + 0.3f * (-0.5f + num);
            float num4 = battle.forest + 0.3f * (-0.5f + num2 * num3 / 2f);
            _ = battle.temperature;
            _ = hex.temperature;
            if (settings.sourceHex != null)
            {
                float num5 = 1f;
                if (battle.gDefender != null && battle.gDefender.GetLocationHost() is TownLocation)
                {
                    num5 = Mathf.Clamp01((float)Mathf.Abs(hex.Position.z) * 0.2f);
                }
                float num6 = (settings.sourceHex.HaveFlag(ETerrainType.Forest) ? 1f : 0.2f);
                if (num4 * num5 * num6 > 0.15f && num > 0.5f)
                {
                    hex.SetFlag(ETerrainType.Forest);
                }
                else if (settings.sourceHex.HaveFlag(ETerrainType.Tundra))
                {
                    hex.SetFlag(ETerrainType.Tundra);
                }
                else if (settings.sourceHex.HaveFlag(ETerrainType.Desert))
                {
                    hex.SetFlag(ETerrainType.Desert);
                }
                else if (base.random.GetInt(0, 100) > 50 && settings.sourceHex.HaveFlag(ETerrainType.Swamp))
                {
                    hex.SetFlag(ETerrainType.Swamp);
                }
            }
            if (!hex.HaveAnyFlag())
            {
                hex.SetFlag(ETerrainType.GrassLand);
            }
        }

        protected override void SpawnResource(MeshCell meshCell, Chunk c)
        {
        }
    }
}
