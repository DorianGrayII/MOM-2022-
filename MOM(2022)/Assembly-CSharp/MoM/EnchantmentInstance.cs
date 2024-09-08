using DBDef;
using MHUtils;
using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class EnchantmentInstance : IEventDisplayName
    {
        [ProtoMember(3)]
        public string nameID;

        [ProtoMember(4)]
        public int countDown;

        [ProtoMember(5)]
        public DBReference<Enchantment> source;

        [ProtoMember(6)]
        public Reference owner;

        [ProtoMember(7)]
        public int upkeepMana;

        [ProtoMember(8)]
        public string parameters;

        [ProtoMember(9)]
        public Vector3i positionData;

        [ProtoMember(10)]
        public bool battleEnchantment;

        [ProtoMember(11)]
        public int dispelCost;

        [ProtoMember(12)]
        public int enchantmentHolder;

        [ProtoMember(13)]
        public bool buildingEnchantment;

        [ProtoMember(14)]
        public int intParametr;

        [ProtoIgnore]
        public EnchantmentManager manager;

        public EEnchantmentCategory GetEnchantmentType()
        {
            return this.source.Get().enchCategory;
        }

        public int GetEnchantmentCost(PlayerWizard wizard)
        {
            if (this.upkeepMana <= 0)
            {
                return 0;
            }
            return this.upkeepMana;
        }

        public string GetEventDisplayName()
        {
            return this.nameID;
        }
    }
}
