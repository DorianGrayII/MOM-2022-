namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Gets the Scale of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
    public class GetScale : FsmStateAction
    {
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        public FsmVector3 vector;
        [UIHint(UIHint.Variable)]
        public FsmFloat xScale;
        [UIHint(UIHint.Variable)]
        public FsmFloat yScale;
        [UIHint(UIHint.Variable)]
        public FsmFloat zScale;
        public Space space;
        public bool everyFrame;

        private void DoGetScale()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Vector3 vector = (this.space == Space.World) ? ownerDefaultTarget.transform.lossyScale : ownerDefaultTarget.transform.localScale;
                this.vector.set_Value(vector);
                this.xScale.Value = vector.x;
                this.yScale.Value = vector.y;
                this.zScale.Value = vector.z;
            }
        }

        public override void OnEnter()
        {
            this.DoGetScale();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetScale();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.vector = null;
            this.xScale = null;
            this.yScale = null;
            this.zScale = null;
            this.space = Space.World;
            this.everyFrame = false;
        }
    }
}

