// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.ResearchSpells
using System.Collections;
using System.Collections.Generic;
using DBDef;
using MHUtils.UI;
using MOM;
using UnityEngine.UI;

public class ResearchSpells : SpellBookScreen
{
    public Button btResearch;

    public Spell selectedSpell;

    protected PlayerWizard wizard;

    public override ISpellCaster GetSpellCaster()
    {
        return this.wizard;
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s.name == "ButtonClose")
        {
            UIManager.Close(this);
        }
    }

    public override IEnumerator PreStart()
    {
        this.wizard = GameManager.GetHumanWizard();
        yield return base.PreStart();
        MagicAndResearch sm = this.wizard.GetMagicAndResearch();
        if (sm.curentlyResearched != null)
        {
            this.selectedSpell = sm.curentlyResearched.Get();
        }
        base.UpdateGridPage();
        this.btResearch.onClick.AddListener(delegate
        {
            sm.curentlyResearched = this.GetSelectedSpell();
            base.btClose.onClick.Invoke();
            HUD.Get().UpdateResearchButton();
        });
    }

    public override IEnumerator PostClose()
    {
        yield return base.PostClose();
        HUD.Get()?.UpdateEndTurnButtons();
    }

    protected override Spell GetOriginalSelection()
    {
        MagicAndResearch magicAndResearch = this.wizard.GetMagicAndResearch();
        if (magicAndResearch.curentlyResearched != null)
        {
            return magicAndResearch.curentlyResearched.Get();
        }
        return null;
    }

    protected override Spell GetSelectedSpell()
    {
        return this.selectedSpell;
    }

    protected override List<DBReference<Spell>> GetSpells()
    {
        MagicAndResearch magicAndResearch = this.wizard.GetMagicAndResearch();
        magicAndResearch.curentResearchOptions.SortInPlace((DBReference<Spell> a, DBReference<Spell> b) => a.Get().researchCost.CompareTo(b.Get().researchCost));
        return magicAndResearch.curentResearchOptions;
    }

    protected override void SelectSpell(Spell s)
    {
        this.selectedSpell = s;
    }

    protected override void UpdateMainButtonState()
    {
        bool interactable = this.GetOriginalSelection() != this.GetSelectedSpell();
        this.btResearch.interactable = interactable;
    }

    protected override bool WorldMode()
    {
        return true;
    }
}
