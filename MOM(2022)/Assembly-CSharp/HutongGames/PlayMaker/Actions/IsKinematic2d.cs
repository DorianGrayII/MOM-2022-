using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Tests if a Game Object's Rigid Body 2D is Kinematic.")]
    public class IsKinematic2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("the GameObject with a Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Event Sent if Kinematic")]
        public FsmEvent trueEvent;

        [Tooltip("Event sent if not Kinematic")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the Kinematic state")]
        public FsmBool store;

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.store = null;
            this.everyFrame = false;
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
    }
}
