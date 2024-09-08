namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Physics2D), HutongGames.PlayMaker.Tooltip("Gets the Mass of a Game Object's Rigid Body 2D.")]
    public class GetMass2d : ComponentAction<Rigidbody2D>
    {
        [RequiredField, CheckForComponent(typeof(Rigidbody2D)), HutongGames.PlayMaker.Tooltip("The GameObject with a Rigidbody2D attached.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the mass of gameObject.")]
        public FsmFloat storeResult;

        private void DoGetMass()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (base.UpdateCache(ownerDefaultTarget))
            {
                this.storeResult.Value = base.rigidbody2d.mass;
            }
        }

        public override void OnEnter()
        {
            this.DoGetMass();
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.storeResult = null;
        }
    }
}

