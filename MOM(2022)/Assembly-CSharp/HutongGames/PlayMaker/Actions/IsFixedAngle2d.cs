namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Is the rigidbody2D constrained from rotating?Note: Prefer SetRigidBody2dConstraints when working in Unity 5")]
    public class IsFixedAngle2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Event sent if the Rigidbody2D does have fixed angle")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if the Rigidbody2D doesn't have fixed angle")]
        public FsmEvent falseEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the fixedAngle flag")]
        public FsmBool store;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoIsFixedAngle()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                bool flag = false;
                flag = (base.rigidbody2d.constraints & RigidbodyConstraints2D.FreezeRotation) != RigidbodyConstraints2D.None;
                this.store.Value = flag;
                base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
            }
        }

        public override void OnEnter()
        {
            this.DoIsFixedAngle();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoIsFixedAngle();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.store = null;
            this.everyFrame = false;
        }
    }
}

