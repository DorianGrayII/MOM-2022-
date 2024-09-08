namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Forces a Game Object's Rigid Body 2D to Sleep at least one frame.")]
    public class Sleep2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody2d attached")]
        public FsmOwnerDefault gameObject;

        private void DoSleep()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody2d.Sleep();
            }
        }

        public override void OnEnter()
        {
            this.DoSleep();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
        }
    }
}

