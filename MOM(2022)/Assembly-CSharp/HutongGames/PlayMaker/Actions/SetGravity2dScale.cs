namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Sets The degree to which this object is affected by gravity.  NOTE: Game object must have a rigidbody 2D.")]
    public class SetGravity2dScale : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody 2d attached")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The gravity scale effect")]
        public FsmFloat gravityScale;

        private void DoSetGravityScale()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody2d.gravityScale = this.gravityScale.Value;
            }
        }

        public override void OnEnter()
        {
            this.DoSetGravityScale();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.gravityScale = 1f;
        }
    }
}

