using System;
using System.Collections.Generic;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    [ProtoContract]
    public class MeshChunkSectionSerializer
    {
        [ProtoMember(1)]
        private List<MHVector3> vertices;

        [ProtoMember(2)]
        private List<MHVector3> normals;

        [ProtoMember(4)]
        private List<MHVector4> colors;

        [ProtoMember(5)]
        private List<MHVector2> uvs;

        [ProtoMember(6)]
        private List<MHVector4> tangent;

        public void Add(MeshChunkSection f)
        {
            int num = ((f.vertices != null) ? f.vertices.Count : 0);
            this.vertices = new List<MHVector3>(num);
            if (num > 0)
            {
                this.vertices = f.vertices.ConvertAll((Converter<Vector3, MHVector3>)((Vector3 x) => x));
            }
            num = ((f.normals != null) ? f.normals.Count : 0);
            this.normals = new List<MHVector3>(num);
            if (num > 0)
            {
                this.normals = f.normals.ConvertAll((Converter<Vector3, MHVector3>)((Vector3 x) => x));
            }
            num = ((f.colors != null) ? f.colors.Count : 0);
            this.colors = new List<MHVector4>(num);
            if (num > 0)
            {
                this.colors = f.colors.ConvertAll((Color x) => new MHVector4(x.r, x.g, x.b, x.a));
            }
            num = ((f.tangent != null) ? f.tangent.Length : 0);
            this.tangent = new List<MHVector4>(num);
            if (num > 0)
            {
                this.tangent = new List<Vector4>(f.tangent).ConvertAll((Vector4 x) => new MHVector4(x.x, x.y, x.z, x.w));
            }
            num = ((f.uvs != null) ? f.uvs.Count : 0);
            this.uvs = new List<MHVector2>(num);
            if (num > 0)
            {
                this.uvs = f.uvs.ConvertAll((Converter<Vector2, MHVector2>)((Vector2 x) => x));
            }
        }

        public void ReadPartial(MeshChunkSection f)
        {
            int num = ((this.vertices != null) ? this.vertices.Count : 0);
            f.vertices = new List<Vector3>(num);
            if (num > 0)
            {
                f.vertices = this.vertices.ConvertAll((Converter<MHVector3, Vector3>)((MHVector3 x) => x));
            }
            num = ((this.normals != null) ? this.normals.Count : 0);
            f.normals = new List<Vector3>(num);
            if (num > 0)
            {
                f.normals = this.normals.ConvertAll((Converter<MHVector3, Vector3>)((MHVector3 x) => x));
            }
        }

        public void ReadTo(MeshChunkSection f)
        {
            int num = ((this.vertices != null) ? this.vertices.Count : 0);
            f.vertices = new List<Vector3>(num);
            if (num > 0)
            {
                f.vertices = this.vertices.ConvertAll((Converter<MHVector3, Vector3>)((MHVector3 x) => x));
            }
            num = ((this.normals != null) ? this.normals.Count : 0);
            f.normals = new List<Vector3>(num);
            if (num > 0)
            {
                f.normals = this.normals.ConvertAll((Converter<MHVector3, Vector3>)((MHVector3 x) => x));
            }
            num = ((this.colors != null) ? this.colors.Count : 0);
            f.colors = new List<Color>(num);
            if (num > 0)
            {
                f.colors = this.colors.ConvertAll((MHVector4 x) => new Color(x.x, x.y, x.z, x.w));
            }
            num = ((this.tangent != null) ? this.tangent.Count : 0);
            f.tangent = null;
            if (num > 0)
            {
                f.tangent = this.tangent.ConvertAll((Converter<MHVector4, Vector4>)((MHVector4 x) => x)).ToArray();
            }
            num = ((this.uvs != null) ? this.uvs.Count : 0);
            f.uvs = new List<Vector2>(num);
            if (num > 0)
            {
                f.uvs = this.uvs.ConvertAll((Converter<MHVector2, Vector2>)((MHVector2 x) => x));
            }
        }
    }
}
