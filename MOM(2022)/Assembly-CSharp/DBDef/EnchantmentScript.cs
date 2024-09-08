namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("ENCHANTMENT_SCRIPT", "")]
    public class EnchantmentScript : DBClass
    {
        public static string abbreviation = "";
        [Prototype("TriggerType", true)]
        public EEnchantmentType triggerType;
        [Prototype("StringData", false)]
        public string stringData;
        [Prototype("FIntData", false)]
        public FInt fIntData;
        [Prototype("Script", false)]
        public string script;
        [Prototype("DamagePool", false)]
        public ESkillDamagePool damagePool;
        [Prototype("BattleAttackEffect", false)]
        public ESkillBattleAttackEffect battleAttackEffect;
        [Prototype("Tag", false)]
        public Tag tag;

        public static explicit operator EnchantmentScript(Enum e)
        {
            return DataBase.Get<EnchantmentScript>(e, false);
        }

        public static explicit operator EnchantmentScript(string e)
        {
            return DataBase.Get<EnchantmentScript>(e, true);
        }
    }
}

