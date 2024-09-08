using System;
using System.Collections.Generic;
using System.IO;
using DBDef;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;

public class AchievementManager
{
    public enum Achievement
    {
        SpellmasterOfMagic = 0,
        WarmasterOfMagic = 1,
        MasterArcheologist = 2,
        NodeMaster = 3,
        WizardSupreme = 4,
        LookInTheMyrror = 5,
        TasteOfDespair = 6,
        CleansingTheLand = 7,
        PowerOfKnowledge = 8,
        WhichWizard = 9,
        PortalMaster = 10,
        JustGettingStarted = 11,
        MAX = 12
    }

    public static AchievementBlock data;

    public static void Progress(Achievement a, string achData = null, bool forcedSend = false)
    {
        if (AchievementManager.data == null)
        {
            AchievementManager.data = AchievementBlock.Factory();
        }
        if (AchievementManager.data.achProgress[(int)a] == 0)
        {
            switch (a)
            {
            case Achievement.MasterArcheologist:
                AchievementManager.data.conqueredRuins++;
                if (AchievementManager.data.conqueredRuins >= 100)
                {
                    AchievementManager.data.achProgress[(int)a]++;
                }
                break;
            case Achievement.NodeMaster:
                if (GameManager.Get() != null && GameManager.Get().achievementChaosNode && GameManager.Get().achievementNatureNode && GameManager.Get().achievementSorceryNode)
                {
                    AchievementManager.data.achProgress[(int)a]++;
                }
                break;
            case Achievement.CleansingTheLand:
                AchievementManager.data.conqueredNeutralTowns++;
                if (AchievementManager.data.conqueredNeutralTowns >= 50)
                {
                    AchievementManager.data.achProgress[(int)a]++;
                }
                break;
            case Achievement.WhichWizard:
            {
                AchievementBlock achievementBlock = AchievementManager.data;
                if (achievementBlock.winWithWizard == null)
                {
                    achievementBlock.winWithWizard = new List<string>();
                }
                if (!AchievementManager.data.winWithWizard.Contains(achData))
                {
                    AchievementManager.data.winWithWizard.Add(achData);
                    if (AchievementManager.data.winWithWizard.Count > 4)
                    {
                        AchievementManager.data.achProgress[(int)a]++;
                    }
                }
                break;
            }
            case Achievement.PortalMaster:
            {
                AchievementBlock achievementBlock = AchievementManager.data;
                if (achievementBlock.portalMastering == null)
                {
                    achievementBlock.portalMastering = new List<string>();
                }
                if (!AchievementManager.data.portalMastering.Contains(achData))
                {
                    List<global::DBDef.Location> list = DataBase.GetType<global::DBDef.Location>().FindAll((global::DBDef.Location o) => o.locationType == ELocationType.PlaneTower);
                    AchievementManager.data.portalMastering.Add(achData);
                    if (AchievementManager.data.portalMastering.Count >= list.Count)
                    {
                        AchievementManager.data.achProgress[(int)a]++;
                    }
                }
                break;
            }
            default:
                AchievementManager.data.achProgress[(int)a]++;
                break;
            }
            AchievementManager.Send();
            AchievementManager.Save();
        }
        else if (forcedSend)
        {
            AchievementManager.Send();
            AchievementManager.Save();
        }
    }

    public static void Save()
    {
        string pROFILES = MHApplication.PROFILES;
        if (!Directory.Exists(pROFILES))
        {
            Directory.CreateDirectory(pROFILES);
        }
        if (AchievementManager.data == null)
        {
            AchievementManager.data = AchievementBlock.Factory();
        }
        string path = Path.Combine(pROFILES, "achievements.bin");
        using (MemoryStream memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, AchievementManager.data);
            memoryStream.Position = 0L;
            byte[] bytes = memoryStream.ToArray();
            File.WriteAllBytes(path, bytes);
        }
    }

    public static void Load()
    {
        string pROFILES = MHApplication.PROFILES;
        if (!Directory.Exists(pROFILES))
        {
            Directory.CreateDirectory(pROFILES);
        }
        string path = Path.Combine(pROFILES, "achievements.bin");
        if (!File.Exists(path))
        {
            AchievementManager.data = AchievementBlock.Factory();
            return;
        }
        try
        {
            byte[] array = File.ReadAllBytes(path);
            using (MemoryStream memoryStream = new MemoryStream(array))
            {
                memoryStream.Write(array, 0, array.Length);
                memoryStream.Position = 0L;
                AchievementManager.data = Serializer.Deserialize<AchievementBlock>(memoryStream);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Warning! Hall of Fame cannot be loaded: \n" + ex);
            AchievementManager.data = AchievementBlock.Factory();
        }
    }

    public static void Send()
    {
        if (AchievementManager.data == null)
        {
            return;
        }
        for (int i = 0; i < 12; i++)
        {
            Achievement achievement = (Achievement)i;
            string achievement2 = achievement.ToString();
            if (AchievementManager.data.achProgress != null && AchievementManager.data.achProgress.Count > i && AchievementManager.data.achProgress[i] > 0)
            {
                Integration.SetAchievement(achievement2);
            }
        }
        Integration.SubmitAchievements();
    }
}
