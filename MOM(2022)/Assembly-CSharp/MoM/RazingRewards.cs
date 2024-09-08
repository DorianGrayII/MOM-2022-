// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.RazingRewards
using System.Collections;
using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using UnityEngine.UI;

public class RazingRewards : ScreenBase
{
    private static RazingRewards instance;

    public Button btOK;

    public CanvasGroup canvasGroup;

    public ScrollRect scrollRect;

    public List<AdventureOutcome> p_adventureOutcomes;

    public GameObject textContainer;

    private Dictionary<AdventureOutcome.Types, AdventureOutcome> outcomePrefabs = new Dictionary<AdventureOutcome.Types, AdventureOutcome>();

    private List<AdventureOutcomeDelta.Outcome> outcomes;

    public override IEnumerator PreStart()
    {
        this.outcomePrefabs.Clear();
        foreach (AdventureOutcome p_adventureOutcome in this.p_adventureOutcomes)
        {
            if (p_adventureOutcome.typeData == null)
            {
                Debug.LogError(p_adventureOutcome.name + " has no typeData so will be ignored");
                continue;
            }
            foreach (AdventureOutcome.TypeData typeDatum in p_adventureOutcome.typeData)
            {
                this.outcomePrefabs.Add(typeDatum.outcomeType, p_adventureOutcome);
            }
        }
        yield return base.PreStart();
    }

    public static void OpenPopup(ScreenBase parent, List<AdventureOutcomeDelta.Outcome> outcomes)
    {
        RazingRewards.instance = UIManager.Open<RazingRewards>(UIManager.Layer.Popup, parent);
        RazingRewards.instance.outcomes = outcomes;
        if (outcomes == null || outcomes.Count <= 0)
        {
            return;
        }
        foreach (AdventureOutcomeDelta.Outcome outcome in outcomes)
        {
            if (RazingRewards.instance.outcomePrefabs.TryGetValue(outcome.outcomeType, out var value))
            {
                GameObjectUtils.Instantiate(value.gameObject, RazingRewards.instance.textContainer.transform).GetComponent<AdventureOutcome>().Set(outcome);
            }
        }
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btOK)
        {
            UIManager.Close(this);
        }
    }
}
