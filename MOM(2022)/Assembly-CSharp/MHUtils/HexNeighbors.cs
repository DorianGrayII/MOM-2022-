using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace MHUtils
{
    public class HexNeighbors
    {
        public static Vector3i[] neighbours = new Vector3i[6]
        {
            new Vector3i(0, 1, -1),
            new Vector3i(1, 0, -1),
            new Vector3i(1, -1, 0),
            new Vector3i(0, -1, 1),
            new Vector3i(-1, 0, 1),
            new Vector3i(-1, 1, 0)
        };

        public static Dictionary<int, ReadOnlyCollection<Vector3i>> cachedCentralCircles = new Dictionary<int, ReadOnlyCollection<Vector3i>>();

        public static Dictionary<Multitype<int, int>, ReadOnlyCollection<Vector3i>> cachedCentralCircles2 = new Dictionary<Multitype<int, int>, ReadOnlyCollection<Vector3i>>();

        public static List<Vector3i> GetSharedRange(Vector3i centerA, int radiusA, Vector3i centerB, int radiusB, bool onlyBorder)
        {
            if (radiusA < 0 || radiusB < 0)
            {
                return new List<Vector3i>();
            }
            if (radiusB < radiusA)
            {
                return HexNeighbors.GetSharedRange(centerB, radiusB, centerA, radiusA, onlyBorder);
            }
            List<Vector3i> list = new List<Vector3i>();
            for (int i = -radiusA; i <= radiusA; i++)
            {
                for (int j = Mathf.Max(-radiusA, -i - radiusA); j <= Mathf.Min(radiusA, -i + radiusA); j++)
                {
                    int z = -i - j;
                    Vector3i vector3i = new Vector3i(i, j, z);
                    Vector3i vector3i2 = vector3i + centerA;
                    int num = HexCoordinates.HexDistance(Vector3i.zero, vector3i);
                    int num2 = HexCoordinates.HexDistance(centerB, vector3i2);
                    if (onlyBorder)
                    {
                        if ((num2 <= radiusB && num == radiusA) || (num < radiusA && num2 == radiusB))
                        {
                            list.Add(vector3i2);
                        }
                    }
                    else if (num2 <= radiusB && num <= radiusA)
                    {
                        list.Add(vector3i2);
                    }
                }
            }
            return list;
        }

        public static List<Vector3i> GetRange(Vector3i startPosition, int maxDistance)
        {
            return HexNeighbors.GetRange(startPosition, maxDistance, 0);
        }

        public static List<Vector3i> GetRange(Vector3i startPosition, int maxDistance, int minDistance)
        {
            if (maxDistance < 0 || minDistance > maxDistance)
            {
                return new List<Vector3i>();
            }
            List<Vector3i> list = new List<Vector3i>(HexNeighbors.GetRangeSimple(maxDistance, minDistance));
            if (startPosition != Vector3i.zero)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] += startPosition;
                }
            }
            return list;
        }

        public static ReadOnlyCollection<Vector3i> GetRangeSimple(int range)
        {
            if (!HexNeighbors.cachedCentralCircles.ContainsKey(range))
            {
                HexNeighbors.InitializeCircleCenteredOf(range);
            }
            return HexNeighbors.cachedCentralCircles[range];
        }

        public static ReadOnlyCollection<Vector3i> GetRangeSimple(int maxRange, int minRange)
        {
            if (minRange == 0)
            {
                return HexNeighbors.GetRangeSimple(maxRange);
            }
            ReadOnlyCollection<Vector3i> readOnlyCollection = null;
            Multitype<int, int> key = new Multitype<int, int>(maxRange, minRange);
            if (!HexNeighbors.cachedCentralCircles2.ContainsKey(key))
            {
                foreach (KeyValuePair<Multitype<int, int>, ReadOnlyCollection<Vector3i>> item in HexNeighbors.cachedCentralCircles2)
                {
                    if (item.Key.t0 == maxRange && item.Key.t1 == minRange)
                    {
                        return item.Value;
                    }
                }
                ReadOnlyCollection<Vector3i> rangeSimple = HexNeighbors.GetRangeSimple(maxRange);
                ReadOnlyCollection<Vector3i> rangeSimple2 = HexNeighbors.GetRangeSimple(minRange - 1);
                readOnlyCollection = new ReadOnlyCollection<Vector3i>(new List<Vector3i>(rangeSimple.Except(rangeSimple2)));
                HexNeighbors.cachedCentralCircles2[key] = readOnlyCollection;
            }
            else
            {
                readOnlyCollection = HexNeighbors.cachedCentralCircles2[key];
            }
            return readOnlyCollection;
        }

        public static void InitializeCircleCenteredRings()
        {
            for (int i = 0; i < 20; i++)
            {
                HexNeighbors.InitializeCircleCenteredOf(i);
            }
        }

        private static void InitializeCircleCenteredOf(int size)
        {
            List<Vector3i> list = new List<Vector3i>();
            list.Add(Vector3i.zero);
            for (int i = 0; i <= size; i++)
            {
                int num = -i;
                int j;
                int z;
                for (j = 0; j < i; j++)
                {
                    z = -num - j;
                    list.Add(new Vector3i(num, j, z));
                }
                num = i;
                for (j = -i + 1; j <= 0; j++)
                {
                    z = -num - j;
                    list.Add(new Vector3i(num, j, z));
                }
                z = -i;
                for (num = 0; num < i; num++)
                {
                    j = -num - z;
                    list.Add(new Vector3i(num, j, z));
                }
                z = i;
                for (num = -i + 1; num <= 0; num++)
                {
                    j = -num - z;
                    list.Add(new Vector3i(num, j, z));
                }
                j = -i;
                for (z = 0; z < i; z++)
                {
                    num = -z - j;
                    list.Add(new Vector3i(num, j, z));
                }
                j = i;
                for (z = -i + 1; z <= 0; z++)
                {
                    num = -z - j;
                    list.Add(new Vector3i(num, j, z));
                }
            }
            HexNeighbors.cachedCentralCircles[size] = new ReadOnlyCollection<Vector3i>(list);
        }

        public static List<Vector3i> GetHexCentersWithinSquare(Rect r)
        {
            Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(r.center);
            List<Vector3i> list = new List<Vector3i>();
            list.Add(hexCoordAt);
            int num = 1;
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (Vector3i item in HexNeighbors.GetRange(hexCoordAt, num, num))
                {
                    if (r.Contains(HexCoordinates.HexToWorld(item)))
                    {
                        flag = true;
                        list.Add(item);
                    }
                }
                num++;
            }
            return list;
        }

        public static Vector3i[] GetHexagonalQuad(Vector3 position)
        {
            Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(position);
            int num = 0;
            float num2 = float.MaxValue;
            for (int i = 0; i < HexNeighbors.neighbours.Length; i++)
            {
                Vector3 vector = HexCoordinates.HexToWorld3D(HexNeighbors.neighbours[i] + hexCoordAt);
                float magnitude = (position - vector).magnitude;
                if (num2 > magnitude)
                {
                    num = i;
                    num2 = magnitude;
                }
            }
            return new Vector3i[4]
            {
                hexCoordAt,
                hexCoordAt + HexNeighbors.neighbours[num],
                hexCoordAt + HexNeighbors.neighbours[(num + 5) % 6],
                hexCoordAt + HexNeighbors.neighbours[(num + 1) % 6]
            };
        }

        public static Vector3i[] GetHexagonalTriangle(Vector2 position)
        {
            float num = 2f / 3f;
            float num2 = 1f / 3f;
            float num3 = num2 * Mathf.Sqrt(3f);
            float num4 = num * position.x;
            float num5 = num3 * position.y - num2 * position.x;
            float num6 = 0f - num4 - num5;
            int num7 = Mathf.RoundToInt(num4);
            int num8 = Mathf.RoundToInt(num5);
            int num9 = Mathf.RoundToInt(num6);
            float f = num4 - (float)num7;
            float f2 = num5 - (float)num8;
            float f3 = num6 - (float)num9;
            float num10 = Mathf.Abs(f);
            float num11 = Mathf.Abs(f2);
            float num12 = Mathf.Abs(f3);
            Vector3i[] array = new Vector3i[3];
            if (num12 > num11 && num12 > num10)
            {
                num9 = -num7 - num8;
                array[0] = new Vector3i(num7, num8, num9);
            }
            else if (num11 > num10)
            {
                num8 = -num7 - num9;
                array[0] = new Vector3i(num7, num8, num9);
            }
            else
            {
                num7 = -num9 - num8;
                array[0] = new Vector3i(num7, num8, num9);
            }
            Vector2 lhs = position - HexCoordinates.HexToWorld(array[0]);
            float num13 = Mathf.Sqrt(3f);
            Vector2 rhs = new Vector2(1f, 0f);
            Vector2 rhs2 = new Vector2(-0.5f, num13 * 0.5f);
            Vector2 rhs3 = new Vector2(-0.5f, (0f - num13) * 0.5f);
            float num14 = Vector2.Dot(lhs, rhs);
            float num15 = Vector2.Dot(lhs, rhs2);
            float num16 = Vector2.Dot(lhs, rhs3);
            if (num14 > 0f)
            {
                if (num15 > 0f)
                {
                    array[1] = new Vector3i(num7, num8 + 1, num9 - 1);
                    array[2] = new Vector3i(num7 + 1, num8, num9 - 1);
                }
                else if (num16 > 0f)
                {
                    array[1] = new Vector3i(num7 + 1, num8 - 1, num9);
                    array[2] = new Vector3i(num7, num8 - 1, num9 + 1);
                }
                else
                {
                    array[1] = new Vector3i(num7 + 1, num8, num9 - 1);
                    array[2] = new Vector3i(num7 + 1, num8 - 1, num9);
                }
            }
            else if (num15 > 0f)
            {
                if (num16 > 0f)
                {
                    array[1] = new Vector3i(num7 - 1, num8 + 1, num9);
                    array[2] = new Vector3i(num7 - 1, num8, num9 + 1);
                }
                else
                {
                    array[1] = new Vector3i(num7, num8 + 1, num9 - 1);
                    array[2] = new Vector3i(num7 - 1, num8 + 1, num9);
                }
            }
            else
            {
                array[1] = new Vector3i(num7 - 1, num8, num9 + 1);
                array[2] = new Vector3i(num7, num8 - 1, num9 + 1);
            }
            return array;
        }
    }
}
