using ProtoBuf;
using UnityEngine;

namespace MHUtils
{
[ProtoContract]
public struct MHVector3
{
    [ProtoMember(1)]
    public float x;

    [ProtoMember(2)]
    public float y;

    [ProtoMember(3)]
    public float z;

    public MHVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator MHVector3(Vector3 d)
    {
        return new MHVector3(d.x, d.y, d.z);
    }

    public static implicit operator Vector3(MHVector3 d)
    {
        return new Vector3(d.x, d.y, d.z);
    }

    public Vector3 GetVector()
    {
        return new Vector3(this.x, this.y, this.z);
    }

    public static bool operator ==(MHVector3 a, MHVector3 b)
    {
        if (a.x == b.x && a.y == b.y)
        {
            return a.z == b.z;
        }
        return false;
    }

    public static bool operator !=(MHVector3 a, MHVector3 b)
    {
        return !(a == b);
    }

    public static MHVector3 operator -(MHVector3 a, MHVector3 b)
    {
        return new MHVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static MHVector3 operator -(MHVector3 a, Vector3 b)
    {
        return new MHVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static MHVector3 operator -(Vector3 a, MHVector3 b)
    {
        return new MHVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static MHVector3 operator +(MHVector3 a, MHVector3 b)
    {
        return new MHVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static MHVector3 operator +(MHVector3 a, Vector3 b)
    {
        return new MHVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static MHVector3 operator +(Vector3 a, MHVector3 b)
    {
        return new MHVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static MHVector3 operator +(MHVector3 a, int b)
    {
        return new MHVector3(a.x + (float)b, a.y + (float)b, a.z + (float)b);
    }

    public static MHVector3 operator +(int a, MHVector3 b)
    {
        return new MHVector3((float)a + b.x, (float)a + b.y, (float)a + b.z);
    }

    public static MHVector3 operator *(MHVector3 a, MHVector3 b)
    {
        return new MHVector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static MHVector3 operator *(MHVector3 a, int b)
    {
        return new MHVector3(a.x * (float)b, a.y * (float)b, a.z * (float)b);
    }

    public static MHVector3 operator *(int a, MHVector3 b)
    {
        return new MHVector3((float)a * b.x, (float)a * b.y, (float)a * b.z);
    }

    public static MHVector3 operator /(MHVector3 a, float b)
    {
        return new MHVector3(a.x / b, a.y / b, a.z / b);
    }

    public float SqrMagnitude()
    {
        return ((Vector3)this).sqrMagnitude;
    }

    public float Magnitude()
    {
        return ((Vector3)this).magnitude;
    }

    public void Normalze()
    {
        Vector3 vector = this.Normalized();
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }

    public Vector3 Normalized()
    {
        return ((Vector3)this).normalized;
    }

        public override bool Equals(object obj)
        {
            if (!(obj is MHVector3))
            {
                return false;
            }

            var vector = (MHVector3)obj;
            return x == vector.x &&
                   y == vector.y &&
                   z == vector.z;
        }

        public override int GetHashCode()
        {
            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }
    }
}
