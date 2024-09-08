using Steamworks;
using System;
using UnityEngine;

public class Integration : MonoBehaviour
{
    private static Integration instance;
    private bool postStart;
    private bool listOfAchi;

    public static bool GetAchievement(string name)
    {
        bool flag;
        if (!IsReady())
        {
            return false;
        }
        SteamUserStats.GetAchievement(name, out flag);
        return flag;
    }

    public static string GetName()
    {
        return (IsReady() ? SteamFriends.GetPersonaName() : "NotInitialized");
    }

    public void Initialize()
    {
        base.gameObject.AddComponent<SteamManager>();
    }

    public static bool IsReady()
    {
        return SteamManager.Initialized;
    }

    public static void ResetAchievements()
    {
        for (int i = 0; i < 12; i++)
        {
            SteamUserStats.ClearAchievement(((AchievementManager.Achievement) i).ToString());
        }
    }

    public static void SetAchievement(string name)
    {
        if (IsReady())
        {
            bool flag;
            SteamUserStats.GetAchievement(name, out flag);
            if (!flag)
            {
                SteamUserStats.SetAchievement(name);
            }
            else
            {
                Debug.Log("Achievement " + name + " is not resend to steam");
            }
        }
    }

    private void Start()
    {
        instance = this;
        this.Initialize();
    }

    public static void SubmitAchievements()
    {
        if (IsReady())
        {
            SteamUserStats.StoreStats();
        }
    }

    private void Update()
    {
        if (!this.postStart && IsReady())
        {
            this.postStart = true;
            CSteamID steamID = SteamUser.GetSteamID();
            Debug.Log("Steam user stats downloaded with: " + SteamUserStats.RequestUserStats(steamID).ToString());
        }
    }
}

