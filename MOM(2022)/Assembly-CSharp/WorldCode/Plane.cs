// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WorldCode.Plane
using System;
using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class Plane
{
    public const int TERRAIN_LAYER = 9;

    public DBReference<global::DBDef.Plane> planeSource;

    public bool battlePlane;

    public bool arcanusType;

    public GameObject goTerrain;

    public DataHeatMaps dataHeatMaps;

    protected MHRandom random;

    protected HeightTexture riverHeightTexture;

    protected PlaneSettings settings;

    protected Dictionary<Vector3i, float> height;

    protected Dictionary<Vector3i, Hex> hexes;

    protected HashSet<Vector3i> landHexes;

    protected HashSet<Vector3i> waterHexes;

    protected HashSet<Vector3i> hexesWithMeshToUpdate;

    protected Vector2i size;

    public V3iRect area;

    public V3iRect pathfindingArea;

    protected PerlinMap perlin;

    public PlaneMeshData meshData;

    protected float[] heightSorted;

    protected Texture2D terrainData;

    public Texture2D settlerDataTexture;

    public bool isSettlerDataReady;

    protected bool terrainDataValid;

    protected TerrainMarkers markers;

    public Material material;

    protected Material fgMaterial;

    protected List<List<Vector3i>> islands;

    public List<HashSet<Vector3i>> water;

    public Dictionary<Vector3i, int> waterBodies;

    private SearcherDataV2 searcherData;

    public HashSet<Vector3i> temporaryVisibleArea;

    public HashSet<Vector3i> exclusionPoints;

    private bool markersDirty;

    public int searcherIteration;

    public SettlerDataManager settlerData;

    public int meshQuality;

    public Plane()
    {
        MHZombieMemoryDetector.Track(this);
    }

    public void Destroy()
    {
        if (this.markers != null)
        {
            this.markers.Destroy();
        }
        World.RemovePlane(this);
        if (this.settlerDataTexture != null)
        {
            global::UnityEngine.Object.Destroy(this.settlerDataTexture);
        }
        if (!(this.goTerrain != null))
        {
            return;
        }
        if (this.hexes != null)
        {
            foreach (KeyValuePair<Vector3i, Hex> hex in this.hexes)
            {
                if (hex.Value.foregroundMesh != null)
                {
                    global::UnityEngine.Object.Destroy(hex.Value.foregroundMesh);
                }
            }
        }
        global::UnityEngine.Object.Destroy(this.goTerrain);
        global::UnityEngine.Object.Destroy(this.material);
        global::UnityEngine.Object.Destroy(this.fgMaterial);
        global::UnityEngine.Object.Destroy(this.terrainData);
    }

    public void Cleanup()
    {
        Debug.LogWarning("Cleanup plane for reuse in case this implementation is needed");
    }

    public IEnumerator RecreatePlane(DBReference<global::DBDef.Plane> planeSource, string name, int seed, Vector2i size, PlaneSettings settings)
    {
        this.meshData = new PlaneMeshData();
        this.planeSource = planeSource;
        this.arcanusType = planeSource.Get() == (global::DBDef.Plane)PLANE.ARCANUS;
        this.random = new MHRandom(seed);
        this.settings = settings;
        this.size = size;
        this.meshQuality = Settings.GetMeshQuality();
        this.markers = new TerrainMarkers();
        this.markers.Initialize(size.x * 2, size.y * 2, this);
        TerrainTextures.Get().InitializeCache();
        MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0f);
        yield return null;
        MHTimer t = MHTimer.StartNew();
        if (planeSource.Get() == (global::DBDef.Plane)PLANE.ARCANUS)
        {
            this.meshData = ProtoLibrary.GetInstance().arcanus.Get();
        }
        else
        {
            this.meshData = ProtoLibrary.GetInstance().myrror.Get();
        }
        bool error = false;
        yield return MHNonThread.ExecuteSequence(new global::MHUtils.Callback[2] { I1_PlanHeight, I2_InitializeHexData }, null, null, delegate
        {
            error = true;
        }, delegate(object o)
        {
            MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, (float)o * 0.2f);
        });
        Debug.Log("T-1 " + t.GetTime());
        this.goTerrain = this.meshData.GenerateInitialGO(name, this.GetTerrainMaterial());
        MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.7f);
        Debug.Log("T-2 " + t.GetTime());
        yield return null;
        this.I11_InitializeForeground(null);
        MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.8f);
        Debug.Log("T-3 " + t.GetTime());
        this.I12_DesignForegroundMesh(null);
        MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.9f);
        Debug.Log("T-4 " + t.GetTime());
        yield return null;
        this.I13_InstantiateResourcesAndEffects(null);
        MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 1f);
        Debug.Log("T-5 " + t.GetTime());
        yield return null;
        this.FinalizePlaneFormation(null);
    }

    public IEnumerator InitializePlane(DBReference<global::DBDef.Plane> planeSource, string name, int seed, Vector2i size, PlaneSettings settings)
    {
        this.meshData = new PlaneMeshData();
        this.planeSource = planeSource;
        this.arcanusType = planeSource.Get() == (global::DBDef.Plane)PLANE.ARCANUS;
        this.random = new MHRandom(seed);
        this.settings = settings;
        this.size = size;
        this.meshQuality = Settings.GetMeshQuality();
        this.markers = new TerrainMarkers();
        this.markers.Initialize(size.x * 2, size.y * 2, this);
        TerrainTextures.Get().InitializeCache();
        MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0f);
        yield return null;
        MHTimer t = MHTimer.StartNew();
        bool error = false;
        yield return MHNonThread.ExecuteSequence(new global::MHUtils.Callback[5] { I1_PlanHeight, I2_InitializeHexData, I3_ConstructVerticesData, I4_PlanRivers, I5_PlanSubdivide }, null, null, delegate
        {
            error = true;
        }, delegate(object o)
        {
            MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, (float)o * 0.1f);
        });
        if (!error)
        {
            MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.1f);
            Debug.Log("T1 " + t.GetTime());
            yield return null;
            this.I6_Triangulate();
            MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.2f);
            Debug.Log("T2 " + t.GetTime());
            yield return null;
            yield return this.I7_UpdateMeshByTerrainHeight();
            MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.7f);
            Debug.Log("T3 " + t.GetTime());
            yield return null;
            yield return MHNonThread.ExecuteSequence(new global::MHUtils.Callback[3] { I8_SmoothenRivers, I9_PlanRiverMeshes, I10_ImprintRiversIntoTerrain }, null, null, delegate
            {
                error = true;
            }, delegate(object o)
            {
                MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.8f + (float)o * 0.1f);
            });
            if (!error)
            {
                MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.8f);
                Debug.Log("T4 " + t.GetTime());
                yield return null;
                this.goTerrain = this.meshData.GenerateInitialGO(name, this.GetTerrainMaterial());
                MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.9f);
                Debug.Log("T5 " + t.GetTime());
                yield return null;
                this.I11_InitializeForeground(null);
                MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.93f);
                Debug.Log("T6 " + t.GetTime());
                yield return null;
                this.I12_DesignForegroundMesh(null);
                MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 0.95f);
                Debug.Log("T7 " + t.GetTime());
                yield return null;
                this.I13_InstantiateResourcesAndEffects(null);
                MHEventSystem.TriggerEvent<global::WorldCode.Plane>(this, 1f);
                Debug.Log("T8 " + t.GetTime());
                yield return null;
                this.FinalizePlaneFormation(null);
            }
        }
    }

    public DataHeatMaps GetDataHeatMaps()
    {
        if (this.dataHeatMaps == null)
        {
            this.dataHeatMaps = new DataHeatMaps(this);
        }
        return this.dataHeatMaps;
    }

    protected virtual void I1_PlanHeight(object o)
    {
        this.pathfindingArea = new V3iRect(this.size.x, this.size.y, Vector3i.zero, !this.battlePlane);
        this.area = new V3iRect(this.size.x, this.size.y, Vector3i.zero, !this.battlePlane);
        List<Vector3i> areaHex = this.area.GetAreaHex();
        this.perlin = new PerlinMap();
        this.perlin.Initialize(this.random, 130f);
        float num = (float)this.size.y / (float)this.size.x;
        this.height = new Dictionary<Vector3i, float>();
        new List<Vector3i>();
        new List<Vector3i>();
        foreach (Vector3i item in areaHex)
        {
            this.height[item] = global::WorldCode.Plane.GetTerrainHeight(item, this.perlin, this.pathfindingArea, num, this.settings.mapBorders);
        }
        float waterLevel = this.settings.waterLevel;
        this.heightSorted = new float[this.height.Values.Count];
        this.height.Values.CopyTo(this.heightSorted, 0);
        Array.Sort(this.heightSorted);
        int num2 = this.heightSorted.Length - 1;
        int num3 = (int)(waterLevel * (float)num2);
        float num4 = ((num3 != num2) ? ((this.heightSorted[num3] + this.heightSorted[num3 + 1]) / 2f) : this.heightSorted[num3]);
        foreach (Vector3i item2 in new List<Vector3i>(this.height.Keys))
        {
            if (num4 < 0.001f)
            {
                this.height[item2] = Mathf.Lerp(0.51f, 1f, this.height[item2]);
            }
            else if (num4 > 0.999f)
            {
                this.height[item2] = Mathf.Lerp(0f, 0.5f, this.height[item2]);
            }
            else if (this.height[item2] < num4)
            {
                this.height[item2] = Mathf.Lerp(0f, 0.5f, this.height[item2] / num4);
            }
            else
            {
                float num5 = Mathf.Lerp(0.51f, 1f, (this.height[item2] - num4) / (1f - num4));
                Vector2 uV = this.pathfindingArea.GetUV(item2);
                uV.y *= num;
                float num6 = this.perlin.ProduceValueAtLayer(PerlinMap.Layer.Height4, uV);
                num5 -= 0.5f;
                num5 = Mathf.Clamp01(num6 * (num5 + 0.25f)) + 0.5f;
                num5 = Mathf.Clamp(num5, 0.51f, 1f);
                this.height[item2] = num5;
            }
            if (num4 > 0.001f)
            {
                Vector2i vector2i = this.area.ConvertHexTo2DIntUVLoation(item2);
                if (vector2i.x < 2 || vector2i.x >= this.area.AreaWidth - 2 || vector2i.y < 3 || vector2i.y >= this.area.AreaHeight - 3)
                {
                    this.height[item2] = 0f;
                }
            }
        }
    }

    protected virtual void I2_InitializeHexData(object o)
    {
        this.height.Values.CopyTo(this.heightSorted, 0);
        Array.Sort(this.heightSorted);
        float yAspectRatio = (float)this.size.y / (float)this.size.x;
        this.hexes = new Dictionary<Vector3i, Hex>();
        foreach (KeyValuePair<Vector3i, float> item in this.height)
        {
            Hex hex = new Hex();
            hex.Position = item.Key;
            if (item.Value <= 0.5f)
            {
                hex.SetFlag(ETerrainType.Sea);
            }
            else if (item.Value > 1f - this.settings.mountainAboveLevel)
            {
                hex.SetFlag(ETerrainType.Mountain);
            }
            else if (item.Value > 1f - this.settings.hillsAboveLevel)
            {
                hex.SetFlag(ETerrainType.Hill);
            }
            else
            {
                this.SetHexBio(hex, this.perlin, this.pathfindingArea, yAspectRatio, this.settings);
            }
            this.hexes[item.Key] = hex;
            hex.GetTerrain(this.random, this);
        }
        new HashSet<Vector3i>();
        foreach (KeyValuePair<Vector3i, Hex> hex2 in this.hexes)
        {
            Terrain terrain = hex2.Value.GetTerrain();
            if (terrain.resourcesSpawnChance == null)
            {
                continue;
            }
            int num = terrain.resourcesSpawnChance.Length;
            int @int = this.random.GetInt(0, num);
            for (int i = 0; i < num; i++)
            {
                int num2 = (@int + i) % num;
                if (terrain.resourcesSpawnChance[num2].chance >= this.random.GetFloat(0f, 1f) && terrain.resourcesSpawnChance[num2].resource != null && DLCManager.IsDlcActive(terrain.resourcesSpawnChance[num2].resource.dlc))
                {
                    hex2.Value.Resource = terrain.resourcesSpawnChance[num2].resource;
                    break;
                }
            }
        }
    }

    private void I3_ConstructVerticesData(object o)
    {
        foreach (KeyValuePair<Vector3i, Hex> hex in this.hexes)
        {
            this.meshData.AddCell(hex.Value);
        }
        foreach (KeyValuePair<Vector3i, Hex> hex2 in this.hexes)
        {
            MeshCell meshCell = this.meshData.GetMeshCell(hex2.Key);
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3i pos = hex2.Key + HexNeighbors.neighbours[i];
                MeshCell meshCell2 = this.meshData.GetMeshCell(pos);
                if (meshCell2 != null)
                {
                    meshCell.LinkWithNeighbor(meshCell2, i);
                    meshCell2.LinkWithNeighbor(meshCell, (i + 3) % 6);
                }
            }
        }
        foreach (KeyValuePair<Vector3i, Hex> hex3 in this.hexes)
        {
            Vector3 position = HexCoordinates.HexToWorld3D(hex3.Key);
            Vertex vertex = this.meshData.AddVertex(position);
            MeshCell meshCell3 = this.meshData.GetMeshCell(hex3.Key);
            meshCell3.AddVertex(vertex);
            meshCell3.center = vertex;
            for (int j = 0; j < 6; j++)
            {
                if (meshCell3.GetCorner(j) == null)
                {
                    Vector3i vector3i = hex3.Key + HexNeighbors.neighbours[j];
                    Vector3i vector3i2 = hex3.Key + HexNeighbors.neighbours[(j + 1) % 6];
                    MeshCell meshCell4 = this.meshData.GetMeshCell(vector3i);
                    MeshCell meshCell5 = this.meshData.GetMeshCell(vector3i2);
                    Vector3 position2 = meshCell3.FindCorner(vector3i, vector3i2);
                    Vertex v = this.meshData.AddVertex(position2);
                    meshCell3.AddCorner(v, j);
                    meshCell4?.AddCorner(v, (j + 2) % 6);
                    meshCell5?.AddCorner(v, (j + 4) % 6);
                }
            }
        }
        this.meshData.CellDataTexture = new Color[this.meshData.GetLocalCells().Count];
        foreach (KeyValuePair<Vector3i, ProtoRef<MeshCell>> localCell in this.meshData.GetLocalCells())
        {
            int num = (localCell.Key.x + this.area.AreaWidth) % this.area.AreaWidth;
            int num2 = (localCell.Key.y + this.area.AreaHeight) % this.area.AreaHeight;
            Color dataColor = localCell.Value.Get().GetDataColor();
            this.meshData.CellDataTexture[num + num2 * this.area.AreaWidth] = dataColor;
        }
    }

    protected virtual void I4_PlanRivers(object o)
    {
        foreach (KeyValuePair<Vector3i, Hex> hex in this.hexes)
        {
            if (!hex.Value.HaveAnyFlag(6) || hex.Value.HaveFlag(ETerrainType.Coast))
            {
                continue;
            }
            bool flag = false;
            Vector3i[] neighbours = HexNeighbors.neighbours;
            foreach (Vector3i vector3i in neighbours)
            {
                Vector3i key = hex.Key + vector3i;
                if (this.hexes.ContainsKey(key))
                {
                    if (this.hexes[key].HaveFlag(ETerrainType.Coast))
                    {
                        flag = false;
                        break;
                    }
                    if (!this.hexes[key].HaveAnyFlag(6))
                    {
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                this.meshData.AddRiverStartOption(hex.Key);
            }
        }
        while (this.meshData.GetRiverStartCount() > 0)
        {
            Vector3i pos = this.meshData.GetNewRiverStart(this.random);
            int clearRadius = 1;
            if (RiverFactory.CreateRiver(this.random, this.meshData, pos))
            {
                clearRadius = 5;
            }
            this.meshData.GetRiverStartOptions().RemoveAll((Vector3i obj) => HexCoordinates.HexDistance(obj, pos) <= clearRadius);
        }
    }

    private void I5_PlanSubdivide(object o)
    {
        foreach (KeyValuePair<Vector2i, ProtoRef<Chunk>> chunk in this.meshData.GetChunks())
        {
            if (chunk.Value.Get().GetCells() == null)
            {
                continue;
            }
            foreach (ProtoRef<MeshCell> cell in chunk.Value.Get().GetCells())
            {
                ((MeshCell)cell).CalculateTriangulateDivision();
            }
        }
    }

    private void I6_Triangulate()
    {
        List<object> list = new List<object>(this.meshData.GetChunks().GetDict().Values);
        _ = new BasicTask[list.Count];
        CallbackRet callbackRet = delegate(object o)
        {
            (o as ProtoRef<Chunk>).Get().TriangulateData();
            return (object)null;
        };
        for (int i = 0; i < list.Count; i++)
        {
            callbackRet(list[i]);
        }
    }

    private IEnumerator I7_UpdateMeshByTerrainHeight()
    {
        List<object> data = new List<object>(this.meshData.GetLocalCells().GetDict().Values);
        yield return MHNonThread.CreateMulti(delegate(object o)
        {
            foreach (ProtoRef<Vertex> vertex in (o as ProtoRef<MeshCell>).Get().vertices)
            {
                if (vertex.Get().position.y == 0f)
                {
                    vertex.Get().SetVertexHeight(this.meshData);
                }
            }
            return (object)null;
        }, data);
    }

    private void I8_SmoothenRivers(object o)
    {
        List<List<MHVector3>> list = new List<List<MHVector3>>();
        for (int i = 0; i < int.MaxValue; i++)
        {
            List<MHVector3> list2 = new List<MHVector3>();
            List<Vertex> river = this.meshData.GetRiver(i);
            if (river == null || river.Count < 4)
            {
                break;
            }
            for (int j = 0; j < river.Count; j++)
            {
                list2.Add(river[j].position);
            }
            list.Add(list2);
        }
        float t = 0.3f;
        for (int k = 0; k < list.Count; k++)
        {
            List<MHVector3> list3 = list[k];
            Vector3 vector = list[k][0];
            for (int l = 1; l < list3.Count - 5; l++)
            {
                Vector3 vector2 = vector;
                MHVector3 mHVector = list3[l];
                MHVector3 mHVector2 = list3[l + 1];
                MHVector3 mHVector3 = mHVector - vector2;
                MHVector3 mHVector4 = (mHVector2 - vector2) / 2f;
                vector = mHVector;
                list3[l] = Vector3.Lerp(vector2 + mHVector3, vector2 + mHVector4, t);
            }
        }
        for (int m = 0; m < list.Count; m++)
        {
            List<MHVector3> list4 = list[m];
            List<MHVector3> list5 = new List<MHVector3>();
            for (int n = 1; n < list4.Count - 2; n++)
            {
                Vector3 vector3 = list4[n - 1];
                Vector3 vector4 = list4[n];
                Vector3 vector5 = list4[n + 1];
                Vector3 vector6 = vector4 - vector3;
                Vector3 vector7 = vector4 + vector6;
                Vector3 vector8 = vector4 + (vector7 - vector4) * 1f / 3f;
                _ = vector4 + (vector7 - vector4) * 2f / 3f;
                Vector3 vector9 = vector5 + (vector7 - vector5) * 1f / 3f;
                Vector3 vector10 = vector5 + (vector7 - vector5) * 2f / 3f;
                list5.Add(vector4);
                if (list4.Count > n + 2)
                {
                    Vector3 vector11 = list4[n + 2];
                    Vector3 rhs = vector5 - vector3;
                    Vector3 rhs2 = vector11 - vector3;
                    vector6.Normalize();
                    rhs.Normalize();
                    rhs2.Normalize();
                    float num = Vector3.Dot(vector6, rhs);
                    float num2 = Vector3.Dot(vector6, rhs2);
                    if (num > num2)
                    {
                        Vector3 vector12 = vector4 + (vector9 - vector4) * 1f / 3f;
                        Vector3 vector13 = vector5 + (vector8 - vector5) * 1f / 3f;
                        list5.Add(vector12);
                        list5.Add((vector12 + vector13) / 2f);
                        list5.Add(vector13);
                    }
                    else
                    {
                        list5.Add(vector4 + (vector10 - vector4) * 1f / 4f);
                        list5.Add(vector4 + (vector9 - vector4) * 1f / 2f);
                        list5.Add(vector5 + (vector4 - vector5) * 1f / 4f);
                    }
                }
                else
                {
                    list5.Add(vector4 + (vector10 - vector4) * 1f / 4f);
                    list5.Add(vector4 + (vector9 - vector4) * 1f / 2f);
                    list5.Add(vector5 + (vector4 - vector5) * 1f / 4f);
                }
            }
            list5.Add(list4[list4.Count - 2]);
            list5.Add(list4[list4.Count - 1]);
            int num3 = 1;
            List<MHVector3> list6 = new List<MHVector3>(list5.Count * (1 + num3));
            for (int num4 = 0; num4 < list5.Count - 1; num4++)
            {
                list6.Add(list5[num4]);
                MHVector3 mHVector5 = list5[num4 + 1] - list5[num4];
                for (int num5 = 1; num5 <= num3; num5++)
                {
                    list6.Add(list5[num4] + mHVector5 * num5 / (num3 + 1));
                }
            }
            list5 = list6;
            for (int num6 = 0; num6 < list5.Count - 2; num6++)
            {
                MHVector3 mHVector6 = list5[num6];
                MHVector3 mHVector7 = list5[num6 + 1];
                MHVector3 mHVector8 = list5[num6 + 2];
                MHVector3 mHVector9 = mHVector7 - mHVector6;
                MHVector3 mHVector10 = (mHVector8 - mHVector6) / 2f;
                list5[num6] = Vector3.Lerp(mHVector6 + mHVector9, mHVector6 + mHVector10, t);
            }
            list[m] = list5;
        }
        this.meshData.RiversEased = list;
    }

    private void I9_PlanRiverMeshes(object o)
    {
        this.meshData.RiverTrangles = new List<List<MeshTriangle2D>>();
        List<List<(Vector3, Vector2)>> list = new List<List<(Vector3, Vector2)>>();
        float num = 0.3f;
        foreach (List<MHVector3> item in this.meshData.RiversEased)
        {
            float num2 = 0f;
            float num3 = 0.125f;
            List<(Vector3, Vector2)> list2 = new List<(Vector3, Vector2)>();
            for (int i = 1; i < item.Count - 1; i++)
            {
                MHVector3 mHVector = item[i - 1];
                MHVector3 mHVector2 = item[i];
                MHVector3 mHVector3 = item[i + 1];
                float num4 = (mHVector2 - mHVector).Magnitude();
                float num5 = (mHVector3 - mHVector2).Magnitude();
                mHVector.y = 0f;
                mHVector2.y = 0f;
                mHVector3.y = 0f;
                Vector3 vector;
                if (i == 1)
                {
                    vector = mHVector2 - mHVector;
                    vector = new Vector3(vector.z, 0f, 0f - vector.x);
                    vector.Normalize();
                    list2.Add((mHVector + vector * num, new Vector2(1f, num2)));
                    list2.Add((mHVector - vector * num, new Vector2(0f, num2)));
                }
                num2 += num4 * num3;
                vector = mHVector3 - mHVector;
                vector = new Vector3(vector.z, 0f, 0f - vector.x);
                vector.Normalize();
                list2.Add((mHVector2 + vector * num, new Vector2(1f, num2)));
                list2.Add((mHVector2 - vector * num, new Vector2(0f, num2)));
                if (i == item.Count - 2)
                {
                    vector = mHVector3 - mHVector2;
                    vector = new Vector3(vector.z, 0f, 0f - vector.x);
                    vector.Normalize();
                    list2.Add((mHVector3 + vector * num, new Vector2(1f, num2 + num5 * num3)));
                    list2.Add((mHVector3 - vector * num, new Vector2(0f, num2 + num5 * num3)));
                }
            }
            list.Add(list2);
            List<MeshTriangle2D> list3 = new List<MeshTriangle2D>();
            this.meshData.RiverTrangles.Add(list3);
            for (int j = 0; j < list2.Count - 2; j += 2)
            {
                (Vector3, Vector2) p = list2[j];
                (Vector3, Vector2) tuple = list2[j + 1];
                (Vector3, Vector2) tuple2 = list2[j + 2];
                (Vector3, Vector2) p2 = list2[j + 3];
                list3.Add(MeshTriangle2D.Create(p, tuple, tuple2));
                list3.Add(MeshTriangle2D.Create(tuple, tuple2, p2));
            }
        }
    }

    public void Discover(List<Vector3i> path, int range)
    {
        if (path == null || range <= 0)
        {
            return;
        }
        if (this.temporaryVisibleArea == null)
        {
            this.temporaryVisibleArea = new HashSet<Vector3i>();
        }
        foreach (Vector3i item in path)
        {
            foreach (Vector3i item2 in HexNeighbors.GetRange(item, range))
            {
                Vector3i positionWrapping = this.GetPositionWrapping(item2);
                if (this.area.IsInside(positionWrapping))
                {
                    this.temporaryVisibleArea.Add(this.GetPositionWrapping(positionWrapping));
                }
            }
        }
    }

    private void I10_ImprintRiversIntoTerrain(object o)
    {
        List<(Vector3, float)> list = new List<(Vector3, float)>();
        for (int i = 0; i < this.meshData.RiverCount; i++)
        {
            float num = 0f;
            Vector3 vector = Vector3.zero;
            List<Vertex> river = this.meshData.GetRiver(i);
            for (int j = 0; j < river.Count; j++)
            {
                MHVector3 position = river[j].position;
                if (num == 0f)
                {
                    MHVector3 mHVector = river[river.Count - 1].position - position;
                    num = (mHVector / 2f).Magnitude();
                    vector = position + mHVector / 2f;
                    continue;
                }
                float num2 = (vector - position).Magnitude();
                if (num2 > num)
                {
                    num = num2;
                }
            }
            list.Add((vector, num + 1f));
        }
        foreach (KeyValuePair<Vector3i, ProtoRef<MeshCell>> localCell in this.meshData.GetLocalCells())
        {
            bool flag = false;
            for (int k = 0; k < 6; k++)
            {
                flag = localCell.Value.Get().GetCorner(k).isRiver;
                if (flag)
                {
                    break;
                }
            }
            if (!flag)
            {
                continue;
            }
            for (int l = 0; l < list.Count; l++)
            {
                (Vector3, float) tuple = list[l];
                MeshCell meshCell = localCell.Value.Get();
                if (!((meshCell.center.position - tuple.Item1).Magnitude() < tuple.Item2))
                {
                    continue;
                }
                List<MeshTriangle2D> list2 = this.meshData.RiverTrangles[l];
                foreach (ProtoRef<Vertex> vertex in meshCell.vertices)
                {
                    Vertex v = vertex.Get();
                    MeshTriangle2D meshTriangle2D = list2.Find((MeshTriangle2D obj) => obj.IsWithin(v.position));
                    if (meshTriangle2D != null)
                    {
                        Vector2 uV = meshTriangle2D.GetUV(v.position);
                        int num3 = (int)uV.y;
                        float value = (float)num3 / 255f;
                        float z = uV.y - (float)num3;
                        v.riverUVVA.x = Mathf.Clamp01(uV.x);
                        v.riverUVVA.y = Mathf.Clamp01(value);
                        v.riverUVVA.z = z;
                        v.riverUVVA.w = 1f;
                        Vector3 vector2 = (meshTriangle2D.p3 - meshTriangle2D.p1).Normalized() * 0.5f + new Vector3(0.5f, 0f, 0.5f);
                        v.riverUVNormal = new Vector2(vector2.x, vector2.z);
                    }
                }
            }
        }
    }

    private void I11_InitializeForeground(object o)
    {
        foreach (KeyValuePair<Vector2i, ProtoRef<Chunk>> chunk in this.meshData.GetChunks())
        {
            foreach (ProtoRef<MeshCell> cell in chunk.Value.Get().GetCells())
            {
                if (cell.Get().hex == null && this.hexes != null)
                {
                    Vector3i position = cell.Get().position;
                    if (this.hexes.ContainsKey(position))
                    {
                        Hex hex = this.hexes[position];
                        if (cell.Get().ownerTerrainFlags != 0)
                        {
                            hex.flagSettings = cell.Get().ownerTerrainFlags;
                        }
                        cell.Get().hex = hex;
                    }
                }
                ForegroundFactory.PlanForegroundForHex(cell.Get().hex, this);
            }
        }
    }

    private void I12_DesignForegroundMesh(object o)
    {
        foreach (KeyValuePair<Vector2i, ProtoRef<Chunk>> chunk in this.meshData.GetChunks())
        {
            if (chunk.Value.Get().GetCells() == null)
            {
                continue;
            }
            foreach (ProtoRef<MeshCell> cell in chunk.Value.Get().GetCells())
            {
                if (cell.Get() == null)
                {
                    Debug.LogError("missing cell in a cell array");
                    break;
                }
                cell.Get().hex.ProduceForegroundObjects(this.GetForegroundManterial(), chunk.Value.Get());
            }
        }
    }

    private void I13_InstantiateResourcesAndEffects(object o)
    {
        if (this.meshData == null || this.meshData.GetChunks() == null)
        {
            return;
        }
        foreach (KeyValuePair<Vector2i, ProtoRef<Chunk>> chunk in this.meshData.GetChunks())
        {
            if (chunk.Value.Get().GetCells() == null)
            {
                continue;
            }
            foreach (ProtoRef<MeshCell> cell in chunk.Value.Get().GetCells())
            {
                this.SpawnResource(cell.Get(), chunk.Value.Get());
                this.SpawnDecorSpawn(cell.Get(), chunk.Value.Get());
            }
        }
    }

    protected virtual void FinalizePlaneFormation(object o)
    {
    }

    protected virtual void SpawnResource(MeshCell meshCell, Chunk c)
    {
        if (!(meshCell.hex.Resource != null))
        {
            return;
        }
        Resource resource = meshCell.hex.Resource.Get();
        _ = meshCell.hex.Position == new Vector3i(-9, 21, -12);
        if (resource == null)
        {
            return;
        }
        GameObject gameObject = AssetManager.Get<GameObject>(resource.GetModel3dName());
        if (gameObject == null)
        {
            Debug.Log("Spawn resource :" + resource.dbName + " model " + resource.descriptionInfo.graphic + " at " + meshCell.hex.Position.ToString() + " graphic " + gameObject?.ToString() + " failed");
            return;
        }
        Vector3 position = HexCoordinates.HexToWorld3D(meshCell.hex.Position);
        GameObject gameObject2 = GameObjectUtils.Instantiate(gameObject, c.go.transform);
        gameObject2.transform.localRotation = Quaternion.Euler(Vector3.up * this.random.GetFloat(0f, 360f));
        gameObject2.transform.position = position;
        meshCell.hex.resourceInstance = gameObject2;
        bool allowUnderwater = this.IsLand(meshCell.hex.Position);
        List<GameObject> list = null;
        foreach (Transform item in gameObject2.transform)
        {
            Vector3 position2 = item.position;
            position2.y = this.GetHeightAt(position2, allowUnderwater);
            GroundOffset component = item.gameObject.GetComponent<GroundOffset>();
            if (component != null && component.killUnderWaterAsset)
            {
                if (position2.y < 0f)
                {
                    if (list == null)
                    {
                        list = new List<GameObject>();
                    }
                    list.Add(item.gameObject);
                    continue;
                }
                position2.y += component.heightOffset;
            }
            item.position = position2;
        }
        if (list == null)
        {
            return;
        }
        foreach (GameObject item2 in list)
        {
            global::UnityEngine.Object.Destroy(item2);
        }
    }

    protected virtual void SpawnDecorSpawn(MeshCell meshCell, Chunk c)
    {
        meshCell.hex.RemoveDecors();
        if (meshCell.hex.GetTerrain().decorSpawnChance == null)
        {
            return;
        }
        Decor[] decorSpawnChance = meshCell.hex.GetTerrain().decorSpawnChance;
        foreach (Decor decor in decorSpawnChance)
        {
            if (!(this.random.GetFloat(0f, 1f) < decor.chance))
            {
                continue;
            }
            GameObject gameObject = AssetManager.Get<GameObject>(decor.name);
            if (gameObject == null)
            {
                Debug.Log("Spawn decor :" + decor.name + " at " + meshCell.hex.Position.ToString() + " failed ");
                continue;
            }
            Vector3 position = HexCoordinates.HexToWorld3D(meshCell.hex.Position);
            GameObject gameObject2 = GameObjectUtils.Instantiate(gameObject, c.go.transform);
            Quaternion localRotation = Quaternion.Euler(Vector3.up * this.random.GetFloat(0f, 360f));
            gameObject2.transform.localRotation = localRotation;
            gameObject2.transform.position = position;
            meshCell.hex.AddDecor(gameObject2);
            List<GameObject> list = null;
            foreach (Transform item in gameObject2.transform)
            {
                if (decor.billboardingRotation)
                {
                    Quaternion rotation = item.rotation;
                    Vector3 eulerAngles = item.rotation.eulerAngles;
                    eulerAngles.y += 0f - gameObject2.transform.rotation.eulerAngles.y;
                    rotation.eulerAngles = eulerAngles;
                    item.rotation = rotation;
                }
                Vector3 position2 = item.position;
                position2.y = this.GetHeightAt(position2, allowUnderwater: true);
                GroundOffset component = item.gameObject.GetComponent<GroundOffset>();
                if (component != null)
                {
                    if (position2.y < 0f)
                    {
                        if (list == null)
                        {
                            list = new List<GameObject>();
                        }
                        list.Add(item.gameObject);
                        continue;
                    }
                    position2.y += component.heightOffset;
                }
                item.position = position2;
            }
            if (list == null)
            {
                continue;
            }
            foreach (GameObject item2 in list)
            {
                global::UnityEngine.Object.Destroy(item2);
            }
        }
    }

    public void AddDecor(string decorName, Vector3i position)
    {
        GameObject gameObject = AssetManager.Get<GameObject>(decorName);
        if (gameObject == null)
        {
            Debug.LogError("Cannot find decor: " + decorName);
            return;
        }
        Hex hexAt = this.GetHexAt(position);
        Vector3 position2 = HexCoordinates.HexToWorld3D(position);
        Vector2i p = Chunk.CellToChunk(position);
        Chunk chunk = this.meshData.GetChunk(p);
        GameObject gameObject2 = GameObjectUtils.Instantiate(gameObject, chunk.go.transform);
        Quaternion localRotation = Quaternion.Euler(Vector3.up * this.random.GetFloat(0f, 360f));
        gameObject2.transform.localRotation = localRotation;
        gameObject2.transform.position = position2;
        hexAt.additionalDecorInstance = gameObject2;
        if (World.GetActivePlane() != this)
        {
            gameObject2.gameObject.SetActive(value: false);
        }
        foreach (Transform item in gameObject2.transform)
        {
            Quaternion rotation = item.rotation;
            Vector3 eulerAngles = item.rotation.eulerAngles;
            eulerAngles.y += 0f - gameObject2.transform.rotation.eulerAngles.y;
            rotation.eulerAngles = eulerAngles;
            item.rotation = rotation;
            Vector3 position3 = item.position;
            position3.y = this.GetHeightAt(position3);
            item.position = position3;
        }
    }

    public void UpdateTerrainAt(Vector3i pos, Terrain terrain)
    {
        if (this.hexes != null)
        {
            if (this.hexesWithMeshToUpdate == null)
            {
                this.hexesWithMeshToUpdate = new HashSet<Vector3i>();
            }
            this.hexesWithMeshToUpdate.Add(pos);
            if (this.hexes.ContainsKey(pos))
            {
                this.hexes[pos].SetTerrain(terrain, this);
                return;
            }
            Vector3i vector3i = pos;
            Debug.LogError("InitialUpdateHMeshAt for hex at " + vector3i.ToString() + " was not possible");
        }
    }

    public void RebuildUpdatedTerrains(HashSet<Vector3i> locations)
    {
        if (locations != null)
        {
            if (this.hexesWithMeshToUpdate == null)
            {
                this.hexesWithMeshToUpdate = new HashSet<Vector3i>();
            }
            foreach (Vector3i location in locations)
            {
                this.hexesWithMeshToUpdate.Add(location);
            }
        }
        if (this.hexesWithMeshToUpdate == null)
        {
            return;
        }
        PlaneMeshData meshData = this.GetPlaneData();
        Func<MeshCell, int> func = delegate(MeshCell m)
        {
            if (m == null || m.vertices == null)
            {
                return 1;
            }
            foreach (ProtoRef<Vertex> vertex in m.vertices)
            {
                vertex.Get().SetVertexHeight(meshData, update: true);
            }
            return 0;
        };
        HashSet<Chunk> hashSet = new HashSet<Chunk>();
        foreach (Vector3i item in this.hexesWithMeshToUpdate)
        {
            Vector3i[] neighbours = HexNeighbors.neighbours;
            foreach (Vector3i vector3i in neighbours)
            {
                func(meshData.GetMeshCell(vector3i + item));
                hashSet.Add(meshData.GetChunk(vector3i + item));
            }
            func(meshData.GetMeshCell(item));
            hashSet.Add(meshData.GetChunk(item));
        }
        foreach (Chunk item2 in hashSet)
        {
            item2.RebuildMeshObject();
        }
        this.terrainDataValid = false;
        foreach (Vector3i item3 in this.hexesWithMeshToUpdate)
        {
            Hex hexAt = this.GetHexAt(item3);
            if (hexAt != null)
            {
                ForegroundFactory.PlanForegroundForHex(hexAt, this);
                ForegroundFactory.ProduceForegroundForHex(hexAt, this);
                MeshCell meshCell = meshData.GetMeshCell(hexAt.Position);
                Chunk chunk = meshData.GetChunk(hexAt.Position);
                if (meshCell != null && chunk != null)
                {
                    this.SpawnDecorSpawn(meshCell, chunk);
                }
            }
        }
        this.hexesWithMeshToUpdate = null;
        MinimapManager.Get().TerrainDirty(this.arcanusType);
    }

    public void InvalidateTerrain()
    {
        this.terrainDataValid = false;
    }

    public bool AnyLocationInRange(Vector3i centre, int range)
    {
        if (GameManager.Get().locations == null || !GameManager.Get().locations.ContainsKey(this))
        {
            return false;
        }
        foreach (global::MOM.Location item in GameManager.Get().locations[this])
        {
            if (HexCoordinates.HexDistance(item.Position, centre) <= range)
            {
                return true;
            }
        }
        return false;
    }

    private Material GetTerrainMaterial(bool canReturnNull = false)
    {
        if (this.material == null)
        {
            if (this.planeSource.Get() == (global::DBDef.Plane)PLANE.MYRROR)
            {
                this.material = new Material(AssetManager.Get().myrrorTerrainMaterial);
            }
            else
            {
                this.material = new Material(AssetManager.Get().arcanusTerrainMaterial);
            }
            this.material.SetTexture("_Diffuse", TerrainTextures.Get().diffuse);
            this.material.SetTexture("_Normal", TerrainTextures.Get().normal);
            this.material.SetTexture("_Specular", TerrainTextures.Get().specular);
            this.material.SetTexture("_Mixer", TerrainTextures.Get().mixer);
            this.material.SetVector("_ArraySize", new Vector4(TerrainTextures.Get().diffuseNames.Count, TerrainTextures.Get().normalNames.Count, TerrainTextures.Get().specularNames.Count, TerrainTextures.Get().mixerNames.Count));
            if (this.terrainData == null)
            {
                this.terrainData = new Texture2D(this.area.AreaWidth, this.area.AreaHeight, TextureFormat.RGBA32, mipChain: false, linear: true);
                this.terrainData.filterMode = FilterMode.Point;
            }
            this.material.SetTexture("_TerrainData", this.terrainData);
            this.material.SetTexture("_SettlerMap", null);
            Terrain terrain = DataBase.GetType<Terrain>().Find((Terrain o) => o.terrainType == ETerrainType.Sea && o.plane == this.planeSource.Get());
            if (terrain != null)
            {
                int num = TerrainTextures.Get().IndexOfDiffuse(terrain.terrainGraphic.diffuse);
                int num2 = TerrainTextures.Get().IndexOfDiffuse(terrain.terrainGraphic.normal);
                int num3 = TerrainTextures.Get().IndexOfDiffuse(terrain.terrainGraphic.specular);
                this.material.SetVector("_Beach", new Vector4((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f, 0f));
            }
            this.material.SetTexture("_MarkersData", this.markers.dataTexture);
            this.material.SetVector("_MarkersSettings", this.markers.merkerResolutionTexture);
            MemoryManager.Register(this.material);
            MHZombieMemoryDetector.Track(this.material);
        }
        return this.material;
    }

    public void SetSettlerDataTo(bool active)
    {
        this.material.SetTexture("_SettlerMap", active ? this.settlerDataTexture : null);
    }

    public Material GetForegroundManterial()
    {
        if (AssetManager.Get().foregroundMaterial == null)
        {
            return null;
        }
        if (this.fgMaterial == null)
        {
            this.fgMaterial = new Material(AssetManager.Get().foregroundMaterial);
            MemoryManager.Register(this.fgMaterial);
            MHZombieMemoryDetector.Track(this.fgMaterial);
        }
        return this.fgMaterial;
    }

    public Dictionary<Vector3i, Hex> GetHexes()
    {
        return this.hexes;
    }

    public Hex GetHexAt(Vector3i pos)
    {
        if (this.hexes.ContainsKey(pos))
        {
            return this.hexes[pos];
        }
        return null;
    }

    public Hex GetHexAtWrapped(Vector3i pos)
    {
        return this.GetHexAt(this.GetPositionWrapping(pos));
    }

    public bool IsLand(Vector3i pos)
    {
        if (this.hexes != null)
        {
            return this.GetLandHexes().Contains(pos);
        }
        return false;
    }

    public HashSet<Vector3i> GetLandHexes()
    {
        if (this.landHexes == null)
        {
            if (this.hexes == null)
            {
                return null;
            }
            this.landHexes = new HashSet<Vector3i>();
            this.waterHexes = new HashSet<Vector3i>();
            foreach (KeyValuePair<Vector3i, Hex> hex in this.hexes)
            {
                if (hex.Value.HaveFlag(ETerrainType.Sea))
                {
                    this.waterHexes.Add(hex.Key);
                }
                else
                {
                    this.landHexes.Add(hex.Key);
                }
            }
        }
        return this.landHexes;
    }

    public PlaneMeshData GetPlaneData()
    {
        return this.meshData;
    }

    public RoadManager GetRoadManagers()
    {
        if (GameManager.Get() == null || this.battlePlane)
        {
            return null;
        }
        GameManager gameManager;
        if (this.arcanusType)
        {
            gameManager = GameManager.Get();
            if (gameManager.arcanusRoads == null)
            {
                gameManager.arcanusRoads = new RoadManager(this);
            }
            return GameManager.Get().arcanusRoads;
        }
        gameManager = GameManager.Get();
        if (gameManager.myrrorRoads == null)
        {
            gameManager.myrrorRoads = new RoadManager(this);
        }
        return GameManager.Get().myrrorRoads;
    }

    public List<Vector3i> GetIslandFor(Vector3i vec)
    {
        foreach (List<Vector3i> island in this.GetIslands())
        {
            if (island.Contains(vec))
            {
                return island;
            }
        }
        return null;
    }

    public HashSet<Vector3i> GetWaterBodyFor(Vector3i vec)
    {
        if (this.water == null)
        {
            return null;
        }
        foreach (HashSet<Vector3i> item in this.water)
        {
            if (item.Contains(vec))
            {
                return item;
            }
        }
        return null;
    }

    public List<List<Vector3i>> GetIslands()
    {
        if (this.islands == null)
        {
            List<Vector3i> list = new List<Vector3i>(this.GetLandHexes());
            this.islands = new List<List<Vector3i>>();
            foreach (Vector3i item2 in list)
            {
                MeshCell meshCell = this.meshData.GetMeshCell(item2);
                this.GetHexAt(item2).RecreateViaRiver(meshCell.viaRiverFlags);
            }
            while (list.Count > 0)
            {
                RequestDataV2 requestDataV = RequestDataV2.CreateRequest(this, list[0], FInt.MAX, null);
                PathfinderV2.FindArea(requestDataV);
                List<Vector3i> area = requestDataV.GetArea();
                list = list.FindAll((Vector3i o) => !area.Contains(o));
                this.islands.Add(area);
            }
            list = new List<Vector3i>(this.waterHexes);
            this.water = new List<HashSet<Vector3i>>();
            this.waterBodies = new Dictionary<Vector3i, int>();
            List<Vector3i> list2 = new List<Vector3i>();
            while (list.Count > 0)
            {
                list2.Clear();
                list2.Add(list[0]);
                list.RemoveAt(0);
                for (int i = 0; i < list2.Count; i++)
                {
                    Vector3i vector3i = list2[i];
                    for (int j = 0; j < HexNeighbors.neighbours.Length; j++)
                    {
                        Vector3i item = vector3i + HexNeighbors.neighbours[j];
                        if (list.Contains(item))
                        {
                            list2.Add(item);
                            list.Remove(item);
                        }
                    }
                }
                this.water.Add(new HashSet<Vector3i>(list2));
                foreach (Vector3i item3 in list2)
                {
                    this.waterBodies[item3] = list2.Count;
                }
            }
        }
        return this.islands;
    }

    private void SetActive(bool state)
    {
        foreach (Transform item in this.goTerrain.transform)
        {
            foreach (Transform item2 in item)
            {
                Renderer component = item2.gameObject.GetComponent<Renderer>();
                if (item2.gameObject.GetComponent<ParticleSystem>() != null || component == null)
                {
                    item2.gameObject.SetActive(state);
                }
                else
                {
                    component.enabled = state;
                }
            }
        }
        if (state)
        {
            this.UpdateMaterialsWithTextures();
        }
    }

    public void UpdateVisibility(bool active)
    {
        if (!(this.goTerrain == null))
        {
            this.SetActive(active);
        }
    }

    public void UpdatePlane(bool focused)
    {
        if (this.goTerrain == null)
        {
            return;
        }
        if (this.terrainData == null)
        {
            Debug.LogError("This texture should already be created and assigned to the shader!");
        }
        if (!this.terrainDataValid)
        {
            foreach (Vector3i item in this.area.GetAreaHex())
            {
                if (this.hexes.ContainsKey(item))
                {
                    Hex hex = this.hexes[item];
                    int x = (item.x + this.area.AreaWidth) % this.area.AreaWidth;
                    int y = (item.y + this.area.AreaHeight) % this.area.AreaHeight;
                    int num = TerrainTextures.Get().IndexOfDiffuse(hex.GetTerrain().terrainGraphic.diffuse);
                    int num2 = TerrainTextures.Get().IndexOfNormal(hex.GetTerrain().terrainGraphic.normal);
                    int num3 = TerrainTextures.Get().IndexOfSpecular(hex.GetTerrain().terrainGraphic.specular);
                    if (num < 0)
                    {
                        num = 254;
                    }
                    if (!hex.ActiveHex)
                    {
                        num = 255;
                    }
                    if (num2 < 0)
                    {
                        num2 = 255;
                    }
                    if (num3 < 0)
                    {
                        num3 = 255;
                    }
                    Color color = new Color((float)num / 255f, (float)num2 / 255f, (float)num3 / 255f, 1f - hex.GetRotation());
                    this.terrainData.SetPixel(x, y, color);
                }
            }
            this.terrainDataValid = true;
            this.terrainData.Apply();
        }
        this.UpdateChunksPositionForWrap();
        if (focused)
        {
            this.markers.Update(this.markersDirty);
        }
    }

    public void ShowBuildRoad(bool b)
    {
        this.material.SetVector("_MarkersAddons", new Vector4(b ? 1 : 0, 0f, 0f, 0f));
        this.fgMaterial.SetVector("_MarkersAddons", new Vector4(b ? 1 : 0, 0f, 0f, 0f));
        World.GetWater(this.arcanusType).GetComponent<MeshRenderer>().sharedMaterial.SetVector("_MarkersAddons", new Vector4(b ? 1 : 0, 0f, 0f, 0f));
    }

    private void UpdateMaterialsWithTextures()
    {
        Material foregroundManterial = this.GetForegroundManterial();
        this.material.SetTexture("_MarkersData", this.markers.dataTexture);
        foregroundManterial.SetVector("_MarkersSettings", this.markers.merkerResolutionTexture);
        foregroundManterial.SetTexture("_MarkersData", this.markers.dataTexture);
        if (this.planeSource.Get() == (global::DBDef.Plane)PLANE.ARCANUS)
        {
            MeshRenderer component = World.GetArcanusWater().GetComponent<MeshRenderer>();
            component.sharedMaterial.SetVector("_MarkersSettings", this.markers.merkerResolutionTexture);
            component.sharedMaterial.SetTexture("_MarkersData", this.markers.dataTexture);
        }
        else
        {
            MeshRenderer component2 = World.GetMyrrorWater().GetComponent<MeshRenderer>();
            component2.sharedMaterial.SetVector("_MarkersSettings", this.markers.merkerResolutionTexture);
            component2.sharedMaterial.SetTexture("_MarkersData", this.markers.dataTexture);
        }
    }

    public virtual void UpdateChunksPositionForWrap()
    {
        int areaWidth = this.area.AreaWidth;
        Vector3 cameraPosition = CameraController.GetCameraPosition();
        if ((float)areaWidth * 1.5f * 0.5f < cameraPosition.x)
        {
            cameraPosition.x = (float)(-areaWidth) * 1.5f * 0.5f;
            CameraController.MoveInstantlyCameraPosition(cameraPosition);
        }
        else if ((float)(-areaWidth) * 1.5f * 0.5f > cameraPosition.x)
        {
            cameraPosition.x = (float)areaWidth * 1.5f * 0.5f;
            CameraController.MoveInstantlyCameraPosition(cameraPosition);
        }
        if (this.meshData != null)
        {
            this.meshData.UpdateChunksPositionForWrap(areaWidth);
        }
    }

    public void UpdateHeightsAfterTerrainChange(Vector3i position)
    {
        GameManager gameManager = GameManager.Get();
        global::MOM.Location locationAt = gameManager.GetLocationAt(position, this);
        if (locationAt != null && locationAt.IsModelVisible())
        {
            locationAt.SetHexPosition(offset: HexCoordinates.HexToWorld3D(locationAt.Position), position: locationAt.Position);
        }
        global::MOM.Group groupAt = gameManager.GetGroupAt(position, this);
        if (groupAt != null)
        {
            groupAt.DestroyMapFormation();
            groupAt.UpdateMapFormation();
        }
        Hex hexAt = this.GetHexAt(position);
        if (!(hexAt.additionalDecorInstance != null))
        {
            return;
        }
        GameObject additionalDecorInstance = hexAt.additionalDecorInstance;
        List<GameObject> list = null;
        foreach (Transform item in additionalDecorInstance.transform)
        {
            Quaternion rotation = item.rotation;
            Vector3 eulerAngles = item.rotation.eulerAngles;
            eulerAngles.y += 0f - additionalDecorInstance.transform.rotation.eulerAngles.y;
            rotation.eulerAngles = eulerAngles;
            item.rotation = rotation;
            Vector3 position2 = item.position;
            position2.y = this.GetHeightAt(position2, allowUnderwater: true);
            GroundOffset component = item.gameObject.GetComponent<GroundOffset>();
            if (component != null)
            {
                if (position2.y < 0f)
                {
                    if (list == null)
                    {
                        list = new List<GameObject>();
                    }
                    list.Add(item.gameObject);
                    continue;
                }
                position2.y += component.heightOffset;
            }
            item.position = position2;
        }
        if (list == null)
        {
            return;
        }
        foreach (GameObject item2 in list)
        {
            global::UnityEngine.Object.Destroy(item2);
        }
    }

    public void ShowSettlementValue(bool value)
    {
        if (!value)
        {
            World.GetActivePlane().material.SetTexture("_TerrainData", this.terrainData);
        }
    }

    protected static float GetTerrainHeight(Vector3i hex, PerlinMap perlinMap, V3iRect rect, float yAspectRatio, float smoothenBorders)
    {
        Vector2 uV = rect.GetUV(hex);
        uV.y *= yAspectRatio;
        float num = perlinMap.ProduceValueAtLayer(PerlinMap.Layer.Height1, uV);
        float num2 = perlinMap.ProduceValueAtLayer(PerlinMap.Layer.Height2, uV);
        float num3 = perlinMap.ProduceValueAtLayer(PerlinMap.Layer.Height3, uV);
        int num4 = rect.DistanceToBorder(hex);
        if (smoothenBorders > 2f)
        {
            return Mathf.Min(1f, (float)(num4 + 1) / smoothenBorders) * Mathf.Clamp01((num + num3) * num2);
        }
        return Mathf.Clamp01((num + num3) * num2);
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
        float num4 = num2 * num3;
        if ((num3 > 0.85f || num4 > 0.3f) && num > 0.25f)
        {
            hex.SetFlag(ETerrainType.Forest);
        }
        else if (hex.temperature + num * 0.3f < 0.25f)
        {
            hex.SetFlag(ETerrainType.Tundra);
        }
        else if (hex.temperature - num * 0.2f > 0.9f)
        {
            hex.SetFlag(ETerrainType.Desert);
        }
        else if (num > 0.75f)
        {
            hex.SetFlag(ETerrainType.Swamp);
        }
        if (!hex.HaveAnyFlag())
        {
            hex.SetFlag(ETerrainType.GrassLand);
        }
    }

    public void DEBUG_DrawToTexture()
    {
        GameObject gameObject = GameObject.Find("IMG");
        if (!(gameObject != null))
        {
            return;
        }
        Texture2D texture2D = new Texture2D(this.size.x, this.size.y);
        for (int i = 0; i < this.size.y; i++)
        {
            bool flag = false;
            for (int j = 0; j < this.size.x; j++)
            {
                Vector3i localizedPoint = this.area.GetLocalizedPoint((float)j / (float)this.size.x, (float)i / (float)this.size.y);
                if (this.hexes.ContainsKey(localizedPoint))
                {
                    Hex hex = this.hexes[localizedPoint];
                    if (!flag)
                    {
                        flag = true;
                        Debug.Log(i + " Temperature " + hex.temperature);
                    }
                    if (hex.HaveFlag(ETerrainType.Sea))
                    {
                        float num = this.height[localizedPoint] * 2f;
                        texture2D.SetPixel(j, i, new Color(0.2f, 0f, 0.3f + 0.5f * (num * num)));
                    }
                    else if (hex.HaveFlag(ETerrainType.Mountain))
                    {
                        texture2D.SetPixel(j, i, new Color(0.4f, 0.4f, 0.4f));
                    }
                    else if (hex.HaveFlag(ETerrainType.Hill))
                    {
                        texture2D.SetPixel(j, i, new Color(0.4f, 0.6f, 0.4f));
                    }
                    else if (hex.HaveFlag(ETerrainType.Tundra))
                    {
                        texture2D.SetPixel(j, i, new Color(0.4f, 0.7f, 0.5f));
                    }
                    else if (hex.HaveFlag(ETerrainType.Swamp))
                    {
                        texture2D.SetPixel(j, i, new Color(0.2f, 0.4f, 0.1f));
                    }
                    else if (hex.HaveFlag(ETerrainType.Desert))
                    {
                        texture2D.SetPixel(j, i, new Color(0.6f, 0.6f, 0f));
                    }
                    else if (hex.HaveFlag(ETerrainType.Forest))
                    {
                        texture2D.SetPixel(j, i, new Color(0.3f, 0.5f, 0.3f));
                    }
                    else
                    {
                        texture2D.SetPixel(j, i, new Color(0.2f, 0.6f, 0.2f));
                    }
                }
                else
                {
                    texture2D.SetPixel(j, i, Color.magenta);
                }
            }
        }
        texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
        gameObject.GetComponent<RawImage>().texture = texture2D;
    }

    public List<Vector3i> GetPositionWrapping(List<Vector3i> pos)
    {
        if (pos == null)
        {
            return null;
        }
        for (int i = 0; i < pos.Count; i++)
        {
            pos[i] = this.GetPositionWrapping(pos[i]);
        }
        return pos;
    }

    public Vector3i GetPositionWrapping(Vector3i pos)
    {
        return this.area.KeepHorizontalInside(pos);
    }

    public int GetDistanceWrapping(Vector3i a, Vector3i b)
    {
        return this.area.HexDistance(a, b);
    }

    public Chunk GetChunkFor(Vector3i pos)
    {
        Vector2i p = Chunk.CellToChunk(pos);
        return this.meshData.GetChunk(p);
    }

    public float GetHeightAt(Vector3 pos, bool allowUnderwater = false)
    {
        Ray ray = new Ray(pos + Vector3.up * 5f, Vector3.down);
        Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(pos);
        Chunk chunkFor = this.GetChunkFor(hexCoordAt);
        if (chunkFor == null)
        {
            return -1f;
        }
        if (chunkFor.GetMeshCollider().Raycast(ray, out var hitInfo, 8f))
        {
            if (allowUnderwater || hitInfo.point.y >= 0f)
            {
                return hitInfo.point.y;
            }
            return 0f;
        }
        return 0f;
    }

    public static global::WorldCode.Plane Get()
    {
        return World.GetActivePlane();
    }

    public static TerrainMarkers GetMarkers()
    {
        return World.GetActivePlane().markers;
    }

    public TerrainMarkers GetMarkers_()
    {
        return this.markers;
    }

    public int GetFoodInArea(Vector3i pos)
    {
        pos = this.GetPositionWrapping(pos);
        List<Vector3i> range = HexNeighbors.GetRange(pos, 2);
        FInt zERO = FInt.ZERO;
        foreach (Vector3i item in range)
        {
            Hex hexAt = this.GetHexAt(item);
            if (hexAt != null)
            {
                zERO += hexAt.GetFood();
            }
        }
        return zERO.ToInt();
    }

    public SearcherDataV2 GetSearcherData()
    {
        if (this.searcherData == null)
        {
            this.searcherData = new SearcherDataV2();
            this.searcherData.InitializeData(this);
            if (!this.battlePlane)
            {
                this.searcherData.InitializeLocationsAndUnits(this);
            }
            else
            {
                this.searcherData.InitializeLocationsAndUnits(Battle.GetBattle());
            }
        }
        return this.searcherData;
    }

    public void UpdateUnitPosition(Vector3i from, Vector3i to, IPlanePosition ipp)
    {
        this.GetSearcherData().UpdateUnitPosition(from, to, ipp);
        this.markersDirty = true;
    }

    public void ClearUnitPosition(Vector3i pos, bool isHosted = true)
    {
        SearcherDataV2 searcherDataV = this.GetSearcherData();
        List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this);
        if (!isHosted && locationsOfThePlane != null && locationsOfThePlane.Find((global::MOM.Location o) => o.GetPosition() == pos) != null)
        {
            global::MOM.Location location = locationsOfThePlane.Find((global::MOM.Location o) => o.GetPosition() == pos);
            if (location != null)
            {
                searcherDataV.UpdateGroupToPosition(pos, location.GetGroup());
            }
        }
        else
        {
            searcherDataV.ClearUnitPosition(pos);
        }
        this.markersDirty = true;
    }

    public void ClearSearcherData()
    {
        this.searcherData = null;
    }
}
