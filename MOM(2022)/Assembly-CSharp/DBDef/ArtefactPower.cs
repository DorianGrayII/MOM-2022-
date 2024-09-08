// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.ArtefactPower
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;

[ClassPrototype("ARTEFACT_POWER", "")]
public class ArtefactPower : DBClass
{
    public static string abbreviation = "";

    [Prototype("Cost", true)]
    public int cost;

    [Prototype("Skill", true)]
    public Skill skill;

    [Prototype("ETypes", false)]
    public EEquipmentType[] eTypes;

    public static explicit operator ArtefactPower(Enum e)
    {
        return DataBase.Get<ArtefactPower>(e);
    }

    public static explicit operator ArtefactPower(string e)
    {
        return DataBase.Get<ArtefactPower>(e, reportMissing: true);
    }

    public void Set_eTypes(List<object> list)
    {
        if (list != null && list.Count != 0)
        {
            this.eTypes = new EEquipmentType[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                this.eTypes[i] = (EEquipmentType)list[i];
            }
        }
    }
}
