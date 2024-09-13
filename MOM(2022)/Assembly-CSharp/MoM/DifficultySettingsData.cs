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

        public static NetDictionary<string, int> GetDefaults(int iDifficultyLevel, bool quickStart = false)
        {
            List<global::DBDef.Difficulty> lstDifficulty = DataBase.GetType<global::DBDef.Difficulty>();
            if (lstDifficulty == null || lstDifficulty.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            NetDictionary<string, int> netDictionary = new NetDictionary<string, int>();
            foreach (global::DBDef.Difficulty diff in lstDifficulty)
            {
                int num = 0;
                int num2 = -1;
                num = 0;
                if (diff.fullValue > 0f)
                {
                    for (int i = 0; i < diff.setting.Length; i++)
                    {
                        if (quickStart && diff.setting[i].quickStart)
                        {
                            num2 = i;
                        }
                        if (diff.setting[i].collection <= iDifficultyLevel)
                        {
                            num = i;
                        }
                    }
                }
                if (quickStart && num2 != -1)
                {
                    num = num2;
                }
                netDictionary[diff.name] = num;
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

        public static void SetDefaults(int iDifficultyLevel, bool quickStart = false)
        {
            NetDictionary<string, int> defaults = DifficultySettingsData.GetDefaults(iDifficultyLevel, quickStart);
            List<global::DBDef.Difficulty> lstDifficulty = DataBase.GetType<global::DBDef.Difficulty>();
            DifficultySettingsData.current.dSettings = null;
            DifficultySettingsData difficultySettingsData = DifficultySettingsData.current;
            if (difficultySettingsData.settingsNamed == null)
            {
                difficultySettingsData.settingsNamed = new NetDictionary<string, int>();
            }
            for (int i = 0; i < lstDifficulty.Count; i++)
            {
                DifficultySettingsData.current.settingsNamed[lstDifficulty[i].name] = defaults[lstDifficulty[i].name];
                PlayerPrefs.SetInt("DIFF:" + lstDifficulty[i].name, defaults[lstDifficulty[i].name]);
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
            List<global::DBDef.Difficulty> lstDifficulty = DataBase.GetType<global::DBDef.Difficulty>();
            if (lstDifficulty == null || lstDifficulty.Count <= 0)
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
            for (int i = 0; i < lstDifficulty.Count; i++)
            {
                global::DBDef.Difficulty difficulty = lstDifficulty[i];
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

        const int dfEasy = 1;
        const int dfMedium = 2;
        const int dfHard = 3;
        const int dfExtreme = 4;

        public static DifficultySettingsData Create(int iDifficultyLevel = 0)
        {
            DifficultySettingsData difficultySettingsData = new DifficultySettingsData();
            List<global::DBDef.Difficulty> lstDifficulty = DataBase.GetType<global::DBDef.Difficulty>();
            if (lstDifficulty == null || lstDifficulty.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            NetDictionary<string, int> defaults = DifficultySettingsData.GetDefaults(iDifficultyLevel);
            difficultySettingsData.interruptDelay = 35;
            DifficultySettingsData difficultySettingsData2 = difficultySettingsData;
            if (difficultySettingsData2.settingsNamed == null)
            {
                difficultySettingsData2.settingsNamed = new NetDictionary<string, int>();
            }
            for (int i = 0; i < lstDifficulty.Count; i++)
            {
                global::DBDef.Difficulty diff = lstDifficulty[i];
                string strKey = "DIFF:" + diff.name;

                if (PlayerPrefs.HasKey(strKey))
                {
                    int num = PlayerPrefs.GetInt(strKey);
                    if (diff.setting.Length > num)
                    {
                        difficultySettingsData.settingsNamed[diff.name] = num;
                        continue;
                    }
                }
                difficultySettingsData.settingsNamed[diff.name] = defaults[diff.name];
            }
            DifficultySettingsData.current = difficultySettingsData;
            return difficultySettingsData;
        }

        public static int GetCurentDifficultyRank()
        {
            for (int iRank = dfEasy; iRank <= dfExtreme; iRank++)
            {
                bool flag = true;
                foreach (KeyValuePair<string, int> @default in DifficultySettingsData.GetDefaults(iRank))
                {
                    if (DifficultySettingsData.current.settingsNamed.ContainsKey(@default.Key) && 
                        DifficultySettingsData.current.settingsNamed[@default.Key] != @default.Value)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return iRank;
                }
            }
            return -1;
        }

        public static int GetCurentScoreMultiplier()
        {
            List<global::DBDef.Difficulty> lstDifficulty = DataBase.GetType<global::DBDef.Difficulty>();
            if (lstDifficulty == null || lstDifficulty.Count <= 0)
            {
                Debug.LogError("DIFFICULTY not found in database");
            }
            float fDifficulty = 0f;
            float[] array = new float[4];
            int[] array2 = new int[4] { 100, 150, 200, 250 };
            foreach (global::DBDef.Difficulty diff in lstDifficulty)
            {
                int a = diff.setting.Length - 1;
                int iIndexFound = Array.FindIndex(diff.setting, (DifficultyOption o) => o.difficulty == diff.fullValue);
                if (iIndexFound >= 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        DifficultyOption diffOption = diff.setting[Mathf.Min(a, i + iIndexFound)];
                        array[i] += diffOption.difficulty;
                    }
                }
                else
                {
                    Debug.LogWarning("Unknown difficulty rank : " + diff.name + " full value : " + diff.fullValue);
                    for (int i = 0; i < diff.setting.Length; i++)
                    {
                        DifficultyOption diffOption = diff.setting[i];
                        Debug.LogWarning("   difficulty : " + diffOption.difficulty + " value : " + diffOption.value);
                    }
                }
            }
            foreach (global::DBDef.Difficulty diff in lstDifficulty)
            {
                if (DifficultySettingsData.current.settingsNamed.ContainsKey(diff.name))
                {
                    int num3 = DifficultySettingsData.current.settingsNamed[diff.name];
                    fDifficulty += diff.setting[num3].difficulty;
                }
            }
            if (fDifficulty > array[2])
            {
                float fFactor = (fDifficulty - array[2]) / (array[3] - array[2]);
                return Mathf.RoundToInt((float)array2[2] + (float)(array2[3] - array2[2]) * fFactor);
            }
            else if (fDifficulty > array[1])
            {
                float fFactor = (fDifficulty - array[1]) / (array[2] - array[1]);
                return Mathf.RoundToInt((float)array2[1] + (float)(array2[2] - array2[1]) * fFactor);
            }
            else
            {
                float fFactor = (fDifficulty - array[0]) / (array[1] - array[0]);
                return Mathf.RoundToInt((float)array2[0] + (float)(array2[1] - array2[0]) * fFactor);
            }
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
                    Debug.LogWarning("Unknown difficulty rank : " + name);
                }
                return -1;
            }
            if (DifficultySettingsData.current.settingsNamed != null && DifficultySettingsData.current.settingsNamed.ContainsKey(name))
            {
                return DifficultySettingsData.current.settingsNamed[name];
            }
            if (withWarnings)
            {
                Debug.LogWarning("Unknown difficulty setting : " + name);
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
                    Debug.LogWarning("Unknown difficulty rank : " + name);
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
                Debug.LogWarning("Unknown difficulty setting : " + name);
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
