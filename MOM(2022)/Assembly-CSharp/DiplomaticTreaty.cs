// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// DiplomaticTreaty
using DBDef;
using MOM;
using ProtoBuf;

[ProtoContract]
public class DiplomaticTreaty
{
    [ProtoMember(1)]
    public DBReference<Treaty> source;

    [ProtoMember(2)]
    public int length;

    [ProtoMember(3)]
    public int turnStarted;

    public DiplomaticTreaty()
    {
    }

    public DiplomaticTreaty(Treaty s)
    {
        this.source = s;
        if (s.length > 0)
        {
            this.length = s.length;
        }
        this.turnStarted = TurnManager.GetTurnNumber();
    }

    public int TimeLeft()
    {
        if (this.length < 1)
        {
            return -1;
        }
        int num = TurnManager.GetTurnNumber() - this.turnStarted;
        return this.length - num;
    }

    public bool Equal(DiplomaticTreaty t)
    {
        if (t == null)
        {
            return false;
        }
        if (this.source.dbName != t.source.dbName)
        {
            return false;
        }
        return true;
    }
}
