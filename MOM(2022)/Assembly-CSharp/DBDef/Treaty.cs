// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DBDef.Treaty
using System;
using DBDef;
using MHUtils;

[ClassPrototype("TREATY", "")]
public class Treaty : DBClass, IDescriptionInfoType
{
    public static string abbreviation = "";

    [Prototype("DescriptionInfo", true)]
    public DescriptionInfo descriptionInfo;

    [Prototype("AgreementRequired", false)]
    public bool agreementRequired;

    [Prototype("Length", false)]
    public int length;

    [Prototype("TreatyEvaluationScript", false)]
    public string treatyEvaluationScript;

    [Prototype("TreatyStartScript", false)]
    public string treatyStartScript;

    [Prototype("TreatyTurnScript", false)]
    public string treatyTurnScript;

    [Prototype("TreatyBreakScript", false)]
    public string treatyBreakScript;

    [Prototype("TreatyEndScript", false)]
    public string treatyEndScript;

    public DescriptionInfo GetDescriptionInfo()
    {
        return this.descriptionInfo;
    }

    public static explicit operator Treaty(Enum e)
    {
        return DataBase.Get<Treaty>(e);
    }

    public static explicit operator Treaty(string e)
    {
        return DataBase.Get<Treaty>(e, reportMissing: true);
    }
}
