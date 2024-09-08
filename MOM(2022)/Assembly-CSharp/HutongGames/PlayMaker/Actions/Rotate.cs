namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Rotates a Game Object around each Axis. Use a Vector3 Variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
    public class Rotate : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The game object to rotate.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("A rotation vector specifying rotation around x, y, and z axis. NOTE: You can override individual axis below."), UIHint(UIHint.Variable)]
        public FsmVector3 vector;
        [HutongGames.PlayMaker.Tooltip("Rotation around x axis.")]
        public FsmFloat xAngle;
        [HutongGames.PlayMaker.Tooltip("Rotation around y axis.")]
        public FsmFloat yAngle;
        [HutongGames.PlayMaker.Tooltip("Rotation around z axis.")]
        public FsmFloat zAngle;
        [HutongGames.PlayMaker.Tooltip("Rotate in local or world space.")]
        public Space space;
        [HutongGames.PlayMaker.Tooltip("Rotation is specified in degrees per second. In other words, the amount to rotate in over one second. This allows rotations to be frame rate independent. It is the same as multiplying the rotation by Time.deltaTime.")]
        public bool perSecond;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Perform the rotation in LateUpdate. This is useful if you want to override the rotation of objects that are animated or otherwise rotated in Update.")]
        public bool lateUpdate;
        [HutongGames.PlayMaker.Tooltip("Perform the rotation in FixedUpdate. This is useful when working with rigid bodies and physics.")]
        public bool fixedUpdate;

        private void DoRotate()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                Vector3 eulers = this.vector.IsNone ? new Vector3(this.xAngle.Value, this.yAngle.Value, this.zAngle.Value) : this.vector.get_Value();
                if (!this.xAngle.IsNone)
                {
                    eulers.x = this.xAngle.Value;
                }
                if (!this.yAngle.IsNone)
                {
                    eulers.y = this.yAngle.Value;
                }
                if (!this.zAngle.IsNone)
                {
                    eulers.z = this.zAngle.Value;
                }
                if (!this.perSecond)
                {
                    ownerDefaultTarget.transform.Rotate(eulers, this.space);
                }
                else
                {
                    ownerDefaultTarget.transform.Rotate(eulers * Time.deltaTime, this.space);
                }
            }
        }

        public override void OnEnter()
        {
            if (!this.everyFrame && (!this.lateUpdate && !this.fixedUpdate))
            {
                this.DoRotate();
                base.Finish();
            }
        }

        public override void OnFixedUpdate()
        {
            if (this.fixedUpdate)
            {
                this.DoRotate();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnLateUpdate()
        {
            if (this.lateUpdate)
            {
                this.DoRotate();
            }
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnPreprocess()
        {
            if (this.fixedUpdate)
            {
                base.Fsm.HandleFixedUpdate = true;
            }
            if (this.lateUpdate)
            {
                base.Fsm.HandleLateUpdate = true;
            }
        }

        public override void OnUpdate()
        {
            if (!this.lateUpdate && !this.fixedUpdate)
            {
                this.DoRotate();
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
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
            this.space = Space.Self;
            this.perSecond = false;
            this.everyFrame = true;
            this.lateUpdate = false;
            this.fixedUpdate = false;
        }
    }
}

