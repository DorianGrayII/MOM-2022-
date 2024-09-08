using DBDef;
using MHUtils;
using ProtoBuf;

namespace MOM
{
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

        public void ConsiderWalls(Battle b)
        {
            if (b != null && HexCoordinates.HexDistance(this.source.GetPosition(), this.destination.GetPosition()) > 1 && b.battleWalls != null)
            {
                this.isThroughWall = b.AttactThroughWall(this.source.GetPosition(), this.destination.GetPosition());
            }
        }

        public int GetWallDefenceModifier()
        {
            if (!this.isThroughWall)
            {
                return 0;
            }
            return 3;
        }

        public void ProduceDamages(MHRandom random)
        {
            this.dmg = ScriptLibrary.Call(this.skillScript.activatorMain, this.source, this.destination, this.skill, this.skillScript, this.attackStack.battle, this.dmg, random, null) as int[];
        }

        public void ApplyDamages(MHRandom random)
        {
            ScriptLibrary.Call(this.skillScript.activatorSecondary, this.source, this.destination, this.skill, this.skillScript, this.attackStack.battle, this.dmg, random, this);
        }
    }
}
