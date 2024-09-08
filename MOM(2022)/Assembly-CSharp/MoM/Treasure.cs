using DBDef;
using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class Treasure
    {
        [ProtoMember(1)]
        public int[] goldMana;

        [ProtoMember(2)]
        public int[] artefacts;

        [ProtoMember(3)]
        public bool[] heroSpellSpecial;

        [ProtoMember(4)]
        public DBReference<Race> guardianRealm;
    }
}
