using DBDef;
using MOM;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[Extension]
public static class ISkillableExtension
{
    [Extension]
    public static void AddSkill(ISkillable obj, Skill skill)
    {
        if (!GetSkills(obj).Contains(skill) || skill.stackable)
        {
            obj.GetSkillManager().Add(skill);
            if (skill.applicationScript != null)
            {
                object[] parameters = new object[] { obj, skill, skill.applicationScript };
                ScriptLibrary.Call(skill.applicationScript.activatorMain, parameters);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
        }
    }

    [Extension]
    public static SkillManager CopySkillManager(ISkillable obj, ISkillable newOwner)
    {
        return obj.GetSkillManager().CopySkillManager(newOwner);
    }

    [Extension]
    public static void CopySkillManagerFrom(ISkillable obj, ISkillable source, List<Skill> exceptions)
    {
        ResetSkillManager(obj);
        obj.GetSkillManager().CopySkillManagerFrom(source, exceptions);
    }

    [Extension]
    public static void CopySkillManagerFrom(ISkillable obj, SkillManager source, List<Skill> exceptions)
    {
        ResetSkillManager(obj);
        obj.GetSkillManager().CopySkillManagerFrom(source, exceptions);
    }

    [Extension]
    public static List<DBReference<Skill>> GetSkills(ISkillable obj)
    {
        return obj.GetSkillManager().GetSkills();
    }

    [Extension]
    public static List<SkillScript> GetSkillsByType(ISkillable obj, ESkillType eType, bool canReturnNull)
    {
        return obj.GetSkillManager().GetSkillsByType(eType, canReturnNull);
    }

    [Extension]
    public static List<Skill> GetSkillsConverted(ISkillable obj)
    {
        List<Skill> skills = new List<Skill>(GetSkills(obj).Count);
        GetSkills(obj).ForEach(delegate (DBReference<Skill> o) {
            skills.Add(o.Get());
        });
        return skills;
    }

    [Extension]
    public static void RemoveSkill(ISkillable obj, Skill skill)
    {
        obj.GetSkillManager().Remove(skill);
        if (skill.removalScript != null)
        {
            object[] parameters = new object[] { obj, skill, skill.removalScript };
            ScriptLibrary.Call(skill.removalScript.activatorMain, parameters);
        }
        if (obj is IAttributable)
        {
            (obj as IAttributable).GetAttributes().SetDirty();
        }
    }

    [Extension]
    public static void ResetSkillManager(ISkillable obj)
    {
        List<DBReference<Skill>> skills = GetSkills(obj);
        for (int i = skills.Count - 1; i >= 0; i--)
        {
            RemoveSkill(obj, skills[i]);
        }
        obj.GetSkillManager().ResetSkillManager();
    }

    [Extension]
    public static void TriggerSkillScripts(ISkillable obj, ESkillType type)
    {
        obj.GetSkillManager().TriggerScripts(type, null, obj);
    }
}

