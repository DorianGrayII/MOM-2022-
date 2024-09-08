using System;
using System.Collections.Generic;

public class DLCManager
{
    public const string USE_DLC_KEY = "UseDLC";
    private static int useDLC;
    public static List<DLCs> paidDlcList;
    public static HashSet<DLCs> ownedDLC;
    public static Dictionary<string, DLCs> dlcIdentifiers;

    static DLCManager()
    {
        List<DLCs> list1 = new List<DLCs>();
        list1.Add(DLCs.Dlc2);
        list1.Add(DLCs.Dlc3);
        paidDlcList = list1;
        ownedDLC = new HashSet<DLCs>();
        Dictionary<string, DLCs> dictionary1 = new Dictionary<string, DLCs>();
        dictionary1.Add("Dlc0", DLCs.Dlc0);
        dictionary1.Add("Dlc1", DLCs.Dlc1);
        dictionary1.Add("Dlc2", DLCs.Dlc2);
        dictionary1.Add("Dlc3", DLCs.Dlc3);
        dlcIdentifiers = dictionary1;
    }

    public static int GetActiveDLC()
    {
        return useDLC;
    }

    public static bool IsDlcActive(int d)
    {
        return ((d >= 0) ? ((useDLC & d) > 0) : true);
    }

    public static bool IsDlcActive(string d)
    {
        return ((d != null) ? (IsDlcOwned(d) ? (dlcIdentifiers.ContainsKey(d) && ((useDLC & dlcIdentifiers[d]) > 0)) : false) : true);
    }

    public static bool IsDlcOwned(DLCs d)
    {
        return ownedDLC.Contains(d);
    }

    public static bool IsDlcOwned(string d)
    {
        return ((d != null) ? (dlcIdentifiers.ContainsKey(d) && IsDlcOwned(dlcIdentifiers[d])) : true);
    }

    public static void SetActiveDLC(int d)
    {
        useDLC = d;
    }

    public enum DLCs
    {
        Dlc0 = 0x40000000,
        Dlc1 = 1,
        Dlc2 = 2,
        Dlc3 = 4
    }
}

