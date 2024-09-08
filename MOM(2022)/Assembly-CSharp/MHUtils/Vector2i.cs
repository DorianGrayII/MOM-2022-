namespace MHUtils
{
    using ProtoBuf;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential), ProtoContract]
    public struct Vector2i
    {
        [ProtoMember(1)]
        public int x;
        [ProtoMember(2)]
        public int y;
        public static readonly Vector2i zero;
        public static readonly Vector2i one;
        public static readonly Vector2i up;
        public static readonly Vector2i down;
        public static readonly Vector2i left;
        public static readonly Vector2i right;
        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            return (this.x.GetHashCode() ^ (this.y.GetHashCode() << 2));
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2i))
            {
                return false;
            }
            Vector2i vectori = (Vector2i) other;
            return ((this.x == vectori.x) && (this.y == vectori.y));
        }

        public override string ToString()
        {
            string[] textArray1 = new string[] { "Vector2i(", this.x.ToString(), ", ", this.y.ToString(), ")" };
            return string.Concat(textArray1);
        }

        public static Vector2i Min(Vector2i a, Vector2i b)
        {
            return new Vector2i(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
        }

        public static Vector2i Max(Vector2i a, Vector2i b)
        {
            return new Vector2i(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }

        public static bool operator ==(Vector2i a, Vector2i b)
        {
            return ((a.x == b.x) && (a.y == b.y));
        }

        public static bool operator !=(Vector2i a, Vector2i b)
        {
            return ((a.x != b.x) || (a.y != b.y));
        }

        public static Vector2i operator -(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.x - b.x, a.y - b.y);
        }

        public static Vector2i operator +(Vector2i a, Vector2i b)
        {
            return new Vector2i(a.x + b.x, a.y + b.y);
        }

        public static Vector2i operator *(Vector2i a, int b)
        {
            return new Vector2i(a.x * b, a.y * b);
        }

        public static Vector2i operator *(int a, Vector2i b)
        {
            return new Vector2i(b.x * a, b.y * a);
        }

        public static Vector2i operator *(Vector2i a, float b)
        {
            return new Vector2i(Mathf.RoundToInt(a.x * b), Mathf.RoundToInt(a.y * b));
        }

        public static Vector2i operator *(float a, Vector2i b)
        {
            return new Vector2i(Mathf.RoundToInt(b.x * a), Mathf.RoundToInt(b.y * a));
        }

        public int SqMagnitude()
        {
            return ((this.x * this.x) + (this.y * this.y));
        }

        public static Vector2i Convert(Vector2 floatVector)
        {
            return Convert(floatVector.x, floatVector.y);
        }

        public static Vector2i Convert(float x, float y)
        {
            return new Vector2i(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }

        static Vector2i()
        {
            zero = new Vector2i(0, 0);
            one = new Vector2i(1, 1);
            up = new Vector2i(0, 1);
            down = new Vector2i(0, -1);
            left = new Vector2i(-1, 0);
            right = new Vector2i(1, 0);
        }
    }
}

