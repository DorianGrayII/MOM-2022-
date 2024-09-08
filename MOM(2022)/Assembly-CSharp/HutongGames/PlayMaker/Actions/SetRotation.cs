using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
    public class SetRotation : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject to rotate.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Use a stored quaternion, or vector angles below.")]
        public FsmQuaternion quaternion;

        [UIHint(UIHint.Variable)]
        [Title("Euler Angles")]
        [Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
        public FsmVector3 vector;

        public FsmFloat xAngle;

        public FsmFloat yAngle;

        public FsmFloat zAngle;

        [Tooltip("Use local or world space.")]
        public Space space;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
        public bool lateUpdate;

        public override void Reset()
        {
            this.gameObject = null;
            this.quaternion = null;
            this.vector = null;
            this.xAngle = new FsmFloat
            {
                UseVariable = true
            };
            this.yAngle = new FsmFloat
            {
                UseVariable = true
            };
            this.zAngle = new FsmFloat
            {
                UseVariable = true
            };
            this.space = Space.World;
            this.everyFrame = false;
            this.lateUpdate = false;
        }

        public override void OnPreprocess()
        {
            if (this.lateUpdate)
            {
                base.Fsm.HandleLateUpdate = true;
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

        public override void OnUpdate()
        {
            if (!this.lateUpdate)
            {
                this.DoSetRotation();
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

        private void DoSetRotation()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                Vector3 vector = ((!this.quaternion.IsNone) ? this.quaternion.Value.eulerAngles : (this.vector.IsNone ? ((this.space == Space.Self) ? ownerDefaultTarget.transform.localEulerAngles : ownerDefaultTarget.transform.eulerAngles) : this.vector.Value));
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
    }
}
