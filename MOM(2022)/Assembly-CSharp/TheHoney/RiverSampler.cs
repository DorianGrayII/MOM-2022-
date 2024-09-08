namespace TheHoney
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class RiverSampler
    {
        public static float Y_TEXTUE_UV_STEP = 0.1f;
        private List<Sampler> samplers;

        public static RiverSampler CreateFromRiver(List<Vector3> river, float width)
        {
            if ((river == null) || (river.Count < 3))
            {
                return null;
            }
            RiverSampler sampler = new RiverSampler {
                samplers = new List<Sampler>()
            };
            Vector2 preStart = river[0];
            Vector2 start = river[0];
            Vector2 end = river[1];
            Vector2 postEnd = river[2];
            float y = 0f;
            float num2 = Y_TEXTUE_UV_STEP;
            for (int i = 3; start != end; i++)
            {
                Vector2 vector5 = end - start;
                float magnitude = vector5.magnitude;
                float num3 = y + (num2 * magnitude);
                Sampler item = Sampler.Create(preStart, start, end, postEnd, width, new Vector2(0f, y), new Vector2(1f, y), new Vector2(0f, num3), new Vector2(1f, num3));
                sampler.samplers.Add(item);
                y = num3;
                preStart = start;
                start = end;
                end = postEnd;
                if (river.Count > i)
                {
                    postEnd = river[i];
                }
            }
            return sampler;
        }

        public void DEBUG()
        {
            GameObject obj1 = new GameObject("River");
            obj1.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            obj1.AddComponent<MeshFilter>().mesh = mesh;
            mesh.vertices = this.DEBUG_Vertices().ToArray();
            mesh.triangles = this.DEBUG_Indices().ToArray();
            mesh.colors = this.DEBUG_Colors().ToArray();
            mesh.uv = this.DEBUG_UVs().ToArray();
        }

        private List<Color> DEBUG_Colors()
        {
            List<Color> list = new List<Color>();
            foreach (Sampler sampler in this.samplers)
            {
                list.Add(new Color(sampler.uv[0].x - ((int) sampler.uv[0].x), sampler.uv[0].y - ((int) sampler.uv[0].y), 0f));
                list.Add(new Color(sampler.uv[1].x - ((int) sampler.uv[1].x), sampler.uv[1].y - ((int) sampler.uv[1].y), 0f));
                list.Add(new Color(sampler.uv[2].x - ((int) sampler.uv[2].x), sampler.uv[2].y - ((int) sampler.uv[2].y), 0f));
                list.Add(new Color(sampler.uv[3].x - ((int) sampler.uv[3].x), sampler.uv[3].y - ((int) sampler.uv[3].y), 0f));
            }
            return list;
        }

        private List<int> DEBUG_Indices()
        {
            List<int> list = new List<int>();
            int item = 0;
            for (int i = 0; i < this.samplers.Count; i++)
            {
                list.Add(item);
                list.Add(item + 2);
                list.Add(item + 1);
                list.Add(item + 1);
                list.Add(item + 2);
                list.Add(item + 3);
                item += 4;
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

        public int FindSamplerForPoint(Vector2 position)
        {
            for (int i = 0; i < this.samplers.Count; i++)
            {
                Sampler sampler = this.samplers[i];
                if (sampler.IsWithin(position))
                {
                    return i;
                }
            }
            return -1;
        }

        public Vector2 GetUVForPoint(Vector2 v, int sampler)
        {
            if (sampler == -1)
            {
                sampler = this.FindSamplerForPoint(v);
            }
            return ((sampler != -1) ? this.samplers[sampler].GetPointData(v) : Vector2.zero);
        }
    }
}

