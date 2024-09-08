using System;
using System.Collections.Generic;
using UnityEngine;

namespace MHUtils
{
    [Serializable]
    public struct V3iRect
    {
        public int width;

        public int height;

        public bool horizontalWrap;

        public Vector3i center;

        public Vector3i A00 => this.center + V3iRect.GetLocalizedPoint(-this.width + 1, -this.height + 1);

        public Vector3i A10 => this.center + V3iRect.GetLocalizedPoint(this.width, -this.height + 1);

        public Vector3i A01 => this.center + V3iRect.GetLocalizedPoint(-this.width + 1, this.height);

        public Vector3i A11 => this.center + V3iRect.GetLocalizedPoint(this.width, this.height);

        public int AreaWidth => this.width * 2;

        public int AreaHeight => this.height * 2;

        public V3iRect(int width, int height, bool horizontalWrap)
        {
            if (width / 2 * 2 != width || height / 2 * 2 != height)
            {
                Debug.LogError("Invalid size!");
                width = width / 2 * 2;
                height = height / 2 * 2;
            }
            this.width = width;
            this.height = height;
            this.horizontalWrap = horizontalWrap;
            this.center = V3iRect.GetLocalizedPoint(width, height);
        }

        public V3iRect(int width, int height, Vector3i center, bool horizontalWrap)
        {
            if (width / 2 * 2 != width || height / 2 * 2 != height)
            {
                Debug.LogError("Invalid size!");
                width = width / 2 * 2;
                height = height / 2 * 2;
            }
            this.width = width;
            this.height = height;
            this.center = center;
            this.horizontalWrap = horizontalWrap;
        }

        public V3iRect(IEnumerable<Vector3i> includedPoints)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            bool flag = true;
            foreach (Vector3i includedPoint in includedPoints)
            {
                Vector2i vector2i = V3iRect.HexTo2D(includedPoint, Vector3i.zero);
                if (flag)
                {
                    flag = false;
                    num = vector2i.x;
                    num2 = vector2i.x;
                    num3 = vector2i.y;
                    num4 = vector2i.y;
                }
                else
                {
                    num = Mathf.Min(num, vector2i.x);
                    num2 = Mathf.Max(num2, vector2i.x);
                    num3 = Mathf.Min(num3, vector2i.y);
                    num4 = Mathf.Max(num4, vector2i.y);
                }
            }
            int num5 = (1 + num2 - num) / 2;
            int num6 = (1 + num4 - num3) / 2;
            Vector2i vector2i2 = new Vector2i(num + num5, num3 + num6);
            this.width = num5 * 2;
            this.height = num6 * 2;
            this.center = V3iRect.GetLocalizedPoint(vector2i2.x, vector2i2.y);
            this.horizontalWrap = false;
        }

        public bool Empty()
        {
            if (this.width == 0 && this.height == 0)
            {
                return this.center == Vector3i.zero;
            }
            return false;
        }

        public List<Vector3i> GetAreaHex()
        {
            List<Vector3i> list = new List<Vector3i>();
            for (int i = -this.width + 1; i <= this.width; i++)
            {
                for (int j = -this.height + 1; j <= this.height; j++)
                {
                    Vector3i item = this.center + V3iRect.GetLocalizedPoint(i, j);
                    list.Add(item);
                }
            }
            return list;
        }

        public override string ToString()
        {
            return "V3iRect(" + this.A00.ToString() + "," + this.A11.ToString() + ")";
        }

        public static int HalfRoundDown(int val)
        {
            return ((val - val) & 1) / 2;
        }

        public static int HalfRoundUp(int val)
        {
            return Mathf.CeilToInt(((float)val - 0.1f) * 0.5f);
        }

        public static Vector3i GetLocalizedPoint(int column, int row)
        {
            return Vector3i.BuildHexCoord(column, row + V3iRect.HalfRoundUp(-column));
        }

        public Vector3i GetLocalizedPoint(float u, float v)
        {
            int column = Mathf.RoundToInt((float)this.AreaWidth * (u - 0.5f));
            int row = Mathf.RoundToInt((float)this.AreaHeight * (v - 0.5f));
            return V3iRect.GetLocalizedPoint(column, row);
        }

        public Vector2i ConvertHexTo2DCenteredLoation(Vector3i worldPosition)
        {
            return V3iRect.HexTo2D(worldPosition, this.center);
        }

        public static Vector2i HexTo2D(Vector3i worldPosition, Vector3i center)
        {
            int num = worldPosition.x - center.x;
            int y = worldPosition.y - center.x - V3iRect.HalfRoundUp(-num);
            return new Vector2i(num, y);
        }

        public Vector2i ConvertHexTo2DIntUVLoation(Vector3i worldPosition)
        {
            Vector2i result = this.ConvertHexTo2DCenteredLoation(worldPosition);
            result.x += this.width;
            result.y += this.height;
            return result;
        }

        public bool IsInside(Vector3i worldPosition, bool allowOutWrapping = false)
        {
            Vector2i vector2i = this.ConvertHexTo2DCenteredLoation(worldPosition);
            if (allowOutWrapping || !this.horizontalWrap)
            {
                if (vector2i.x <= -this.width || vector2i.x > this.width || vector2i.y <= -this.height || vector2i.y > this.height)
                {
                    return false;
                }
            }
            else if (vector2i.y <= -this.height || vector2i.y > this.height)
            {
                return false;
            }
            return true;
        }

        public int DistanceToBorder(Vector3i worldPosition)
        {
            Vector2i vector2i = this.ConvertHexTo2DCenteredLoation(worldPosition);
            return Math.Min(this.width - Math.Abs(vector2i.x), this.height - Math.Abs(vector2i.y));
        }

        public Vector2 GetUV(Vector3i pos)
        {
            Vector2i vector2i = this.ConvertHexTo2DCenteredLoation(pos);
            return new Vector2((float)(vector2i.x + this.width) / (float)this.AreaWidth, (float)(vector2i.y + this.height) / (float)this.AreaHeight);
        }

        public Vector3 KeepHorizontalInside(Vector3 pos)
        {
            Vector3i hexCoordAt = HexCoordinates.GetHexCoordAt(pos);
            if (this.IsInside(hexCoordAt))
            {
                return pos;
            }
            Vector3 vector = pos - HexCoordinates.HexToWorld3D(hexCoordAt);
            Vector3i pos2 = this.KeepHorizontalInside(hexCoordAt);
            return vector + HexCoordinates.HexToWorld3D(pos2);
        }

        public Vector3i KeepHorizontalInside(Vector3i pos)
        {
            if (!this.horizontalWrap)
            {
                return pos;
            }
            return Vector3i.WrapByWidth(pos, this.width * 2);
        }

        public Vector3i UnvrappedNext(Vector3i point, Vector3i neighbour)
        {
            if (!this.horizontalWrap)
            {
                return point;
            }
            int num = neighbour.x - point.x;
            if (num < -1)
            {
                point.x = (short)(point.x - this.AreaWidth);
                point.y = (short)(point.y + this.AreaWidth / 2);
                point.z = (short)(point.z + this.AreaWidth / 2);
            }
            else if (num > 1)
            {
                point.x = (short)(point.x + this.AreaWidth);
                point.y = (short)(point.y - this.AreaWidth / 2);
                point.z = (short)(point.z - this.AreaWidth / 2);
            }
            return point;
        }

        public int HexDistance(Vector3i a, Vector3i b)
        {
            int num = HexCoordinates.HexDistance(a, b);
            if (this.horizontalWrap && (float)num > (float)this.width * 1.2f)
            {
                if (a.x > b.x)
                {
                    a.x = (short)(a.x - this.width * 2);
                    a.y = (short)(a.y + this.width);
                    a.z = (short)(a.z + this.width);
                }
                else
                {
                    b.x = (short)(b.x - this.width * 2);
                    b.y = (short)(b.y + this.width);
                    b.z = (short)(b.z + this.width);
                }
                num = HexCoordinates.HexDistance(a, b);
            }
            return num;
        }
    }
}
