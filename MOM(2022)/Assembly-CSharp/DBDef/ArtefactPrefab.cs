// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.ArtefactPrefab
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;

[ClassPrototype("ARTEFACT_PREFAB", "")]
public class ArtefactPrefab : DBClass, IDescriptionInfoType
{
    public static string abbreviation = "";

    [Prototype("DescriptionInfo", true)]
    public DescriptionInfo descriptionInfo;

    [Prototype("Cost", true)]
    public int cost;

    [Prototype("EType", true)]
    public EEquipmentType eType;

    [Prototype("AlternativeGraphic", false)]
    public ArtefactGraphic[] alternativeGraphic;

    public DescriptionInfo GetDescriptionInfo()
    {
        return this.descriptionInfo;
    }

    public static explicit operator ArtefactPrefab(Enum e)
    {
        return DataBase.Get<ArtefactPrefab>(e);
    }

    public static explicit operator ArtefactPrefab(string e)
    {
        return DataBase.Get<ArtefactPrefab>(e, reportMissing: true);
    }

    public void Set_alternativeGraphic(List<object> list)
    {
        if (list == null || list.Count == 0)
        {
            return;
        }
        this.alternativeGraphic = new ArtefactGraphic[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            if (!(list[i] is ArtefactGraphic))
            {
                Debug.LogError("alternativeGraphic of type ArtefactGraphic received invalid type from array! " + list[i]);
            }
            this.alternativeGraphic[i] = list[i] as ArtefactGraphic;
        }
    }
}
