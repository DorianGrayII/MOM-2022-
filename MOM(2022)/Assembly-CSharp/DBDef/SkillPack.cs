using System;
using System.Collections.Generic;
using MHUtils;
using UnityEngine;

namespace DBDef
{
    [ClassPrototype("SKILL_PACK", "")]
    public class SkillPack : DBClass
    {
        public static string abbreviation = "";

        [Prototype("Skill", false)]
        public Skill[] skills;

        public static explicit operator SkillPack(Enum e)
        {
            return DataBase.Get<SkillPack>(e);
        }

        public static explicit operator SkillPack(string e)
        {
            return DataBase.Get<SkillPack>(e, reportMissing: true);
        }

        public void Set_skills(List<object> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            this.skills = new Skill[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i] is Skill))
                {
                    Debug.LogError("skills of type Skill received invalid type from array! " + list[i]);
                }
                this.skills[i] = list[i] as Skill;
            }
        }
    }
}
