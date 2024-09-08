using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class ExtensionItemSpell
    {
        [ProtoMember(1)]
        public int extraMana;

        [ProtoMember(2)]
        public int extraSkill;

        [ProtoMember(3)]
        public int extraPower;

        public void Clear()
        {
            this.extraMana = 0;
            this.extraSkill = 0;
            this.extraPower = 0;
        }
    }
}
