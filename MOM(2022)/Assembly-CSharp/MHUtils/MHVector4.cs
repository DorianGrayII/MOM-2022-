// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.MHVector4
using MHUtils;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public struct MHVector4
{
    [ProtoMember(1)]
    public float x;

    [ProtoMember(2)]
    public float y;

    [ProtoMember(3)]
    public float z;

    [ProtoMember(4)]
    public float w;

    public MHVector4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public static implicit operator MHVector4(Vector4 d)
    {
        return new MHVector4(d.x, d.y, d.z, d.w);
    }

    public static implicit operator Vector4(MHVector4 d)
    {
        return new Vector4(d.x, d.y, d.z, d.w);
    }

    public Vector4 GetVector()
    {
        return new Vector4(this.x, this.y, this.z, this.w);
    }

    public static bool operator ==(MHVector4 a, MHVector4 b)
    {
        if (a.x == b.x && a.y == b.y && a.z == b.z)
        {
            return a.w == b.w;
        }
        return false;
    }

    public static bool operator !=(MHVector4 a, MHVector4 b)
    {
        return !(a == b);
    }

    public static MHVector4 operator -(MHVector4 a, MHVector4 b)
    {
        return new MHVector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    public static MHVector4 operator +(MHVector4 a, MHVector4 b)
    {
        return new MHVector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static MHVector4 operator *(MHVector4 a, MHVector4 b)
    {
        return new MHVector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
    }

    public static MHVector4 operator /(MHVector4 a, float b)
    {
        return new MHVector4(a.x / b, a.y / b, a.z / b, a.w / b);
    }

    public Color GetColor()
    {
        return new Color(this.x, this.y, this.z, this.w);
    }
}
