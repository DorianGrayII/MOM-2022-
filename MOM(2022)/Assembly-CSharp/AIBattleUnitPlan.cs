using DBDef;
using System;
using System.Collections.Generic;

public class AIBattleUnitPlan
{
    public DBClass source;
    public int sourceValue;
    public bool obsolete;
    public int attributeCount;
    public List<AIAttackOption> attacknOptions = new List<AIAttackOption>();
}

