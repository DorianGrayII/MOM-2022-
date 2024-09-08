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

    public void UpdateVersion()
    {
        if (this.version != null)
        {
            return;
        }
        foreach (KeyValuePair<string, ModSettings> mod in ModManager.Get().GetModList())
        {
            if (mod.Value.name == this.name)
            {
                this.version = mod.Value.version ?? "";
                break;
            }
        }
    }

    public bool IsValid()
    {
        return string.IsNullOrEmpty(ModManager.Get().ValidateMod(this));
    }

    public string GetPath()
    {
        return ModManager.Get().GetPath(this);
    }
}
