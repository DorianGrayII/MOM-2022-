using System;
using System.Collections.Generic;
using System.IO;
using MHUtils;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class HallOfFameBlock
{
    [ProtoMember(1)]
    public List<HofEntry> values;

    public void Add(string portrait, string name, string race, int score)
    {
        this.EnsureInitialization();
        HofEntry item = new HofEntry
        {
            wizardPortrait = portrait,
            wizardName = name,
            wizardRace = race,
            wizardScore = score,
            isNew = true
        };
        this.values.ForEach(delegate(HofEntry o)
        {
            o.isNew = false;
        });
        this.values.Add(item);
        this.values.SortInPlace((HofEntry a, HofEntry b) => -a.wizardScore.CompareTo(b.wizardScore));
        if (this.values.Count > 10)
        {
            int count = this.values.Count - 10;
            this.values.RemoveRange(10, count);
        }
        HallOfFameBlock.Save(this);
    }

    public void EnsureInitialization()
    {
        if (this.values == null)
        {
            this.values = new List<HofEntry>();
        }
    }

    public static void Save(HallOfFameBlock block)
    {
        string pROFILES = MHApplication.PROFILES;
        if (!Directory.Exists(pROFILES))
        {
            Directory.CreateDirectory(pROFILES);
        }
        string path = Path.Combine(pROFILES, "hof.bin");
        using (MemoryStream memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, block);
            memoryStream.Position = 0L;
            byte[] bytes = memoryStream.ToArray();
            File.WriteAllBytes(path, bytes);
        }
    }

    public static HallOfFameBlock Load()
    {
        string pROFILES = MHApplication.PROFILES;
        if (!Directory.Exists(pROFILES))
        {
            Directory.CreateDirectory(pROFILES);
        }
        string path = Path.Combine(pROFILES, "hof.bin");
        if (!File.Exists(path))
        {
            return new HallOfFameBlock();
        }
        try
        {
            byte[] array = File.ReadAllBytes(path);
            using (MemoryStream memoryStream = new MemoryStream(array))
            {
                memoryStream.Write(array, 0, array.Length);
                memoryStream.Position = 0L;
                return Serializer.Deserialize<HallOfFameBlock>(memoryStream);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Warning! Hall of Fame cannot be loaded: \n" + ex);
            return new HallOfFameBlock();
        }
    }
}
