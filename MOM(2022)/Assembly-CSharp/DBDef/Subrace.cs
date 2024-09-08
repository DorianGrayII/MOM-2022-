// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.Subrace
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;

[ClassPrototype("SUBRACE", "")]
public class Subrace : DBClass, IDescriptionInfoType
{
    public static string abbreviation = "";

    [Prototype("DescriptionInfo", false)]
    public DescriptionInfo descriptionInfo;

    [Prototype("OptionalModel3dName", false)]
    public string model3d;

    [Prototype("Race", true)]
    public Race race;

    [Prototype("Marker", false)]
    public string marker;

    [Prototype("OriginalScaleValue", false)]
    public int originalScaleValue;

    [Prototype("Tags", false)]
    public CountedTag[] tags;

    [Prototype("NaturalHealing", false)]
    public bool naturalHealing;

    [Prototype("GainsXP", false)]
    public bool gainsXP;

    [Prototype("Unresurrectable", false)]
    public bool unresurrectable;

    [Prototype("Skills", false)]
    public Skill[] skills;

    [Prototype("SpellPack", false)]
    public SpellPack[] spellPack;

    [Prototype("Audio", false)]
    public SubraceAudio audio;

    [Prototype("Dlc", false)]
    public string dlc;

    [Prototype("StrategicValueOverride", false)]
    public int strategicValueOverride;

    [Prototype("FixedDeathDir", false)]
    public bool fixedDeathDir;

    [Prototype("Selfdestructing", false)]
    public bool selfdestructing;

    public DescriptionInfo GetDescriptionInfo()
    {
        return this.descriptionInfo;
    }

    public static explicit operator Subrace(Enum e)
    {
        return DataBase.Get<Subrace>(e);
    }

    public static explicit operator Subrace(string e)
    {
        return DataBase.Get<Subrace>(e, reportMissing: true);
    }

    public void Set_tags(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.tags = new CountedTag[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is CountedTag))
            {
                Debug.LogError("tags of type CountedTag received invalid type from array! " + list[i]);
            }
            this.tags[i] = list[i] as CountedTag;
        }
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

    public void Set_spellPack(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.spellPack = new SpellPack[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is SpellPack))
            {
                Debug.LogError("spellPack of type SpellPack received invalid type from array! " + list[i]);
            }
            this.spellPack[i] = list[i] as SpellPack;
        }
    }
}
