// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.MHVector2
using MHUtils;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public struct MHVector2
{
    [ProtoMember(1)]
    public float x;

    [ProtoMember(2)]
    public float y;

    public MHVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator MHVector2(Vector2 d)
    {
        return new MHVector2(d.x, d.y);
    }

    public static implicit operator Vector2(MHVector2 d)
    {
        return new Vector2(d.x, d.y);
    }

    public Vector2 GetVector()
    {
        return new Vector2(this.x, this.y);
    }

    public static bool operator ==(MHVector2 a, MHVector2 b)
    {
        if (a.x == b.x)
        {
            return a.y == b.y;
        }
        return false;
    }

    public static bool operator !=(MHVector2 a, MHVector2 b)
    {
        return !(a == b);
    }

    public static MHVector2 operator -(MHVector2 a, MHVector2 b)
    {
        return new MHVector2(a.x - b.x, a.y - b.y);
    }

    public static MHVector2 operator +(MHVector2 a, MHVector2 b)
    {
        return new MHVector2(a.x + b.x, a.y + b.y);
    }

    public static MHVector2 operator *(MHVector2 a, MHVector2 b)
    {
        return new MHVector2(a.x * b.x, a.y * b.y);
    }

    public static MHVector2 operator *(MHVector2 a, Vector2 b)
    {
        return new MHVector2(a.x * b.x, a.y * b.y);
    }

    public static MHVector2 operator *(Vector2 a, MHVector2 b)
    {
        return new MHVector2(a.x * b.x, a.y * b.y);
    }

    public static MHVector2 operator *(MHVector2 a, float b)
    {
        return new MHVector2(a.x * b, a.y * b);
    }

    public static MHVector2 operator *(float a, MHVector2 b)
    {
        return new MHVector2(a * b.x, a * b.y);
    }

    public static MHVector2 operator /(MHVector2 a, float b)
    {
        return new MHVector2(a.x / b, a.y / b);
    }
}
