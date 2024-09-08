using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public interface IEntity
    {
        int GetID();

        void SetID(int id);
    }
}
