using ProtoBuf;

[ProtoContract]
public class HofEntry
{
    [ProtoMember(1)]
    public string wizardPortrait;

    [ProtoMember(2)]
    public string wizardName;

    [ProtoMember(3)]
    public string wizardRace;

    [ProtoMember(4)]
    public int wizardScore;

    [ProtoMember(5)]
    public bool isNew;
}
