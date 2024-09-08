namespace WorldCode
{
    using ProtoBuf;
    using System;
    using UnityEngine;

    [ProtoContract]
    public class VertexTriangle
    {
        [ProtoMember(1)]
        public Vertex[] vertices;

        public VertexTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.vertices = new Vertex[] { v1, v2, v3 };
            v1.triangles.Add(this);
            v2.triangles.Add(this);
            v3.triangles.Add(this);
        }

        public Vector3 GetNormal()
        {
            Vector3 vector = Vector3.Cross((Vector3) (this.vertices[1].position - this.vertices[0].position), (Vector3) (this.vertices[2].position - this.vertices[0].position));
            if (vector.y < 0f)
            {
                vector *= -1f;
            }
            return vector;
        }
    }
}

