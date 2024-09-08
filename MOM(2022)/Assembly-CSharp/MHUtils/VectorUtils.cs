// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.VectorUtils
using System;
using UnityEngine;

public static class VectorUtils
{
    public static Vector2 Vector3To2D(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 Vector2To3D(Vector2 v)
    {
        return new Vector3(v.x, 0f, v.y);
    }

    public static void RotateVector2(ref Vector2 v, float degrees)
    {
        float num = Mathf.Sin(degrees * ((float)Math.PI / 180f));
        float num2 = Mathf.Cos(degrees * ((float)Math.PI / 180f));
        float x = v.x;
        float y = v.y;
        v.x = num2 * x - num * y;
        v.y = num * x + num2 * y;
    }

    public static Vector3 InvertedScale(Vector3 lossyScale, Vector3 localScale)
    {
        return new Vector3((localScale.x == 0f) ? 0f : (lossyScale.x / localScale.x), (localScale.y == 0f) ? 0f : (lossyScale.y / localScale.y), (localScale.z == 0f) ? 0f : (lossyScale.z / localScale.z));
    }

    public static Vector3 GetInfluenceFromTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 position)
    {
        Vector3 lhs = c - b;
        Vector3 lhs2 = a - c;
        Vector3 lhs3 = b - a;
        Vector3 rhs = position - a;
        Vector3 rhs2 = position - b;
        Vector3 rhs3 = position - c;
        float magnitude = Vector3.Cross(lhs, rhs2).magnitude;
        float magnitude2 = Vector3.Cross(lhs3, rhs).magnitude;
        float magnitude3 = Vector3.Cross(lhs2, rhs3).magnitude;
        float num = magnitude + magnitude3 + magnitude2;
        return new Vector3(magnitude / num, magnitude3 / num, magnitude2 / num);
    }

    public static bool BetweenXY(this Vector2 vector, float value)
    {
        if (vector.x >= value)
        {
            return vector.y <= value;
        }
        return false;
    }
}
