using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public class ModOrder
{
    [XmlAttribute]
    public string name;
    [XmlAttribute]
    public int order;
    [XmlAttribute]
    public bool active;
    [XmlIgnore]
    public string nameVersion;
    [XmlIgnore]
    public string version;

    public string GetNameVersion()
    {
        if (this.nameVersion == null)
        {
            this.UpdateVersion();
            this.nameVersion = this.name + " ver. " + this.version;
        }
        return this.nameVersion;
    }

    public string GetPath()
    {
        return ModManager.Get().GetPath(this);
    }

    public bool IsValid()
    {
        return string.IsNullOrEmpty(ModManager.Get().ValidateMod(this));
    }

    public void UpdateVersion()
    {
        if (this.version == null)
        {
            foreach (KeyValuePair<string, ModSettings> pair in ModManager.Get().GetModList())
            {
                if (pair.Value.name == this.name)
                {
                    string version = pair.Value.version;
                    if (pair.Value.version == null)
                    {
                        string local1 = pair.Value.version;
                        version = "";
                    }
                    this.version = version;
                    break;
                }
            }
        }
    }
}

