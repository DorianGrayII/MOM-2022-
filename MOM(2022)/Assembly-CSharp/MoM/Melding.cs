namespace MOM
{
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class Melding
    {
        [ProtoMember(1)]
        public int meldOwner;
        [ProtoMember(2)]
        public int strength;
    }
}

