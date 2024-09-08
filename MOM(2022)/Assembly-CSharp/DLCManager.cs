using System.Collections.Generic;

public class DLCManager
{
    public enum DLCs
    {
        Dlc0 = 1073741824,
        Dlc1 = 1,
        Dlc2 = 2,
        Dlc3 = 4
    }

    public const string USE_DLC_KEY = "UseDLC";

    private static int useDLC;

    public static List<DLCs> paidDlcList = new List<DLCs>
    {
        DLCs.Dlc2,
        DLCs.Dlc3
    };

    public static HashSet<DLCs> ownedDLC = new HashSet<DLCs>();

    public static Dictionary<string, DLCs> dlcIdentifiers = new Dictionary<string, DLCs>
    {
        {
            "Dlc0",
            DLCs.Dlc0
        },
        {
            "Dlc1",
            DLCs.Dlc1
        },
        {
            "Dlc2",
            DLCs.Dlc2
        },
        {
            "Dlc3",
            DLCs.Dlc3
        }
    };

    public static bool IsDlcOwned(DLCs d)
    {
        if (DLCManager.ownedDLC.Contains(d))
        {
            return true;
        }
        return false;
    }

    public static bool IsDlcOwned(string d)
    {
        if (d == null)
        {
            return true;
        }
        if (DLCManager.dlcIdentifiers.ContainsKey(d))
        {
            return DLCManager.IsDlcOwned(DLCManager.dlcIdentifiers[d]);
        }
        return false;
    }

    public static void SetActiveDLC(int d)
    {
        DLCManager.useDLC = d;
    }

    public static bool IsDlcActive(string d)
    {
        if (d == null)
        {
            return true;
        }
        if (!DLCManager.IsDlcOwned(d))
        {
            return false;
        }
        if (DLCManager.dlcIdentifiers.ContainsKey(d))
        {
            return (int)((uint)DLCManager.useDLC & (uint)DLCManager.dlcIdentifiers[d]) > 0;
        }
        return false;
    }

    public static bool IsDlcActive(int d)
    {
        if (d < 0)
        {
            return true;
        }
        return (DLCManager.useDLC & d) > 0;
    }

    public static int GetActiveDLC()
    {
        return DLCManager.useDLC;
    }
}
