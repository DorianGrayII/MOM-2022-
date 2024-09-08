using System.Collections.Generic;

namespace MHUtils
{
    public class MHDataStorage
    {
        private static Dictionary<string, object> data = new Dictionary<string, object>();

        public static void Set(string name, object d)
        {
            MHDataStorage.data[name] = d;
        }

        public static object Get(string name)
        {
            if (MHDataStorage.data.ContainsKey(name))
            {
                return MHDataStorage.data[name];
            }
            return null;
        }

        public static T Get<T>(string name) where T : class
        {
            if (MHDataStorage.data.ContainsKey(name))
            {
                return MHDataStorage.data[name] as T;
            }
            return null;
        }
    }
}
