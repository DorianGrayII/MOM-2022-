using System;
using MHUtils;

namespace DBDef
{
    [ClassPrototype("SUBRACE_AUDIO", "")]
    public class SubraceAudio : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Move", false)]
        public string move;

        [Prototype("AttackMelee", false)]
        public string attackMelee;

        [Prototype("AttackRanged", false)]
        public string attackRanged;

        [Prototype("AttackRangedHit", false)]
        public string attackRangedHit;

        [Prototype("GetHit", false)]
        public string getHit;

        [Prototype("Die", false)]
        public string die;

        [Prototype("Build", false)]
        public string build;

        public static explicit operator SubraceAudio(Enum e)
        {
            return DataBase.Get<SubraceAudio>(e);
        }

        public static explicit operator SubraceAudio(string e)
        {
            return DataBase.Get<SubraceAudio>(e, reportMissing: true);
        }
    }
}
