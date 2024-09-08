using System.Collections.Generic;
using DBDef;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace WorldCode
{
    [ProtoContract]
    public class ForegroundMeshPlan
    {
        [ProtoMember(1)]
        public ForegroundMeshPlanSerializer serializedData;

        [ProtoIgnore]
        public List<Vector3> vertices = new List<Vector3>();

        [ProtoIgnore]
        public List<Vector2> uv = new List<Vector2>();

        [ProtoIgnore]
        public List<Vector2> uv2 = new List<Vector2>();

        [ProtoIgnore]
        public List<int> indices = new List<int>();

        [ProtoIgnore]
        public List<Color> colors = new List<Color>();

        [ProtoIgnore]
        private float treeWaveingOffsetStore;

        [ProtoBeforeSerialization]
        public void BeforeSerialization()
        {
            this.serializedData = new ForegroundMeshPlanSerializer();
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

        public bool Add(Vector3 position, Foliage foliage, MHRandom random, bool allowRotation)
        {
            ForegroundInstance tree = ForegroundFactory.GetTree(foliage);
            if (string.IsNullOrEmpty(tree.name))
            {
                return true;
            }
            if (this.indices.Count + tree.indices.Length > 65000)
            {
                return false;
            }
            int count = this.vertices.Count;
            Quaternion quaternion = Quaternion.Euler(0f, random.GetFloat(0f, 360f), 0f);
            float @float = random.GetFloat(0.8f, 1.2f);
            Color b = Color.Lerp(foliage.color1, foliage.color2, random.GetFloat(0f, 1f));
            b = Color.Lerp(foliage.color3, b, random.GetFloat(0f, 1f));
            Vector2 vector = new Vector2(0f, this.treeWaveingOffsetStore);
            for (int i = 0; i < tree.vertices.Length; i++)
            {
                Vector3 vector2 = tree.vertices[i];
                if (allowRotation)
                {
                    vector2 = quaternion * vector2;
                }
                vector2 *= @float;
                vector2 += position;
                this.vertices.Add(vector2);
                this.uv2.Add(tree.uv2[i] + vector);
                this.colors.Add(b);
            }
            this.uv.AddRange(tree.uv);
            for (int j = 0; j < tree.indices.Length; j++)
            {
                this.indices.Add(tree.indices[j] + count);
            }
            return true;
        }
    }
}
