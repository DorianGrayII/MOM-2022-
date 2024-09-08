using DBDef;
using DBEnum;
using ProtoBuf;

namespace MOM
{
    [ProtoContract]
    public class DeadHero
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public int xp;

        [ProtoMember(3)]
        public DBReference<Subrace> dbSource;

        [ProtoMember(4)]
        public SkillManager skillManager;

        [ProtoMember(5)]
        public bool canNaturalHeal;

        [ProtoMember(7)]
        public bool canGainXP;

        [ProtoMember(8)]
        public Attributes attributes;

        public static DeadHero Create(Unit u)
        {
            return new DeadHero
            {
                name = u.GetName(),
                xp = u.xp,
                dbSource = u.dbSource,
                canGainXP = u.canGainXP,
                canNaturalHeal = u.canNaturalHeal,
                skillManager = u.GetSkillManager(),
                attributes = u.attributes
            };
        }

        public static Unit ConvertDeadHeroToUnit(DeadHero deadHero)
        {
            Unit unit = Unit.CreateFrom(deadHero.dbSource);
            unit.customName = deadHero.name;
            unit.xp = deadHero.xp;
            unit.canGainXP = deadHero.canGainXP;
            unit.canNaturalHeal = deadHero.canNaturalHeal;
            if (deadHero.attributes != null)
            {
                unit.attributes = deadHero.attributes.Clone(unit);
            }
            if (deadHero.skillManager != null)
            {
                unit.CopySkillManagerFrom(deadHero.skillManager);
            }
            unit.currentFigureHP = unit.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
            return unit;
        }
    }
}
