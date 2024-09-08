using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Detect 2D trigger collisions between Game Objects that have RigidBody2D/Collider2D components.")]
    public class Trigger2dEvent : FsmStateAction
    {
        [Tooltip("The GameObject to detect collisions on.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The type of trigger event to detect.")]
        public Trigger2DType trigger;

        [UIHint(UIHint.TagMenu)]
        [Tooltip("Filter by Tag.")]
        public FsmString collideTag;

        [Tooltip("Event to send if the trigger event is detected.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
        public FsmGameObject storeCollider;

        private PlayMakerProxyBase cachedProxy;

        public override void Reset()
        {
            this.gameObject = null;
            this.trigger = Trigger2DType.OnTriggerEnter2D;
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
                case Trigger2DType.OnTriggerEnter2D:
                    base.Fsm.HandleTriggerEnter2D = true;
                    break;
                case Trigger2DType.OnTriggerStay2D:
                    base.Fsm.HandleTriggerStay2D = true;
                    break;
                case Trigger2DType.OnTriggerExit2D:
                    base.Fsm.HandleTriggerExit2D = true;
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
                case Trigger2DType.OnTriggerEnter2D:
                    this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerEnter2D>(value);
                    break;
                case Trigger2DType.OnTriggerStay2D:
                    this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerStay2D>(value);
                    break;
                case Trigger2DType.OnTriggerExit2D:
                    this.cachedProxy = PlayMakerFSM.GetEventHandlerComponent<PlayMakerTriggerExit2D>(value);
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
                case Trigger2DType.OnTriggerEnter2D:
                    this.cachedProxy.AddTrigger2DEventCallback(TriggerEnter2D);
                    break;
                case Trigger2DType.OnTriggerStay2D:
                    this.cachedProxy.AddTrigger2DEventCallback(TriggerStay2D);
                    break;
                case Trigger2DType.OnTriggerExit2D:
                    this.cachedProxy.AddTrigger2DEventCallback(TriggerExit2D);
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
                case Trigger2DType.OnTriggerEnter2D:
                    this.cachedProxy.RemoveTrigger2DEventCallback(TriggerEnter2D);
                    break;
                case Trigger2DType.OnTriggerStay2D:
                    this.cachedProxy.RemoveTrigger2DEventCallback(TriggerStay2D);
                    break;
                case Trigger2DType.OnTriggerExit2D:
                    this.cachedProxy.RemoveTrigger2DEventCallback(TriggerExit2D);
                    break;
                }
            }
        }

        private void StoreCollisionInfo(Collider2D collisionInfo)
        {
            this.storeCollider.Value = collisionInfo.gameObject;
        }

        public override void DoTriggerEnter2D(Collider2D other)
        {
            if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
            {
                this.TriggerEnter2D(other);
            }
        }

        public override void DoTriggerStay2D(Collider2D other)
        {
            if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
            {
                this.TriggerStay2D(other);
            }
        }

        public override void DoTriggerExit2D(Collider2D other)
        {
            if (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
            {
                this.TriggerExit2D(other);
            }
        }

        private void TriggerEnter2D(Collider2D other)
        {
            if (this.trigger == Trigger2DType.OnTriggerEnter2D && FsmStateAction.TagMatches(this.collideTag, other))
            {
                this.StoreCollisionInfo(other);
                base.Fsm.Event(this.sendEvent);
            }
        }

        private void TriggerStay2D(Collider2D other)
        {
            if (this.trigger == Trigger2DType.OnTriggerStay2D && FsmStateAction.TagMatches(this.collideTag, other))
            {
                this.StoreCollisionInfo(other);
                base.Fsm.Event(this.sendEvent);
            }
        }

        private void TriggerExit2D(Collider2D other)
        {
            if (this.trigger == Trigger2DType.OnTriggerExit2D && FsmStateAction.TagMatches(this.collideTag, other))
            {
                this.StoreCollisionInfo(other);
                base.Fsm.Event(this.sendEvent);
            }
        }

        public override string ErrorCheck()
        {
            return ActionHelpers.CheckPhysics2dSetup(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
        }
    }
}
