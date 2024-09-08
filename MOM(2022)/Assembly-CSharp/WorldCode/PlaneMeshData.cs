// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WorldCode.PlaneMeshData
using System.Collections.Generic;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class PlaneMeshData : ProtoLibraryItem
{
    [ProtoMember(1)]
    private NetDictionary<Vector2i, ProtoRef<Chunk>> chunks;

    [ProtoMember(2)]
    private NetDictionary<Vector3i, ProtoRef<MeshCell>> localCells;

    [ProtoMember(3)]
    private List<ProtoRef<Vertex>> vertices;

    [ProtoIgnore]
    private List<Vector3i> riverStartOptions;

    [ProtoIgnore]
    private Color[] cellDataTexture;

    [ProtoIgnore]
    private Color[] dirtSplatData;

    [ProtoIgnore]
    private List<List<Vertex>> rivers;

    [ProtoIgnore]
    private List<List<MHVector3>> riversEased;

    [ProtoIgnore]
    private List<List<MeshTriangle2D>> riverTrangles;

    [ProtoIgnore]
    public int RiverCount
    {
        get
        {
            if (this.rivers == null)
            {
                return 0;
            }
            return this.rivers.Count;
        }
    }

    [ProtoIgnore]
    public Color[] CellDataTexture
    {
        get
        {
            return this.cellDataTexture;
        }
        set
        {
            this.cellDataTexture = value;
        }
    }

    [ProtoIgnore]
    public Color[] DirtSplatData
    {
        get
        {
            return this.dirtSplatData;
        }
        set
        {
            this.dirtSplatData = value;
        }
    }

    [ProtoIgnore]
    public List<List<MHVector3>> RiversEased
    {
        get
        {
            return this.riversEased;
        }
        set
        {
            this.riversEased = value;
        }
    }

    [ProtoIgnore]
    public List<List<MeshTriangle2D>> RiverTrangles
    {
        get
        {
            return this.riverTrangles;
        }
        set
        {
            this.riverTrangles = value;
        }
    }

    public PlaneMeshData()
    {
        MHZombieMemoryDetector.Track(this);
    }

    public NetDictionary<Vector2i, ProtoRef<Chunk>> GetChunks()
    {
        return this.chunks;
    }

    public Chunk GetChunk(Vector2i p)
    {
        if (this.chunks == null)
        {
            this.chunks = new NetDictionary<Vector2i, ProtoRef<Chunk>>();
        }
        if (this.chunks.ContainsKey(p))
        {
            return this.chunks[p].Get();
        }
        return null;
    }

    public Chunk GetChunk(Vector3i planePos)
    {
        Vector2i p = Chunk.CellToChunk(planePos);
        return this.GetChunk(p);
    }

    public void UpdateChunksPositionForWrap(int planeWidth)
    {
        if (this.chunks == null)
        {
            return;
        }
        Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(CameraController.GetCameraPosition());
        foreach (KeyValuePair<Vector2i, ProtoRef<Chunk>> chunk in this.chunks)
        {
            int num = chunk.Key.x * 5 + 2;
            int num2 = num + planeWidth;
            int num3 = num - planeWidth;
            int num4 = Mathf.Abs(num - hexCoordAt.x);
            int num5 = Mathf.Abs(num2 - hexCoordAt.x);
            int num6 = Mathf.Abs(num3 - hexCoordAt.x);
            if (num4 < num5 && num4 < num6)
            {
                chunk.Value.Get().SetChunkPosition(Vector3.zero);
            }
            else if (num5 < num6)
            {
                chunk.Value.Get().SetChunkPosition(Vector3.right * 1.5f * planeWidth);
            }
            else
            {
                chunk.Value.Get().SetChunkPosition(-Vector3.right * 1.5f * planeWidth);
            }
        }
    }

    public MeshCell GetMeshCell(Vector3i pos)
    {
        if (this.localCells == null)
        {
            this.localCells = new NetDictionary<Vector3i, ProtoRef<MeshCell>>();
        }
        if (this.localCells.ContainsKey(pos))
        {
            return this.localCells[pos].Get();
        }
        return null;
    }

    public bool HasCell(Vector3i pos)
    {
        if (this.localCells == null || !this.localCells.ContainsKey(pos))
        {
            return false;
        }
        return true;
    }

    public MeshCell AddCell(Hex hex)
    {
        Vector2i vector2i = Chunk.CellToChunk(hex.Position);
        Chunk chunk = this.GetChunk(vector2i);
        if (chunk == null)
        {
            chunk = new Chunk();
            chunk.position = vector2i;
            this.chunks[vector2i] = chunk;
            chunk.parent = this;
        }
        if (this.localCells == null)
        {
            this.localCells = new NetDictionary<Vector3i, ProtoRef<MeshCell>>();
        }
        MeshCell meshCell = new MeshCell();
        meshCell.LinkWithHex(hex);
        meshCell.ProduceRotationMatrix();
        this.localCells[hex.Position] = meshCell;
        chunk.AddCell(meshCell);
        meshCell.parent = this;
        return meshCell;
    }

    public NetDictionary<Vector3i, ProtoRef<MeshCell>> GetLocalCells()
    {
        return this.localCells;
    }

    public Vertex GetVertex(int index)
    {
        if (this.vertices == null)
        {
            this.vertices = new List<ProtoRef<Vertex>>();
        }
        if (this.vertices.Count > index)
        {
            return this.vertices[index].Get();
        }
        Debug.LogError("Get call of not created Vertex: " + index);
        return null;
    }

    public Vertex AddVertex(Vector3 position)
    {
        if (this.vertices == null)
        {
            this.vertices = new List<ProtoRef<Vertex>>();
        }
        Vertex vertex = new Vertex(position);
        this.vertices.Add(vertex);
        return vertex;
    }

    public List<ProtoRef<Vertex>> GetVertices()
    {
        return this.vertices;
    }

    public List<Vector3i> GetRiverStartOptions()
    {
        return this.riverStartOptions;
    }

    public int GetRiverStartCount()
    {
        return this.riverStartOptions.Count;
    }

    public void AddRiverStartOption(Vector3i pos)
    {
        if (this.riverStartOptions == null)
        {
            this.riverStartOptions = new List<Vector3i>();
        }
        this.riverStartOptions.Add(pos);
    }

    public void AddRiver(List<Vertex> river)
    {
        if (this.rivers == null)
        {
            this.rivers = new List<List<Vertex>>();
        }
        if (river.Count < 2)
        {
            return;
        }
        this.rivers.Add(river);
        float num = 3f;
        for (int i = 0; i < this.riverStartOptions.Count; i++)
        {
            Vector3 vector = HexCoordinates.HexToWorld3D(this.riverStartOptions[i]);
            if ((river[0].position - vector).SqrMagnitude() < num * 2.5f)
            {
                this.riverStartOptions.RemoveAt(i);
                i--;
                break;
            }
            foreach (Vertex item in river)
            {
                if ((item.position - vector).SqrMagnitude() < num)
                {
                    this.riverStartOptions.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }
    }

    public Vector3i GetNewRiverStart(MHRandom r)
    {
        if (this.riverStartOptions != null && this.riverStartOptions.Count > 0)
        {
            int @int = r.GetInt(0, this.riverStartOptions.Count);
            return this.riverStartOptions[@int];
        }
        return Vector3i.invalid;
    }

    public List<Vertex> GetRiver(int index)
    {
        if (this.rivers != null && this.rivers.Count > index)
        {
            return this.rivers[index];
        }
        return null;
    }

    public GameObject GenerateInitialGO(string name, Material terrainMaterial)
    {
        GameObject gameObject = new GameObject("Plane " + name);
        foreach (KeyValuePair<Vector2i, ProtoRef<Chunk>> chunk in this.chunks)
        {
            if (chunk.Value.Get().GetCells() != null && chunk.Value.Get().GetCells().Count != 0)
            {
                chunk.Value.Get().ProduceMeshObject(terrainMaterial).transform.parent = gameObject.transform;
            }
        }
        return gameObject;
    }
}
