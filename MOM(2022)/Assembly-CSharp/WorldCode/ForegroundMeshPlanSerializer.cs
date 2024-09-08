namespace WorldCode
{
    using MHUtils;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ProtoContract]
    public class ForegroundMeshPlanSerializer
    {
        [ProtoMember(1)]
        public List<MHVector3> vertices;
        [ProtoMember(2)]
        public List<MHVector2> uv;
        [ProtoMember(3)]
        public List<MHVector2> uv2;
        [ProtoMember(4)]
        public List<int> indices;
        [ProtoMember(5)]
        public List<MHVector4> colors;

        public void Add(ForegroundMeshPlan f)
        {
            int capacity = (f.vertices != null) ? f.vertices.Count : 0;
            this.vertices = new List<MHVector3>(capacity);
            if (capacity > 0)
            {
                foreach (Vector3 vector in f.vertices)
                {
                    this.vertices.Add(vector);
                }
            }
            capacity = (f.uv != null) ? f.uv.Count : 0;
            this.uv = new List<MHVector2>(capacity);
            if (capacity > 0)
            {
                foreach (Vector2 vector2 in f.uv)
                {
                    this.uv.Add(vector2);
                }
            }
            capacity = (f.uv2 != null) ? f.uv2.Count : 0;
            this.uv2 = new List<MHVector2>(capacity);
            if (capacity > 0)
            {
                foreach (Vector2 vector3 in f.uv2)
                {
                    this.uv2.Add(vector3);
                }
            }
            capacity = (f.indices != null) ? f.indices.Count : 0;
            this.indices = new List<int>(capacity);
            if (capacity > 0)
            {
                foreach (int num2 in f.indices)
                {
                    this.indices.Add(num2);
                }
            }
            capacity = (f.colors != null) ? f.colors.Count : 0;
            this.colors = new List<MHVector4>(capacity);
            if (capacity > 0)
            {
                foreach (Color color in f.colors)
                {
                    MHVector4 item = new MHVector4(color.r, color.g, color.b, color.a);
                    this.colors.Add(item);
                }
            }
        }

        public void ReadTo(ForegroundMeshPlan f)
        {
            int capacity = (this.vertices != null) ? this.vertices.Count : 0;
            f.vertices = new List<Vector3>(capacity);
            if (capacity > 0)
            {
                foreach (MHVector3 vector in this.vertices)
                {
                    f.vertices.Add((Vector3) vector);
                }
            }
            capacity = (this.uv != null) ? this.uv.Count : 0;
            f.uv = new List<Vector2>(capacity);
            if (capacity > 0)
            {
                foreach (MHVector2 vector2 in this.uv)
                {
                    f.uv.Add((Vector2) vector2);
                }
            }
            capacity = (this.uv2 != null) ? this.uv2.Count : 0;
            f.uv2 = new List<Vector2>(capacity);
            if (capacity > 0)
            {
                foreach (MHVector2 vector3 in this.uv2)
                {
                    f.uv2.Add((Vector2) vector3);
                }
            }
            capacity = (this.indices != null) ? this.indices.Count : 0;
            f.indices = new List<int>(capacity);
            if (capacity > 0)
            {
                foreach (int num2 in this.indices)
                {
                    f.indices.Add(num2);
                }
            }
            capacity = (this.colors != null) ? this.colors.Count : 0;
            f.colors = new List<Color>(capacity);
            if (capacity > 0)
            {
                foreach (MHVector4 vector4 in this.colors)
                {
                    Color item = new Color(vector4.x, vector4.y, vector4.z, vector4.w);
                    f.colors.Add(item);
                }
            }
        }
    }
}

