namespace MOM.Adventures
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public class LogicOptionalGroup
    {
        public static List<string> signs;
        [XmlAttribute, DefaultValue((string) null)]
        public string sign = signs[0];
        [XmlAttribute, DefaultValue(0)]
        public int value;
        [XmlAttribute, DefaultValue((string) null)]
        public string typeData;

        static LogicOptionalGroup()
        {
            List<string> list1 = new List<string>();
            list1.Add(">=");
            list1.Add(">");
            list1.Add("=");
            list1.Add("<");
            list1.Add("<=");
            signs = list1;
        }
    }
}

