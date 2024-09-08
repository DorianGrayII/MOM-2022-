namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Transforms a Direction from a Game Object's local space to world space.")]
    public class TransformDirection : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [RequiredField]
        public FsmVector3 localDirection;
        [RequiredField, UIHint(UIHint.Variable)]
        public FsmVector3 storeResult;
        public bool everyFrame;

        private void DoTransformDirection()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this.storeResult.set_Value(ownerDefaultTarget.transform.TransformDirection(this.localDirection.get_Value()));
            }
        }

        public override void OnEnter()
        {
            this.DoTransformDirection();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoTransformDirection();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.localDirection = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

