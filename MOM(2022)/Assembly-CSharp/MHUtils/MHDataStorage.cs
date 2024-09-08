namespace MHUtils
{
    using System;
    using System.Collections.Generic;

    public class MHDataStorage
    {
        private static Dictionary<string, object> data = new Dictionary<string, object>();

        public static object Get(string name)
        {
            return (!data.ContainsKey(name) ? null : data[name]);
        }

        public static T Get<T>(string name) where T: class
        {
            if (data.ContainsKey(name))
            {
                return (data[name] as T);
            }
            return default(T);
        }

        public static void Set(string name, object d)
        {
            data[name] = d;
        }
    }
}

