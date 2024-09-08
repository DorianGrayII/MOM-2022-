using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MOM
{
    public static class Globals
    {
        public static Dictionary<string, object> pureDB = new Dictionary<string, object>();

        public static Dictionary<string, Dictionary<string, object>> dataBase = new Dictionary<string, Dictionary<string, object>>();

        public static Dictionary<Type, Dictionary<Enum, object>> typeDataBase = new Dictionary<Type, Dictionary<Enum, object>>();

        private static Dictionary<string, Type> nameToType = new Dictionary<string, Type>();

        private static Dictionary<Type, Dictionary<string, object>> factoryResults = new Dictionary<Type, Dictionary<string, object>>();

        private static Dictionary<Type, Dictionary<Enum, object>> typeFactoryResults = new Dictionary<Type, Dictionary<Enum, object>>();

        private static Dictionary<Type, Dictionary<string, object>> customRuntimeData = new Dictionary<Type, Dictionary<string, object>>();

        public static CultureInfo floatingPointCultureInfo;

        public static void InitializeDynamicData()
        {
            Globals.customRuntimeData[typeof(int)] = new Dictionary<string, object>();
            Globals.customRuntimeData[typeof(string)] = new Dictionary<string, object>();
            Globals.customRuntimeData[typeof(bool)] = new Dictionary<string, object>();
            Globals.customRuntimeData[typeof(float)] = new Dictionary<string, object>();
        }

        public static void SetDynamicData<T>(string ID, T data)
        {
            if (Globals.customRuntimeData.Count == 0)
            {
                Globals.InitializeDynamicData();
            }
            if (!string.IsNullOrEmpty(ID))
            {
                Type typeFromHandle = typeof(T);
                if (typeFromHandle == typeof(string))
                {
                    typeFromHandle = typeof(string);
                }
                if (!Globals.customRuntimeData.ContainsKey(typeFromHandle))
                {
                    Globals.customRuntimeData[typeFromHandle] = new Dictionary<string, object>();
                }
                Globals.customRuntimeData[typeFromHandle][ID] = data;
            }
        }

        public static T GetDynamicData<T>(string ID)
        {
            if (Globals.customRuntimeData.Count == 0)
            {
                Globals.InitializeDynamicData();
            }
            if (string.IsNullOrEmpty(ID))
            {
                return default(T);
            }
            Type typeFromHandle = typeof(T);
            if (typeFromHandle == typeof(string))
            {
                typeFromHandle = typeof(string);
            }
            if (Globals.customRuntimeData.ContainsKey(typeFromHandle))
            {
                if (Globals.customRuntimeData[typeFromHandle].ContainsKey(ID))
                {
                    return (T)Globals.customRuntimeData[typeFromHandle][ID];
                }
                Debug.LogWarning("[ERROR]Getting unexpected ID of " + ID + " of type " + typeFromHandle.ToString());
                return default(T);
            }
            Debug.LogWarning("[ERROR]Getting unexpected type " + typeFromHandle.ToString() + " with name " + ID);
            return default(T);
        }

        public static void RemoveDynamicData<T>(string ID)
        {
            if (Globals.customRuntimeData.Count == 0)
            {
                Globals.InitializeDynamicData();
            }
            if (!string.IsNullOrEmpty(ID))
            {
                Type typeFromHandle = typeof(T);
                if (typeFromHandle == typeof(string))
                {
                    typeFromHandle = typeof(string);
                }
                if (Globals.customRuntimeData.ContainsKey(typeFromHandle) && Globals.customRuntimeData[typeFromHandle].ContainsKey(ID))
                {
                    Globals.customRuntimeData[typeFromHandle].Remove(ID);
                }
            }
        }

        public static CultureInfo GetCultureInfo()
        {
            return Globals.floatingPointCultureInfo;
        }
    }
}
