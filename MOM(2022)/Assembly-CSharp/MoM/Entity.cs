namespace MOM
{
    using ProtoBuf;
    using System;

    [ProtoInclude(100, typeof(BaseUnit)), ProtoInclude(0x65, typeof(GameManager)), ProtoInclude(0x66, typeof(Group)), ProtoInclude(0x67, typeof(Location)), ProtoInclude(0x68, typeof(PlayerWizard)), ProtoContract]
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

