using MOM;
using System;

public interface ISpellCaster
{
    int GetMana();
    string GetName();
    SpellManager GetSpellManager();
    int GetTotalCastingSkill();
    PlayerWizard GetWizardOwner();
    int GetWizardOwnerID();
    void SetMana(int m);
}

