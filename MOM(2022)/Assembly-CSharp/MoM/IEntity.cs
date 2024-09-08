// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.IEntity
using ProtoBuf;

[ProtoContract]
public interface IEntity
{
    int GetID();

    void SetID(int id);
}
