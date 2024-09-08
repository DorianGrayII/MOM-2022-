namespace DBDef
{
    using MHUtils;
    using System;

    [ClassPrototype("SKILL_SCRIPT", "")]
    public class SkillScript : DBClass
    {
        public static string abbreviation = "";
        [Prototype("TriggerType", true)]
        public ESkillType triggerType;
        [Prototype("Priority", true)]
        public int priority;
        [Prototype("AllowMeleeVsFly", false)]
        public bool allowMeleeVsFly;
        [Prototype("TriggerScript", false)]
        public string trigger;
        [Prototype("ActivatorMainScript", false)]
        public string activatorMain;
        [Prototype("ActivatorSecondaryScript", false)]
        public string activatorSecondary;
        [Prototype("DamagePool", false)]
        public ESkillDamagePool damagePool;
        [Prototype("FIntParam", false)]
        public FInt fIntParam;
        [Prototype("StringData", false)]
        public string stringData;
        [Prototype("BattleAttackEffect", false)]
        public ESkillBattleAttackEffect battleAttackEffect;

        public static explicit operator SkillScript(Enum e)
        {
            return DataBase.Get<SkillScript>(e, false);
        }

        public static explicit operator SkillScript(string e)
        {
            return DataBase.Get<SkillScript>(e, true);
        }
    }
}

