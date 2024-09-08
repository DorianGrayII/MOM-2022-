// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.ArtefactPowerSet
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;

[ClassPrototype("ARTEFACT_POWER_SET", "")]
public class ArtefactPowerSet : DBClass
{
    public static string abbreviation = "";

    [Prototype("Power", true)]
    public ArtefactPower[] power;

    [Prototype("RequiredTag", false)]
    public CountedTag[] requiredTag;

    public static explicit operator ArtefactPowerSet(Enum e)
    {
        return DataBase.Get<ArtefactPowerSet>(e);
    }

    public static explicit operator ArtefactPowerSet(string e)
    {
        return DataBase.Get<ArtefactPowerSet>(e, reportMissing: true);
    }

    public void Set_power(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.power = new ArtefactPower[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is ArtefactPower))
            {
                Debug.LogError("power of type ArtefactPower received invalid type from array! " + list[i]);
            }
            this.power[i] = list[i] as ArtefactPower;
        }
    }

    public void Set_requiredTag(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.requiredTag = new CountedTag[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is CountedTag))
            {
                Debug.LogError("requiredTag of type CountedTag received invalid type from array! " + list[i]);
            }
            this.requiredTag[i] = list[i] as CountedTag;
        }
    }
}
