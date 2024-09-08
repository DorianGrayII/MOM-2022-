namespace MHUtils
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UnityEngine;

    public class HexNeighbors
    {
        public static Vector3i[] neighbours = new Vector3i[] { new Vector3i(0, 1, -1), new Vector3i(1, 0, -1), new Vector3i(1, -1, 0), new Vector3i(0, -1, 1), new Vector3i(-1, 0, 1), new Vector3i(-1, 1, 0) };
        public static Dictionary<int, ReadOnlyCollection<Vector3i>> cachedCentralCircles = new Dictionary<int, ReadOnlyCollection<Vector3i>>();
        public static Dictionary<Multitype<int, int>, ReadOnlyCollection<Vector3i>> cachedCentralCircles2 = new Dictionary<Multitype<int, int>, ReadOnlyCollection<Vector3i>>();

        public static Vector3i[] GetHexagonalQuad(Vector3 position)
        {
            Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(position);
            int index = 0;
            float maxValue = float.MaxValue;
            for (int i = 0; i < neighbours.Length; i++)
            {
                Vector3 vector = HexCoordinates.HexToWorld3D(neighbours[i] + hexCoordAt);
                Vector3 vector2 = position - vector;
                float magnitude = vector2.magnitude;
                if (maxValue > magnitude)
                {
                    index = i;
                    maxValue = magnitude;
                }
            }
            return new Vector3i[] { hexCoordAt, (hexCoordAt + neighbours[index]), (hexCoordAt + neighbours[(index + 5) % 6]), (hexCoordAt + neighbours[(index + 1) % 6]) };
        }

        public static Vector3i[] GetHexagonalTriangle(Vector2 position)
        {
            float num = 0.6666667f;
            float num2 = 0.3333333f;
            float f = num * position.x;
            float num4 = ((num2 * Mathf.Sqrt(3f)) * position.y) - (num2 * position.x);
            float num5 = -f - num4;
            int x = Mathf.RoundToInt(f);
            int y = Mathf.RoundToInt(num4);
            int z = Mathf.RoundToInt(num5);
            float num9 = f - x;
            float num11 = Mathf.Abs(num9);
            float num12 = Mathf.Abs((float) (num4 - y));
            float num13 = Mathf.Abs((float) (num5 - z));
            Vector3i[] vectoriArray = new Vector3i[3];
            if ((num13 <= num12) || (num13 <= num11))
            {
                vectoriArray[0] = (num12 <= num11) ? new Vector3i(-z - y, y, z) : new Vector3i(x, -x - z, z);
            }
            else
            {
                z = -x - y;
                vectoriArray[0] = new Vector3i(x, y, z);
            }
            float num14 = Mathf.Sqrt(3f);
            Vector2 rhs = new Vector2(1f, 0f);
            Vector2 vector2 = new Vector2(-0.5f, num14 * 0.5f);
            Vector2 vector3 = new Vector2(-0.5f, -num14 * 0.5f);
            Vector2 lhs = position - HexCoordinates.HexToWorld(vectoriArray[0]);
            float num16 = Vector2.Dot(lhs, vector2);
            float num17 = Vector2.Dot(lhs, vector3);
            if (Vector2.Dot(lhs, rhs) > 0f)
            {
                if (num16 > 0f)
                {
                    vectoriArray[1] = new Vector3i(x, y + 1, z - 1);
                    vectoriArray[2] = new Vector3i(x + 1, y, z - 1);
                }
                else if (num17 > 0f)
                {
                    vectoriArray[1] = new Vector3i(x + 1, y - 1, z);
                    vectoriArray[2] = new Vector3i(x, y - 1, z + 1);
                }
                else
                {
                    vectoriArray[1] = new Vector3i(x + 1, y, z - 1);
                    vectoriArray[2] = new Vector3i(x + 1, y - 1, z);
                }
            }
            else if (num16 <= 0f)
            {
                vectoriArray[1] = new Vector3i(x - 1, y, z + 1);
                vectoriArray[2] = new Vector3i(x, y - 1, z + 1);
            }
            else if (num17 > 0f)
            {
                vectoriArray[1] = new Vector3i(x - 1, y + 1, z);
                vectoriArray[2] = new Vector3i(x - 1, y, z + 1);
            }
            else
            {
                vectoriArray[1] = new Vector3i(x, y + 1, z - 1);
                vectoriArray[2] = new Vector3i(x - 1, y + 1, z);
            }
            return vectoriArray;
        }

        public static List<Vector3i> GetHexCentersWithinSquare(Rect r)
        {
            Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(r.center);
            List<Vector3i> list = new List<Vector3i> {
                hexCoordAt
            };
            int maxDistance = 1;
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (Vector3i vectori2 in GetRange(hexCoordAt, maxDistance, maxDistance))
                {
                    if (r.Contains(HexCoordinates.HexToWorld(vectori2)))
                    {
                        flag = true;
                        list.Add(vectori2);
                    }
                }
                maxDistance++;
            }
            return list;
        }

        public static List<Vector3i> GetRange(Vector3i startPosition, int maxDistance)
        {
            return GetRange(startPosition, maxDistance, 0);
        }

        public static List<Vector3i> GetRange(Vector3i startPosition, int maxDistance, int minDistance)
        {
            if ((maxDistance < 0) || (minDistance > maxDistance))
            {
                return new List<Vector3i>();
            }
            List<Vector3i> list = new List<Vector3i>(GetRangeSimple(maxDistance, minDistance));
            if (startPosition != Vector3i.zero)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    List<Vector3i> list2 = list;
                    int num2 = i;
                    list2[num2] += startPosition;
                }
            }
            return list;
        }

        public static ReadOnlyCollection<Vector3i> GetRangeSimple(int range)
        {
            if (!cachedCentralCircles.ContainsKey(range))
            {
                InitializeCircleCenteredOf(range);
            }
            return cachedCentralCircles[range];
        }

        public static ReadOnlyCollection<Vector3i> GetRangeSimple(int maxRange, int minRange)
        {
            if (minRange == 0)
            {
                return GetRangeSimple(maxRange);
            }
            ReadOnlyCollection<Vector3i> onlys = null;
            Multitype<int, int> key = new Multitype<int, int>(maxRange, minRange);
            if (cachedCentralCircles2.ContainsKey(key))
            {
                return cachedCentralCircles2[key];
            }
            else
            {
                ReadOnlyCollection<Vector3i> onlys3;
                using (Dictionary<Multitype<int, int>, ReadOnlyCollection<Vector3i>>.Enumerator enumerator = cachedCentralCircles2.GetEnumerator())
                {
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            KeyValuePair<Multitype<int, int>, ReadOnlyCollection<Vector3i>> current = enumerator.Current;
                            if ((current.Key.t0 != maxRange) || (current.Key.t1 != minRange))
                            {
                                continue;
                            }
                            onlys3 = current.Value;
                        }
                        else
                        {
                            goto TR_0003;
                        }
                        break;
                    }
                }
                return onlys3;
            }
        TR_0003:
            onlys = new ReadOnlyCollection<Vector3i>(new List<Vector3i>(Enumerable.Except<Vector3i>(GetRangeSimple(maxRange), GetRangeSimple(minRange - 1))));
            cachedCentralCircles2[key] = onlys;
            return onlys;
        }

        public static List<Vector3i> GetSharedRange(Vector3i centerA, int radiusA, Vector3i centerB, int radiusB, bool onlyBorder)
        {
            if ((radiusA < 0) || (radiusB < 0))
            {
                return new List<Vector3i>();
            }
            if (radiusB < radiusA)
            {
                return GetSharedRange(centerB, radiusB, centerA, radiusA, onlyBorder);
            }
            List<Vector3i> list = new List<Vector3i>();
            int x = -radiusA;
            while (x <= radiusA)
            {
                int y = Mathf.Max(-radiusA, -x - radiusA);
                while (true)
                {
                    if (y > Mathf.Min(radiusA, -x + radiusA))
                    {
                        x++;
                        break;
                    }
                    int z = -x - y;
                    Vector3i hexB = new Vector3i(x, y, z);
                    Vector3i vectori2 = hexB + centerA;
                    int num4 = HexCoordinates.HexDistance(Vector3i.zero, hexB);
                    int num5 = HexCoordinates.HexDistance(centerB, vectori2);
                    if (onlyBorder)
                    {
                        if (((num5 <= radiusB) && (num4 == radiusA)) || ((num4 < radiusA) && (num5 == radiusB)))
                        {
                            list.Add(vectori2);
                        }
                    }
                    else if ((num5 <= radiusB) && (num4 <= radiusA))
                    {
                        list.Add(vectori2);
                    }
                    y++;
                }
            }
            return list;
        }

        private static void InitializeCircleCenteredOf(int size)
        {
            List<Vector3i> list = new List<Vector3i> {
                Vector3i.zero
            };
            int num = 0;
            while (num <= size)
            {
                int x = -num;
                int y = 0;
                while (true)
                {
                    int num4;
                    if (y >= num)
                    {
                        x = num;
                        y = -num + 1;
                        while (true)
                        {
                            if (y > 0)
                            {
                                num4 = -num;
                                x = 0;
                                while (true)
                                {
                                    if (x >= num)
                                    {
                                        num4 = num;
                                        x = -num + 1;
                                        while (true)
                                        {
                                            if (x > 0)
                                            {
                                                y = -num;
                                                num4 = 0;
                                                while (true)
                                                {
                                                    if (num4 >= num)
                                                    {
                                                        y = num;
                                                        num4 = -num + 1;
                                                        while (true)
                                                        {
                                                            if (num4 > 0)
                                                            {
                                                                num++;
                                                                break;
                                                            }
                                                            x = -num4 - y;
                                                            list.Add(new Vector3i(x, y, num4));
                                                            num4++;
                                                        }
                                                        break;
                                                    }
                                                    x = -num4 - y;
                                                    list.Add(new Vector3i(x, y, num4));
                                                    num4++;
                                                }
                                                break;
                                            }
                                            y = -x - num4;
                                            list.Add(new Vector3i(x, y, num4));
                                            x++;
                                        }
                                        break;
                                    }
                                    y = -x - num4;
                                    list.Add(new Vector3i(x, y, num4));
                                    x++;
                                }
                                break;
                            }
                            num4 = -x - y;
                            list.Add(new Vector3i(x, y, num4));
                            y++;
                        }
                        break;
                    }
                    num4 = -x - y;
                    list.Add(new Vector3i(x, y, num4));
                    y++;
                }
            }
            cachedCentralCircles[size] = new ReadOnlyCollection<Vector3i>(list);
        }

        public static void InitializeCircleCenteredRings()
        {
            for (int i = 0; i < 20; i++)
            {
                InitializeCircleCenteredOf(i);
            }
        }
    }
}

