using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MOM.Adventures
{
    public class LogicOptionalGroup
    {
        public static List<string> signs = new List<string> { ">=", ">", "=", "<", "<=" };

        [XmlAttribute]
        [DefaultValue(null)]
        public string sign = LogicOptionalGroup.signs[0];

        [XmlAttribute]
        [DefaultValue(0)]
        public int value;

        [XmlAttribute]
        [DefaultValue(null)]
        public string typeData;
    }
}
