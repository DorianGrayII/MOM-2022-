using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DBDef;
using DBEnum;
using MHUtils;
using UnityEngine;

namespace MOM.Adventures
{
    public class AdventureLibrary
    {
        public static AdventureLibrary currentLibrary;

        public List<Module> modules;

        public HashSet<int> modulesModified = new HashSet<int>();

        public HashSet<string> modulesDeleted = new HashSet<string>();

        private List<Adventure> perPlayerEvents = new List<Adventure>();

        private List<Adventure> simultaneusEvents = new List<Adventure>();

        private List<Adventure> genericEvents = new List<Adventure>();

        private Dictionary<Adventure, Module> advToModuleDictionary = new Dictionary<Adventure, Module>();

        private Dictionary<Module, string> modulePaths = new Dictionary<Module, string>();

        private Adventure midGameCrisis;

        private string curentLanguageSuffix;

        public static AdventureLibrary LoadModulesFromDrive(global::MHUtils.Callback onFinish, global::MHUtils.Callback onError)
        {
            List<string> list = new List<string>(Directory.GetFiles(Path.Combine(MHApplication.EXTERNAL_ASSETS, "StoryModules")));
            list.AddRange(AdventureLibrary.LoadModAdventures());
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Module));
            AdventureLibrary adventureLibrary = new AdventureLibrary
            {
                modules = new List<Module>()
            };
            foreach (string item in list)
            {
                using (Stream stream = new FileStream(item, FileMode.Open, FileAccess.Read))
                {
                    Module module = (Module)xmlSerializer.Deserialize(stream);
                    adventureLibrary.modules.Add(module);
                    adventureLibrary.modulePaths[module] = item;
                }
            }
            adventureLibrary.curentLanguageSuffix = null;
            onFinish?.Invoke(adventureLibrary);
            return adventureLibrary;
        }

        private static List<string> LoadModAdventures()
        {
            List<ModOrder> activeValidMods = ModManager.Get().GetActiveValidMods();
            List<string> list = new List<string>();
            foreach (ModOrder item in activeValidMods)
            {
                string path = item.GetPath();
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("Path for mod " + item.name + " is missing!");
                    continue;
                }
                string path2 = Path.Combine(path, "Adventures");
                if (!Directory.Exists(path2))
                {
                    continue;
                }
                string[] files = Directory.GetFiles(path2);
                foreach (string text in files)
                {
                    if (text.EndsWith(".xml"))
                    {
                        list.Add(text);
                    }
                }
            }
            return list;
        }

        public void Save()
        {
            if (this.modules == null)
            {
                Debug.LogError("Saving when modules does not exists");
                return;
            }
            string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, "StoryModules");
            if (this.modulesDeleted != null)
            {
                foreach (string item in this.modulesDeleted)
                {
                    string path2 = Path.Combine(path, item + ".xml");
                    foreach (KeyValuePair<Module, string> modulePath in this.modulePaths)
                    {
                        if (Path.GetFileNameWithoutExtension(modulePath.Value) == item)
                        {
                            path2 = modulePath.Value;
                            break;
                        }
                    }
                    if (File.Exists(path2))
                    {
                        File.Delete(path2);
                    }
                }
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Module));
            if (this.modulesModified == null)
            {
                return;
            }
            foreach (int v in this.modulesModified)
            {
                Module module = this.modules.Find((Module o) => o.uniqueID == v);
                if (module == null)
                {
                    continue;
                }
                module.Test();
                string path3 = Path.Combine(path, module.name + ".xml");
                foreach (KeyValuePair<Module, string> modulePath2 in this.modulePaths)
                {
                    if (Path.GetFileNameWithoutExtension(modulePath2.Value) == module.name)
                    {
                        path3 = modulePath2.Value;
                        break;
                    }
                }
                using (Stream w = new FileStream(path3, FileMode.Create))
                {
                    XmlWriter xmlWriter = new XmlTextWriter(w, Encoding.Unicode)
                    {
                        Formatting = Formatting.Indented
                    };
                    xmlSerializer.Serialize(xmlWriter, module);
                }
            }
        }

        public void Clear()
        {
            this.perPlayerEvents.Clear();
            this.simultaneusEvents.Clear();
            this.advToModuleDictionary.Clear();
            this.genericEvents.Clear();
        }

        public void PrepareCache()
        {
            this.perPlayerEvents.Clear();
            this.simultaneusEvents.Clear();
            this.advToModuleDictionary.Clear();
            this.genericEvents.Clear();
            DifficultyOption setting = DifficultySettingsData.GetSetting("UI_DIFF_SPECIAL_EVENTS");
            PlayerPrefs.GetInt("UseDLC", 0);
            Array values = Enum.GetValues(typeof(DLCManager.DLCs));
            foreach (Module module in this.modules)
            {
                if (!module.isAllowed || module.adventures == null)
                {
                    continue;
                }
                bool flag = false;
                foreach (object item in values)
                {
                    if (module.name.ToLowerInvariant().Contains(item.ToString().ToLowerInvariant()))
                    {
                        if (!DLCManager.IsDlcActive((int)item))
                        {
                            Debug.Log("[" + item?.ToString() + "] Skipping DLC adventure module " + module.name);
                            flag = true;
                        }
                        else
                        {
                            Debug.Log("[" + item?.ToString() + "] Using DLC adventure module " + module.name);
                        }
                    }
                }
                if (flag)
                {
                    continue;
                }
                if (setting != null)
                {
                    switch (setting.title)
                    {
                    case "UI_DIFF_SPECIAL_EVENTS_NONE":
                        if (module.uniqueID == -1600151959 || module.uniqueID == 40415794 || module.uniqueID == -1007762225 || module.uniqueID == -2010396620 || module.uniqueID == -409307634)
                        {
                            continue;
                        }
                        break;
                    case "UI_DIFF_SPECIAL_EVENTS_ORIGINAL":
                        if (module.uniqueID == 40415794 || module.uniqueID == -1007762225 || module.uniqueID == -2010396620 || module.uniqueID == -409307634)
                        {
                            continue;
                        }
                        break;
                    case "UI_DIFF_SPECIAL_EVENTS_ORIGINAL_MODIFIED":
                        if (module.uniqueID == -1600151959 || module.uniqueID == -1007762225 || module.uniqueID == -2010396620 || module.uniqueID == -409307634)
                        {
                            continue;
                        }
                        break;
                    case "UI_DIFF_SPECIAL_EVENTS_ORIGINAL_AND_NEW":
                        if (module.uniqueID == -1600151959)
                        {
                            continue;
                        }
                        break;
                    }
                }
                foreach (Adventure adventure in module.adventures)
                {
                    if (!adventure.isAllowed)
                    {
                        continue;
                    }
                    adventure.PrepareForGame();
                    adventure.module = module;
                    if (adventure.nodes != null)
                    {
                        foreach (BaseNode node in adventure.nodes)
                        {
                            node.parentEvent = adventure;
                        }
                    }
                    NodeStart start = adventure.GetStart();
                    if (start.adventureStartType == Adventure.AdventureTriggerType.PerPlayer && !start.genericEvent)
                    {
                        this.perPlayerEvents.Add(adventure);
                    }
                    else if (start.genericEvent)
                    {
                        this.genericEvents.Add(adventure);
                    }
                    this.advToModuleDictionary[adventure] = module;
                    if (module.name == "9 DLC2 Tech Dungeon" && adventure.uniqueID == 2)
                    {
                        this.midGameCrisis = adventure;
                    }
                }
            }
        }

        public List<Adventure> GetPerPlayerEvents()
        {
            if (this.perPlayerEvents == null || this.perPlayerEvents.Count == 0)
            {
                this.PrepareCache();
            }
            return this.perPlayerEvents;
        }

        public List<Adventure> GetSimultaneusEvents()
        {
            if (this.perPlayerEvents == null || this.perPlayerEvents.Count == 0)
            {
                this.PrepareCache();
            }
            return this.simultaneusEvents;
        }

        public List<Adventure> GetGenericEvents()
        {
            if (this.perPlayerEvents == null || this.perPlayerEvents.Count == 0)
            {
                this.PrepareCache();
            }
            return this.genericEvents;
        }

        public List<Module> GetModules()
        {
            if (this.perPlayerEvents == null || this.perPlayerEvents.Count == 0)
            {
                this.PrepareCache();
            }
            return this.modules;
        }

        public Adventure GetMidgameCrisis()
        {
            return this.midGameCrisis;
        }

        public bool ReqiresLocalization()
        {
            if (PlayerPrefs.HasKey("Language"))
            {
                string key = PlayerPrefs.GetString("Language");
                Language language = DataBase.GetType<Language>().Find((Language o) => o.dbName == key);
                if (language != null)
                {
                    if ("_EN" == language.nameSuffix && this.curentLanguageSuffix == null)
                    {
                        return false;
                    }
                    return this.curentLanguageSuffix != language.nameSuffix;
                }
            }
            return false;
        }

        public void SetLocalizationCorrect()
        {
            string key = PlayerPrefs.GetString("Language");
            Language language = DataBase.GetType<Language>().Find((Language o) => o.dbName == key);
            if (language != null)
            {
                this.curentLanguageSuffix = language.nameSuffix;
            }
            else
            {
                this.curentLanguageSuffix = null;
            }
        }

        public void AdventureLocalization()
        {
            if (!this.ReqiresLocalization())
            {
                return;
            }
            bool ready = false;
            bool failed = false;
            string directory = Path.Combine(MHApplication.EXTERNAL_ASSETS, "StoryLocalisation");
            string key = PlayerPrefs.GetString("Language");
            Language language = DataBase.GetType<Language>().Find((Language o) => o.dbName == key);
            string languagePostfix = "";
            if (language != null)
            {
                languagePostfix = (string.IsNullOrEmpty(language.nameSuffix) ? "" : language.nameSuffix);
            }
            if (key == ((Language)LANGUAGE.EN)?.dbName)
            {
                AdventureLibrary.currentLibrary = AdventureLibrary.LoadModulesFromDrive(null, null);
                return;
            }
            ModulesImport.ImportBlockOfModules(this.modules, delegate
            {
                ready = true;
            }, delegate
            {
                failed = true;
            }, directory, languagePostfix);
            if (failed)
            {
                Debug.Log("Adventure localization for " + language.nameSuffix + " failed (using bundle)");
            }
            else if (ready)
            {
                Debug.Log("Adventure localization by bundle finished");
            }
            ModulesImport.ImportSingleForModules(this.modules, directory, languagePostfix);
            this.SetLocalizationCorrect();
        }
    }
}
