namespace WorldCode
{
    using ProtoBuf;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential), ProtoContract]
    public struct ForegroundInstance
    {
        [ProtoMember(1)]
        public string name;
        [ProtoMember(5)]
        public int[] indices;
        public Vector3[] vertices;
        public Vector2[] uv;
        public Vector2[] uv2;
        [ProtoMember(2)]
        public float[] fvertices
        {
            get
            {
                if (this.vertices == null)
                {
                    return null;
                }
                float[] numArray = new float[this.vertices.Length * 3];
                for (int i = 0; i < this.vertices.Length; i++)
                {
                    numArray[i * 3] = this.vertices[i].x;
                    numArray[(i * 3) + 1] = this.vertices[i].y;
                    numArray[(i * 3) + 2] = this.vertices[i].z;
                }
                return numArray;
            }
            set
            {
                if (value == null)
                {
                    this.vertices = null;
                }
                else
                {
                    int num = value.Length / 3;
                    this.vertices = new Vector3[num];
                    for (int i = 0; i < num; i++)
                    {
                        this.vertices[i] = new Vector3(value[i * 3], value[(i * 3) + 1], value[(i * 3) + 2]);
                    }
                }
            }
        }
        [ProtoMember(3)]
        public float[] fuv
        {
            get
            {
                if (this.uv == null)
                {
                    return null;
                }
                float[] numArray = new float[this.uv.Length * 2];
                for (int i = 0; i < this.uv.Length; i++)
                {
                    numArray[i * 2] = this.uv[i].x;
                    numArray[(i * 2) + 1] = this.uv[i].y;
                }
                return numArray;
            }
            set
            {
                if (value == null)
                {
                    this.uv = null;
                }
                else
                {
                    int num = value.Length / 2;
                    this.uv = new Vector2[num];
                    for (int i = 0; i < num; i++)
                    {
                        this.uv[i] = new Vector2(value[i * 2], value[(i * 2) + 1]);
                    }
                }
            }
        }
        [ProtoMember(4)]
        public float[] fuv2
        {
            get
            {
                if (this.uv2 == null)
                {
                    return null;
                }
                float[] numArray = new float[this.uv2.Length * 2];
                for (int i = 0; i < this.uv2.Length; i++)
                {
                    numArray[i * 2] = this.uv2[i].x;
                    numArray[(i * 2) + 1] = this.uv2[i].y;
                }
                return numArray;
            }
            set
            {
                if (value == null)
                {
                    this.uv2 = null;
                }
                else
                {
                    int num = value.Length / 2;
                    this.uv2 = new Vector2[num];
                    for (int i = 0; i < num; i++)
                    {
                        this.uv2[i] = new Vector2(value[i * 2], value[(i * 2) + 1]);
                    }
                }
            }
        }
    }
}

