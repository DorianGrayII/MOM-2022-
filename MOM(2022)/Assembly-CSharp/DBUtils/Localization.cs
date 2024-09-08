using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;

namespace DBUtils
{
    public class Localization
    {
        private const string aEnter = "\\n";

        private const string Enter = "\n";

        private const string aTab = "\\t";

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

        public static void GuaranteeLibrary()
        {
            if (Localization.keys != null)
            {
                return;
            }
            string language = "English";
            if (PlayerPrefs.HasKey("Language"))
            {
                string key = PlayerPrefs.GetString("Language");
                Language language2 = DataBase.GetType<Language>().Find((Language o) => o.dbName == key);
                if (language2 != null)
                {
                    language = language2.languageID;
                }
            }
            Localization.LoadLibraryByName(language);
        }

        public static void LoadLibraryByName(string language)
        {
            Localization.keys = new Dictionary<string, string>();
            foreach (global::DBDef.Localization item in DataBase.GetType<global::DBDef.Localization>())
            {
                if (!(item.language == language))
                {
                    continue;
                }
                Loc[] loc = item.loc;
                foreach (Loc loc2 in loc)
                {
                    if (Localization.keys.ContainsKey(loc2.key))
                    {
                        Debug.LogWarning("Localization override for " + loc2.key);
                    }
                    string text = loc2.value.Replace("\\n", "\n");
                    text = text.Replace("\\t", "\t");
                    Localization.keys[loc2.key] = text;
                }
            }
        }

        public static string Get(string id, bool symbolParse = true, params object[] parameters)
        {
            if (!DataBase.IsInitialized())
            {
                return id;
            }
            Localization.GuaranteeLibrary();
            if (string.IsNullOrEmpty(id))
            {
                return "";
            }
            if (id.Length < 5)
            {
                return id;
            }
            string[] array = id.Split(' '); //, StringSplitOptions.None);
            string text = "";
            for (int i = 0; i < array.Length; i++)
            {
                string text2 = array[i];
                text = ((text2.Length >= 5) ? (text + Localization.SimpleGet(text2, safe: false)) : (text + text2));
                if (i < array.Length - 1 && !text.EndsWith("\n"))
                {
                    text += " ";
                }
            }
            if (symbolParse)
            {
                text = Localization.ProcessSymbols(text);
            }
            if (parameters != null && parameters.Length != 0)
            {
                text = string.Format(text, parameters);
            }
            return text;
        }

        public static string SimpleGet(string id, bool safe = true)
        {
            if (safe)
            {
                Localization.GuaranteeLibrary();
                if (string.IsNullOrEmpty(id))
                {
                    return "";
                }
            }
            if (id.Length < 5)
            {
                return id;
            }
            if (Localization.keys.ContainsKey(id))
            {
                return Localization.keys[id];
            }
            return id;
        }

        public static string ProcessSymbols(string source)
        {
            while (true)
            {
                int num = source.IndexOf("{ICON:");
                if (num > -1)
                {
                    int num2 = num + "{ICON:".Length;
                    int num3 = source.IndexOf('}', num2);
                    if (num3 > -1)
                    {
                        string text = source.Substring(num2, num3 - num2);
                        string text2 = "";
                        if (num > 0)
                        {
                            text2 = source.Substring(0, num);
                        }
                        text2 = text2 + "<sprite name=" + text + ">";
                        num3++;
                        if (num3 < source.Length)
                        {
                            text2 += source.Substring(num3, source.Length - num3);
                        }
                        source = text2;
                        continue;
                    }
                }
                num = source.IndexOf("{COLOR:");
                if (num > -1)
                {
                    int num4 = num + "{COLOR:".Length;
                    int num5 = source.IndexOf('}', num4);
                    if (num5 > -1)
                    {
                        string text3 = source.Substring(num4, num5 - num4);
                        string text4 = "";
                        if (num > 0)
                        {
                            text4 = source.Substring(0, num);
                        }
                        text4 = text4 + "<color=" + text3 + ">";
                        num5++;
                        if (num5 < source.Length)
                        {
                            text4 += source.Substring(num5, source.Length - num5);
                        }
                        source = text4;
                        continue;
                    }
                }
                num = source.IndexOf("{END_COLOR}");
                if (num > -1)
                {
                    source = source.Replace("{END_COLOR}", "</color>");
                    continue;
                }
                num = source.IndexOf("{INDENT:");
                if (num > -1)
                {
                    int num6 = num + "{INDENT:".Length;
                    int num7 = source.IndexOf('}', num6);
                    if (num7 > -1)
                    {
                        string text5 = source.Substring(num6, num7 - num6);
                        string text6 = "";
                        if (num > 0)
                        {
                            text6 = source.Substring(0, num);
                        }
                        text6 = text6 + "<indent=" + text5 + ">";
                        num7++;
                        if (num7 < source.Length)
                        {
                            text6 += source.Substring(num7, source.Length - num7);
                        }
                        source = text6;
                        continue;
                    }
                }
                num = source.IndexOf("{END_INDENT}");
                if (num > -1)
                {
                    source = source.Replace("{END_INDENT}", "</indent>");
                    continue;
                }
                num = source.IndexOf("{LINK:");
                if (num > -1)
                {
                    int num8 = num + "{LINK:".Length;
                    int num9 = source.IndexOf('}', num8);
                    if (num9 > -1)
                    {
                        string text7 = source.Substring(num8, num9 - num8);
                        string text8 = "";
                        if (num > 0)
                        {
                            text8 = source.Substring(0, num);
                        }
                        text8 = text8 + "<link=" + text7 + ">";
                        num9++;
                        if (num9 < source.Length)
                        {
                            text8 += source.Substring(num9, source.Length - num9);
                        }
                        source = text8;
                        continue;
                    }
                }
                num = source.IndexOf("{END_LINK}");
                if (num > -1)
                {
                    source = source.Replace("{END_LINK}", "</link>");
                    continue;
                }
                num = source.IndexOf("{KEY_ACTION:");
                if (num <= -1)
                {
                    break;
                }
                int num10 = num + "{KEY_ACTION:".Length;
                int num11 = source.IndexOf('}', num10);
                if (num11 <= -1)
                {
                    break;
                }
                string value = source.Substring(num10, num11 - num10);
                string text9 = "";
                if (num > 0)
                {
                    text9 = source.Substring(0, num);
                }
                Enum.TryParse<Settings.KeyActions>(value, out var result);
                string text10 = null;
                if (result != 0)
                {
                    KeyCode keyForAction = SettingsBlock.GetKeyForAction(result);
                    if (keyForAction != 0)
                    {
                        text10 = keyForAction.ToString();
                    }
                }
                if (text10 != null)
                {
                    text9 += text10;
                }
                num11++;
                if (num11 < source.Length)
                {
                    text9 += source.Substring(num11, source.Length - num11);
                }
                source = text9;
            }
            return source;
        }
    }
}
