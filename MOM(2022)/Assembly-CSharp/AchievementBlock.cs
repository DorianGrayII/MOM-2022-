using System.Collections.Generic;
using ProtoBuf;

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
        AchievementBlock achievementBlock = new AchievementBlock();
        achievementBlock.achProgress = new List<int>();
        for (int i = 0; i < 12; i++)
        {
            achievementBlock.achProgress.Add(0);
        }
        return achievementBlock;
    }
}
