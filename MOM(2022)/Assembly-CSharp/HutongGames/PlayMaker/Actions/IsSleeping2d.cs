using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Tests if a Game Object's Rigidbody 2D is sleeping.")]
    public class IsSleeping2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Event sent if sleeping")]
        public FsmEvent trueEvent;

        [Tooltip("Event sent if not sleeping")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the value in a Boolean variable")]
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
            this.DoIsSleeping();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoIsSleeping();
        }

        private void DoIsSleeping()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                bool flag = base.rigidbody2d.IsSleeping();
                this.store.Value = flag;
                base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
            }
        }
    }
}
