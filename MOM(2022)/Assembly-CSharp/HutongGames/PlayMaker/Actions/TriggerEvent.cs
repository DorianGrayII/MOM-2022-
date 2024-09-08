// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// HutongGames.PlayMaker.Actions.TriggerEvent
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory(ActionCategory.Physics)]
[global::HutongGames.PlayMaker.Tooltip("Detect trigger collisions between GameObjects that have RigidBody/Collider components.")]
public class TriggerEvent : FsmStateAction
{
    [global::HutongGames.PlayMaker.Tooltip("The GameObject to detect trigger events on.")]
    public FsmOwnerDefault gameObject;

    [global::HutongGames.PlayMaker.Tooltip("The type of trigger event to detect.")]
    public TriggerType trigger;

    [UIHint(UIHint.TagMenu)]
    [global::HutongGames.PlayMaker.Tooltip("Filter by Tag.")]
    public FsmString collideTag;

    [global::HutongGames.PlayMaker.Tooltip("Event to send if the trigger event is detected.")]
    public FsmEvent sendEvent;

    [UIHint(UIHint.Variable)]
    [global::HutongGames.PlayMaker.Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
    public FsmGameObject storeCollider;

    private PlayMakerProxyBase cachedProxy;

    public override void Reset()
    {
        this.gameObject = null;
        this.trigger = TriggerType.OnTriggerEnter;
        this.collideTag = "";
        this.sendEvent = null;
        this.storeCollider = null;
    }

    public override void OnPreprocess()
    {
        if (this.gameObject == null)
        {
            this.gameObject = new FsmOwnerDefault();
        }
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            switch (this.trigger)
            {
            case TriggerType.OnTriggerEnter:
                base.Fsm.HandleTriggerEnter = true;
                break;
            case TriggerType.OnTriggerStay:
                base.Fsm.HandleTriggerStay = true;
                break;
            case TriggerType.OnTriggerExit:
                base.Fsm.HandleTriggerExit = true;
                break;
            }
        }
        else
        {
            this.GetProxyComponent();
        }
    }

    public override void OnEnter()
    {
        if (this.gameObject.OwnerOption != 0)
        {
            if (this.cachedProxy == null)
            {
                this.GetProxyComponent();
            }
            this.AddCallback();
            this.gameObject.GameObject.OnChange += UpdateCallback;
        }
    }

    public override void OnExit()
    {
        if (this.gameObject.OwnerOption != 0)
        {
            this.RemoveCallback();
            this.gameObject.GameObject.OnChange -= UpdateCallback;
        }
    }

    private void UpdateCallback()
    {
        this.RemoveCallback();
        this.GetProxyComponent();
        this.AddCallback();
    }

    private void GetProxyComponent()
    {
        this.cachedProxy = null;
        GameObject value = this.gameObject.GameObject.Value;
        if (!(value == null))
        {
            switch (this.trigger)
            {
            case TriggerType.OnTriggerEnter:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerEnter>(value);
                break;
            case TriggerType.OnTriggerStay:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerStay>(value);
                break;
            case TriggerType.OnTriggerExit:
                this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerExit>(value);
                break;
            }
        }
    }

    private void AddCallback()
    {
        if (!(this.cachedProxy == null))
        {
            switch (this.trigger)
            {
            case TriggerType.OnTriggerEnter:
                this.cachedProxy.AddTriggerEventCallback(TriggerEnter);
                break;
            case TriggerType.OnTriggerStay:
                this.cachedProxy.AddTriggerEventCallback(TriggerStay);
                break;
            case TriggerType.OnTriggerExit:
                this.cachedProxy.AddTriggerEventCallback(TriggerExit);
                break;
            }
        }
    }

    private void RemoveCallback()
    {
        if (!(this.cachedProxy == null))
        {
            switch (this.trigger)
            {
            case TriggerType.OnTriggerEnter:
                this.cachedProxy.RemoveTriggerEventCallback(TriggerEnter);
                break;
            case TriggerType.OnTriggerStay:
                this.cachedProxy.RemoveTriggerEventCallback(TriggerStay);
                break;
            case TriggerType.OnTriggerExit:
                this.cachedProxy.RemoveTriggerEventCallback(TriggerExit);
                break;
            }
        }
    }

    private void StoreCollisionInfo(Collider collisionInfo)
    {
        this.storeCollider.Value = collisionInfo.gameObject;
    }

    public override void DoTriggerEnter(Collider other)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.TriggerEnter(other);
        }
    }

    public override void DoTriggerStay(Collider other)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.TriggerStay(other);
        }
    }

    public override void DoTriggerExit(Collider other)
    {
        if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
        {
            this.TriggerExit(other);
        }
    }

    private void TriggerEnter(Collider other)
    {
        if (this.trigger == TriggerType.OnTriggerEnter && FsmStateAction.TagMatches(this.collideTag, other))
        {
            this.StoreCollisionInfo(other);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void TriggerStay(Collider other)
    {
        if (this.trigger == TriggerType.OnTriggerStay && FsmStateAction.TagMatches(this.collideTag, other))
        {
            this.StoreCollisionInfo(other);
            base.Fsm.Event(this.sendEvent);
        }
    }

    private void TriggerExit(Collider other)
    {
        if (this.trigger == TriggerType.OnTriggerExit && FsmStateAction.TagMatches(this.collideTag, other))
        {
            this.StoreCollisionInfo(other);
            base.Fsm.Event(this.sendEvent);
        }
    }

    public override string ErrorCheck()
    {
        return ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
    }
}
