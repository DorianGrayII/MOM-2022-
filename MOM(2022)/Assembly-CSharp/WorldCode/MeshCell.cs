using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    [ProtoContract]
    public class MeshCell : ProtoLibraryItem
    {
        [ProtoMember(1)]
        public ProtoRef<MeshCell>[] neighbourCells;

        [ProtoMember(2)]
        public ProtoRef<Vertex>[] corners;

        [ProtoMember(3)]
        public ProtoRef<PlaneMeshData> parent;

        [ProtoMember(4)]
        public HashSet<ProtoRef<Vertex>> vertices;

        [ProtoMember(5)]
        public Vector3i position;

        [ProtoMember(6)]
        public int ownerTerrainFlags;

        [ProtoMember(7)]
        public int viaRiverFlags;

        [ProtoIgnore]
        public Vertex center;

        [ProtoIgnore]
        public Array6 subDivision;

        [ProtoIgnore]
        public List<VertexTriangle> triangles = new List<VertexTriangle>();

        [ProtoIgnore]
        public NetDictionary<int, List<Vertex>> regionalizedVertices = new NetDictionary<int, List<Vertex>>();

        [ProtoIgnore]
        public Hex hex;

        [ProtoIgnore]
        public Quaternion rotationMatrix;

        public bool Sea => this.hex.HaveFlag(ETerrainType.Sea);

        public bool Hill => this.hex.HaveFlag(ETerrainType.Hill);

        public bool Mountain => this.hex.HaveFlag(ETerrainType.Mountain);

        public bool Coast => this.hex.HaveFlag(ETerrainType.Coast);

        [ProtoBeforeSerialization]
        public void BeforeSerialization()
        {
            if (this.hex != null)
            {
                this.ownerTerrainFlags = this.hex.flagSettings;
            }
        }

        public void LinkWithNeighbor(MeshCell mc, int position)
        {
            if (this.neighbourCells == null)
            {
                this.neighbourCells = new ProtoRef<MeshCell>[6];
            }
            this.neighbourCells[position] = mc;
            if (mc.hex.HaveFlag(ETerrainType.Sea) != this.hex.HaveFlag(ETerrainType.Sea))
            {
                mc.hex.SetFlag(ETerrainType.Coast);
                this.hex.SetFlag(ETerrainType.Coast);
            }
        }

        public Vector3 FindCorner(Vector3i neighborA, Vector3i neighborB)
        {
            Vector3 vector = HexCoordinates.HexToWorld3D(neighborA);
            Vector3 vector2 = HexCoordinates.HexToWorld3D(neighborB);
            Vector3 vector3 = HexCoordinates.HexToWorld3D(this.hex.Position);
            Vector3 vector4 = vector + (vector2 - vector) * 0.5f;
            return vector3 + (vector4 - vector3) * 2f / 3f;
        }

        public void LinkWithHex(Hex h)
        {
            this.hex = h;
            this.position = h.Position;
        }

        public void ProduceRotationMatrix()
        {
            this.rotationMatrix = Quaternion.Euler(0f, this.hex.GetRotation() * 360f, 0f);
        }

        public void AddVertex(Vertex v)
        {
            if (this.vertices == null)
            {
                this.vertices = new HashSet<ProtoRef<Vertex>>();
            }
            this.vertices.Add(v);
            v.parentCells.Add(this);
        }

        public void AddCorner(Vertex v, int index)
        {
            this.AddVertex(v);
            if (this.corners == null)
            {
                this.corners = new ProtoRef<Vertex>[6];
            }
            this.corners[index] = v;
            if (this.corners[(index + 1) % 6] != null)
            {
                ProtoRef<Vertex> protoRef = this.corners[(index + 1) % 6];
                if (v.neighbourCorners == null)
                {
                    v.neighbourCorners = new Array3<Vertex>();
                }
                if (!v.neighbourCorners.Contains(protoRef.Get()))
                {
                    v.neighbourCorners.Add(protoRef.Get());
                }
                if (protoRef.Get().neighbourCorners == null)
                {
                    protoRef.Get().neighbourCorners = new Array3<Vertex>();
                }
                if (!protoRef.Get().neighbourCorners.Contains(v))
                {
                    protoRef.Get().neighbourCorners.Add(v);
                }
            }
        }

        public Vertex GetCorner(int index)
        {
            if (this.corners == null)
            {
                return null;
            }
            return this.corners[index]?.Get();
        }

        public void CalculateTriangulateDivision()
        {
            for (int i = 0; i < 6; i++)
            {
                this.subDivision[i] = Settings.GetMeshQuality();
            }
        }

        public void SubdivideCell()
        {
            this.vertices = new HashSet<ProtoRef<Vertex>>();
            for (int i = 0; i < 6; i++)
            {
                this.regionalizedVertices[i] = new List<Vertex>();
            }
            for (int j = 0; j < 6; j++)
            {
                int num = this.GetDivisionFor(j, 0) + 1;
                int num2 = this.GetDivisionFor(j, 1) + 1;
                int num3 = this.GetDivisionFor(j, 2) + 1;
                Vertex vertex = this.corners[(j + 5) % 6].Get();
                Vertex vertex2 = this.corners[(j + 6) % 6].Get();
                List<Vertex> list = null;
                List<Vertex> list2 = null;
                List<Vertex> list3 = null;
                if (this.center.pointsTowardVertex.ContainsKey(vertex))
                {
                    list = this.center.pointsTowardVertex[vertex];
                }
                else
                {
                    list = new List<Vertex>();
                    list.Add(this.center);
                    for (int k = 1; k < num; k++)
                    {
                        Vertex vertex3 = new Vertex(Vector3.Lerp(this.center.position, vertex.position, (float)k / (float)num));
                        list.Add(vertex3);
                        vertex3.parentCells = vertex.parentCells;
                    }
                    list.Add(vertex);
                    if (this.center.pointsTowardVertex == null)
                    {
                        Debug.LogError("[Impossible exception?] center Hex corner pointsTowardVertex NULL!");
                    }
                    this.center.pointsTowardVertex[vertex] = list;
                }
                if (this.center.pointsTowardVertex.ContainsKey(vertex2))
                {
                    list2 = this.center.pointsTowardVertex[vertex2];
                }
                else
                {
                    list2 = new List<Vertex>();
                    list2.Add(this.center);
                    for (int l = 1; l < num2; l++)
                    {
                        Vertex vertex4 = new Vertex(Vector3.Lerp(this.center.position, vertex2.position, (float)l / (float)num2));
                        list2.Add(vertex4);
                        vertex4.parentCells = vertex2.parentCells;
                    }
                    list2.Add(vertex2);
                    if (this.center.pointsTowardVertex == null)
                    {
                        Debug.LogError("[Impossible exception?] center Hex corner pointsTowardVertex NULL!");
                    }
                    this.center.pointsTowardVertex[vertex2] = list2;
                }
                if (vertex.pointsTowardVertex.ContainsKey(vertex2))
                {
                    list3 = vertex.pointsTowardVertex[vertex2];
                }
                else if (vertex2.pointsTowardVertex.ContainsKey(vertex))
                {
                    list3 = new List<Vertex>(vertex2.pointsTowardVertex[vertex]);
                    list3.Reverse();
                }
                else
                {
                    list3 = new List<Vertex>();
                    list3.Add(vertex);
                    int riverIndex = ((vertex.isRiver && vertex2.isRiver) ? vertex.riverIndex : (-1));
                    for (int m = 1; m < num3; m++)
                    {
                        Vertex vertex5 = new Vertex(Vector3.Lerp(vertex.position, vertex2.position, (float)m / (float)num3));
                        list3.Add(vertex5);
                        vertex5.riverIndex = riverIndex;
                        if (m * 2 < num3)
                        {
                            vertex5.parentCells = vertex.parentCells;
                        }
                        else
                        {
                            vertex5.parentCells = vertex2.parentCells;
                        }
                    }
                    list3.Add(vertex2);
                    if (vertex.pointsTowardVertex == null)
                    {
                        Debug.LogError("[Impossible exception?] lVert Hex corner pointsTowardVertex NULL!");
                        throw new NotSupportedException();
                    }
                    if (vertex2.pointsTowardVertex == null)
                    {
                        Debug.LogError("[Impossible exception?] rVert Hex corner pointsTowardVertex NULL!");
                        throw new NotSupportedException();
                    }
                    vertex.pointsTowardVertex[vertex2] = list3;
                }
                List<Vertex> list4 = new List<Vertex>();
                List<Vertex> list5 = new List<Vertex>();
                list4.Add(this.center);
                this.vertices.Add(this.center);
                this.regionalizedVertices[j].Add(this.center);
                int num4 = 1;
                int num5 = 1;
                while (true)
                {
                    if (num4 == num && num5 == num2)
                    {
                        for (int n = 0; n < list3.Count; n++)
                        {
                            Vertex vertex6 = list3[n];
                            list5.Add(vertex6);
                            this.vertices.Add(vertex6);
                            this.regionalizedVertices[j].Add(vertex6);
                        }
                    }
                    else
                    {
                        int num6 = Mathf.RoundToInt(0.5f * ((float)num4 / (float)num + (float)num5 / (float)num2) * (float)num3) - 1;
                        Vertex vertex7 = list[num4];
                        Vertex vertex8 = list2[num5];
                        list5.Add(vertex7);
                        this.vertices.Add(vertex7);
                        this.regionalizedVertices[j].Add(vertex7);
                        for (int num7 = 0; num7 < num6; num7++)
                        {
                            float num8 = (float)(num7 + 1) / (float)(num6 + 1);
                            Vertex vertex9 = new Vertex(Vector3.Lerp(vertex7.position, vertex8.position, num8));
                            if (num8 < 0.5f)
                            {
                                vertex9.parentCells = vertex7.parentCells;
                            }
                            else
                            {
                                vertex9.parentCells = vertex8.parentCells;
                            }
                            list5.Add(vertex9);
                            this.vertices.Add(vertex9);
                            this.regionalizedVertices[j].Add(vertex9);
                        }
                        list5.Add(vertex8);
                        this.vertices.Add(vertex8);
                        this.regionalizedVertices[j].Add(vertex8);
                    }
                    int num9 = 0;
                    int num10 = 0;
                    while (list4.Count - 1 != num9 || list5.Count - 1 != num10)
                    {
                        Vertex v = list4[num9];
                        Vertex v2 = list5[num10];
                        float num11 = (float)(num10 + 1) / (float)list5.Count;
                        float num12 = (float)(num9 + 1) / (float)list4.Count;
                        Vertex v3;
                        if (num11 < num12 + 0.001f)
                        {
                            num10++;
                            v3 = list5[num10];
                        }
                        else
                        {
                            num9++;
                            v3 = list4[num9];
                        }
                        VertexTriangle item = new VertexTriangle(v2, v3, v);
                        this.triangles.Add(item);
                    }
                    if (num4 == num && num5 == num2)
                    {
                        break;
                    }
                    list4 = list5;
                    list5 = new List<Vertex>();
                    float num13 = (float)(num4 + 1) / (float)num;
                    float num14 = (float)(num5 + 1) / (float)num2;
                    if (num13 == num14)
                    {
                        num4++;
                        num5++;
                    }
                    else if (num13 < num14)
                    {
                        num4++;
                    }
                    else
                    {
                        num5++;
                    }
                }
            }
        }

        public int GetDivisionFor(int localTrianglePart, int border)
        {
            int num = this.subDivision[localTrianglePart];
            int num2 = 0;
            switch (border)
            {
            case 0:
                num2 = this.subDivision[(localTrianglePart + 5) % 6];
                break;
            case 1:
                num2 = this.subDivision[(localTrianglePart + 1) % 6];
                break;
            default:
                if (this.neighbourCells[localTrianglePart] != null)
                {
                    num2 = this.neighbourCells[localTrianglePart].Get().subDivision[(localTrianglePart + 3) % 6];
                }
                break;
            }
            if (num2 <= num)
            {
                return num;
            }
            return num2;
        }

        public Texture2DCache GetTextureMixer()
        {
            Terrain terrain = this.hex.GetTerrain();
            return TerrainTextures.Get().GetMixerC(terrain.terrainGraphic.mixer);
        }

        public float GetTextureMixer(Vector2 uv)
        {
            return this.GetTextureMixer()?.GetBilinear(uv.x, uv.y) ?? (-1f);
        }

        public Texture2DCache GetTextureHeight()
        {
            Terrain terrain = this.hex.GetTerrain();
            return TerrainTextures.Get().GetHeightC(terrain.terrainGraphic.height);
        }

        public float GetTextureHeight(Vector2 uv)
        {
            return this.GetTextureHeight()?.GetBilinear(uv.x, uv.y) ?? (-1f);
        }

        public Vector3 GetHeightAndMixerFor(Vector3 globalCoord)
        {
            Vector3 vector = HexCoordinates.HexToWorld3D(this.hex.Position);
            Vector3 vector2 = globalCoord - vector;
            vector2.y = 0f;
            Vector3 vector3 = this.rotationMatrix * vector2;
            Vector2 vector4 = new Vector2(vector3.x, vector3.z);
            float num = Mathf.Sqrt(3f);
            float num2 = 1.3f;
            Vector2 uv = vector4 / (num * 2f * num2);
            uv.x += 0.5f;
            uv.y += 0.5f;
            float textureMixer = this.GetTextureMixer(uv);
            return new Vector3(this.GetTextureHeight(uv), textureMixer, vector4.magnitude);
        }

        public Color GetDataColor()
        {
            int num = TerrainTextures.Get().IndexOfDiffuse(this.hex.GetTerrain().terrainGraphic.diffuse);
            int num2 = TerrainTextures.Get().IndexOfNormal(this.hex.GetTerrain().terrainGraphic.normal);
            int num3 = TerrainTextures.Get().IndexOfSpecular(this.hex.GetTerrain().terrainGraphic.specular);
            return new Color(this.hex.GetRotation(), (num < 0) ? 1f : ((float)num / 255f), (num2 < 0) ? 1f : ((float)num2 / 255f), (num3 < 0) ? 1f : ((float)num3 / 255f));
        }
    }
}
