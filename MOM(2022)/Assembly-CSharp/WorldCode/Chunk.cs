// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WorldCode.Chunk
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class Chunk : ProtoLibraryItem
{
    public const int SIZE = 5;

    [ProtoMember(1)]
    public Vector2i position;

    [ProtoMember(2)]
    public MeshChunkSection mcs;

    [ProtoMember(3)]
    private List<ProtoRef<MeshCell>> localCells;

    [ProtoMember(4)]
    public ProtoRef<PlaneMeshData> parent;

    [ProtoIgnore]
    private List<ForegroundMeshPlan> foregroundBundles;

    [ProtoIgnore]
    private MeshCollider meshCollider;

    [ProtoIgnore]
    public GameObject go;

    public Chunk()
    {
        MHZombieMemoryDetector.Track(this);
    }

    public void AddCell(MeshCell c)
    {
        if (this.localCells == null)
        {
            this.localCells = new List<ProtoRef<MeshCell>>();
        }
        this.localCells.Add(c);
    }

    public List<ProtoRef<MeshCell>> GetCells()
    {
        return this.localCells;
    }

    public static Vector2i CellToChunk(Vector3i pos)
    {
        return new Vector2i(Mathf.FloorToInt((float)pos.x / 5f), Mathf.FloorToInt((float)pos.y / 5f));
    }

    public void TriangulateData()
    {
        foreach (ProtoRef<MeshCell> localCell in this.localCells)
        {
            MeshCell meshCell = (MeshCell)localCell;
            if (!meshCell.Coast && meshCell.Sea)
            {
                if (meshCell.neighbourCells == null)
                {
                    continue;
                }
                bool flag = true;
                ProtoRef<MeshCell>[] neighbourCells = meshCell.neighbourCells;
                foreach (ProtoRef<MeshCell> protoRef in neighbourCells)
                {
                    if (protoRef != null)
                    {
                        MeshCell meshCell2 = protoRef.Get();
                        if (meshCell2 != null && (meshCell2.Coast || !meshCell2.Sea || meshCell2.Hill || meshCell2.Mountain))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    continue;
                }
            }
            meshCell.SubdivideCell();
        }
    }

    public void AddForeground(Vector3 pos, Foliage foliage, MHRandom random)
    {
        if (this.foregroundBundles == null)
        {
            this.foregroundBundles = new List<ForegroundMeshPlan>();
        }
        while (true)
        {
            if (this.foregroundBundles.Count == 0)
            {
                this.foregroundBundles.Add(new ForegroundMeshPlan());
            }
            if (!this.foregroundBundles[this.foregroundBundles.Count - 1].Add(pos, foliage, random, allowRotation: false))
            {
                this.foregroundBundles.Add(new ForegroundMeshPlan());
                continue;
            }
            break;
        }
    }

    public GameObject ProduceMeshObject(Material terrainMaterial)
    {
        Vector2i vector2i = this.position;
        this.go = new GameObject("Chunk " + vector2i.ToString());
        if (this.mcs == null)
        {
            foreach (ProtoRef<MeshCell> localCell in this.localCells)
            {
                if (this.mcs == null)
                {
                    this.mcs = new MeshChunkSection();
                }
                foreach (VertexTriangle triangle in localCell.Get().triangles)
                {
                    this.mcs.AddTriangle(triangle);
                }
            }
        }
        if (this.mcs != null && this.mcs.indices != null)
        {
            GameObject gO = this.mcs.GetGO(terrainMaterial);
            gO.transform.parent = this.go.transform;
            this.meshCollider = gO.GetComponent<MeshCollider>();
        }
        return this.go;
    }

    public void RebuildMeshObject()
    {
        if (!(this.go == null))
        {
            this.mcs.UpdateMesh(this.go);
        }
    }

    public void SetChunkPosition(Vector3 pos)
    {
        this.go.transform.position = pos;
    }

    private void I3_ConstructVerticesData(object o)
    {
        _ = this.localCells;
    }

    public MeshCollider GetMeshCollider()
    {
        return this.meshCollider;
    }
}
