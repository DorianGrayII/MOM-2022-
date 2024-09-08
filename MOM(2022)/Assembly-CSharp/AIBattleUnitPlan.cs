using System.Collections.Generic;
using DBDef;

public class AIBattleUnitPlan
{
    public DBClass source;

    public int sourceValue;

    public bool obsolete;

    public int attributeCount;

    public List<AIAttackOption> attacknOptions = new List<AIAttackOption>();
}
