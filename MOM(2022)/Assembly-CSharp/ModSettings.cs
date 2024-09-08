using System;
using System.Xml.Serialization;

public class ModSettings
{
    [XmlElement]
    public string prefix;
    [XmlElement]
    public string name;
    [XmlElement]
    public string title;
    [XmlElement]
    public string description;
    [XmlElement]
    public string author;
    [XmlElement]
    public string version;
    [XmlElement]
    public string icon;
    [XmlElement]
    public string[] prerequisites;
}

