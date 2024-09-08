// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.DBReference<T>
using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;

[ProtoContract]
public class DBReference<T> : DBRefBase where T : DBClass
{
    [ProtoIgnore]
    private T reference;

    [ProtoMember(1)]
    public string dbName;

    public override object GetObject()
    {
        return this.Get();
    }

    public T Get()
    {
        if (this.reference == null)
        {
            this.reference = DataBase.Get<T>(this.dbName, reportMissing: true);
        }
        return this.reference;
    }

    public T GetNoWarnig()
    {
        if (this.reference == null)
        {
            this.reference = DataBase.Get<T>(this.dbName, reportMissing: false);
        }
        return this.reference;
    }

    public static implicit operator T(DBReference<T> d)
    {
        if (d == null)
        {
            return null;
        }
        return d.Get();
    }

    public static implicit operator DBReference<T>(T d)
    {
        if (d == null)
        {
            return null;
        }
        return new DBReference<T>
        {
            dbName = d.dbName,
            reference = d
        };
    }

    public override bool Equals(object obj)
    {
        return DBReference<T>.Equals2(this, obj);
    }

    public static bool Equals2(object a, object b)
    {
        bool flag = object.Equals(a, null);
        bool flag2 = object.Equals(b, null);
        if (flag && flag2)
        {
            return true;
        }
        if (flag != flag2)
        {
            return false;
        }
        T val = null;
        T val2 = null;
        if (a is DBReference<T>)
        {
            val = (a as DBReference<T>).Get();
        }
        else
        {
            if (!(a is T))
            {
                return false;
            }
            val = a as T;
        }
        if (b is DBReference<T>)
        {
            val2 = (b as DBReference<T>).Get();
        }
        else
        {
            if (!(b is T))
            {
                return false;
            }
            val2 = b as T;
        }
        return object.Equals(val, val2);
    }

    public static bool operator ==(DBReference<T> a, object b)
    {
        return DBReference<T>.Equals2(a, b);
    }

    public static bool operator !=(DBReference<T> a, object b)
    {
        return !DBReference<T>.Equals2(a, b);
    }

    public override int GetHashCode()
    {
        return this.Get().GetHashCode();
    }

    public override string ToString()
    {
        if (this.dbName == null)
        {
            return base.ToString();
        }
        return this.dbName;
    }
}
