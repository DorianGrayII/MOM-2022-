// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// ModManager
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DBUtils;
using MHUtils;
using MOM;
using UnityEngine;

public class ModManager
{
    public static ModManager instance;

    public Dictionary<string, ModSettings> modPaths;

    public List<ModOrder> modsActiveAndValid;

    public ModOrderList modOrder;

    public bool orderRequiresSave;

    public static ModManager Get()
    {
        if (ModManager.instance == null)
        {
            ModManager.instance = new ModManager();
        }
        return ModManager.instance;
    }

    public ModOrderList GetModOrderList()
    {
        if (this.modOrder == null)
        {
            try
            {
                string path = Path.Combine(MHApplication.PROFILES, "_mods.xml");
                if (File.Exists(path))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModOrderList));
                    using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(path)))
                    {
                        this.modOrder = xmlSerializer.Deserialize(stream) as ModOrderList;
                    }
                }
            }
            catch (Exception message)
            {
                Debug.Log(message);
            }
        }
        if (this.modOrder == null)
        {
            this.modOrder = new ModOrderList();
        }
        return this.modOrder;
    }

    public void UpdateOrder(List<ModOrder> data)
    {
        this.GetModOrderList().order = data;
    }

    public void SaveModOrderListIfNeeded()
    {
        if (this.orderRequiresSave)
        {
            this.SaveModOrderList();
        }
    }

    public void SaveModOrderList()
    {
        if (this.modOrder == null)
        {
            return;
        }
        try
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModOrderList));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memoryStream, this.modOrder);
                string path = Path.Combine(MHApplication.PROFILES, "_mods.xml");
                byte[] bytes = memoryStream.ToArray();
                File.WriteAllBytes(path, bytes);
                this.orderRequiresSave = false;
            }
        }
        catch (Exception message)
        {
            Debug.LogWarning(message);
        }
    }

    public Dictionary<string, ModSettings> GetModList()
    {
        if (this.modPaths != null)
        {
            return this.modPaths;
        }
        this.modPaths = new Dictionary<string, ModSettings>();
        string mODS = MHApplication.MODS;
        if (!Directory.Exists(mODS))
        {
            Directory.CreateDirectory(mODS);
        }
        bool flag = true;
        string[] directories = Directory.GetDirectories(mODS);
        foreach (string path in directories)
        {
            string text = Path.Combine(mODS, path);
            string path2 = Path.Combine(text, "settings.xml");
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                using (Stream stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                {
                    ModSettings modSettings = (ModSettings)xmlSerializer.Deserialize(stream);
                    foreach (ModSettings value in this.modPaths.Values)
                    {
                        if (value.name == modSettings.name)
                        {
                            flag = false;
                            Debug.LogWarning("Mod name conflict: " + value.name + ", path: " + text);
                        }
                    }
                    this.modPaths[text] = modSettings;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Loading mod failed at " + text + "\n" + ex);
            }
        }
        foreach (string allInstalledMod in SteamWorkshop.GetAllInstalledMods())
        {
            string path3 = Path.Combine(allInstalledMod, "settings.xml");
            try
            {
                XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(ModSettings));
                using (Stream stream2 = new FileStream(path3, FileMode.Open, FileAccess.Read))
                {
                    ModSettings modSettings2 = (ModSettings)xmlSerializer2.Deserialize(stream2);
                    bool flag2 = true;
                    foreach (ModSettings value2 in this.modPaths.Values)
                    {
                        if (value2.name == modSettings2.name)
                        {
                            flag2 = false;
                            Debug.LogWarning("Mod name " + modSettings2.name + " conflict in workshop is an override, skip. Path: " + value2);
                        }
                    }
                    if (flag2)
                    {
                        this.modPaths[allInstalledMod] = modSettings2;
                    }
                }
            }
            catch (Exception ex2)
            {
                Debug.LogWarning("Loading mod failed at " + allInstalledMod + "\n" + ex2);
            }
        }
        if (!flag)
        {
            this.modPaths.Clear();
            PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_MOD_DUPLICATION_NONE_WILL_BE_LOADED", "UI_OK");
        }
        return this.modPaths;
    }

    public List<string> UpdateOrderList()
    {
        ModOrderList modOrderList = this.GetModOrderList();
        Dictionary<string, ModSettings> modList = this.GetModList();
        foreach (KeyValuePair<string, ModSettings> i in modList)
        {
            if (modOrderList.order.Find((ModOrder o) => i.Value.name == o.name) == null)
            {
                ModOrder modOrder = new ModOrder();
                modOrder.name = i.Value.name;
                modOrder.order = -1;
                modOrder.active = false;
                modOrderList.order.Add(modOrder);
                this.orderRequiresSave = true;
            }
        }
        modOrderList.order.Sort(delegate(ModOrder a, ModOrder b)
        {
            int num = a.active.CompareTo(b.active);
            switch (num)
            {
            case 1:
                this.orderRequiresSave = true;
                break;
            case 0:
                num = a.order.CompareTo(b.order);
                if (num == 1)
                {
                    this.orderRequiresSave = true;
                }
                break;
            }
            return num;
        });
        List<string> list = null;
        foreach (ModOrder item in modOrderList.order)
        {
            bool flag = false;
            foreach (KeyValuePair<string, ModSettings> item2 in modList)
            {
                if (item.name == item2.Value.name)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                if (list == null)
                {
                    list = new List<string>();
                }
                list.Add(item.name);
            }
        }
        return list;
    }

    public string ValidateMod(ModOrder modOrder)
    {
        Dictionary<string, ModSettings> modList = this.GetModList();
        if (modList == null)
        {
            return "Mod list is null";
        }
        foreach (KeyValuePair<string, ModSettings> item in modList)
        {
            if (item.Value.name == modOrder.name)
            {
                return this.ValidateMod(item.Value);
            }
        }
        return "Installed mod list missing mod " + modOrder.name;
    }

    public string ValidateMod(ModSettings modSettings)
    {
        if (modSettings.prerequisites == null)
        {
            return null;
        }
        if (this.GetModList() == null)
        {
            return "Mod list is null";
        }
        ModOrderList modOrderList = this.GetModOrderList();
        if (modOrderList.order == null)
        {
            return "Mod order list is null";
        }
        string text = "";
        int num = modOrderList.order.FindIndex((ModOrder m) => m.name == modSettings.name);
        string[] prerequisites = modSettings.prerequisites;
        foreach (string req in prerequisites)
        {
            int num2 = modOrderList.order.FindIndex((ModOrder m) => m.GetNameVersion().StartsWith(req));
            if (num2 < 0)
            {
                text = text + Localization.Get("UI_MOD_REQ_FAILED1", true, modSettings.name, req) + "\n";
                continue;
            }
            if (num2 >= num)
            {
                text = text + Localization.Get("UI_MOD_REQ_FAILED2", true, modSettings.name, req) + "\n";
            }
            if (!modOrderList.order[num2].active)
            {
                text = text + Localization.Get("UI_MOD_REQ_FAILED3", true, modSettings.name, req) + "\n";
            }
        }
        return text;
    }

    public string GetPath(string modName)
    {
        Dictionary<string, ModSettings> modList = this.GetModList();
        if (modList == null)
        {
            return null;
        }
        foreach (KeyValuePair<string, ModSettings> item in modList)
        {
            if (item.Value.name == modName)
            {
                return item.Key;
            }
        }
        return null;
    }

    public string GetPath(ModOrder modOrder)
    {
        Dictionary<string, ModSettings> modList = this.GetModList();
        if (modList == null)
        {
            return null;
        }
        foreach (KeyValuePair<string, ModSettings> item in modList)
        {
            if (item.Value.name == modOrder.name)
            {
                return item.Key;
            }
        }
        return null;
    }

    public List<ModOrder> GetActiveValidMods()
    {
        if (this.modsActiveAndValid == null)
        {
            this.modsActiveAndValid = new List<ModOrder>();
            ModOrderList modOrderList = ModManager.Get().GetModOrderList();
            if (modOrderList?.order != null)
            {
                foreach (ModOrder item in modOrderList.order)
                {
                    if (item.active && item.IsValid())
                    {
                        this.modsActiveAndValid.Add(item);
                    }
                }
            }
        }
        return this.modsActiveAndValid;
    }

    public ModOrder GetModOrderByPath(string path)
    {
        if (path == null || this.modPaths == null)
        {
            return null;
        }
        foreach (KeyValuePair<string, ModSettings> modPath in this.modPaths)
        {
            if (path.StartsWith(modPath.Key))
            {
                return this.GetModOrderByName(modPath.Value.name);
            }
        }
        return null;
    }

    public ModOrder GetModOrderByName(string name)
    {
        ModOrderList modOrderList = ModManager.Get().GetModOrderList();
        if (modOrderList != null)
        {
            foreach (ModOrder item in modOrderList.order)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
        }
        return null;
    }

    public ModOrder GetModOrderByModSettings(ModSettings ms)
    {
        ModOrderList modOrderList = ModManager.Get().GetModOrderList();
        if (modOrderList != null)
        {
            foreach (ModOrder item in modOrderList.order)
            {
                if (item.name == ms.name)
                {
                    return item;
                }
            }
        }
        return null;
    }
}
