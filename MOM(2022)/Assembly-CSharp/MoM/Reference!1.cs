// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Reference<T>
using MOM;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class Reference<T> where T : Entity
{
    [ProtoIgnore]
    private Entity reference;

    [ProtoMember(1)]
    public int ID;

    public Entity GetEntity()
    {
        if (this.reference == null)
        {
            Entity entity = EntityManager.GetEntity(this.ID);
            if (entity != null)
            {
                this.reference = entity;
            }
        }
        return this.reference;
    }

    public T Get()
    {
        return this.GetEntity() as T;
    }

    public static implicit operator T(Reference<T> d)
    {
        if (d == null)
        {
            return null;
        }
        return d.Get();
    }

    public static implicit operator Reference<T>(T d)
    {
        if (d == null)
        {
            return null;
        }
        Reference<T> reference = new Reference<T>();
        if (d == null)
        {
            Debug.LogError("Invalid type of " + d);
            return null;
        }
        reference.ID = d.GetID();
        if (reference.ID == 0)
        {
            Debug.LogError("0 reference is invalid for " + d);
        }
        reference.reference = d;
        return reference;
    }

    public override bool Equals(object obj)
    {
        return Reference<T>.Equals2(this, obj);
    }

    public static bool Equals2(object a, object b)
    {
        bool flag = object.Equals(a, null);
        bool flag2 = object.Equals(b, null);
        if (!flag)
        {
            if (a is Reference<T>)
            {
                flag = (a as Reference<T>).Get() == null;
            }
            if (a is Reference)
            {
                flag = (a as Reference).GetEntity() == null;
            }
        }
        if (!flag2)
        {
            if (b is Reference<T>)
            {
                flag2 = (b as Reference<T>).Get() == null;
            }
            if (b is Reference)
            {
                flag2 = (b as Reference).GetEntity() == null;
            }
        }
        if (flag && flag2)
        {
            return true;
        }
        if (flag != flag2)
        {
            return false;
        }
        Entity entity = null;
        Entity entity2 = null;
        if (a is Reference<T>)
        {
            entity = (a as Reference<T>).Get();
        }
        else if (a is Reference)
        {
            entity = (a as Reference).GetEntity();
        }
        else
        {
            if (!(a is Entity))
            {
                return false;
            }
            entity = a as Entity;
        }
        if (b is Reference<T>)
        {
            entity2 = (b as Reference<T>).Get();
        }
        else if (b is Reference)
        {
            entity2 = (b as Reference).GetEntity();
        }
        else
        {
            if (!(b is Entity))
            {
                return false;
            }
            entity2 = b as Entity;
        }
        return object.Equals(entity, entity2);
    }

    public static bool operator ==(Reference<T> a, object b)
    {
        return Reference<T>.Equals2(a, b);
    }

    public static bool operator !=(Reference<T> a, object b)
    {
        return !Reference<T>.Equals2(a, b);
    }

    public override int GetHashCode()
    {
        return this.Get().GetHashCode();
    }

    public override string ToString()
    {
        return this.ID.ToString();
    }
}
