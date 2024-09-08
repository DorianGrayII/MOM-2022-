using System;
using System.IO;
using MHUtils;
using UnityEngine;

public class GameVersion
{
    private static int majorVer = 1;

    private static int minorVer = 9;

    private static bool initialized = false;

    private static string dbVersion;

    private static string scriptVersion;

    private static string gameVersion;

    private static string gameVersionFull;

    private static void Initialize()
    {
        if (!GameVersion.initialized)
        {
            GameVersion.initialized = true;
            GameVersion.dbVersion = DataBase.GetDBHash().ToString();
            GameVersion.scriptVersion = ScriptLoader.GetScriptHash().ToString();
            GameVersion.gameVersion = GameVersion.GetMajorVersion() + "." + GameVersion.GetMinorVersion();
            GameVersion.gameVersionFull = GameVersion.GetVersion(create: false);
        }
    }

    public static string GetGameVersion()
    {
        GameVersion.Initialize();
        return GameVersion.gameVersion;
    }

    public static string GetGameVersionFull()
    {
        GameVersion.Initialize();
        return GameVersion.gameVersionFull;
    }

    public static bool IsDBModified()
    {
        if (string.IsNullOrEmpty(GameVersion.dbVersion))
        {
            if (string.IsNullOrEmpty(GameVersion.gameVersionFull))
            {
                string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
                if (File.Exists(path))
                {
                    GameVersion.gameVersionFull = File.ReadAllText(path);
                }
            }
            if (GameVersion.gameVersionFull != null && GameVersion.gameVersionFull.Length > 6)
            {
                string[] array = GameVersion.gameVersionFull.Split('.');//, StringSplitOptions.None);
                if (array.Length > 3)
                {
                    GameVersion.dbVersion = array[3].Substring(0, 3);
                }
            }
        }
        return GameVersion.Trim(GameVersion.dbVersion, 3) == GameVersion.Trim(DataBase.GetDBHash().ToString(), 3);
    }

    public static string GetVersion(bool create)
    {
        if (!create)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
            if (File.Exists(path))
            {
                GameVersion.gameVersionFull = File.ReadAllText(path);
                if (GameVersion.gameVersionFull.Length > 6)
                {
                    return GameVersion.gameVersionFull.Substring(0, GameVersion.gameVersionFull.Length - 12) + GameVersion.Trim(GameVersion.dbVersion, 3) + GameVersion.Trim(GameVersion.scriptVersion, 3) + "." + GameVersion.gameVersionFull.Substring(GameVersion.gameVersionFull.Length - 5, 5);
                }
            }
        }
        return GameVersion.GetMajorVersion() + "." + GameVersion.GetMinorVersion() + "." + GameVersion.GetVersionIndexer() + "." + GameVersion.GetHashSHort() + "." + GameVersion.Trim(GameVersion.GetChangeset(), 5);
    }

    private static string GetMajorVersion()
    {
        return GameVersion.majorVer.ToString();
    }

    private static string GetMinorVersion()
    {
        return GameVersion.Trim(GameVersion.minorVer.ToString(), 2);
    }

    private static string GetVersionIndexer()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
        int num = 1;
        if (File.Exists(path))
        {
            string text = "";
            using (StreamReader streamReader = File.OpenText(path))
            {
                text = streamReader.ReadLine();
            }
            string[] array = text.Split('.');//, StringSplitOptions.None);
            if (array.Length > 2)
            {
                try
                {
                    if (array[1] == GameVersion.GetMinorVersion())
                    {
                        num = Convert.ToInt32(array[2]);
                        num++;
                    }
                }
                catch (Exception message)
                {
                    Debug.Log(message);
                }
            }
        }
        return GameVersion.Trim(num.ToString(), 2);
    }

    private static string GetHashSHort()
    {
        string s = DataBase.GetDBHash().ToString();
        return string.Concat(str1: GameVersion.Trim(ScriptLoader.GetScriptHash().ToString(), 3), str0: GameVersion.Trim(s, 3));
    }

    public static string SaveBuildVersion()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
        string version = GameVersion.GetVersion(create: true);
        if (!File.Exists(path))
        {
            using (StreamWriter streamWriter = File.CreateText(path))
            {
                streamWriter.WriteLine(version);
            }
        }
        else
        {
            File.WriteAllText(path, version);
        }
        return version;
    }

    private static string GetChangeset()
    {
        return "";
    }

    private static string Trim(string s, int size)
    {
        if (s == null)
        {
            return "".PadLeft(size, '0');
        }
        if (s.Length <= size)
        {
            return s.PadLeft(size, '0');
        }
        return s.Substring(s.Length - size);
    }

    public static int FirstDifference(string a, string b)
    {
        if (a == null || b == null)
        {
            return 1;
        }
        string[] array = a.Split('.'); //, StringSplitOptions.None);
        string[] array2 = b.Split('.'); //, StringSplitOptions.None);
        for (int i = 1; i < 6; i++)
        {
            if (array.Length < i || array2.Length < i || array[i - 1] != array2[i - 1])
            {
                return i;
            }
        }
        return 10;
    }
}
