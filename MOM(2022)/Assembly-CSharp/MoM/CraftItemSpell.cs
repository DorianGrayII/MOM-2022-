using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class CraftItemSpell
    {
        [ProtoMember(1)]
        public int cost;

        [ProtoMember(2)]
        public Artefact artefact = new Artefact();
    }
}
