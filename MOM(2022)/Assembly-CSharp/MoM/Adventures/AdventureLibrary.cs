namespace MOM.Adventures
{
    using DBDef;
    using DBEnum;
    using MHUtils;
    using MOM;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using UnityEngine;

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

        public void AdventureLocalization()
        {
            if (this.ReqiresLocalization())
            {
                string dbName;
                bool ready = false;
                bool failed = false;
                string directory = Path.Combine(MHApplication.EXTERNAL_ASSETS, "StoryLocalisation");
                string key = PlayerPrefs.GetString("Language");
                Language language = DataBase.GetType<Language>().Find(o => o.dbName == key);
                string languagePostfix = "";
                if (language != null)
                {
                    languagePostfix = string.IsNullOrEmpty(language.nameSuffix) ? "" : language.nameSuffix;
                }
                Language eN = (Language) LANGUAGE.EN;
                if (eN != null)
                {
                    dbName = eN.dbName;
                }
                else
                {
                    Language local1 = eN;
                    dbName = null;
                }
                if (key == dbName)
                {
                    currentLibrary = LoadModulesFromDrive(null, null);
                }
                else
                {
                    ModulesImport.ImportBlockOfModules(this.modules, () => ready = true, () => failed = true, directory, languagePostfix);
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

        public void Clear()
        {
            this.perPlayerEvents.Clear();
            this.simultaneusEvents.Clear();
            this.advToModuleDictionary.Clear();
            this.genericEvents.Clear();
        }

        public List<Adventure> GetGenericEvents()
        {
            if ((this.perPlayerEvents == null) || (this.perPlayerEvents.Count == 0))
            {
                this.PrepareCache();
            }
            return this.genericEvents;
        }

        public Adventure GetMidgameCrisis()
        {
            return this.midGameCrisis;
        }

        public List<Module> GetModules()
        {
            if ((this.perPlayerEvents == null) || (this.perPlayerEvents.Count == 0))
            {
                this.PrepareCache();
            }
            return this.modules;
        }

        public List<Adventure> GetPerPlayerEvents()
        {
            if ((this.perPlayerEvents == null) || (this.perPlayerEvents.Count == 0))
            {
                this.PrepareCache();
            }
            return this.perPlayerEvents;
        }

        public List<Adventure> GetSimultaneusEvents()
        {
            if ((this.perPlayerEvents == null) || (this.perPlayerEvents.Count == 0))
            {
                this.PrepareCache();
            }
            return this.simultaneusEvents;
        }

        private static List<string> LoadModAdventures()
        {
            List<string> list = new List<string>();
            foreach (ModOrder order in ModManager.Get().GetActiveValidMods())
            {
                string path = order.GetPath();
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("Path for mod " + order.name + " is missing!");
                    continue;
                }
                string str2 = Path.Combine(path, "Adventures");
                if (Directory.Exists(str2))
                {
                    foreach (string str3 in Directory.GetFiles(str2))
                    {
                        if (str3.EndsWith(".xml"))
                        {
                            list.Add(str3);
                        }
                    }
                }
            }
            return list;
        }

        public static AdventureLibrary LoadModulesFromDrive(MHUtils.Callback onFinish, MHUtils.Callback onError)
        {
            List<string> list1 = new List<string>(Directory.GetFiles(Path.Combine(MHApplication.EXTERNAL_ASSETS, "StoryModules")));
            list1.AddRange(LoadModAdventures());
            XmlSerializer serializer = new XmlSerializer(typeof(Module));
            AdventureLibrary o = new AdventureLibrary {
                modules = new List<Module>()
            };
            foreach (string str in list1)
            {
                using (Stream stream = new FileStream(str, FileMode.Open, FileAccess.Read))
                {
                    Module item = (Module) serializer.Deserialize(stream);
                    o.modules.Add(item);
                    o.modulePaths[item] = str;
                }
            }
            o.curentLanguageSuffix = null;
            if (onFinish != null)
            {
                onFinish(o);
            }
            return o;
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
                if (module.isAllowed && (module.adventures != null))
                {
                    bool flag = false;
                    foreach (object obj2 in values)
                    {
                        if (module.name.ToLowerInvariant().Contains(obj2.ToString().ToLowerInvariant()))
                        {
                            if (DLCManager.IsDlcActive((int) obj2))
                            {
                                Debug.Log("[" + obj2?.ToString() + "] Using DLC adventure module " + module.name);
                                continue;
                            }
                            Debug.Log("[" + obj2?.ToString() + "] Skipping DLC adventure module " + module.name);
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        if (setting != null)
                        {
                            string title = setting.title;
                            if (title == "UI_DIFF_SPECIAL_EVENTS_NONE")
                            {
                                if ((module.uniqueID == -1600151959) || ((module.uniqueID == 0x268b232) || ((module.uniqueID == -1007762225) || ((module.uniqueID == -2010396620) || (module.uniqueID == -409307634)))))
                                {
                                    continue;
                                }
                            }
                            else if (title == "UI_DIFF_SPECIAL_EVENTS_ORIGINAL")
                            {
                                if ((module.uniqueID == 0x268b232) || ((module.uniqueID == -1007762225) || ((module.uniqueID == -2010396620) || (module.uniqueID == -409307634))))
                                {
                                    continue;
                                }
                            }
                            else if (title != "UI_DIFF_SPECIAL_EVENTS_ORIGINAL_MODIFIED")
                            {
                                if ((title == "UI_DIFF_SPECIAL_EVENTS_ORIGINAL_AND_NEW") && (module.uniqueID == -1600151959))
                                {
                                    continue;
                                }
                            }
                            else if ((module.uniqueID == -1600151959) || ((module.uniqueID == -1007762225) || ((module.uniqueID == -2010396620) || (module.uniqueID == -409307634))))
                            {
                                continue;
                            }
                        }
                        foreach (Adventure adventure in module.adventures)
                        {
                            if (adventure.isAllowed)
                            {
                                adventure.PrepareForGame();
                                adventure.module = module;
                                if (adventure.nodes != null)
                                {
                                    using (List<BaseNode>.Enumerator enumerator4 = adventure.nodes.GetEnumerator())
                                    {
                                        while (enumerator4.MoveNext())
                                        {
                                            enumerator4.Current.parentEvent = adventure;
                                        }
                                    }
                                }
                                NodeStart start = adventure.GetStart();
                                if ((start.adventureStartType == Adventure.AdventureTriggerType.PerPlayer) && !start.genericEvent)
                                {
                                    this.perPlayerEvents.Add(adventure);
                                }
                                else if (start.genericEvent)
                                {
                                    this.genericEvents.Add(adventure);
                                }
                                this.advToModuleDictionary[adventure] = module;
                                if ((module.name == "9 DLC2 Tech Dungeon") && (adventure.uniqueID == 2))
                                {
                                    this.midGameCrisis = adventure;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool ReqiresLocalization()
        {
            if (!PlayerPrefs.HasKey("Language"))
            {
                return false;
            }
            string key = PlayerPrefs.GetString("Language");
            Language language = DataBase.GetType<Language>().Find(o => o.dbName == key);
            return ((language != null) && ((("_EN" != language.nameSuffix) || (this.curentLanguageSuffix != null)) ? (this.curentLanguageSuffix != language.nameSuffix) : false));
        }

        public void Save()
        {
            if (this.modules == null)
            {
                Debug.LogError("Saving when modules does not exists");
            }
            else
            {
                string str = Path.Combine(MHApplication.EXTERNAL_ASSETS, "StoryModules");
                if (this.modulesDeleted != null)
                {
                    foreach (string str2 in this.modulesDeleted)
                    {
                        string path = Path.Combine(str, str2 + ".xml");
                        foreach (KeyValuePair<Module, string> pair in this.modulePaths)
                        {
                            if (Path.GetFileNameWithoutExtension(pair.Value) == str2)
                            {
                                path = pair.Value;
                                break;
                            }
                        }
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                }
                XmlSerializer serializer = new XmlSerializer(typeof(Module));
                if (this.modulesModified != null)
                {
                    foreach (int v in this.modulesModified)
                    {
                        Module module = this.modules.Find(o => o.uniqueID == v);
                        if (module != null)
                        {
                            module.Test(true);
                            string path = Path.Combine(str, module.name + ".xml");
                            foreach (KeyValuePair<Module, string> pair2 in this.modulePaths)
                            {
                                if (Path.GetFileNameWithoutExtension(pair2.Value) == module.name)
                                {
                                    path = pair2.Value;
                                    break;
                                }
                            }
                            using (Stream stream = new FileStream(path, FileMode.Create))
                            {
                                XmlTextWriter writer1 = new XmlTextWriter(stream, Encoding.Unicode);
                                writer1.Formatting = Formatting.Indented;
                                serializer.Serialize((XmlWriter) writer1, module);
                            }
                        }
                    }
                }
            }
        }

        public void SetLocalizationCorrect()
        {
            string key = PlayerPrefs.GetString("Language");
            Language language = DataBase.GetType<Language>().Find(o => o.dbName == key);
            if (language != null)
            {
                this.curentLanguageSuffix = language.nameSuffix;
            }
            else
            {
                this.curentLanguageSuffix = null;
            }
        }
    }
}

