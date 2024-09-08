using ProtoBuf;

namespace MOM
{
    [ProtoInclude(100, typeof(BaseUnit))]
    [ProtoInclude(101, typeof(GameManager))]
    [ProtoInclude(102, typeof(Group))]
    [ProtoInclude(103, typeof(Location))]
    [ProtoInclude(104, typeof(PlayerWizard))]
    [ProtoContract]
    public class Entity
    {
        public virtual int GetID()
        {
            return 0;
        }

        public virtual void SetID(int id)
        {
        }
    }
}
