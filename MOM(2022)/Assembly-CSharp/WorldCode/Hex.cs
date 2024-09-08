using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;

namespace WorldCode
{
    public class Hex
    {
        private static int indexer = 1;

        public const int HEIGHT = 6;

        public const int NON_GRASSLAND = 240;

        public const int LOWLAND = 248;

        public float temperature;

        private Vector3i position;

        private bool activeHex = true;

        public GameObject resourceInstance;

        public GameObject additionalDecorInstance;

        private Terrain terrain;

        private int terrainIndex;

        public int flagSettings;

        public ForegroundMeshPlan foregroundPlan;

        private float rotation;

        private int seaAround = -1;

        public bool[] viaRiver;

        public Mesh foregroundMesh;

        private DBReference<Resource> resource;

        public int index = ++Hex.indexer;

        public List<GameObject> decorativeObjects;

        public int customMPCost;

        public bool ActiveHex
        {
            get
            {
                return this.activeHex;
            }
            set
            {
                this.activeHex = value;
                if (FSMCoreGame.Get() == null)
                {
                    return;
                }
                this.UpdateTownNearby();
                bool flag = false;
                foreach (KeyValuePair<Vector3i, Hex> hex in World.GetArcanus().GetHexes())
                {
                    if (hex.Value != this)
                    {
                        continue;
                    }
                    flag = true;
                    World.GetArcanus().InvalidateTerrain();
                    GameManager gameManager = GameManager.Get();
                    if (gameManager.corruptedArcanus == null)
                    {
                        gameManager.corruptedArcanus = new List<Vector3i>();
                    }
                    if (GameManager.Get().corruptedArcanus.Contains(hex.Key))
                    {
                        if (this.activeHex)
                        {
                            GameManager.Get().corruptedArcanus.Remove(hex.Key);
                        }
                    }
                    else if (!this.activeHex)
                    {
                        GameManager.Get().corruptedArcanus.Add(hex.Key);
                    }
                    break;
                }
                if (flag)
                {
                    return;
                }
                foreach (KeyValuePair<Vector3i, Hex> hex2 in World.GetMyrror().GetHexes())
                {
                    if (hex2.Value != this)
                    {
                        continue;
                    }
                    flag = true;
                    World.GetMyrror().InvalidateTerrain();
                    GameManager gameManager = GameManager.Get();
                    if (gameManager.corruptedMyrror == null)
                    {
                        gameManager.corruptedMyrror = new List<Vector3i>();
                    }
                    if (GameManager.Get().corruptedMyrror.Contains(hex2.Key))
                    {
                        if (this.activeHex)
                        {
                            GameManager.Get().corruptedMyrror.Remove(hex2.Key);
                        }
                    }
                    else if (!this.activeHex)
                    {
                        GameManager.Get().corruptedMyrror.Add(hex2.Key);
                    }
                    break;
                }
            }
        }

        public DBReference<Resource> Resource
        {
            get
            {
                return this.resource;
            }
            set
            {
                this.resource = value;
                if (FSMCoreGame.Get() == null)
                {
                    return;
                }
                foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
                {
                    if (entity.Value is TownLocation)
                    {
                        TownLocation townLocation = entity.Value as TownLocation;
                        if (HexCoordinates.HexDistance(this.Position, townLocation.GetPosition()) <= 2)
                        {
                            townLocation.ResetTownResources();
                        }
                    }
                }
                GameManager.Get().RecordTerrainChange(new TerrainChange(this, value, updateResource: true));
                VerticalMarkerManager.Get().UpdateInfoOnMarker(this);
            }
        }

        public Vector3i Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

        public List<Enchantment> CoastRiverBonusEnchantments()
        {
            List<Enchantment> list = new List<Enchantment>();
            bool num = this.HaveFlag(ETerrainType.Coast);
            bool flag = this.HaveFlag(ETerrainType.RiverBank);
            if (num)
            {
                if (flag)
                {
                    list.Add((Enchantment)ENCH.TOWN_ON_COAST_WITH_RIVER);
                }
                else if (this.terrain != (Terrain)TERRAIN.SEA)
                {
                    list.Add((Enchantment)ENCH.TOWN_ON_COAST);
                }
            }
            else if (flag)
            {
                list.Add((Enchantment)ENCH.TOWN_ON_RIVER);
            }
            return list;
        }

        public void SetFlag(ETerrainType setting)
        {
            this.flagSettings |= (int)setting;
        }

        public void RemoveFlag(ETerrainType setting)
        {
            this.flagSettings &= (int)(~setting);
        }

        public bool HaveFlag(ETerrainType setting)
        {
            return (int)((uint)this.flagSettings & (uint)setting) > 0;
        }

        public bool HaveAnyFlag(int flagCollection)
        {
            return (this.flagSettings & flagCollection) > 0;
        }

        public bool HaveAnyFlag()
        {
            return this.flagSettings > 0;
        }

        public bool HaveAllFlags(int flagCollection)
        {
            return (this.flagSettings & flagCollection) == flagCollection;
        }

        public void ClearAllFlags()
        {
            this.flagSettings = 0;
        }

        public int GetTerrainArrayIndex()
        {
            if (this.terrainIndex == -1)
            {
                this.terrainIndex = DataBase.GetType<Terrain>().IndexOf(this.GetTerrain());
            }
            return this.terrainIndex;
        }

        public Terrain GetTerrain()
        {
            return this.terrain;
        }

        public void SetTerrain(Terrain t, Plane p)
        {
            this.terrain = t;
            this.terrainIndex = -1;
            bool num = this.HaveFlag(ETerrainType.Coast);
            bool flag = this.HaveFlag(ETerrainType.RiverBank);
            this.flagSettings = 0;
            this.SetFlag(t.terrainType);
            if (num)
            {
                this.SetFlag(ETerrainType.Coast);
            }
            if (flag)
            {
                this.SetFlag(ETerrainType.RiverBank);
            }
            GameManager.Get().RecordTerrainChange(new TerrainChange(this, p));
            p.ClearSearcherData();
        }

        public Terrain GetTerrain(MHRandom r, Plane p)
        {
            if (this.terrain != null)
            {
                return this.terrain;
            }
            this.terrain = Hex.FindTerrainOfType(r, this.flagSettings, p);
            if (this.terrain == null)
            {
                Debug.LogError("Terrain with flags: " + this.flagSettings + " not found");
            }
            this.terrainIndex = -1;
            if (this.terrain.terrainGraphic.blockRotation)
            {
                this.rotation = 0f;
            }
            else
            {
                this.rotation = r.GetFloat(0f, 1f);
            }
            if (this.terrain == null)
            {
                Debug.LogError("Terrain missing type to fulfill type requirement: " + this.flagSettings);
            }
            return this.terrain;
        }

        public FInt GetFood()
        {
            if (!this.ActiveHex)
            {
                return FInt.ZERO;
            }
            FInt foodProduction = this.GetTerrain().foodProduction;
            if (this.resource != null)
            {
                foodProduction += this.resource.Get().bonusTypes.food;
            }
            global::MOM.Location locationAt = GameManager.Get().GetLocationAt(this.Position);
            if (locationAt != null && locationAt.locationType == ELocationType.Node)
            {
                if (locationAt.source == (MagicNode)(Enum)MAGIC_NODE.NATURE)
                {
                    foodProduction += Mathf.Max(0f, 2f - foodProduction.ToFloat());
                }
                if (locationAt.source == (MagicNode)(Enum)MAGIC_NODE.SORCERY)
                {
                    foodProduction += Mathf.Max(0f, 2.5f - foodProduction.ToFloat());
                }
            }
            if (this.viaRiver != null)
            {
                foodProduction += 0.5f;
            }
            return foodProduction;
        }

        public FInt GetProduction()
        {
            if (!this.ActiveHex)
            {
                return FInt.ZERO;
            }
            return this.GetTerrain().production;
        }

        public float GetRotation()
        {
            return this.rotation;
        }

        public int MovementCost()
        {
            if (this.customMPCost > 0)
            {
                return this.customMPCost;
            }
            return this.terrain.movementCost;
        }

        public bool IsLand()
        {
            return !this.HaveFlag(ETerrainType.Sea);
        }

        public int SeaAround(Plane p)
        {
            if (this.seaAround == -1)
            {
                this.seaAround = 0;
                foreach (Vector3i item in HexNeighbors.GetRange(this.Position, 1))
                {
                    Vector3i positionWrapping = p.GetPositionWrapping(item);
                    if (p.GetHexAt(positionWrapping).HaveFlag(ETerrainType.Sea))
                    {
                        this.seaAround++;
                    }
                }
            }
            return this.seaAround;
        }

        private static Terrain FindTerrainOfType(MHRandom random, int flags, Plane p)
        {
            List<Terrain> type = DataBase.GetType<Terrain>();
            List<Terrain> list = type.FindAll((Terrain o) => o.plane == p.planeSource.Get() && (int)((uint)o.terrainType & (uint)flags) > 0);
            if (list.Count == 0)
            {
                list = type.FindAll((Terrain o) => (int)((uint)o.terrainType & (uint)flags) > 0);
            }
            if (list.Count == 0)
            {
                return null;
            }
            int @int = random.GetInt(0, list.Count);
            Terrain terrain = list[@int];
            if (terrain.spawnLimitation > FInt.ZERO)
            {
                Terrain terrain2 = terrain;
                int num = 0;
                while (num < 10)
                {
                    if (random.GetFloat(0f, 1f) > terrain.spawnLimitation)
                    {
                        return terrain;
                    }
                    if (terrain2.spawnLimitation > terrain.spawnLimitation)
                    {
                        terrain2 = terrain;
                    }
                    num++;
                    @int = random.GetInt(0, list.Count);
                    terrain = list[@int];
                }
                terrain = terrain2;
            }
            return terrain;
        }

        public void ProduceForegroundObjects(Material foregroundMaterial, Chunk chunk)
        {
            if (this.foregroundPlan != null)
            {
                GameObject gameObject = new GameObject("Foreground_" + this.Position.ToString());
                gameObject.transform.parent = chunk.go.transform;
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                Mesh mesh2 = (meshFilter.mesh = new Mesh());
                mesh2.vertices = this.foregroundPlan.vertices.ToArray();
                mesh2.triangles = this.foregroundPlan.indices.ToArray();
                mesh2.uv = this.foregroundPlan.uv.ToArray();
                mesh2.colors = this.foregroundPlan.colors.ToArray();
                meshRenderer.material = foregroundMaterial;
                mesh2.RecalculateBounds();
                mesh2.RecalculateNormals();
                this.foregroundMesh = mesh2;
            }
        }

        private void UpdateTownNearby()
        {
            foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
            {
                if (entity.Value is TownLocation)
                {
                    TownLocation townLocation = entity.Value as TownLocation;
                    if (HexCoordinates.HexDistance(this.Position, townLocation.GetPosition()) < 3)
                    {
                        townLocation.ResetTownResources();
                    }
                }
            }
        }

        public void UpdateHexProduction()
        {
            if (this.ActiveHex)
            {
                this.UpdateTownNearby();
            }
        }

        public void RecreateViaRiver(int source)
        {
            if (this.viaRiver == null && source > 0)
            {
                this.viaRiver = new bool[6];
                for (int i = 0; i < 6; i++)
                {
                    this.viaRiver[i] = ((source >> i) & 1) > 0;
                }
            }
        }

        public void AddDecor(GameObject go)
        {
            if (this.decorativeObjects == null)
            {
                this.decorativeObjects = new List<GameObject>();
            }
            this.decorativeObjects.Add(go);
        }

        public void RemoveDecors()
        {
            if (this.decorativeObjects == null)
            {
                return;
            }
            foreach (GameObject decorativeObject in this.decorativeObjects)
            {
                global::UnityEngine.Object.Destroy(decorativeObject);
            }
            this.decorativeObjects = null;
        }
    }
}
