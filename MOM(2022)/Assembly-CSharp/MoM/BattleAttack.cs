namespace MOM
{
    using DBDef;
    using MHUtils;
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class BattleAttack
    {
        public FInt initiative;
        public int[] dmg;
        public BattleUnit source;
        public BattleUnit destination;
        public ESkillType type;
        public Skill skill;
        public SkillScript skillScript;
        public BattleAttackStack attackStack;
        public bool isPiercing;
        public bool isIllusion;
        public bool isFirstStrike;
        public bool isAntiFirstStrike;
        public bool isThroughWall;
        public int addonToIndex = -1;

        public void ApplyDamages(MHRandom random)
        {
            object[] parameters = new object[] { this.source, this.destination, this.skill, this.skillScript, this.attackStack.battle, this.dmg, random, this };
            ScriptLibrary.Call(this.skillScript.activatorSecondary, parameters);
        }

        public void ConsiderWalls(Battle b)
        {
            if ((b != null) && ((HexCoordinates.HexDistance(this.source.GetPosition(), this.destination.GetPosition()) > 1) && (b.battleWalls != null)))
            {
                this.isThroughWall = b.AttactThroughWall(this.source.GetPosition(), this.destination.GetPosition(), false);
            }
        }

        public int GetWallDefenceModifier()
        {
            return (this.isThroughWall ? 3 : 0);
        }

        public void ProduceDamages(MHRandom random)
        {
            object[] parameters = new object[8];
            parameters[0] = this.source;
            parameters[1] = this.destination;
            parameters[2] = this.skill;
            parameters[3] = this.skillScript;
            parameters[4] = this.attackStack.battle;
            parameters[5] = this.dmg;
            parameters[6] = random;
            this.dmg = ScriptLibrary.Call(this.skillScript.activatorMain, parameters) as int[];
        }
    }
}

