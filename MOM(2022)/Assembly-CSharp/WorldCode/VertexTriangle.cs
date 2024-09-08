using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    [ProtoContract]
    public class VertexTriangle
    {
        [ProtoMember(1)]
        public Vertex[] vertices;

        public VertexTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.vertices = new Vertex[3];
            this.vertices[0] = v1;
            this.vertices[1] = v2;
            this.vertices[2] = v3;
            v1.triangles.Add(this);
            v2.triangles.Add(this);
            v3.triangles.Add(this);
        }

        public Vector3 GetNormal()
        {
            Vector3 result = Vector3.Cross(this.vertices[1].position - this.vertices[0].position, this.vertices[2].position - this.vertices[0].position);
            if (result.y < 0f)
            {
                result *= -1f;
            }
            return result;
        }
    }
}
