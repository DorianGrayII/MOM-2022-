namespace MHUtils
{
    using ProtoBuf;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), ProtoContract]
    public struct Vector3i
    {
        [ProtoMember(1)]
        public short x;
        [ProtoMember(2)]
        public short y;
        [ProtoMember(3)]
        public short z;
        public static readonly Vector3i zero;
        public static readonly Vector3i one;
        public static readonly Vector3i forward;
        public static readonly Vector3i back;
        public static readonly Vector3i up;
        public static readonly Vector3i down;
        public static readonly Vector3i left;
        public static readonly Vector3i right;
        public static readonly Vector3i invalid;
        public static readonly Vector3i[] directions;
        public Vector3i(int x, int y, int z)
        {
            this.x = (short) x;
            this.y = (short) y;
            this.z = (short) z;
        }

        public Vector3i(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3i(int x, int y)
        {
            this.x = (short) x;
            this.y = (short) y;
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
            int num = b.y - a.y;
            int num2 = b.z - a.z;
            int num1 = b.x - a.x;
            return (((num1 * num1) + (num * num)) + (num2 * num2));
        }

        public int DistanceSquared(Vector3i v)
        {
            return DistanceSquared(this, v);
        }

        public override int GetHashCode()
        {
            return ((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() << 5));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector3i))
            {
                return false;
            }
            Vector3i vectori = (Vector3i) other;
            return ((this.x == vectori.x) && ((this.y == vectori.y) && (this.z == vectori.z)));
        }

        public override string ToString()
        {
            string[] textArray1 = new string[] { "Vector3i(", this.x.ToString(), ", ", this.y.ToString(), ", ", this.z.ToString(), ")" };
            return string.Concat(textArray1);
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float) this.x, (float) this.y, (float) this.z);
        }

        public Vector3i NormalizedAndScaled(float scale)
        {
            scale *= 2f;
            float num = (Mathf.Abs((int) this.x) + Mathf.Abs((int) this.y)) + Mathf.Abs((int) this.z);
            float f = (scale * this.y) / num;
            float num3 = (scale * this.z) / num;
            float single1 = (scale * this.x) / num;
            int x = Mathf.RoundToInt(single1);
            int y = Mathf.RoundToInt(f);
            int z = Mathf.RoundToInt(num3);
            int num10 = -y - z;
            int num11 = -x - z;
            int num12 = -x - y;
            float num7 = Mathf.Abs((float) ((-f - num3) - num10));
            float num8 = Mathf.Abs((float) ((-single1 - num3) - num11));
            float num9 = Mathf.Abs((float) ((-single1 - f) - num12));
            return (((num7 >= num8) || (num7 >= num9)) ? ((num8 >= num9) ? new Vector3i(x, y, num12) : new Vector3i(x, num11, z)) : new Vector3i(num10, y, z));
        }

        public Vector3i NormalizedAndScaledToHorizontalAxis(float scale)
        {
            if (scale == 0f)
            {
                return zero;
            }
            Vector3i vectori = this;
            if (this.x == 0)
            {
                vectori += new Vector3i(1, -1, 0);
            }
            float num = (Mathf.Abs((int) vectori.x) + Mathf.Abs((int) vectori.y)) + Mathf.Abs((int) vectori.z);
            scale = (scale * num) / ((float) Mathf.Abs((int) vectori.x));
            float f = (scale * vectori.y) / num;
            float num3 = (scale * vectori.z) / num;
            float single1 = (scale * vectori.x) / num;
            int x = Mathf.RoundToInt(single1);
            int y = Mathf.RoundToInt(f);
            int z = Mathf.RoundToInt(num3);
            int num10 = -y - z;
            int num11 = -x - z;
            int num12 = -x - y;
            float num7 = Mathf.Abs((float) ((-f - num3) - num10));
            float num8 = Mathf.Abs((float) ((-single1 - num3) - num11));
            float num9 = Mathf.Abs((float) ((-single1 - f) - num12));
            return (((num7 >= num8) || (num7 >= num9)) ? ((num8 >= num9) ? new Vector3i(x, y, num12) : new Vector3i(x, num11, z)) : new Vector3i(num10, y, z));
        }

        public Vector3i NormalizedAndScaledToVerticalAxis(float scale)
        {
            if (scale == 0f)
            {
                return zero;
            }
            Vector3i vectori = this;
            if (this.y == this.z)
            {
                vectori += new Vector3i(1, -1, 0);
            }
            float num = (Mathf.Abs((int) vectori.x) + Mathf.Abs((int) vectori.y)) + Mathf.Abs((int) vectori.z);
            scale = (scale * num) / ((float) Mathf.Abs((int) (vectori.y - vectori.z)));
            float f = (scale * vectori.y) / num;
            float num3 = (scale * vectori.z) / num;
            float single1 = (scale * vectori.x) / num;
            int x = Mathf.RoundToInt(single1);
            int y = Mathf.RoundToInt(f);
            int z = Mathf.RoundToInt(num3);
            int num10 = -y - z;
            int num11 = -x - z;
            int num12 = -x - y;
            float num7 = Mathf.Abs((float) ((-f - num3) - num10));
            float num8 = Mathf.Abs((float) ((-single1 - num3) - num11));
            float num9 = Mathf.Abs((float) ((-single1 - f) - num12));
            return (((num7 >= num8) || (num7 >= num9)) ? ((num8 >= num9) ? new Vector3i(x, y, num12) : new Vector3i(x, num11, z)) : new Vector3i(num10, y, z));
        }

        public static Vector3i Min(Vector3i a, Vector3i b)
        {
            return new Vector3i(Mathf.Min((int) a.x, (int) b.x), Mathf.Min((int) a.y, (int) b.y), Mathf.Min((int) a.z, (int) b.z));
        }

        public static Vector3i Max(Vector3i a, Vector3i b)
        {
            return new Vector3i(Mathf.Max((int) a.x, (int) b.x), Mathf.Max((int) a.y, (int) b.y), Mathf.Max((int) a.z, (int) b.z));
        }

        public static bool operator ==(Vector3i a, Vector3i b)
        {
            return ((a.x == b.x) && ((a.y == b.y) && (a.z == b.z)));
        }

        public static bool operator !=(Vector3i a, Vector3i b)
        {
            return ((a.x != b.x) || ((a.y != b.y) || (a.z != b.z)));
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
            int num = Mathf.Abs((int) a.x);
            int num2 = Mathf.Abs((int) a.y);
            int num3 = Mathf.Abs((int) a.z);
            int x = a.x / b;
            int y = a.y / b;
            int z = a.z / b;
            return (((num <= num2) || (num <= num3)) ? ((num2 <= num3) ? new Vector3i(x, y, -x - y) : new Vector3i(x, -x - z, z)) : new Vector3i(-y - z, y, z));
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3i c1, Vector3 c2)
        {
            return new Vector3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
        }

        public static Vector3 operator -(Vector3 c1, Vector3i c2)
        {
            return new Vector3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
        }

        public static Vector3i operator +(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator +(Vector3i c1, Vector3 c2)
        {
            return new Vector3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
        }

        public static Vector3 operator +(Vector3 c1, Vector3i c2)
        {
            return new Vector3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
        }

        public static Vector3 operator *(Vector3i a, float b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator *(float b, Vector3i a)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator /(Vector3i a, float b)
        {
            return new Vector3(((float) a.x) / b, ((float) a.y) / b, ((float) a.z) / b);
        }

        public int SqMagnitude()
        {
            return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }

        public static Vector3i Convert(Vector3 floatVector)
        {
            return Convert(floatVector.x, floatVector.y, floatVector.z);
        }

        public static Vector3i Convert(float x, float y, float z)
        {
            return new Vector3i(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(z));
        }

        public static unsafe Vector3i WrapByWidth(Vector3i pos, int width)
        {
            if ((width & 1) > 0)
            {
                Debug.LogError("odd width not implemented");
            }
            int num = width / 2;
            if (pos.x <= -num)
            {
                short num2 = (short) num;
                short* numPtr1 = &pos.x;
                numPtr1[0] = (short) (numPtr1[0] + ((short) width));
                short* numPtr2 = &pos.y;
                numPtr2[0] = (short) (numPtr2[0] - num2);
                short* numPtr3 = &pos.z;
                numPtr3[0] = (short) (numPtr3[0] - num2);
                return pos;
            }
            if (pos.x > num)
            {
                short num3 = (short) num;
                short* numPtr4 = &pos.x;
                numPtr4[0] = (short) (numPtr4[0] - ((short) width));
                short* numPtr5 = &pos.y;
                numPtr5[0] = (short) (numPtr5[0] + num3);
                short* numPtr6 = &pos.z;
                numPtr6[0] = (short) (numPtr6[0] + num3);
            }
            return pos;
        }

        static Vector3i()
        {
            zero = new Vector3i(0, 0, 0);
            one = new Vector3i(1, 1, 1);
            forward = new Vector3i(0, 0, 1);
            back = new Vector3i(0, 0, -1);
            up = new Vector3i(0, 1, 0);
            down = new Vector3i(0, -1, 0);
            left = new Vector3i(-1, 0, 0);
            right = new Vector3i(1, 0, 0);
            invalid = new Vector3i(-1, -1, -1);
            directions = new Vector3i[] { left, right, back, forward, down, up };
        }
    }
}

