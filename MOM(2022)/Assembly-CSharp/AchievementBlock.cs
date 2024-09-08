using ProtoBuf;
using System;
using System.Collections.Generic;

[ProtoContract]
public class AchievementBlock
{
    [ProtoMember(1)]
    public List<int> achProgress;
    [ProtoMember(10)]
    public List<string> winWithWizard;
    [ProtoMember(11)]
    public int conqueredRuins;
    [ProtoMember(12)]
    public int conqueredNeutralTowns;
    [ProtoMember(13)]
    public List<string> portalMastering;

    public static AchievementBlock Factory()
    {
        AchievementBlock block = new AchievementBlock {
            achProgress = new List<int>()
        };
        for (int i = 0; i < 12; i++)
        {
            block.achProgress.Add(0);
        }
        return block;
    }
}

