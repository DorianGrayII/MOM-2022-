namespace MOM
{
    using DBDef;
    using DBEnum;
    using ProtoBuf;
    using System;

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

        public static MOM.Unit ConvertDeadHeroToUnit(DeadHero deadHero)
        {
            MOM.Unit owner = MOM.Unit.CreateFrom((Subrace) deadHero.dbSource, false);
            owner.customName = deadHero.name;
            owner.xp = deadHero.xp;
            owner.canGainXP = deadHero.canGainXP;
            owner.canNaturalHeal = deadHero.canNaturalHeal;
            if (deadHero.attributes != null)
            {
                owner.attributes = deadHero.attributes.Clone(owner);
            }
            if (deadHero.skillManager != null)
            {
                ISkillableExtension.CopySkillManagerFrom(owner, deadHero.skillManager, null);
            }
            owner.currentFigureHP = owner.GetAttributes().GetFinal(TAG.HIT_POINTS).ToInt();
            return owner;
        }

        public static DeadHero Create(MOM.Unit u)
        {
            DeadHero hero1 = new DeadHero();
            hero1.name = u.GetName();
            hero1.xp = u.xp;
            hero1.dbSource = u.dbSource;
            hero1.canGainXP = u.canGainXP;
            hero1.canNaturalHeal = u.canNaturalHeal;
            hero1.skillManager = u.GetSkillManager();
            hero1.attributes = u.attributes;
            return hero1;
        }
    }
}

