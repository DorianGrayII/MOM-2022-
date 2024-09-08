using MHUtils;
using ProtoBuf;
using System;
using UnityEngine;

[ProtoContract]
public class MeshTriangle2D
{
    [ProtoMember(1)]
    public MHVector3 p1;
    [ProtoMember(2)]
    public MHVector3 p2;
    [ProtoMember(3)]
    public MHVector3 p3;
    [ProtoMember(4)]
    private MHVector2 uv1;
    [ProtoMember(5)]
    private MHVector2 uv2;
    [ProtoMember(6)]
    private MHVector2 uv3;
    [ProtoMember(7)]
    private MHVector3 p12;
    [ProtoMember(8)]
    private MHVector3 p23;
    [ProtoMember(9)]
    private MHVector3 p31;
    [ProtoMember(10)]
    private MHVector2 r12;
    [ProtoMember(11)]
    private MHVector2 r23;
    [ProtoMember(12)]
    private MHVector2 r31;
    [ProtoMember(13)]
    private MHVector3 circleCenter;
    [ProtoMember(14)]
    private float circleRadius;

    public MeshTriangle2D(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = new Vector3(p1.x, 0f, p1.z);
        this.p2 = new Vector3(p2.x, 0f, p2.z);
        this.p3 = new Vector3(p3.x, 0f, p3.z);
        this.p12 = p2 - p1;
        this.p23 = p3 - p2;
        this.p31 = p1 - p3;
        this.r12 = new Vector2(this.p12.z, -this.p12.x);
        this.r23 = new Vector2(this.p23.z, -this.p23.x);
        this.r31 = new Vector2(this.p31.z, -this.p31.x);
    }

    public static MeshTriangle2D Create(ValueTuple<Vector3, Vector2> p1, ValueTuple<Vector3, Vector2> p2, ValueTuple<Vector3, Vector2> p3)
    {
        MeshTriangle2D triangled1 = new MeshTriangle2D(p1.Item1, p2.Item1, p3.Item1);
        triangled1.uv1 = p1.Item2;
        triangled1.uv2 = p2.Item2;
        triangled1.uv3 = p3.Item2;
        return triangled1;
    }

    public Vector2 GetUV(Vector3 p)
    {
        Vector3 vertexInfluence = this.GetVertexInfluence(p);
        return (Vector2) (((this.uv1 * vertexInfluence.x) + (this.uv2 * vertexInfluence.y)) + (this.uv3 * vertexInfluence.z));
    }

    public Vector3 GetVertexInfluence(Vector3 p)
    {
        p.y = 0f;
        MHVector3 vector = p - this.p1;
        MHVector3 vector2 = p - this.p2;
        MHVector3 vector3 = p - this.p3;
        Vector3 vector4 = new Vector3();
        float num = Mathf.Abs(Vector3.Cross((Vector3) vector2, (Vector3) vector3).y);
        float num2 = Mathf.Abs(Vector3.Cross((Vector3) vector3, (Vector3) vector).y);
        float num3 = Mathf.Abs(Vector3.Cross((Vector3) vector, (Vector3) vector2).y);
        float num4 = (num + num2) + num3;
        if (num4 < Mathf.Epsilon)
        {
            return new Vector3();
        }
        vector4.x = num / num4;
        vector4.y = num2 / num4;
        vector4.z = num3 / num4;
        return vector4;
    }

    public bool IsWithin(Vector3 p)
    {
        p.y = 0f;
        Vector3 vector = p - this.p1;
        Vector3 vector2 = p - this.p2;
        Vector3 vector3 = p - this.p3;
        Vector2 lhs = new Vector2(vector2.x, vector2.z);
        Vector2 vector5 = new Vector2(vector3.x, vector3.z);
        float num = Vector2.Dot(new Vector2(vector.x, vector.z), (Vector2) this.r12);
        float num2 = Vector2.Dot(lhs, (Vector2) this.r23);
        float num3 = Vector2.Dot(vector5, (Vector2) this.r31);
        return (((num < 0f) || ((num2 < 0f) || (num3 < 0f))) ? ((num <= 0f) && ((num2 <= 0f) && (num3 <= 0f))) : true);
    }
}

