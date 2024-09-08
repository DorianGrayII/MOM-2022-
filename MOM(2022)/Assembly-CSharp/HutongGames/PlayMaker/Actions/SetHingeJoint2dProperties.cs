using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Sets the various properties of a HingeJoint2d component")]
    public class SetHingeJoint2dProperties : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The HingeJoint2d target")]
        [CheckForComponent(typeof(HingeJoint2D))]
        public FsmOwnerDefault gameObject;

        [ActionSection("Limits")]
        [Tooltip("Should limits be placed on the range of rotation?")]
        public FsmBool useLimits;

        [Tooltip("Lower angular limit of rotation.")]
        public FsmFloat min;

        [Tooltip("Upper angular limit of rotation")]
        public FsmFloat max;

        [ActionSection("Motor")]
        [Tooltip("Should a motor force be applied automatically to the Rigidbody2D?")]
        public FsmBool useMotor;

        [Tooltip("The desired speed for the Rigidbody2D to reach as it moves with the joint.")]
        public FsmFloat motorSpeed;

        [Tooltip("The maximum force that can be applied to the Rigidbody2D at the joint to attain the target speed.")]
        public FsmFloat maxMotorTorque;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private HingeJoint2D _joint;

        private JointMotor2D _motor;

        private JointAngleLimits2D _limits;

        public override void Reset()
        {
            this.useLimits = new FsmBool
            {
                UseVariable = true
            };
            this.min = new FsmFloat
            {
                UseVariable = true
            };
            this.max = new FsmFloat
            {
                UseVariable = true
            };
            this.useMotor = new FsmBool
            {
                UseVariable = true
            };
            this.motorSpeed = new FsmFloat
            {
                UseVariable = true
            };
            this.maxMotorTorque = new FsmFloat
            {
                UseVariable = true
            };
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                this._joint = ownerDefaultTarget.GetComponent<HingeJoint2D>();
                if (this._joint != null)
                {
                    this._motor = this._joint.motor;
                    this._limits = this._joint.limits;
                }
            }
            this.SetProperties();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.SetProperties();
        }

        private void SetProperties()
        {
            if (!(this._joint == null))
            {
                if (!this.useMotor.IsNone)
                {
                    this._joint.useMotor = this.useMotor.Value;
                }
                if (!this.motorSpeed.IsNone)
                {
                    this._motor.motorSpeed = this.motorSpeed.Value;
                    this._joint.motor = this._motor;
                }
                if (!this.maxMotorTorque.IsNone)
                {
                    this._motor.maxMotorTorque = this.maxMotorTorque.Value;
                    this._joint.motor = this._motor;
                }
                if (!this.useLimits.IsNone)
                {
                    this._joint.useLimits = this.useLimits.Value;
                }
                if (!this.min.IsNone)
                {
                    this._limits.min = this.min.Value;
                    this._joint.limits = this._limits;
                }
                if (!this.max.IsNone)
                {
                    this._limits.max = this.max.Value;
                    this._joint.limits = this._limits;
                }
            }
        }
    }
}
