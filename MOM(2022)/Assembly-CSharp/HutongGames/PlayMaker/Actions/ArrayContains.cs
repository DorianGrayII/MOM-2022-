// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// HutongGames.PlayMaker.Actions.ArrayContains
using System;
using HutongGames.PlayMaker;

[ActionCategory(ActionCategory.Array)]
[Tooltip("Check if an Array contains a value. Optionally get its index.")]
public class ArrayContains : FsmStateAction
{
    [RequiredField]
    [UIHint(UIHint.Variable)]
    [Tooltip("The Array Variable to use.")]
    public FsmArray array;

    [RequiredField]
    [MatchElementType("array")]
    [Tooltip("The value to check against in the array.")]
    public FsmVar value;

    [ActionSection("Result")]
    [Tooltip("The index of the value in the array.")]
    [UIHint(UIHint.Variable)]
    public FsmInt index;

    [Tooltip("Store in a bool whether it contains that element or not (described below)")]
    [UIHint(UIHint.Variable)]
    public FsmBool isContained;

    [Tooltip("Event sent if the array contains that element (described below)")]
    public FsmEvent isContainedEvent;

    [Tooltip("Event sent if the array does not contains that element (described below)")]
    public FsmEvent isNotContainedEvent;

    public override void Reset()
    {
        this.array = null;
        this.value = null;
        this.index = null;
        this.isContained = null;
        this.isContainedEvent = null;
        this.isNotContainedEvent = null;
    }

    public override void OnEnter()
    {
        this.DoCheckContainsValue();
        base.Finish();
    }

    private void DoCheckContainsValue()
    {
        this.value.UpdateValue();
        int num = -1;
        num = ((this.value.GetValue() != null && !this.value.GetValue().Equals(null)) ? Array.IndexOf(this.array.Values, this.value.GetValue()) : Array.FindIndex(this.array.Values, (object x) => x?.Equals(null) ?? true));
        bool flag = num != -1;
        this.isContained.Value = flag;
        this.index.Value = num;
        if (flag)
        {
            base.Fsm.Event(this.isContainedEvent);
        }
        else
        {
            base.Fsm.Event(this.isNotContainedEvent);
        }
    }
}
