namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
    public class SetRotation : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Use a stored quaternion, or vector angles below.")]
        public FsmQuaternion quaternion;
        [UIHint(UIHint.Variable), Title("Euler Angles"), HutongGames.PlayMaker.Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
        public FsmVector3 vector;
        public FsmFloat xAngle;
        public FsmFloat yAngle;
        public FsmFloat zAngle;
        [HutongGames.PlayMaker.Tooltip("Use local or world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        private void DoSetRotation()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Vector3 vector = this.quaternion.IsNone ? (this.vector.IsNone ? ((this.space == Space.Self) ? ownerDefaultTarget.transform.localEulerAngles : ownerDefaultTarget.transform.eulerAngles) : this.vector.get_Value()) : this.quaternion.get_Value().eulerAngles;
                if (!this.xAngle.IsNone)
                {
                    vector.x = this.xAngle.Value;
                }
                if (!this.yAngle.IsNone)
                {
                    vector.y = this.yAngle.Value;
                }
                if (!this.zAngle.IsNone)
                {
                    vector.z = this.zAngle.Value;
                }
                if (this.space == Space.Self)
                {
                    ownerDefaultTarget.transform.localEulerAngles = vector;
                }
                else
                {
                    ownerDefaultTarget.transform.eulerAngles = vector;
                }
            }
        }

        public override void OnEnter()
        {
            if (!this.everyFrame && !this.lateUpdate)
            {
                this.DoSetRotation();
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
            {
                this.DoSetRotation();
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
                this.DoSetRotation();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.quaternion = null;
            this.vector = null;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.xAngle = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.yAngle = num2;
            FsmFloat num3 = new FsmFloat();
            num3.UseVariable = true;
            this.zAngle = num3;
            this.space = Space.World;
            this.everyFrame = false;
            this.lateUpdate = false;
        }
    }
}

