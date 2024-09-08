// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WorldCode.MeshChunkSection
using System.Collections.Generic;
using MOM;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Rendering;
using WorldCode;

[ProtoContract]
public class MeshChunkSection
{
    [ProtoMember(1)]
    public MeshChunkSectionSerializer serializedData;

    [ProtoMember(2)]
    public List<int> indices = new List<int>();

    [ProtoMember(3)]
    public List<ProtoRef<Vertex>> sourceVertices = new List<ProtoRef<Vertex>>();

    [ProtoIgnore]
    public List<Vector3> vertices = new List<Vector3>();

    [ProtoIgnore]
    public List<Vector3> normals = new List<Vector3>();

    [ProtoIgnore]
    public List<Color> colors = new List<Color>();

    [ProtoIgnore]
    public List<Vector2> uvs = new List<Vector2>();

    [ProtoIgnore]
    public Dictionary<Vertex, int> indexedVertices = new Dictionary<Vertex, int>();

    [ProtoIgnore]
    public Vector4[] tangent;

    [ProtoBeforeSerialization]
    public void BeforeSerialization()
    {
        this.serializedData = new MeshChunkSectionSerializer();
        this.serializedData.Add(this);
    }

    [ProtoAfterDeserialization]
    public void AfterDeserialization()
    {
        if (this.serializedData != null)
        {
            this.serializedData.ReadTo(this);
        }
    }

    public void DuringReset()
    {
        if (this.serializedData != null)
        {
            this.serializedData.ReadPartial(this);
        }
    }

    public bool HaveSpaceForTriangles(MeshCell cell)
    {
        return this.vertices.Count + cell.vertices.Count < 65000;
    }

    public void AddTriangle(VertexTriangle vt)
    {
        this.AddVertex(vt.vertices[0]);
        this.AddVertex(vt.vertices[1]);
        this.AddVertex(vt.vertices[2]);
    }

    private void AddVertex(Vertex v)
    {
        int num;
        if (this.indexedVertices.ContainsKey(v))
        {
            num = this.indexedVertices[v];
        }
        else
        {
            num = this.vertices.Count;
            this.sourceVertices.Add(v);
            this.vertices.Add(v.position);
            this.indexedVertices[v] = num;
            this.normals.Add(v.GetNormal());
            this.colors.Add(v.GetVertexDataAsColor());
            this.uvs.Add(v.riverUVNormal);
        }
        this.indices.Add(num);
    }

    public List<ProtoRef<Vertex>> GetSourceVerticesList()
    {
        return this.sourceVertices;
    }

    public GameObject GetGO(Material terrainMaterial)
    {
        Vector4[] array = new Vector4[this.normals.Count];
        for (int i = 0; i < this.normals.Count; i++)
        {
            Vector4 vector = Vector3.Cross(this.normals[i], Vector3.forward);
            vector.w = -1f;
            array[i] = vector;
        }
        GameObject gameObject = new GameObject("(MESH)");
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        mesh.MarkDynamic();
        meshFilter.mesh = mesh;
        mesh.vertices = this.vertices.ToArray();
        mesh.triangles = this.indices.ToArray();
        mesh.normals = this.normals.ToArray();
        mesh.tangents = array;
        mesh.colors = this.colors.ToArray();
        mesh.uv = this.uvs.ToArray();
        meshRenderer.material = terrainMaterial;
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        mesh.RecalculateBounds();
        gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.layer = 9;
        gameObject.AddComponent<ChunkMesh>();
        return gameObject;
    }

    public void UpdateMesh(GameObject go)
    {
        if (this.tangent == null)
        {
            this.tangent = new Vector4[this.normals.Count];
        }
        for (int i = 0; i < this.sourceVertices.Count; i++)
        {
            Vertex vertex = this.sourceVertices[i].Get();
            if (vertex.triangles != null && vertex.triangles.Count > 0)
            {
                this.vertices[i] = vertex.position;
                this.normals[i] = vertex.GetNormal();
            }
            else if (this.vertices[i].y != vertex.position.y)
            {
                this.vertices[i] = vertex.position;
            }
        }
        for (int j = 0; j < this.normals.Count; j++)
        {
            Vector4 vector = Vector3.Cross(this.normals[j], Vector3.forward);
            vector.w = -1f;
            this.tangent[j] = vector;
        }
        Mesh mesh = go.GetComponentInChildren<MeshFilter>().mesh;
        mesh.vertices = this.vertices.ToArray();
        mesh.normals = this.normals.ToArray();
        mesh.tangents = this.tangent;
        mesh.RecalculateBounds();
        mesh.UploadMeshData(markNoLongerReadable: false);
        MeshCollider componentInChildren = go.GetComponentInChildren<MeshCollider>();
        if (componentInChildren != null && componentInChildren.sharedMesh != null && componentInChildren.sharedMesh != mesh)
        {
            Object.Destroy(componentInChildren.sharedMesh);
        }
        componentInChildren.sharedMesh = mesh;
    }
}
