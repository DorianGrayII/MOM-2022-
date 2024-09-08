using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class Melding
    {
        [ProtoMember(1)]
        public int meldOwner;

        [ProtoMember(2)]
        public int strength;
    }
}
