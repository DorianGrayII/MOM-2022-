namespace MOM
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;

    public static class Globals
    {
        public static Dictionary<string, object> pureDB = new Dictionary<string, object>();
        public static Dictionary<string, Dictionary<string, object>> dataBase = new Dictionary<string, Dictionary<string, object>>();
        public static Dictionary<System.Type, Dictionary<Enum, object>> typeDataBase = new Dictionary<System.Type, Dictionary<Enum, object>>();
        private static Dictionary<string, System.Type> nameToType = new Dictionary<string, System.Type>();
        private static Dictionary<System.Type, Dictionary<string, object>> factoryResults = new Dictionary<System.Type, Dictionary<string, object>>();
        private static Dictionary<System.Type, Dictionary<Enum, object>> typeFactoryResults = new Dictionary<System.Type, Dictionary<Enum, object>>();
        private static Dictionary<System.Type, Dictionary<string, object>> customRuntimeData = new Dictionary<System.Type, Dictionary<string, object>>();
        public static CultureInfo floatingPointCultureInfo;

        public static CultureInfo GetCultureInfo()
        {
            return floatingPointCultureInfo;
        }

        public static T GetDynamicData<T>(string ID)
        {
            if (customRuntimeData.Count == 0)
            {
                InitializeDynamicData();
            }
            if (!string.IsNullOrEmpty(ID))
            {
                System.Type key = typeof(T);
                if (key == typeof(string))
                {
                    key = typeof(string);
                }
                if (!customRuntimeData.ContainsKey(key))
                {
                    Debug.LogWarning("[ERROR]Getting unexpected type " + key.ToString() + " with name " + ID);
                    return default(T);
                }
                if (customRuntimeData[key].ContainsKey(ID))
                {
                    return (T) customRuntimeData[key][ID];
                }
                Debug.LogWarning("[ERROR]Getting unexpected ID of " + ID + " of type " + key.ToString());
            }
            return default(T);
        }

        public static void InitializeDynamicData()
        {
            customRuntimeData[typeof(int)] = new Dictionary<string, object>();
            customRuntimeData[typeof(string)] = new Dictionary<string, object>();
            customRuntimeData[typeof(bool)] = new Dictionary<string, object>();
            customRuntimeData[typeof(float)] = new Dictionary<string, object>();
        }

        public static void RemoveDynamicData<T>(string ID)
        {
            if (customRuntimeData.Count == 0)
            {
                InitializeDynamicData();
            }
            if (!string.IsNullOrEmpty(ID))
            {
                System.Type key = typeof(T);
                if (key == typeof(string))
                {
                    key = typeof(string);
                }
                if (customRuntimeData.ContainsKey(key) && customRuntimeData[key].ContainsKey(ID))
                {
                    customRuntimeData[key].Remove(ID);
                }
            }
        }

        public static void SetDynamicData<T>(string ID, T data)
        {
            if (customRuntimeData.Count == 0)
            {
                InitializeDynamicData();
            }
            if (!string.IsNullOrEmpty(ID))
            {
                System.Type key = typeof(T);
                if (key == typeof(string))
                {
                    key = typeof(string);
                }
                if (!customRuntimeData.ContainsKey(key))
                {
                    customRuntimeData[key] = new Dictionary<string, object>();
                }
                customRuntimeData[key][ID] = data;
            }
        }
    }
}

