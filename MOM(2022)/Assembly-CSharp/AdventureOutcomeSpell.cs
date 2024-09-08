using DBDef;
using MOM;

public class AdventureOutcomeSpell : AdventureOutcome
{
    public override void Set(AdventureOutcomeDelta.Outcome o)
    {
        base.Set(o);
        base.GetComponent<SpellItem>().Set(o.thing as Spell);
    }
}
