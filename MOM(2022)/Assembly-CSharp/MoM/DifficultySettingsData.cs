using System;
using System.Collections.Generic;
using System.Globalization;
using DBDef;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class DifficultySettingsData
    {
        public static DifficultySettingsData Current;

        [ProtoMember(1)]
        public int interruptDelay;

        [ProtoIgnore]
        public int townDistance;

        [ProtoMember(11)]
        public NetDictionary<string, int> settingsNamed;

        private Dictionary<string, DifficultyOption> dSettings;

        private int diffScoreCached;

        public static DifficultySettingsData current
        {
            get
            {
                return DifficultySettingsData.Current;
            }
            set
            {
                DifficultySettingsData.Current = value;
            }
        }

        public static void Clear()
        {
            DifficultySettingsData.current = null;
        }

        public static NetDictionary<string, int> GetDefaults(int diffLevel, bool quickStart = false)
        {
            List<global::DBDef.Difficulty> type = DataBase.GetType<global::DBDef.Difficulty>();
            if (type == null || type.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            NetDictionary<string, int> netDictionary = new NetDictionary<string, int>();
            foreach (global::DBDef.Difficulty item in type)
            {
                int num = 0;
                int num2 = -1;
                num = 0;
                if (item.fullValue > 0f)
                {
                    for (int i = 0; i < item.setting.Length; i++)
                    {
                        if (quickStart && item.setting[i].quickStart)
                        {
                            num2 = i;
                        }
                        if (item.setting[i].collection <= diffLevel)
                        {
                            num = i;
                        }
                    }
                }
                if (quickStart && num2 != -1)
                {
                    num = num2;
                }
                netDictionary[item.name] = num;
            }
            return netDictionary;
        }

        public static void SetValue(string name, int orderIndex)
        {
            DataBase.GetType<global::DBDef.Difficulty>().FindIndex((global::DBDef.Difficulty o) => o.name == name);
            DifficultySettingsData difficultySettingsData = DifficultySettingsData.current;
            if (difficultySettingsData.settingsNamed == null)
            {
                difficultySettingsData.settingsNamed = new NetDictionary<string, int>();
            }
            DifficultySettingsData.current.dSettings = null;
            DifficultySettingsData.current.settingsNamed[name] = orderIndex;
            PlayerPrefs.SetInt("DIFF:" + name, orderIndex);
            PlayerPrefs.Save();
        }

        public static void SetDefaults(int diffLevel, bool quickStart = false)
        {
            NetDictionary<string, int> defaults = DifficultySettingsData.GetDefaults(diffLevel, quickStart);
            List<global::DBDef.Difficulty> type = DataBase.GetType<global::DBDef.Difficulty>();
            DifficultySettingsData.current.dSettings = null;
            DifficultySettingsData difficultySettingsData = DifficultySettingsData.current;
            if (difficultySettingsData.settingsNamed == null)
            {
                difficultySettingsData.settingsNamed = new NetDictionary<string, int>();
            }
            for (int i = 0; i < type.Count; i++)
            {
                DifficultySettingsData.current.settingsNamed[type[i].name] = defaults[type[i].name];
                PlayerPrefs.SetInt("DIFF:" + type[i].name, defaults[type[i].name]);
            }
            PlayerPrefs.Save();
        }

        public static void EnsureLoaded(int fallbackDefault = 1)
        {
            if (DifficultySettingsData.current != null)
            {
                return;
            }
            DifficultySettingsData difficultySettingsData = new DifficultySettingsData();
            List<global::DBDef.Difficulty> type = DataBase.GetType<global::DBDef.Difficulty>();
            if (type == null || type.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            NetDictionary<string, int> defaults = DifficultySettingsData.GetDefaults(fallbackDefault);
            difficultySettingsData.interruptDelay = 35;
            DifficultySettingsData difficultySettingsData2 = difficultySettingsData;
            if (difficultySettingsData2.settingsNamed == null)
            {
                difficultySettingsData2.settingsNamed = new NetDictionary<string, int>();
            }
            for (int i = 0; i < type.Count; i++)
            {
                global::DBDef.Difficulty difficulty = type[i];
                string key = "DIFF:" + difficulty.name;
                int num = 0;
                if (PlayerPrefs.HasKey(key))
                {
                    num = PlayerPrefs.GetInt(key);
                    if (difficulty.setting.Length > num)
                    {
                        difficultySettingsData.settingsNamed[difficulty.name] = num;
                        continue;
                    }
                }
                difficultySettingsData.settingsNamed[difficulty.name] = defaults[difficulty.name];
            }
            DifficultySettingsData.current = difficultySettingsData;
        }

        public static DifficultySettingsData Create(int diffLevel = 0)
        {
            DifficultySettingsData difficultySettingsData = new DifficultySettingsData();
            List<global::DBDef.Difficulty> type = DataBase.GetType<global::DBDef.Difficulty>();
            if (type == null || type.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            NetDictionary<string, int> defaults = DifficultySettingsData.GetDefaults(diffLevel);
            difficultySettingsData.interruptDelay = 35;
            DifficultySettingsData difficultySettingsData2 = difficultySettingsData;
            if (difficultySettingsData2.settingsNamed == null)
            {
                difficultySettingsData2.settingsNamed = new NetDictionary<string, int>();
            }
            for (int i = 0; i < type.Count; i++)
            {
                global::DBDef.Difficulty difficulty = type[i];
                string key = "DIFF:" + difficulty.name;
                int num = 0;
                if (PlayerPrefs.HasKey(key))
                {
                    num = PlayerPrefs.GetInt(key);
                    if (difficulty.setting.Length > num)
                    {
                        difficultySettingsData.settingsNamed[difficulty.name] = num;
                        continue;
                    }
                }
                difficultySettingsData.settingsNamed[difficulty.name] = defaults[difficulty.name];
            }
            DifficultySettingsData.current = difficultySettingsData;
            return difficultySettingsData;
        }

        public static int GetCurentDifficultyRank()
        {
            for (int i = 1; i <= 3; i++)
            {
                bool flag = true;
                foreach (KeyValuePair<string, int> @default in DifficultySettingsData.GetDefaults(i))
                {
                    if (DifficultySettingsData.current.settingsNamed.ContainsKey(@default.Key) && DifficultySettingsData.current.settingsNamed[@default.Key] != @default.Value)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetCurentScoreMultiplier()
        {
            List<global::DBDef.Difficulty> type = DataBase.GetType<global::DBDef.Difficulty>();
            if (type == null || type.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            float num = 0f;
            float[] array = new float[3];
            int[] array2 = new int[3] { 100, 150, 200 };
            foreach (global::DBDef.Difficulty v in type)
            {
                int a = v.setting.Length - 1;
                int num2 = Array.FindIndex(v.setting, (DifficultyOption o) => o.difficulty == v.fullValue);
                for (int i = 0; i < 3; i++)
                {
                    DifficultyOption difficultyOption = v.setting[Mathf.Min(a, i + num2)];
                    array[i] += difficultyOption.difficulty;
                }
            }
            foreach (global::DBDef.Difficulty item in type)
            {
                if (DifficultySettingsData.current.settingsNamed.ContainsKey(item.name))
                {
                    int num3 = DifficultySettingsData.current.settingsNamed[item.name];
                    num += item.setting[num3].difficulty;
                }
            }
            if (num > array[1])
            {
                float num4 = array[2] - array[1];
                float num5 = (num - array[1]) / num4;
                return Mathf.RoundToInt((float)array2[1] + (float)(array2[2] - array2[1]) * num5);
            }
            float num6 = array[1] - array[0];
            float num7 = (num - array[0]) / num6;
            return Mathf.RoundToInt((float)array2[0] + (float)(array2[1] - array2[0]) * num7);
        }

        public static int GetCurentScoreMultiplierCached()
        {
            if (DifficultySettingsData.current.diffScoreCached == 0)
            {
                DifficultySettingsData.current.diffScoreCached = DifficultySettingsData.GetCurentScoreMultiplier();
            }
            return DifficultySettingsData.current.diffScoreCached;
        }

        public static DifficultyOption GetSetting(string name)
        {
            DifficultySettingsData.EnsureLoaded();
            if (DifficultySettingsData.current.dSettings == null)
            {
                DifficultySettingsData.current.dSettings = new Dictionary<string, DifficultyOption>();
            }
            if (!DifficultySettingsData.current.dSettings.ContainsKey(name))
            {
                DifficultySettingsData.current.dSettings[name] = DifficultySettingsData.GetSettingOption(name);
            }
            return DifficultySettingsData.current.dSettings[name];
        }

        public static int GetSettingAsInt(string name, bool withWarnings = false)
        {
            try
            {
                DifficultyOption setting = DifficultySettingsData.GetSetting(name);
                if (setting != null)
                {
                    return Convert.ToInt32(setting.value);
                }
            }
            catch (Exception message)
            {
                Debug.LogError(message);
            }
            return 0;
        }

        public static float GetSettingAsFloat(string name)
        {
            try
            {
                DifficultyOption setting = DifficultySettingsData.GetSetting(name);
                if (setting != null)
                {
                    return Convert.ToSingle(setting.value, new CultureInfo("en-US"));
                }
            }
            catch (Exception message)
            {
                Debug.LogError(message);
            }
            return 0f;
        }

        private static int GetSettingIndex(string name, bool withWarnings = false)
        {
            if (DataBase.GetType<global::DBDef.Difficulty>().FindIndex((global::DBDef.Difficulty o) => o.name == name) < 0)
            {
                if (withWarnings)
                {
                    Debug.LogWarning("Unknown difficulty rank");
                }
                return -1;
            }
            if (DifficultySettingsData.current.settingsNamed != null && DifficultySettingsData.current.settingsNamed.ContainsKey(name))
            {
                return DifficultySettingsData.current.settingsNamed[name];
            }
            if (withWarnings)
            {
                Debug.LogWarning("Unknown difficulty setting");
            }
            return -1;
        }

        private static DifficultyOption GetSettingOption(string name, bool withWarnings = false)
        {
            List<global::DBDef.Difficulty> type = DataBase.GetType<global::DBDef.Difficulty>();
            if (type.FindIndex((global::DBDef.Difficulty o) => o.name == name) < 0)
            {
                if (withWarnings)
                {
                    Debug.LogWarning("Unknown difficulty rank");
                }
                return null;
            }
            if (DifficultySettingsData.current.settingsNamed != null && DifficultySettingsData.current.settingsNamed.ContainsKey(name))
            {
                int num = DifficultySettingsData.current.settingsNamed[name];
                return type.Find((global::DBDef.Difficulty o) => o.name == name).setting[num];
            }
            if (withWarnings)
            {
                Debug.LogWarning("Unknown difficulty setting");
            }
            return null;
        }

        public static int GetTownDistance()
        {
            if (DifficultySettingsData.current == null)
            {
                return 4;
            }
            if (DifficultySettingsData.current.townDistance < 4)
            {
                int num = DifficultySettingsData.GetSettingAsInt("UI_TOWN_DISTANCE");
                if (num < 4)
                {
                    num = 4;
                }
                DifficultySettingsData.current.townDistance = num;
            }
            return DifficultySettingsData.current.townDistance;
        }
    }
}
