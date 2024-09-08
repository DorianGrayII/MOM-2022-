using System.Collections.Generic;
using System.Xml.Serialization;

public class ModOrderList
{
    [XmlElement]
    public List<ModOrder> order;
}
