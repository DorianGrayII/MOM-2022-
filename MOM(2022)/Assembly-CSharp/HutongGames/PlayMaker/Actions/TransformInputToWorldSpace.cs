using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [NoActionTargets]
    [ActionCategory(ActionCategory.Input)]
    [Tooltip("Transforms 2d input into a 3d world space vector. E.g., can be used to transform input from a touch joystick to a movement vector.")]
    public class TransformInputToWorldSpace : FsmStateAction
    {
        public enum AxisPlane
        {
            XZ = 0,
            XY = 1,
            YZ = 2
        }

        [UIHint(UIHint.Variable)]
        [Tooltip("The horizontal input.")]
        public FsmFloat horizontalInput;

        [UIHint(UIHint.Variable)]
        [Tooltip("The vertical input.")]
        public FsmFloat verticalInput;

        [Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
        public FsmFloat multiplier;

        [RequiredField]
        [Tooltip("The world plane to map the 2d input onto.")]
        public AxisPlane mapToPlane;

        [Tooltip("Make the result relative to a GameObject, typically the main camera.")]
        public FsmGameObject relativeTo;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the direction vector.")]
        public FsmVector3 storeVector;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the length of the direction vector.")]
        public FsmFloat storeMagnitude;

        public override void Reset()
        {
            this.horizontalInput = null;
            this.verticalInput = null;
            this.multiplier = 1f;
            this.mapToPlane = AxisPlane.XZ;
            this.storeVector = null;
            this.storeMagnitude = null;
        }

        public override void OnUpdate()
        {
            Vector3 vector = default(Vector3);
            Vector3 vector2 = default(Vector3);
            if (this.relativeTo.Value == null)
            {
                switch (this.mapToPlane)
                {
                case AxisPlane.XZ:
                    vector = Vector3.forward;
                    vector2 = Vector3.right;
                    break;
                case AxisPlane.XY:
                    vector = Vector3.up;
                    vector2 = Vector3.right;
                    break;
                case AxisPlane.YZ:
                    vector = Vector3.up;
                    vector2 = Vector3.forward;
                    break;
                }
            }
            else
            {
                Transform transform = this.relativeTo.Value.transform;
                switch (this.mapToPlane)
                {
                case AxisPlane.XZ:
                    vector = transform.TransformDirection(Vector3.forward);
                    vector.y = 0f;
                    vector = vector.normalized;
                    vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
                    break;
                case AxisPlane.XY:
                case AxisPlane.YZ:
                    vector = Vector3.up;
                    vector.z = 0f;
                    vector = vector.normalized;
                    vector2 = transform.TransformDirection(Vector3.right);
                    break;
                }
            }
            float num = (this.horizontalInput.IsNone ? 0f : this.horizontalInput.Value);
            float num2 = (this.verticalInput.IsNone ? 0f : this.verticalInput.Value);
            Vector3 value = num * vector2 + num2 * vector;
            value *= this.multiplier.Value;
            this.storeVector.Value = value;
            if (!this.storeMagnitude.IsNone)
            {
                this.storeMagnitude.Value = value.magnitude;
            }
        }
    }
}
