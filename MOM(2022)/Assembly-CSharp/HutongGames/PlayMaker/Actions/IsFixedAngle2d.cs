using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Is the rigidbody2D constrained from rotating?Note: Prefer SetRigidBody2dConstraints when working in Unity 5")]
    public class IsFixedAngle2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(Rigidbody2D))]
        [Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Event sent if the Rigidbody2D does have fixed angle")]
        public FsmEvent trueEvent;

        [Tooltip("Event sent if the Rigidbody2D doesn't have fixed angle")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the fixedAngle flag")]
        public FsmBool store;

        [Tooltip("Repeat every frame.")]
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

        private void DoIsFixedAngle()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                bool flag = false;
                flag = (base.rigidbody2d.constraints & RigidbodyConstraints2D.FreezeRotation) != 0;
                this.store.Value = flag;
                base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
            }
        }
    }
}
