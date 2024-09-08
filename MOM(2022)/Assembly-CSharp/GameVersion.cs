using MHUtils;
using System;
using System.IO;
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

    public static int FirstDifference(string a, string b)
    {
        if ((a == null) || (b == null))
        {
            return 1;
        }
        string[] strArray = a.Split(".", StringSplitOptions.None);
        string[] strArray2 = b.Split(".", StringSplitOptions.None);
        for (int i = 1; i < 6; i++)
        {
            if ((strArray.Length < i) || ((strArray2.Length < i) || (strArray[i - 1] != strArray2[i - 1])))
            {
                return i;
            }
        }
        return 10;
    }

    private static string GetChangeset()
    {
        return "";
    }

    public static string GetGameVersion()
    {
        Initialize();
        return gameVersion;
    }

    public static string GetGameVersionFull()
    {
        Initialize();
        return gameVersionFull;
    }

    private static string GetHashSHort()
    {
        string s = ScriptLoader.GetScriptHash().ToString();
        return (Trim(DataBase.GetDBHash().ToString(), 3) + Trim(s, 3));
    }

    private static string GetMajorVersion()
    {
        return majorVer.ToString();
    }

    private static string GetMinorVersion()
    {
        return Trim(minorVer.ToString(), 2);
    }

    public static string GetVersion(bool create)
    {
        if (!create)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
            if (File.Exists(path))
            {
                gameVersionFull = File.ReadAllText(path);
                if (gameVersionFull.Length > 6)
                {
                    string[] textArray1 = new string[] { gameVersionFull.Substring(0, gameVersionFull.Length - 12), Trim(dbVersion, 3), Trim(scriptVersion, 3), ".", gameVersionFull.Substring(gameVersionFull.Length - 5, 5) };
                    return string.Concat(textArray1);
                }
            }
        }
        string[] textArray2 = new string[9];
        textArray2[0] = GetMajorVersion();
        textArray2[1] = ".";
        textArray2[2] = GetMinorVersion();
        textArray2[3] = ".";
        textArray2[4] = GetVersionIndexer();
        textArray2[5] = ".";
        textArray2[6] = GetHashSHort();
        textArray2[7] = ".";
        textArray2[8] = Trim(GetChangeset(), 5);
        return string.Concat(textArray2);
    }

    private static string GetVersionIndexer()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
        int num = 1;
        if (File.Exists(path))
        {
            string str2 = "";
            using (StreamReader reader = File.OpenText(path))
            {
                str2 = reader.ReadLine();
            }
            string[] strArray = str2.Split('.', StringSplitOptions.None);
            if (strArray.Length > 2)
            {
                try
                {
                    if (strArray[1] == GetMinorVersion())
                    {
                        num = Convert.ToInt32(strArray[2]) + 1;
                    }
                }
                catch (Exception exception1)
                {
                    Debug.Log(exception1);
                }
            }
        }
        return Trim(num.ToString(), 2);
    }

    private static void Initialize()
    {
        if (!initialized)
        {
            initialized = true;
            dbVersion = DataBase.GetDBHash().ToString();
            scriptVersion = ScriptLoader.GetScriptHash().ToString();
            gameVersion = GetMajorVersion() + "." + GetMinorVersion();
            gameVersionFull = GetVersion(false);
        }
    }

    public static bool IsDBModified()
    {
        if (string.IsNullOrEmpty(dbVersion))
        {
            if (string.IsNullOrEmpty(gameVersionFull))
            {
                string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
                if (File.Exists(path))
                {
                    gameVersionFull = File.ReadAllText(path);
                }
            }
            if ((gameVersionFull != null) && (gameVersionFull.Length > 6))
            {
                string[] strArray = gameVersionFull.Split(".", StringSplitOptions.None);
                if (strArray.Length > 3)
                {
                    dbVersion = strArray[3].Substring(0, 3);
                }
            }
        }
        return (Trim(dbVersion, 3) == Trim(DataBase.GetDBHash().ToString(), 3));
    }

    public static string SaveBuildVersion()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ver.txt");
        string version = GetVersion(true);
        if (File.Exists(path))
        {
            File.WriteAllText(path, version);
        }
        else
        {
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.WriteLine(version);
            }
        }
        return version;
    }

    private static string Trim(string s, int size)
    {
        return ((s != null) ? ((s.Length > size) ? s.Substring(s.Length - size) : s.PadLeft(size, '0')) : "".PadLeft(size, '0'));
    }
}

