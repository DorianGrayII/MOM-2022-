namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [ClassPrototype("SKILL_PACK", "")]
    public class SkillPack : DBClass
    {
        public static string abbreviation = "";
        [Prototype("Skill", false)]
        public Skill[] skills;

        public static explicit operator SkillPack(Enum e)
        {
            return DataBase.Get<SkillPack>(e, false);
        }

        public static explicit operator SkillPack(string e)
        {
            return DataBase.Get<SkillPack>(e, true);
        }

        public void Set_skills(List<object> list)
        {
            if ((list != null) && (list.Count != 0))
            {
                this.skills = new Skill[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    if (!(list[i] is Skill))
                    {
                        string text1;
                        object local1 = list[i];
                        if (local1 != null)
                        {
                            text1 = local1.ToString();
                        }
                        else
                        {
                            object local2 = local1;
                            text1 = null;
                        }
                        Debug.LogError("skills of type Skill received invalid type from array! " + text1);
                    }
                    this.skills[i] = list[i] as Skill;
                }
            }
        }
    }
}

