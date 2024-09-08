using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MOM.Adventures
{
    public class LogicRequirementGroup
    {
        [XmlElement]
        [DefaultValue(null)]
        public List<LogicOptionalGroup> options;
    }
}
