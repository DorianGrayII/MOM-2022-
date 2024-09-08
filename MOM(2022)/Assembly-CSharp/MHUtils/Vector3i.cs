using System;
using ProtoBuf;
using UnityEngine;

namespace MHUtils
{
    [Serializable]
    [ProtoContract]
    public struct Vector3i
    {
        [ProtoMember(1)]
        public short x;

        [ProtoMember(2)]
        public short y;

        [ProtoMember(3)]
        public short z;

        public static readonly Vector3i zero = new Vector3i(0, 0, 0);

        public static readonly Vector3i one = new Vector3i(1, 1, 1);

        public static readonly Vector3i forward = new Vector3i(0, 0, 1);

        public static readonly Vector3i back = new Vector3i(0, 0, -1);

        public static readonly Vector3i up = new Vector3i(0, 1, 0);

        public static readonly Vector3i down = new Vector3i(0, -1, 0);

        public static readonly Vector3i left = new Vector3i(-1, 0, 0);

        public static readonly Vector3i right = new Vector3i(1, 0, 0);

        public static readonly Vector3i invalid = new Vector3i(-1, -1, -1);

        public static readonly Vector3i[] directions = new Vector3i[6]
        {
            Vector3i.left,
            Vector3i.right,
            Vector3i.back,
            Vector3i.forward,
            Vector3i.down,
            Vector3i.up
        };

        public Vector3i(int x, int y, int z)
        {
            this.x = (short)x;
            this.y = (short)y;
            this.z = (short)z;
        }

        public Vector3i(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3i(int x, int y)
        {
            this.x = (short)x;
            this.y = (short)y;
            this.z = 0;
        }

        public Vector3i(short x, short y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public static Vector3i BuildHexCoord(int x, int y)
        {
            return new Vector3i(x, y, -x - y);
        }

        public static int DistanceSquared(Vector3i a, Vector3i b)
        {
            int num = b.x - a.x;
            int num2 = b.y - a.y;
            int num3 = b.z - a.z;
            return num * num + num2 * num2 + num3 * num3;
        }

        public int DistanceSquared(Vector3i v)
        {
            return Vector3i.DistanceSquared(this, v);
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2) ^ (this.z.GetHashCode() << 5);
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3i vector3i))
            {
                return false;
            }
            if (this.x == vector3i.x && this.y == vector3i.y)
            {
                return this.z == vector3i.z;
            }
            return false;
        }

        public override string ToString()
        {
            return "Vector3i(" + this.x + ", " + this.y + ", " + this.z + ")";
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this.x, this.y, this.z);
        }

        public Vector3i NormalizedAndScaled(float scale)
        {
            scale *= 2f;
            float num = Mathf.Abs(this.x) + Mathf.Abs(this.y) + Mathf.Abs(this.z);
            float num2 = scale * (float)this.x / num;
            float num3 = scale * (float)this.y / num;
            float num4 = scale * (float)this.z / num;
            int num5 = Mathf.RoundToInt(num2);
            int num6 = Mathf.RoundToInt(num3);
            int num7 = Mathf.RoundToInt(num4);
            float num8 = 0f - num3 - num4;
            float num9 = 0f - num2 - num4;
            float num10 = 0f - num2 - num3;
            int num11 = -num6 - num7;
            int num12 = -num5 - num7;
            int num13 = -num5 - num6;
            num8 = Mathf.Abs(num8 - (float)num11);
            num9 = Mathf.Abs(num9 - (float)num12);
            num10 = Mathf.Abs(num10 - (float)num13);
            if (num8 < num9 && num8 < num10)
            {
                return new Vector3i(num11, num6, num7);
            }
            if (num9 < num10)
            {
                return new Vector3i(num5, num12, num7);
            }
            return new Vector3i(num5, num6, num13);
        }

        public Vector3i NormalizedAndScaledToHorizontalAxis(float scale)
        {
            if (scale == 0f)
            {
                return Vector3i.zero;
            }
            Vector3i vector3i = this;
            if (this.x == 0)
            {
                vector3i += new Vector3i(1, -1, 0);
            }
            float num = Mathf.Abs(vector3i.x) + Mathf.Abs(vector3i.y) + Mathf.Abs(vector3i.z);
            scale = scale * num / (float)Mathf.Abs(vector3i.x);
            float num2 = scale * (float)vector3i.x / num;
            float num3 = scale * (float)vector3i.y / num;
            float num4 = scale * (float)vector3i.z / num;
            int num5 = Mathf.RoundToInt(num2);
            int num6 = Mathf.RoundToInt(num3);
            int num7 = Mathf.RoundToInt(num4);
            float num8 = 0f - num3 - num4;
            float num9 = 0f - num2 - num4;
            float num10 = 0f - num2 - num3;
            int num11 = -num6 - num7;
            int num12 = -num5 - num7;
            int num13 = -num5 - num6;
            num8 = Mathf.Abs(num8 - (float)num11);
            num9 = Mathf.Abs(num9 - (float)num12);
            num10 = Mathf.Abs(num10 - (float)num13);
            if (num8 < num9 && num8 < num10)
            {
                return new Vector3i(num11, num6, num7);
            }
            if (num9 < num10)
            {
                return new Vector3i(num5, num12, num7);
            }
            return new Vector3i(num5, num6, num13);
        }

        public Vector3i NormalizedAndScaledToVerticalAxis(float scale)
        {
            if (scale == 0f)
            {
                return Vector3i.zero;
            }
            Vector3i vector3i = this;
            if (this.y == this.z)
            {
                vector3i += new Vector3i(1, -1, 0);
            }
            float num = Mathf.Abs(vector3i.x) + Mathf.Abs(vector3i.y) + Mathf.Abs(vector3i.z);
            scale = scale * num / (float)Mathf.Abs(vector3i.y - vector3i.z);
            float num2 = scale * (float)vector3i.x / num;
            float num3 = scale * (float)vector3i.y / num;
            float num4 = scale * (float)vector3i.z / num;
            int num5 = Mathf.RoundToInt(num2);
            int num6 = Mathf.RoundToInt(num3);
            int num7 = Mathf.RoundToInt(num4);
            float num8 = 0f - num3 - num4;
            float num9 = 0f - num2 - num4;
            float num10 = 0f - num2 - num3;
            int num11 = -num6 - num7;
            int num12 = -num5 - num7;
            int num13 = -num5 - num6;
            num8 = Mathf.Abs(num8 - (float)num11);
            num9 = Mathf.Abs(num9 - (float)num12);
            num10 = Mathf.Abs(num10 - (float)num13);
            if (num8 < num9 && num8 < num10)
            {
                return new Vector3i(num11, num6, num7);
            }
            if (num9 < num10)
            {
                return new Vector3i(num5, num12, num7);
            }
            return new Vector3i(num5, num6, num13);
        }

        public static Vector3i Min(Vector3i a, Vector3i b)
        {
            return new Vector3i(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        public static Vector3i Max(Vector3i a, Vector3i b)
        {
            return new Vector3i(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            if (a.x == b.x && a.y == b.y)
            {
                return a.z == b.z;
            }
            return false;
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            if (a.x == b.x && a.y == b.y)
            {
                return a.z != b.z;
            }
            return true;
        }

        public static Vector3i operator *(Vector3i a, int b)
        {
            return new Vector3i(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3i operator *(int b, Vector3i a)
        {
            return new Vector3i(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3i operator /(Vector3i a, int b)
        {
            int num = Mathf.Abs(a.x);
            int num2 = Mathf.Abs(a.y);
            int num3 = Mathf.Abs(a.z);
            int num4 = a.x / b;
            int num5 = a.y / b;
            int num6 = a.z / b;
            if (num > num2 && num > num3)
            {
                return new Vector3i(-num5 - num6, num5, num6);
            }
            if (num2 > num3)
            {
                return new Vector3i(num4, -num4 - num6, num6);
            }
            return new Vector3i(num4, num5, -num4 - num5);
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3i c1, Vector3 c2)
        {
            return new Vector3((float)c1.x - c2.x, (float)c1.y - c2.y, (float)c1.z - c2.z);
        }

        public static Vector3 operator -(Vector3 c1, Vector3i c2)
        {
            return new Vector3(c1.x - (float)c2.x, c1.y - (float)c2.y, c1.z - (float)c2.z);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator +(Vector3i c1, Vector3 c2)
        {
            return new Vector3((float)c1.x + c2.x, (float)c1.y + c2.y, (float)c1.z + c2.z);
        }

        public static Vector3 operator +(Vector3 c1, Vector3i c2)
        {
            return new Vector3(c1.x + (float)c2.x, c1.y + (float)c2.y, c1.z + (float)c2.z);
        }

        public static Vector3 operator *(Vector3i a, float b)
        {
            return new Vector3((float)a.x * b, (float)a.y * b, (float)a.z * b);
        }

        public static Vector3 operator *(float b, Vector3i a)
        {
            return new Vector3((float)a.x * b, (float)a.y * b, (float)a.z * b);
        }

        public static Vector3 operator /(Vector3i a, float b)
        {
            return new Vector3((float)a.x / b, (float)a.y / b, (float)a.z / b);
        }

        public int SqMagnitude()
        {
            return this.x * this.x + this.y * this.y + this.z * this.z;
        }

        public static Vector3i Convert(Vector3 floatVector)
        {
            return Vector3i.Convert(floatVector.x, floatVector.y, floatVector.z);
        }

        public static Vector3i Convert(float x, float y, float z)
        {
            return new Vector3i(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(z));
        }

        public static Vector3i WrapByWidth(Vector3i pos, int width)
        {
            if ((width & 1) > 0)
            {
                Debug.LogError("odd width not implemented");
            }
            int num = width / 2;
            if (pos.x <= -num)
            {
                short num2 = (short)num;
                pos.x += (short)width;
                pos.y -= num2;
                pos.z -= num2;
                return pos;
            }
            if (pos.x > num)
            {
                short num3 = (short)num;
                pos.x -= (short)width;
                pos.y += num3;
                pos.z += num3;
                return pos;
            }
            return pos;
        }
    }
}
