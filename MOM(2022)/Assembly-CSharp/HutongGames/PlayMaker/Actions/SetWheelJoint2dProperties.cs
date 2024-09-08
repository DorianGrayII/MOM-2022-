using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics2D)]
    [Tooltip("Sets the various properties of a WheelJoint2d component")]
    public class SetWheelJoint2dProperties : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The WheelJoint2d target")]
        [CheckForComponent(typeof(WheelJoint2D))]
        public FsmOwnerDefault gameObject;

        [ActionSection("Motor")]
        [Tooltip("Should a motor force be applied automatically to the Rigidbody2D?")]
        public FsmBool useMotor;

        [Tooltip("The desired speed for the Rigidbody2D to reach as it moves with the joint.")]
        public FsmFloat motorSpeed;

        [Tooltip("The maximum force that can be applied to the Rigidbody2D at the joint to attain the target speed.")]
        public FsmFloat maxMotorTorque;

        [ActionSection("Suspension")]
        [Tooltip("The world angle along which the suspension will move. This provides 2D constrained motion similar to a SliderJoint2D. This is typically how suspension works in the real world.")]
        public FsmFloat angle;

        [Tooltip("The amount by which the suspension spring force is reduced in proportion to the movement speed.")]
        public FsmFloat dampingRatio;

        [Tooltip("The frequency at which the suspension spring oscillates.")]
        public FsmFloat frequency;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        private WheelJoint2D _wj2d;

        private JointMotor2D _motor;

        private JointSuspension2D _suspension;

        public override void Reset()
        {
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
            this.angle = new FsmFloat
            {
                UseVariable = true
            };
            this.dampingRatio = new FsmFloat
            {
                UseVariable = true
            };
            this.frequency = new FsmFloat
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
                this._wj2d = ownerDefaultTarget.GetComponent<WheelJoint2D>();
                if (this._wj2d != null)
                {
                    this._motor = this._wj2d.motor;
                    this._suspension = this._wj2d.suspension;
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
            if (!(this._wj2d == null))
            {
                if (!this.useMotor.IsNone)
                {
                    this._wj2d.useMotor = this.useMotor.Value;
                }
                if (!this.motorSpeed.IsNone)
                {
                    this._motor.motorSpeed = this.motorSpeed.Value;
                    this._wj2d.motor = this._motor;
                }
                if (!this.maxMotorTorque.IsNone)
                {
                    this._motor.maxMotorTorque = this.maxMotorTorque.Value;
                    this._wj2d.motor = this._motor;
                }
                if (!this.angle.IsNone)
                {
                    this._suspension.angle = this.angle.Value;
                    this._wj2d.suspension = this._suspension;
                }
                if (!this.dampingRatio.IsNone)
                {
                    this._suspension.dampingRatio = this.dampingRatio.Value;
                    this._wj2d.suspension = this._suspension;
                }
                if (!this.frequency.IsNone)
                {
                    this._suspension.frequency = this.frequency.Value;
                    this._wj2d.suspension = this._suspension;
                }
            }
        }
    }
}
