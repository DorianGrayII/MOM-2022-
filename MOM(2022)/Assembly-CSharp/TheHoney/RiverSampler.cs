using System.Collections.Generic;
using UnityEngine;

namespace TheHoney
{
    public class RiverSampler
    {
        public static float Y_TEXTUE_UV_STEP = 0.1f;

        private List<Sampler> samplers;

        public static RiverSampler CreateFromRiver(List<Vector3> river, float width)
        {
            if (river == null || river.Count < 3)
            {
                return null;
            }
            RiverSampler riverSampler = new RiverSampler();
            riverSampler.samplers = new List<Sampler>();
            Vector2 preStart = river[0];
            Vector2 vector = river[0];
            Vector2 vector2 = river[1];
            Vector2 vector3 = river[2];
            float num = 0f;
            float y_TEXTUE_UV_STEP = RiverSampler.Y_TEXTUE_UV_STEP;
            int num2 = 3;
            while (vector != vector2)
            {
                float magnitude = (vector2 - vector).magnitude;
                float num3 = num + y_TEXTUE_UV_STEP * magnitude;
                Sampler item = Sampler.Create(preStart, vector, vector2, vector3, width, new Vector2(0f, num), new Vector2(1f, num), new Vector2(0f, num3), new Vector2(1f, num3));
                riverSampler.samplers.Add(item);
                num = num3;
                preStart = vector;
                vector = vector2;
                vector2 = vector3;
                if (river.Count > num2)
                {
                    vector3 = river[num2];
                }
                num2++;
            }
            return riverSampler;
        }

        public int FindSamplerForPoint(Vector2 position)
        {
            for (int i = 0; i < this.samplers.Count; i++)
            {
                if (this.samplers[i].IsWithin(position))
                {
                    return i;
                }
            }
            return -1;
        }

        public Vector2 GetUVForPoint(Vector2 v, int sampler = -1)
        {
            if (sampler == -1)
            {
                sampler = this.FindSamplerForPoint(v);
            }
            if (sampler == -1)
            {
                return Vector2.zero;
            }
            return this.samplers[sampler].GetPointData(v);
        }

        public void DEBUG()
        {
            GameObject gameObject = new GameObject("River");
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            Mesh mesh2 = (meshFilter.mesh = new Mesh());
            mesh2.vertices = this.DEBUG_Vertices().ToArray();
            mesh2.triangles = this.DEBUG_Indices().ToArray();
            mesh2.colors = this.DEBUG_Colors().ToArray();
            mesh2.uv = this.DEBUG_UVs().ToArray();
        }

        private List<Vector3> DEBUG_Vertices()
        {
            float y = 2f;
            List<Vector3> list = new List<Vector3>();
            foreach (Sampler sampler in this.samplers)
            {
                list.Add(new Vector3(sampler.startLeft.x, y, sampler.startLeft.y));
                list.Add(new Vector3(sampler.startRight.x, y, sampler.startRight.y));
                list.Add(new Vector3(sampler.endLeft.x, y, sampler.endLeft.y));
                list.Add(new Vector3(sampler.endRight.x, y, sampler.endRight.y));
            }
            return list;
        }

        private List<int> DEBUG_Indices()
        {
            List<int> list = new List<int>();
            int num = 0;
            for (int i = 0; i < this.samplers.Count; i++)
            {
                list.Add(num);
                list.Add(num + 2);
                list.Add(num + 1);
                list.Add(num + 1);
                list.Add(num + 2);
                list.Add(num + 3);
                num += 4;
            }
            return list;
        }

        private List<Color> DEBUG_Colors()
        {
            List<Color> list = new List<Color>();
            foreach (Sampler sampler in this.samplers)
            {
                list.Add(new Color(sampler.uv[0].x - (float)(int)sampler.uv[0].x, sampler.uv[0].y - (float)(int)sampler.uv[0].y, 0f));
                list.Add(new Color(sampler.uv[1].x - (float)(int)sampler.uv[1].x, sampler.uv[1].y - (float)(int)sampler.uv[1].y, 0f));
                list.Add(new Color(sampler.uv[2].x - (float)(int)sampler.uv[2].x, sampler.uv[2].y - (float)(int)sampler.uv[2].y, 0f));
                list.Add(new Color(sampler.uv[3].x - (float)(int)sampler.uv[3].x, sampler.uv[3].y - (float)(int)sampler.uv[3].y, 0f));
            }
            return list;
        }

        private List<Vector2> DEBUG_UVs()
        {
            List<Vector2> list = new List<Vector2>();
            foreach (Sampler sampler in this.samplers)
            {
                list.Add(sampler.uv[0]);
                list.Add(sampler.uv[1]);
                list.Add(sampler.uv[2]);
                list.Add(sampler.uv[3]);
            }
            return list;
        }
    }
}
