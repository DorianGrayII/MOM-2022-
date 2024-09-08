// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.CraftItemSpell
using MOM;
using ProtoBuf;

[ProtoContract]
public class CraftItemSpell
{
    [ProtoMember(1)]
    public int cost;

    [ProtoMember(2)]
    public Artefact artefact = new Artefact();
}
