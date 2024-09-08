namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
    public class SetPosition : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to position.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Use a stored Vector3 position, and/or set individual axis below.")]
        public FsmVector3 vector;
        public FsmFloat x;
        public FsmFloat y;
        public FsmFloat z;
        [HutongGames.PlayMaker.Tooltip("Use local or world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        private void DoSetPosition()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Vector3 vector = !this.vector.IsNone ? this.vector.get_Value() : ((this.space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition);
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
                if (this.space == Space.World)
                {
                    ownerDefaultTarget.transform.position = vector;
                }
                else
                {
                    ownerDefaultTarget.transform.localPosition = vector;
                }
            }
        }

        public override void OnEnter()
        {
            if (!this.everyFrame && !this.lateUpdate)
            {
                this.DoSetPosition();
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
            {
                this.DoSetPosition();
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
                this.DoSetPosition();
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
            this.space = Space.Self;
            this.everyFrame = false;
            this.lateUpdate = false;
        }
    }
}

