using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    [ProtoInclude(1000, typeof(BaseUnit))]
    [ProtoInclude(1002, typeof(BattlePlayer))]
    [ProtoInclude(1003, typeof(GameManager))]
    [ProtoInclude(1004, typeof(Location))]
    [ProtoInclude(1005, typeof(PlayerWizard))]
    public interface IEnchantable
    {
        EnchantmentManager GetEnchantmentManager();

        PlayerWizard GetWizardOwner();

        void FinishedIteratingEnchantments();

        string GetName();
    }
}
