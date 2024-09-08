namespace MHUtils
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct V3iRect
    {
        public int width;
        public int height;
        public bool horizontalWrap;
        public Vector3i center;
        public Vector3i A00
        {
            get
            {
                return (this.center + GetLocalizedPoint((int) (-this.width + 1), (int) (-this.height + 1)));
            }
        }
        public Vector3i A10
        {
            get
            {
                return (this.center + GetLocalizedPoint(this.width, -this.height + 1));
            }
        }
        public Vector3i A01
        {
            get
            {
                return (this.center + GetLocalizedPoint(-this.width + 1, this.height));
            }
        }
        public Vector3i A11
        {
            get
            {
                return (this.center + GetLocalizedPoint(this.width, this.height));
            }
        }
        public int AreaWidth
        {
            get
            {
                return (this.width * 2);
            }
        }
        public int AreaHeight
        {
            get
            {
                return (this.height * 2);
            }
        }
        public V3iRect(int width, int height, bool horizontalWrap)
        {
            if ((((width / 2) * 2) != width) || (((height / 2) * 2) != height))
            {
                Debug.LogError("Invalid size!");
                width = (width / 2) * 2;
                height = (height / 2) * 2;
            }
            this.width = width;
            this.height = height;
            this.horizontalWrap = horizontalWrap;
            this.center = GetLocalizedPoint(width, height);
        }

        public V3iRect(int width, int height, Vector3i center, bool horizontalWrap)
        {
            if ((((width / 2) * 2) != width) || (((height / 2) * 2) != height))
            {
                Debug.LogError("Invalid size!");
                width = (width / 2) * 2;
                height = (height / 2) * 2;
            }
            this.width = width;
            this.height = height;
            this.center = center;
            this.horizontalWrap = horizontalWrap;
        }

        public V3iRect(IEnumerable<Vector3i> includedPoints)
        {
            int a = 0;
            int x = 0;
            int y = 0;
            int num4 = 0;
            bool flag = true;
            using (IEnumerator<Vector3i> enumerator = includedPoints.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Vector2i vectori2 = HexTo2D(enumerator.Current, Vector3i.zero);
                    if (!flag)
                    {
                        a = Mathf.Min(a, vectori2.x);
                        x = Mathf.Max(x, vectori2.x);
                        y = Mathf.Min(y, vectori2.y);
                        num4 = Mathf.Max(num4, vectori2.y);
                        continue;
                    }
                    flag = false;
                    a = vectori2.x;
                    x = vectori2.x;
                    y = vectori2.y;
                    num4 = vectori2.y;
                }
            }
            int num5 = ((1 + x) - a) / 2;
            int num6 = ((1 + num4) - y) / 2;
            Vector2i vectori = new Vector2i(a + num5, y + num6);
            this.width = num5 * 2;
            this.height = num6 * 2;
            this.center = GetLocalizedPoint(vectori.x, vectori.y);
            this.horizontalWrap = false;
        }

        public bool Empty()
        {
            return ((this.width == 0) && ((this.height == 0) && (this.center == Vector3i.zero)));
        }

        public List<Vector3i> GetAreaHex()
        {
            List<Vector3i> list = new List<Vector3i>();
            int column = -this.width + 1;
            while (column <= this.width)
            {
                int row = -this.height + 1;
                while (true)
                {
                    if (row > this.height)
                    {
                        column++;
                        break;
                    }
                    Vector3i item = this.center + GetLocalizedPoint(column, row);
                    list.Add(item);
                    row++;
                }
            }
            return list;
        }

        public override string ToString()
        {
            string[] textArray1 = new string[] { "V3iRect(", this.A00.ToString(), ",", this.A11.ToString(), ")" };
            return string.Concat(textArray1);
        }

        public static int HalfRoundDown(int val)
        {
            return (((val - val) & 1) / 2);
        }

        public static int HalfRoundUp(int val)
        {
            return Mathf.CeilToInt((val - 0.1f) * 0.5f);
        }

        public static Vector3i GetLocalizedPoint(int column, int row)
        {
            return Vector3i.BuildHexCoord(column, row + HalfRoundUp(-column));
        }

        public Vector3i GetLocalizedPoint(float u, float v)
        {
            return GetLocalizedPoint(Mathf.RoundToInt(this.AreaWidth * (u - 0.5f)), Mathf.RoundToInt(this.AreaHeight * (v - 0.5f)));
        }

        public Vector2i ConvertHexTo2DCenteredLoation(Vector3i worldPosition)
        {
            return HexTo2D(worldPosition, this.center);
        }

        public static Vector2i HexTo2D(Vector3i worldPosition, Vector3i center)
        {
            int x = worldPosition.x - center.x;
            return new Vector2i(x, (worldPosition.y - center.x) - HalfRoundUp(-x));
        }

        public unsafe Vector2i ConvertHexTo2DIntUVLoation(Vector3i worldPosition)
        {
            Vector2i vectori = this.ConvertHexTo2DCenteredLoation(worldPosition);
            int* numPtr1 = &vectori.x;
            numPtr1[0] += this.width;
            int* numPtr2 = &vectori.y;
            numPtr2[0] += this.height;
            return vectori;
        }

        public bool IsInside(Vector3i worldPosition, bool allowOutWrapping)
        {
            Vector2i vectori = this.ConvertHexTo2DCenteredLoation(worldPosition);
            return ((allowOutWrapping || !this.horizontalWrap) ? ((vectori.x > -this.width) && ((vectori.x <= this.width) && ((vectori.y > -this.height) && (vectori.y <= this.height)))) : ((vectori.y > -this.height) && (vectori.y <= this.height)));
        }

        public int DistanceToBorder(Vector3i worldPosition)
        {
            Vector2i vectori = this.ConvertHexTo2DCenteredLoation(worldPosition);
            return Math.Min((int) (this.width - Math.Abs(vectori.x)), (int) (this.height - Math.Abs(vectori.y)));
        }

        public Vector2 GetUV(Vector3i pos)
        {
            Vector2i vectori = this.ConvertHexTo2DCenteredLoation(pos);
            return new Vector2(((float) (vectori.x + this.width)) / ((float) this.AreaWidth), ((float) (vectori.y + this.height)) / ((float) this.AreaHeight));
        }

        public Vector3 KeepHorizontalInside(Vector3 pos)
        {
            Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(pos);
            return (!this.IsInside(hexCoordAt, false) ? ((pos - HexCoordinates.HexToWorld3D(hexCoordAt)) + HexCoordinates.HexToWorld3D(this.KeepHorizontalInside(hexCoordAt))) : pos);
        }

        public Vector3i KeepHorizontalInside(Vector3i pos)
        {
            return (this.horizontalWrap ? Vector3i.WrapByWidth(pos, this.width * 2) : pos);
        }

        public Vector3i UnvrappedNext(Vector3i point, Vector3i neighbour)
        {
            if (this.horizontalWrap)
            {
                int num = neighbour.x - point.x;
                if (num < -1)
                {
                    point.x = (short) (point.x - this.AreaWidth);
                    point.y = (short) (point.y + (this.AreaWidth / 2));
                    point.z = (short) (point.z + (this.AreaWidth / 2));
                }
                else if (num > 1)
                {
                    point.x = (short) (point.x + this.AreaWidth);
                    point.y = (short) (point.y - (this.AreaWidth / 2));
                    point.z = (short) (point.z - (this.AreaWidth / 2));
                }
            }
            return point;
        }

        public int HexDistance(Vector3i a, Vector3i b)
        {
            int num = HexCoordinates.HexDistance(a, b);
            if (this.horizontalWrap && (num > (this.width * 1.2f)))
            {
                if (a.x > b.x)
                {
                    a.x = (short) (a.x - (this.width * 2));
                    a.y = (short) (a.y + this.width);
                    a.z = (short) (a.z + this.width);
                }
                else
                {
                    b.x = (short) (b.x - (this.width * 2));
                    b.y = (short) (b.y + this.width);
                    b.z = (short) (b.z + this.width);
                }
                num = HexCoordinates.HexDistance(a, b);
            }
            return num;
        }
    }
}

