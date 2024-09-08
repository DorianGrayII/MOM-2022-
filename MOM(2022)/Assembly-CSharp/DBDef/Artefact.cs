// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.Artefact
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;

[ClassPrototype("ARTEFACT", "")]
public class Artefact : DBClass, IDescriptionInfoType
{
    public static string abbreviation = "";

    [Prototype("DescriptionInfo", true)]
    public DescriptionInfo descriptionInfo;

    [Prototype("EType", true)]
    public EEquipmentType eType;

    [Prototype("Power", false)]
    public ArtefactPower[] power;

    public DescriptionInfo GetDescriptionInfo()
    {
        return this.descriptionInfo;
    }

    public static explicit operator Artefact(Enum e)
    {
        return DataBase.Get<Artefact>(e);
    }

    public static explicit operator Artefact(string e)
    {
        return DataBase.Get<Artefact>(e, reportMissing: true);
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
}
