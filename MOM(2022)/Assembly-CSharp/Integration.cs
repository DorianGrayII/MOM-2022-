using Steamworks;
using UnityEngine;

public class Integration : MonoBehaviour
{
    private static Integration instance;

    private bool postStart;

    private bool listOfAchi;

    private void Start()
    {
        Integration.instance = this;
        this.Initialize();
    }

    public void Initialize()
    {
        base.gameObject.AddComponent<SteamManager>();
    }

    public static bool IsReady()
    {
        return SteamManager.Initialized;
    }

    private void Update()
    {
        if (!this.postStart && Integration.IsReady())
        {
            this.postStart = true;
            CSteamID steamID = SteamUser.GetSteamID();
            Debug.Log("Steam user stats downloaded with: " + SteamUserStats.RequestUserStats(steamID).ToString());
        }
    }

    public static string GetName()
    {
        if (!Integration.IsReady())
        {
            return "NotInitialized";
        }
        return SteamFriends.GetPersonaName();
    }

    public static bool GetAchievement(string name)
    {
        if (!Integration.IsReady())
        {
            return false;
        }
        SteamUserStats.GetAchievement(name, out var pbAchieved);
        return pbAchieved;
    }

    public static void SetAchievement(string name)
    {
        if (Integration.IsReady())
        {
            SteamUserStats.GetAchievement(name, out var pbAchieved);
            if (!pbAchieved)
            {
                SteamUserStats.SetAchievement(name);
            }
            else
            {
                Debug.Log("Achievement " + name + " is not resend to steam");
            }
        }
    }

    public static void SubmitAchievements()
    {
        if (Integration.IsReady())
        {
            SteamUserStats.StoreStats();
        }
    }

    public static void ResetAchievements()
    {
        for (int i = 0; i < 12; i++)
        {
            AchievementManager.Achievement achievement = (AchievementManager.Achievement)i;
            SteamUserStats.ClearAchievement(achievement.ToString());
        }
    }
}
