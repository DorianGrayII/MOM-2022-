using System;
using ProtoBuf;
using UnityEngine;

namespace MHUtils
{
    [Serializable]
    [ProtoContract]
    public struct Vector2i
    {
        [ProtoMember(1)]
        public int x;

        [ProtoMember(2)]
        public int y;

        public static readonly Vector2i zero = new Vector2i(0, 0);

        public static readonly Vector2i one = new Vector2i(1, 1);

        public static readonly Vector2i up = new Vector2i(0, 1);

        public static readonly Vector2i down = new Vector2i(0, -1);

        public static readonly Vector2i left = new Vector2i(-1, 0);

        public static readonly Vector2i right = new Vector2i(1, 0);

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ (this.y.GetHashCode() << 2);
        }

        public override bool Equals(object other)
        {
            if (!(other is Vector2i vector2i))
            {
                return false;
            }
            if (this.x == vector2i.x)
            {
                return this.y == vector2i.y;
            }
            return false;
        }

        public override string ToString()
        {
            return "Vector2i(" + this.x + ", " + this.y + ")";
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
            if (a.x == b.x)
            {
                return a.y == b.y;
            }
            return false;
        }

        public static bool operator !=(Vector2i a, Vector2i b)
        {
            if (a.x == b.x)
            {
                return a.y != b.y;
            }
            return true;
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
            return new Vector2i(Mathf.RoundToInt((float)a.x * b), Mathf.RoundToInt((float)a.y * b));
        }

        public static Vector2i operator *(float a, Vector2i b)
        {
            return new Vector2i(Mathf.RoundToInt((float)b.x * a), Mathf.RoundToInt((float)b.y * a));
        }

        public int SqMagnitude()
        {
            return this.x * this.x + this.y * this.y;
        }

        public static Vector2i Convert(Vector2 floatVector)
        {
            return Vector2i.Convert(floatVector.x, floatVector.y);
        }

        public static Vector2i Convert(float x, float y)
        {
            return new Vector2i(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
        }
    }
}
