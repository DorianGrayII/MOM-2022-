namespace MHUtils
{
    using System;

    public class MHApplication
    {
        public static string DIR_NAME_EXTERNAL_ASSETS = "ExternalAssets";
        public static string DIR_NAME_PROFILES = "Profiles";
        public static string DIR_NAME_CACHE = "SaveCache";
        public static string DIR_NAME_MODS = "Mods";
        public static string EXTERNAL_ASSETS = Path.Combine(Application.dataPath, "..", DIR_NAME_EXTERNAL_ASSETS);
        public static string PROFILES = Path.Combine(Application.persistentDataPath, DIR_NAME_PROFILES);
        public static string CACHE = Path.Combine(Application.persistentDataPath, DIR_NAME_CACHE);
        public static string MODS = Path.Combine(Application.persistentDataPath, DIR_NAME_MODS);
    }
}

