namespace MHUtils
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class HexCoordinates
    {
        public const float HEX_RADIUS = 1f;
        protected static Vector2 Xdir;
        protected static Vector2 Ydir;
        protected static Vector2 Zdir;
        protected static bool dirInitialized;

        private static float CalculateInfluence(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return (Mathf.Abs((float) (((v1.x * (v2.y - v3.y)) + (v2.x * (v3.y - v1.y))) + (v3.x * (v1.y - v2.y)))) * 0.5f);
        }

        public static Vector3i CustomRoundingForHexes(Vector3 position)
        {
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            int z = Mathf.RoundToInt(position.z);
            float num4 = Mathf.Abs((float) (x - position.x));
            float num5 = Mathf.Abs((float) (y - position.y));
            float num6 = Mathf.Abs((float) (z - position.z));
            if ((num6 > num5) && (num6 > num4))
            {
                z = -x - y;
            }
            else if (num5 > num4)
            {
                y = -x - z;
            }
            else
            {
                x = -y - z;
            }
            return new Vector3i(x, y, z);
        }

        public static Vector2 GetDirX()
        {
            if (!dirInitialized)
            {
                InitializeDirections();
            }
            return Xdir;
        }

        public static Vector2 GetDirY()
        {
            if (!dirInitialized)
            {
                InitializeDirections();
            }
            return Ydir;
        }

        public static Vector2 GetDirZ()
        {
            if (!dirInitialized)
            {
                InitializeDirections();
            }
            return Zdir;
        }

        public static Vector3i GetHexCoordAt(Vector2 worldPos)
        {
            return GetHexCoordAt(VectorUtils.Vector2To3D(worldPos));
        }

        public static Vector3i GetHexCoordAt(Vector3 worldPos)
        {
            return CustomRoundingForHexes(WorldToHex(worldPos));
        }

        public static HexTriangle GetHexTriangle(Vector2 pos)
        {
            Vector3i vectori;
            HexTriangle triangle;
            float num = 0.6666667f;
            float num2 = 0.3333333f;
            float num3 = num * pos.x;
            float num4 = ((num2 * Mathf.Sqrt(3f)) * pos.y) - (num2 * pos.x);
            float num5 = -num3 - num4;
            vectori.x = (short) num3;
            vectori.y = (short) num4;
            vectori.z = (short) num5;
            float f = num3 - vectori.x;
            float num7 = num4 - vectori.y;
            float num8 = num5 - vectori.z;
            float num9 = Mathf.Abs(f);
            float num10 = Mathf.Abs(num7);
            float num11 = Mathf.Abs(num8);
            int num12 = (f >= 0f) ? 1 : -1;
            int num13 = (num7 >= 0f) ? 1 : -1;
            int num14 = (num8 >= 0f) ? 1 : -1;
            int num15 = (Mathf.Abs((float) ((f + num7) + num8)) > Mathf.Abs(Mathf.Max(Mathf.Max(f, num7), num8))) ? -1 : 1;
            if ((num11 > num10) && (num11 > num9))
            {
                vectori.z = (short) (-vectori.x - vectori.y);
                triangle.v1 = new Vector3i(vectori.x, vectori.y, vectori.z);
                triangle.v2 = new Vector3i(vectori.x, vectori.y + num13, vectori.z + (num14 * num15));
                triangle.v3 = new Vector3i(vectori.x + num12, vectori.y, vectori.z + (num14 * num15));
            }
            else if (num10 > num9)
            {
                vectori.y = (short) (-vectori.x - vectori.z);
                triangle.v1 = new Vector3i(vectori.x, vectori.y, vectori.z);
                triangle.v2 = new Vector3i(vectori.x, vectori.y + (num13 * num15), vectori.z + num14);
                triangle.v3 = new Vector3i(vectori.x + num12, vectori.y + (num13 * num15), vectori.z);
            }
            else
            {
                vectori.x = (short) (-vectori.y - vectori.z);
                triangle.v1 = new Vector3i(vectori.x, vectori.y, vectori.z);
                triangle.v2 = new Vector3i(vectori.x + (num12 * num15), vectori.y + num13, vectori.z);
                triangle.v3 = new Vector3i(vectori.x + (num12 * num15), vectori.y, vectori.z + num14);
            }
            triangle.uv1 = HexToWorld(triangle.v1);
            triangle.uv2 = HexToWorld(triangle.v2);
            triangle.uv3 = HexToWorld(triangle.v3);
            triangle.influence1 = CalculateInfluence(pos, triangle.uv2, triangle.uv3);
            triangle.influence2 = CalculateInfluence(triangle.uv1, pos, triangle.uv3);
            triangle.influence3 = CalculateInfluence(triangle.uv1, triangle.uv2, pos);
            triangle.poi = pos;
            return triangle;
        }

        public static int HexDistance(Vector3i hexA, Vector3i hexB)
        {
            int[] values = new int[] { Mathf.Abs((int) (hexB.x - hexA.x)), Mathf.Abs((int) (hexB.y - hexA.y)), Mathf.Abs((int) (hexB.z - hexA.z)) };
            return Mathf.Max(values);
        }

        public static int HexMinDistance(Vector3i hexA, List<Vector3i> hexB, bool ignoreSelf)
        {
            if ((hexB == null) || (hexB.Count < 1))
            {
                return 0x7fffffff;
            }
            int num = 0x7fffffff;
            foreach (Vector3i vectori in hexB)
            {
                int num2 = HexDistance(hexA, vectori);
                if ((!ignoreSelf || (num2 != 0)) && (num > num2))
                {
                    num = num2;
                }
            }
            return num;
        }

        public static Vector2i HexToPixelSpace(Vector3i pos)
        {
            return new Vector2i(pos.x, pos.y + Mathf.FloorToInt(((float) pos.x) / 2f));
        }

        public static Vector2 HexToWorld(Vector3i pos)
        {
            return (((GetDirX() * pos.x) + (GetDirY() * pos.y)) + (GetDirZ() * pos.z));
        }

        public static Vector2 HexToWorld(Vector3 pos)
        {
            return (((GetDirX() * pos.x) + (GetDirY() * pos.y)) + (GetDirZ() * pos.z));
        }

        public static Vector3 HexToWorld3D(Vector3i pos)
        {
            Vector2 vector = HexToWorld(pos);
            return new Vector3(vector.x, 0f, vector.y);
        }

        public static Vector3 HexToWorld3D(Vector3 pos)
        {
            Vector2 vector = HexToWorld(pos);
            return new Vector3(vector.x, 0f, vector.y);
        }

        private static void InitializeDirections()
        {
            Quaternion quaternion = Quaternion.Euler(0f, 0f, 240f);
            Vector3 vector = new Vector3(1f, 0f, 0f);
            Xdir = vector;
            Ydir = (Vector2) (Quaternion.Euler(0f, 0f, 120f) * vector);
            Zdir = (Vector2) (quaternion * vector);
        }

        public static Vector3 WorldToHex(Vector3 pos)
        {
            float num = 0.6666667f;
            float num2 = 0.3333333f;
            float x = (num * pos.x) / 1f;
            float y = (((num2 * Mathf.Sqrt(3f)) * pos.z) - (num2 * pos.x)) / 1f;
            return new Vector3(x, y, -x - y);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HexTriangle
        {
            public Vector2 poi;
            public Vector3i v1;
            public Vector3i v2;
            public Vector3i v3;
            public Vector2 uv1;
            public Vector2 uv2;
            public Vector2 uv3;
            public float influence1;
            public float influence2;
            public float influence3;
        }
    }
}

