namespace DBUtils
{
    using DBDef;
    using MHUtils;
    using MOM;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Localization
    {
        private const string aEnter = @"\n";
        private const string Enter = "\n";
        private const string aTab = @"\t";
        private const string Tab = "\t";
        private const char space = ' ';
        private const string color = "{COLOR:";
        private const string endColor = "{END_COLOR}";
        private const string injectIcon = "{ICON:";
        private const string indent = "{INDENT:";
        private const string endIndent = "{END_INDENT}";
        private const string link = "{LINK:";
        private const string endLink = "{END_LINK}";
        private const string keyAction = "{KEY_ACTION:";
        private const char endChar = '}';
        private const string spriteIcon = "<sprite name=";
        private const string endSpriteIcon = ">";
        private const string colorStart = "<color=";
        private const string colorEnd = ">";
        private const string endColorTag = "</color>";
        private const string indentStart = "<indent=";
        private const string indentEnd = ">";
        private const string endIndentTag = "</indent>";
        private const string linkStart = "<link=";
        private const string linkEnd = ">";
        private const string endLinkTag = "</link>";
        private static Dictionary<string, string> keys;

        public static string Get(string id, bool symbolParse, params object[] parameters)
        {
            if (!DataBase.IsInitialized())
            {
                return id;
            }
            GuaranteeLibrary();
            if (string.IsNullOrEmpty(id))
            {
                return "";
            }
            if (id.Length < 5)
            {
                return id;
            }
            string[] strArray = id.Split(' ', StringSplitOptions.None);
            string source = "";
            for (int i = 0; i < strArray.Length; i++)
            {
                string str2 = strArray[i];
                source = (str2.Length >= 5) ? (source + SimpleGet(str2, false)) : (source + str2);
                if ((i < (strArray.Length - 1)) && !source.EndsWith("\n"))
                {
                    source = source + " ";
                }
            }
            if (symbolParse)
            {
                source = ProcessSymbols(source);
            }
            if ((parameters != null) && (parameters.Length != 0))
            {
                source = string.Format(source, parameters);
            }
            return source;
        }

        public static void GuaranteeLibrary()
        {
            if (keys == null)
            {
                string languageID = "English";
                if (PlayerPrefs.HasKey("Language"))
                {
                    string key = PlayerPrefs.GetString("Language");
                    Language language = DataBase.GetType<Language>().Find(o => o.dbName == key);
                    if (language != null)
                    {
                        languageID = language.languageID;
                    }
                }
                LoadLibraryByName(languageID);
            }
        }

        public static void LoadLibraryByName(string language)
        {
            keys = new Dictionary<string, string>();
            foreach (DBDef.Localization localization in DataBase.GetType<DBDef.Localization>())
            {
                if (localization.language == language)
                {
                    foreach (Loc loc in localization.loc)
                    {
                        if (keys.ContainsKey(loc.key))
                        {
                            Debug.LogWarning("Localization override for " + loc.key);
                        }
                        string str = loc.value.Replace(@"\n", "\n").Replace(@"\t", "\t");
                        keys[loc.key] = str;
                    }
                }
            }
        }

        public static string ProcessSymbols(string source)
        {
            while (true)
            {
                int index = source.IndexOf("{ICON:");
                if (index > -1)
                {
                    int startIndex = index + "{ICON:".Length;
                    int num3 = source.IndexOf('}', startIndex);
                    if (num3 > -1)
                    {
                        string str = source.Substring(startIndex, num3 - startIndex);
                        string str2 = "";
                        if (index > 0)
                        {
                            str2 = source.Substring(0, index);
                        }
                        str2 = str2 + "<sprite name=" + str + ">";
                        num3++;
                        if (num3 < source.Length)
                        {
                            str2 = str2 + source.Substring(num3, source.Length - num3);
                        }
                        source = str2;
                        continue;
                    }
                }
                index = source.IndexOf("{COLOR:");
                if (index > -1)
                {
                    int startIndex = index + "{COLOR:".Length;
                    int num5 = source.IndexOf('}', startIndex);
                    if (num5 > -1)
                    {
                        string str3 = source.Substring(startIndex, num5 - startIndex);
                        string str4 = "";
                        if (index > 0)
                        {
                            str4 = source.Substring(0, index);
                        }
                        str4 = str4 + "<color=" + str3 + ">";
                        num5++;
                        if (num5 < source.Length)
                        {
                            str4 = str4 + source.Substring(num5, source.Length - num5);
                        }
                        source = str4;
                        continue;
                    }
                }
                if (source.IndexOf("{END_COLOR}") > -1)
                {
                    source = source.Replace("{END_COLOR}", "</color>");
                }
                else
                {
                    index = source.IndexOf("{INDENT:");
                    if (index > -1)
                    {
                        int startIndex = index + "{INDENT:".Length;
                        int num7 = source.IndexOf('}', startIndex);
                        if (num7 > -1)
                        {
                            string str5 = source.Substring(startIndex, num7 - startIndex);
                            string str6 = "";
                            if (index > 0)
                            {
                                str6 = source.Substring(0, index);
                            }
                            str6 = str6 + "<indent=" + str5 + ">";
                            num7++;
                            if (num7 < source.Length)
                            {
                                str6 = str6 + source.Substring(num7, source.Length - num7);
                            }
                            source = str6;
                            continue;
                        }
                    }
                    if (source.IndexOf("{END_INDENT}") > -1)
                    {
                        source = source.Replace("{END_INDENT}", "</indent>");
                    }
                    else
                    {
                        index = source.IndexOf("{LINK:");
                        if (index > -1)
                        {
                            int startIndex = index + "{LINK:".Length;
                            int num9 = source.IndexOf('}', startIndex);
                            if (num9 > -1)
                            {
                                string str7 = source.Substring(startIndex, num9 - startIndex);
                                string str8 = "";
                                if (index > 0)
                                {
                                    str8 = source.Substring(0, index);
                                }
                                str8 = str8 + "<link=" + str7 + ">";
                                num9++;
                                if (num9 < source.Length)
                                {
                                    str8 = str8 + source.Substring(num9, source.Length - num9);
                                }
                                source = str8;
                                continue;
                            }
                        }
                        if (source.IndexOf("{END_LINK}") > -1)
                        {
                            source = source.Replace("{END_LINK}", "</link>");
                        }
                        else
                        {
                            Settings.KeyActions actions;
                            index = source.IndexOf("{KEY_ACTION:");
                            if (index <= -1)
                            {
                                break;
                            }
                            int startIndex = index + "{KEY_ACTION:".Length;
                            int num11 = source.IndexOf('}', startIndex);
                            if (num11 <= -1)
                            {
                                break;
                            }
                            string str9 = "";
                            if (index > 0)
                            {
                                str9 = source.Substring(0, index);
                            }
                            Enum.TryParse<Settings.KeyActions>(source.Substring(startIndex, num11 - startIndex), out actions);
                            string str10 = null;
                            if (actions != Settings.KeyActions.None)
                            {
                                KeyCode keyForAction = SettingsBlock.GetKeyForAction(actions);
                                if (keyForAction != KeyCode.None)
                                {
                                    str10 = keyForAction.ToString();
                                }
                            }
                            if (str10 != null)
                            {
                                str9 = str9 + str10;
                            }
                            num11++;
                            if (num11 < source.Length)
                            {
                                str9 = str9 + source.Substring(num11, source.Length - num11);
                            }
                            source = str9;
                        }
                    }
                }
            }
            return source;
        }

        public static string SimpleGet(string id, bool safe)
        {
            if (safe)
            {
                GuaranteeLibrary();
                if (string.IsNullOrEmpty(id))
                {
                    return "";
                }
            }
            return ((id.Length >= 5) ? (!keys.ContainsKey(id) ? id : keys[id]) : id);
        }
    }
}

