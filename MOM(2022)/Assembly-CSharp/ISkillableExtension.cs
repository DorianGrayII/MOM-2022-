using System.Collections.Generic;
using DBDef;
using MOM;

public static class ISkillableExtension
{
    public static void AddSkill(this ISkillable obj, Skill skill)
    {
        if (!obj.GetSkills().Contains(skill) || skill.stackable)
        {
            obj.GetSkillManager().Add(skill);
            if (skill.applicationScript != null)
            {
                ScriptLibrary.Call(skill.applicationScript.activatorMain, obj, skill, skill.applicationScript);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
        }
    }

    public static void RemoveSkill(this ISkillable obj, Skill skill)
    {
        obj.GetSkillManager().Remove(skill);
        if (skill.removalScript != null)
        {
            ScriptLibrary.Call(skill.removalScript.activatorMain, obj, skill, skill.removalScript);
        }
        if (obj is IAttributable)
        {
            (obj as IAttributable).GetAttributes().SetDirty();
        }
    }

    public static List<SkillScript> GetSkillsByType(this ISkillable obj, ESkillType eType, bool canReturnNull = true)
    {
        return obj.GetSkillManager().GetSkillsByType(eType, canReturnNull);
    }

    public static List<DBReference<Skill>> GetSkills(this ISkillable obj)
    {
        return obj.GetSkillManager().GetSkills();
    }

    public static List<Skill> GetSkillsConverted(this ISkillable obj)
    {
        List<Skill> skills = new List<Skill>(obj.GetSkills().Count);
        obj.GetSkills().ForEach(delegate(DBReference<Skill> o)
        {
            skills.Add(o.Get());
        });
        return skills;
    }

    public static SkillManager CopySkillManager(this ISkillable obj, ISkillable newOwner)
    {
        return obj.GetSkillManager().CopySkillManager(newOwner);
    }

    public static void CopySkillManagerFrom(this ISkillable obj, ISkillable source, List<Skill> exceptions = null)
    {
        obj.ResetSkillManager();
        obj.GetSkillManager().CopySkillManagerFrom(source, exceptions);
    }

    public static void CopySkillManagerFrom(this ISkillable obj, SkillManager source, List<Skill> exceptions = null)
    {
        obj.ResetSkillManager();
        obj.GetSkillManager().CopySkillManagerFrom(source, exceptions);
    }

    public static void TriggerSkillScripts(this ISkillable obj, ESkillType type)
    {
        obj.GetSkillManager().TriggerScripts(type, null, obj);
    }

    public static void ResetSkillManager(this ISkillable obj)
    {
        List<DBReference<Skill>> skills = obj.GetSkills();
        for (int num = skills.Count - 1; num >= 0; num--)
        {
            obj.RemoveSkill(skills[num]);
        }
        obj.GetSkillManager().ResetSkillManager();
    }
}
