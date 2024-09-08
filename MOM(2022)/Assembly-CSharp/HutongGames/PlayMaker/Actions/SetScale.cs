namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
    public class SetScale : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to scale.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Use stored Vector3 value, and/or set each axis below.")]
        public FsmVector3 vector;
        public FsmFloat x;
        public FsmFloat y;
        public FsmFloat z;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        private void DoSetScale()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Vector3 vector = this.vector.IsNone ? ownerDefaultTarget.transform.localScale : this.vector.get_Value();
                if (!this.x.IsNone)
                {
                    vector.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    vector.y = this.y.Value;
                }
                if (!this.z.IsNone)
                {
                    vector.z = this.z.Value;
                }
                ownerDefaultTarget.transform.localScale = vector;
            }
        }

        public override void OnEnter()
        {
            this.DoSetScale();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
            {
                this.DoSetScale();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnPreprocess()
        {
            if (this.lateUpdate)
            {
                base.Fsm.HandleLateUpdate = true;
            }
        }

        public override void OnUpdate()
        {
            if (!this.lateUpdate)
            {
                this.DoSetScale();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.vector = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.z = num3;
            this.everyFrame = false;
            this.lateUpdate = false;
        }
    }
}

