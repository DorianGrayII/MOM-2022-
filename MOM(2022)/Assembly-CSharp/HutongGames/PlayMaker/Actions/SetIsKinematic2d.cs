namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Controls whether 2D physics affects the Game Object.")]
    public class SetIsKinematic2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with the Rigidbody2D attached")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The isKinematic value")]
        public FsmBool isKinematic;

        private void DoSetIsKinematic()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                base.rigidbody2d.isKinematic = this.isKinematic.Value;
            }
        }

        public override void OnEnter()
        {
            this.DoSetIsKinematic();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.isKinematic = false;
        }
    }
}

