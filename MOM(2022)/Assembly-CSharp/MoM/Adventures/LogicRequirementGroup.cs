// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Adventures.LogicRequirementGroup
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using MOM.Adventures;

public class LogicRequirementGroup
{
    [XmlElement]
    [DefaultValue(null)]
    public List<LogicOptionalGroup> options;
}
