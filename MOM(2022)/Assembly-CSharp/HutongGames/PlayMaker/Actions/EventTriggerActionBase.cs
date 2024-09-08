// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// HutongGames.PlayMaker.Actions.EventTriggerActionBase
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class EventTriggerActionBase : ComponentAction<EventTrigger>
{
    [DisplayOrder(0)]
    [RequiredField]
    [global::HutongGames.PlayMaker.Tooltip("The GameObject with the UI component.")]
    public FsmOwnerDefault gameObject;

    [DisplayOrder(1)]
    [global::HutongGames.PlayMaker.Tooltip("Where to send the event.")]
    public FsmEventTarget eventTarget;

    protected EventTrigger trigger;

    protected EventTrigger.Entry entry;

    public override void Reset()
    {
        this.gameObject = null;
        this.eventTarget = FsmEventTarget.Self;
    }

    protected void Init(EventTriggerType eventTriggerType, UnityAction<BaseEventData> call)
    {
        GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
        if (base.UpdateCacheAddComponent(ownerDefaultTarget))
        {
            this.trigger = base.cachedComponent;
            if (this.entry == null)
            {
                this.entry = new EventTrigger.Entry();
            }
            this.entry.eventID = eventTriggerType;
            this.entry.callback.AddListener(call);
            this.trigger.triggers.Add(this.entry);
        }
    }

    public override void OnExit()
    {
        this.entry.callback.RemoveAllListeners();
        this.trigger.triggers.Remove(this.entry);
    }
}
