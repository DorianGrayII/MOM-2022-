using MOM;

public interface ISpellCaster
{
    SpellManager GetSpellManager();

    PlayerWizard GetWizardOwner();

    int GetWizardOwnerID();

    int GetTotalCastingSkill();

    int GetMana();

    void SetMana(int m);

    string GetName();
}
