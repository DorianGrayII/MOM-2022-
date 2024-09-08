using System.Collections.Generic;
using UnityEngine;

namespace MHUtils
{
    public class HexCoordinates
    {
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

        public const float HEX_RADIUS = 1f;

        protected static Vector2 Xdir;

        protected static Vector2 Ydir;

        protected static Vector2 Zdir;

        protected static bool dirInitialized;

        public static Vector3 WorldToHex(Vector3 pos)
        {
            float num = 2f / 3f;
            float num2 = 1f / 3f;
            float num3 = num2 * Mathf.Sqrt(3f);
            float num4 = num * pos.x / 1f;
            float num5 = (num3 * pos.z - num2 * pos.x) / 1f;
            float z = 0f - num4 - num5;
            return new Vector3(num4, num5, z);
        }

        public static Vector2 GetDirX()
        {
            if (!HexCoordinates.dirInitialized)
            {
                HexCoordinates.InitializeDirections();
            }
            return HexCoordinates.Xdir;
        }

        public static Vector2 GetDirY()
        {
            if (!HexCoordinates.dirInitialized)
            {
                HexCoordinates.InitializeDirections();
            }
            return HexCoordinates.Ydir;
        }

        public static Vector2 GetDirZ()
        {
            if (!HexCoordinates.dirInitialized)
            {
                HexCoordinates.InitializeDirections();
            }
            return HexCoordinates.Zdir;
        }

        private static void InitializeDirections()
        {
            Quaternion quaternion = Quaternion.Euler(0f, 0f, 120f);
            Quaternion quaternion2 = Quaternion.Euler(0f, 0f, 240f);
            Vector3 vector = new Vector3(1f, 0f, 0f);
            HexCoordinates.Xdir = vector;
            HexCoordinates.Ydir = quaternion * vector;
            HexCoordinates.Zdir = quaternion2 * vector;
        }

        public static Vector2 HexToWorld(Vector3i pos)
        {
            return HexCoordinates.GetDirX() * pos.x + HexCoordinates.GetDirY() * pos.y + HexCoordinates.GetDirZ() * pos.z;
        }

        public static Vector2 HexToWorld(Vector3 pos)
        {
            return HexCoordinates.GetDirX() * pos.x + HexCoordinates.GetDirY() * pos.y + HexCoordinates.GetDirZ() * pos.z;
        }

        public static Vector3 HexToWorld3D(Vector3i pos)
        {
            Vector2 vector = HexCoordinates.HexToWorld(pos);
            return new Vector3(vector.x, 0f, vector.y);
        }

        public static Vector3 HexToWorld3D(Vector3 pos)
        {
            Vector2 vector = HexCoordinates.HexToWorld(pos);
            return new Vector3(vector.x, 0f, vector.y);
        }

        public static Vector3i GetHexCoordAt(Vector3 worldPos)
        {
            return HexCoordinates.CustomRoundingForHexes(HexCoordinates.WorldToHex(worldPos));
        }

        public static Vector3i GetHexCoordAt(Vector2 worldPos)
        {
            return HexCoordinates.GetHexCoordAt(VectorUtils.Vector2To3D(worldPos));
        }

        public static Vector2i HexToPixelSpace(Vector3i pos)
        {
            short x = pos.x;
            int y = pos.y + Mathf.FloorToInt((float)pos.x / 2f);
            return new Vector2i(x, y);
        }

        public static Vector3i CustomRoundingForHexes(Vector3 position)
        {
            int num = Mathf.RoundToInt(position.x);
            int num2 = Mathf.RoundToInt(position.y);
            int num3 = Mathf.RoundToInt(position.z);
            float num4 = Mathf.Abs((float)num - position.x);
            float num5 = Mathf.Abs((float)num2 - position.y);
            float num6 = Mathf.Abs((float)num3 - position.z);
            if (num6 > num5 && num6 > num4)
            {
                num3 = -num - num2;
            }
            else if (num5 > num4)
            {
                num2 = -num - num3;
            }
            else
            {
                num = -num2 - num3;
            }
            return new Vector3i(num, num2, num3);
        }

        public static int HexDistance(Vector3i hexA, Vector3i hexB)
        {
            return Mathf.Max(Mathf.Abs(hexB.x - hexA.x), Mathf.Abs(hexB.y - hexA.y), Mathf.Abs(hexB.z - hexA.z));
        }

        public static int HexMinDistance(Vector3i hexA, List<Vector3i> hexB, bool ignoreSelf = false)
        {
            if (hexB == null || hexB.Count < 1)
            {
                return int.MaxValue;
            }
            int num = int.MaxValue;
            foreach (Vector3i item in hexB)
            {
                int num2 = HexCoordinates.HexDistance(hexA, item);
                if ((!ignoreSelf || num2 != 0) && num > num2)
                {
                    num = num2;
                }
            }
            return num;
        }

        public static HexTriangle GetHexTriangle(Vector2 pos)
        {
            float num = 2f / 3f;
            float num2 = 1f / 3f;
            float num3 = num2 * Mathf.Sqrt(3f);
            float num4 = num * pos.x;
            float num5 = num3 * pos.y - num2 * pos.x;
            float num6 = 0f - num4 - num5;
            Vector3i vector3i = default(Vector3i);
            vector3i.x = (short)num4;
            vector3i.y = (short)num5;
            vector3i.z = (short)num6;
            float num7 = num4 - (float)vector3i.x;
            float num8 = num5 - (float)vector3i.y;
            float num9 = num6 - (float)vector3i.z;
            float num10 = Mathf.Abs(num7);
            float num11 = Mathf.Abs(num8);
            float num12 = Mathf.Abs(num9);
            int num13 = ((num7 >= 0f) ? 1 : (-1));
            int num14 = ((num8 >= 0f) ? 1 : (-1));
            int num15 = ((num9 >= 0f) ? 1 : (-1));
            int num16 = ((!(Mathf.Abs(num7 + num8 + num9) > Mathf.Abs(Mathf.Max(Mathf.Max(num7, num8), num9)))) ? 1 : (-1));
            HexTriangle result = default(HexTriangle);
            if (num12 > num11 && num12 > num10)
            {
                vector3i.z = (short)(-vector3i.x - vector3i.y);
                result.v1 = new Vector3i(vector3i.x, vector3i.y, vector3i.z);
                result.v2 = new Vector3i(vector3i.x, vector3i.y + num14, vector3i.z + num15 * num16);
                result.v3 = new Vector3i(vector3i.x + num13, vector3i.y, vector3i.z + num15 * num16);
            }
            else if (num11 > num10)
            {
                vector3i.y = (short)(-vector3i.x - vector3i.z);
                result.v1 = new Vector3i(vector3i.x, vector3i.y, vector3i.z);
                result.v2 = new Vector3i(vector3i.x, vector3i.y + num14 * num16, vector3i.z + num15);
                result.v3 = new Vector3i(vector3i.x + num13, vector3i.y + num14 * num16, vector3i.z);
            }
            else
            {
                vector3i.x = (short)(-vector3i.y - vector3i.z);
                result.v1 = new Vector3i(vector3i.x, vector3i.y, vector3i.z);
                result.v2 = new Vector3i(vector3i.x + num13 * num16, vector3i.y + num14, vector3i.z);
                result.v3 = new Vector3i(vector3i.x + num13 * num16, vector3i.y, vector3i.z + num15);
            }
            result.uv1 = HexCoordinates.HexToWorld(result.v1);
            result.uv2 = HexCoordinates.HexToWorld(result.v2);
            result.uv3 = HexCoordinates.HexToWorld(result.v3);
            result.influence1 = HexCoordinates.CalculateInfluence(pos, result.uv2, result.uv3);
            result.influence2 = HexCoordinates.CalculateInfluence(result.uv1, pos, result.uv3);
            result.influence3 = HexCoordinates.CalculateInfluence(result.uv1, result.uv2, pos);
            result.poi = pos;
            return result;
        }

        private static float CalculateInfluence(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return Mathf.Abs(v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y)) * 0.5f;
        }
    }
}
