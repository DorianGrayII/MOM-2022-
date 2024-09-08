namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Tests if a Game Object's Rigid Body 2D is Kinematic.")]
    public class IsKinematic2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("the GameObject with a Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Event Sent if Kinematic")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event sent if not Kinematic")]
        public FsmEvent falseEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the Kinematic state")]
        public FsmBool store;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoIsKinematic()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                bool isKinematic = base.rigidbody2d.isKinematic;
                this.store.Value = isKinematic;
                base.Fsm.Event(isKinematic ? this.trueEvent : this.falseEvent);
            }
        }

        public override void OnEnter()
        {
            this.DoIsKinematic();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoIsKinematic();
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

