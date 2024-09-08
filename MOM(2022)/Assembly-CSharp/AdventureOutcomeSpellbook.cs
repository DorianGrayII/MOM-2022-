using MOM;

public class AdventureOutcomeSpellbook : AdventureOutcome
{
    public override void Set(AdventureOutcomeDelta.Outcome o)
    {
        base.Set(o);
        base.GetComponentInChildren<GISpellbookItem>().Set(o.thing);
    }
}
