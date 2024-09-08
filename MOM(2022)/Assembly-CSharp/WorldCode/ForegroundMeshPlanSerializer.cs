using System.Collections.Generic;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
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
            int num = ((f.vertices != null) ? f.vertices.Count : 0);
            this.vertices = new List<MHVector3>(num);
            if (num > 0)
            {
                foreach (Vector3 vertex in f.vertices)
                {
                    this.vertices.Add(vertex);
                }
            }
            num = ((f.uv != null) ? f.uv.Count : 0);
            this.uv = new List<MHVector2>(num);
            if (num > 0)
            {
                foreach (Vector2 item2 in f.uv)
                {
                    this.uv.Add(item2);
                }
            }
            num = ((f.uv2 != null) ? f.uv2.Count : 0);
            this.uv2 = new List<MHVector2>(num);
            if (num > 0)
            {
                foreach (Vector2 item3 in f.uv2)
                {
                    this.uv2.Add(item3);
                }
            }
            num = ((f.indices != null) ? f.indices.Count : 0);
            this.indices = new List<int>(num);
            if (num > 0)
            {
                foreach (int index in f.indices)
                {
                    this.indices.Add(index);
                }
            }
            num = ((f.colors != null) ? f.colors.Count : 0);
            this.colors = new List<MHVector4>(num);
            if (num <= 0)
            {
                return;
            }
            foreach (Color color in f.colors)
            {
                MHVector4 item = new MHVector4(color.r, color.g, color.b, color.a);
                this.colors.Add(item);
            }
        }

        public void ReadTo(ForegroundMeshPlan f)
        {
            int num = ((this.vertices != null) ? this.vertices.Count : 0);
            f.vertices = new List<Vector3>(num);
            if (num > 0)
            {
                foreach (MHVector3 vertex in this.vertices)
                {
                    f.vertices.Add(vertex);
                }
            }
            num = ((this.uv != null) ? this.uv.Count : 0);
            f.uv = new List<Vector2>(num);
            if (num > 0)
            {
                foreach (MHVector2 item2 in this.uv)
                {
                    f.uv.Add(item2);
                }
            }
            num = ((this.uv2 != null) ? this.uv2.Count : 0);
            f.uv2 = new List<Vector2>(num);
            if (num > 0)
            {
                foreach (MHVector2 item3 in this.uv2)
                {
                    f.uv2.Add(item3);
                }
            }
            num = ((this.indices != null) ? this.indices.Count : 0);
            f.indices = new List<int>(num);
            if (num > 0)
            {
                foreach (int index in this.indices)
                {
                    f.indices.Add(index);
                }
            }
            num = ((this.colors != null) ? this.colors.Count : 0);
            f.colors = new List<Color>(num);
            if (num <= 0)
            {
                return;
            }
            foreach (MHVector4 color in this.colors)
            {
                Color item = new Color(color.x, color.y, color.z, color.w);
                f.colors.Add(item);
            }
        }
    }
}
