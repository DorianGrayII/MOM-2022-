// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// WorldCode.Vertex
using System;
using System.Collections.Generic;
using MHUtils;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class Vertex : ProtoLibraryItem
{
    public static MHRandom random;

    [ProtoMember(1)]
    public MHVector3 position;

    [ProtoIgnore]
    public Vector3 basePosition;

    [ProtoIgnore]
    public Array3<MeshCell> parentCells = new Array3<MeshCell>();

    [ProtoIgnore]
    public int riverIndex;

    [ProtoIgnore]
    public bool isRiver;

    [ProtoIgnore]
    public Array3<Vertex> neighbourCorners;

    [ProtoIgnore]
    public NetDictionary<Vertex, List<Vertex>> pointsTowardVertex = new NetDictionary<Vertex, List<Vertex>>();

    [ProtoIgnore]
    public List<VertexTriangle> triangles = new List<VertexTriangle>();

    [ProtoIgnore]
    public MHVector2 riverUVNormal;

    [ProtoIgnore]
    public MHVector4 riverUVVA;

    public Vertex()
    {
    }

    public Vertex(Vector3 pos)
    {
        this.position = pos;
        this.basePosition = new Vector3(pos.x, 0f, pos.z);
    }

    public void SetVertexHeight(PlaneMeshData meshData, bool update = false)
    {
        float scaleHeight = 2f;
        Vector3i[] hexagonalQuad = HexNeighbors.GetHexagonalQuad(this.position);
        MeshCell meshCell = meshData.GetMeshCell(hexagonalQuad[0]);
        MeshCell meshCell2 = meshData.GetMeshCell(hexagonalQuad[1]);
        MeshCell meshCell3 = meshData.GetMeshCell(hexagonalQuad[2]);
        MeshCell meshCell4 = meshData.GetMeshCell(hexagonalQuad[3]);
        Vector4 vector = ((meshCell != null) ? ((Vector4)meshCell.GetHeightAndMixerFor(this.position)) : new Vector4(0f, 0.5f, 0f, 0f));
        Vector4 vector2 = ((meshCell2 != null) ? ((Vector4)meshCell2.GetHeightAndMixerFor(this.position)) : new Vector4(0f, 0.5f, 0f, 0f));
        Vector4 vector3 = ((meshCell3 != null) ? ((Vector4)meshCell3.GetHeightAndMixerFor(this.position)) : new Vector4(0f, 0.5f, 0f, 0f));
        Vector4 vector4 = ((meshCell4 != null) ? ((Vector4)meshCell4.GetHeightAndMixerFor(this.position)) : new Vector4(0f, 0.5f, 0f, 0f));
        Func<float, float, float> func = delegate(float h, float dist)
        {
            h = (h - 0.5f) * scaleHeight;
            if (h <= 0f)
            {
                return h;
            }
            dist = Mathf.Clamp01(dist - 1f);
            h *= Mathf.Lerp(1f, 0f, dist * dist * 2f);
            return h;
        };
        float x = vector.x;
        float x2 = vector2.x;
        float x3 = vector3.x;
        float x4 = vector4.x;
        vector.x = func(vector.x, vector.z);
        vector2.x = func(vector2.x, vector2.z);
        vector3.x = func(vector3.x, vector3.z);
        vector4.x = func(vector4.x, vector4.z);
        vector.w = vector.x;
        vector2.w = vector2.x;
        vector3.w = vector3.x;
        vector4.w = vector4.x;
        Vector4[] array = new Vector4[4] { vector, vector2, vector3, vector4 };
        Array.Sort(array, (Vector4 A, Vector4 B) => -A.x.CompareTo(B.x));
        if (array[0].y < 0f)
        {
            array[0].y = 1f;
        }
        array[0].w = array[0].y;
        if (array[1].y < 0f)
        {
            array[1].y = 1f;
        }
        array[1].w = Mathf.Clamp01(array[1].y * (1f - array[0].w));
        if (array[2].y < 0f)
        {
            array[2].y = 1f;
        }
        array[2].w = Mathf.Clamp01(array[2].y * (1f - array[0].w - array[1].w));
        if (array[3].y < 0f)
        {
            array[3].y = 1f;
        }
        array[3].w = Mathf.Clamp01(array[3].y * (1f - array[0].w - array[1].w - array[2].w));
        float num = array[0].x * array[0].w + array[1].x * array[1].w + array[2].x * array[2].w + array[3].x * array[3].w;
        if (num >= -0.2f)
        {
            vector.x = x - 0.5f;
            vector2.x = x2 - 0.5f;
            vector3.x = x3 - 0.5f;
            vector4.x = x4 - 0.5f;
            float num2 = num;
            int num3 = 3;
            while (num3 > 0 && array[num3].x < 0f)
            {
                float num4 = Mathf.Clamp01((array[num3].z - 0.5f) * (0.8f + Mathf.Max(0f, num / 1.5f)));
                float num5 = Mathf.Lerp(1f, 0f, num4 * num4);
                float num6 = array[num3].y * array[num3].y * num5;
                num2 = Mathf.Min(Mathf.Lerp(num, array[num3].x, 0.7f * num6), num2);
                num3--;
            }
            num = num2;
        }
        if (num < -0.2f)
        {
            num = -0.2f;
        }
        if (this.position.y != num)
        {
            if (update)
            {
                ProtoLibrary.GetInstance(testSettings: false)?.RecordHeightChange(this, this.position.y);
            }
            this.position.y = num;
        }
    }

    public Vector3 GetNormal()
    {
        Vector3 zero = Vector3.zero;
        foreach (VertexTriangle triangle in this.triangles)
        {
            if (triangle != null)
            {
                zero += triangle.GetNormal();
            }
        }
        return zero / this.triangles.Count;
    }

    public Color GetVertexDataAsColor()
    {
        return this.riverUVVA.GetColor();
    }
}
