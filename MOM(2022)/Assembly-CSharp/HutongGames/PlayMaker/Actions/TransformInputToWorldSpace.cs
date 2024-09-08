namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [NoActionTargets, ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Transforms 2d input into a 3d world space vector. E.g., can be used to transform input from a touch joystick to a movement vector.")]
    public class TransformInputToWorldSpace : FsmStateAction
    {
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The horizontal input.")]
        public FsmFloat horizontalInput;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("The vertical input.")]
        public FsmFloat verticalInput;
        [HutongGames.PlayMaker.Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
        public FsmFloat multiplier;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The world plane to map the 2d input onto.")]
        public AxisPlane mapToPlane;
        [HutongGames.PlayMaker.Tooltip("Make the result relative to a GameObject, typically the main camera.")]
        public FsmGameObject relativeTo;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the direction vector.")]
        public FsmVector3 storeVector;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the length of the direction vector.")]
        public FsmFloat storeMagnitude;

        public override void OnUpdate()
        {
            Vector3 normalized = new Vector3();
            Vector3 right = new Vector3();
            if (this.relativeTo.get_Value() != null)
            {
                Transform transform = this.relativeTo.get_Value().transform;
                AxisPlane mapToPlane = this.mapToPlane;
                if (mapToPlane == AxisPlane.XZ)
                {
                    normalized = transform.TransformDirection(Vector3.forward);
                    normalized.y = 0f;
                    normalized = normalized.normalized;
                    right = new Vector3(normalized.z, 0f, -normalized.x);
                }
                else if ((mapToPlane - 1) <= AxisPlane.XY)
                {
                    normalized = Vector3.up;
                    normalized.z = 0f;
                    normalized = normalized.normalized;
                    right = transform.TransformDirection(Vector3.right);
                }
            }
            else
            {
                switch (this.mapToPlane)
                {
                    case AxisPlane.XZ:
                        normalized = Vector3.forward;
                        right = Vector3.right;
                        break;

                    case AxisPlane.XY:
                        normalized = Vector3.up;
                        right = Vector3.right;
                        break;

                    case AxisPlane.YZ:
                        normalized = Vector3.up;
                        right = Vector3.forward;
                        break;

                    default:
                        break;
                }
            }
            float num = this.verticalInput.IsNone ? 0f : this.verticalInput.Value;
            Vector3 vector3 = (Vector3) ((((this.horizontalInput.IsNone ? 0f : this.horizontalInput.Value) * right) + (num * normalized)) * this.multiplier.Value);
            this.storeVector.set_Value(vector3);
            if (!this.storeMagnitude.IsNone)
            {
                this.storeMagnitude.Value = vector3.magnitude;
            }
        }

        public override void Reset()
        {
            this.horizontalInput = null;
            this.verticalInput = null;
            this.multiplier = 1f;
            this.mapToPlane = AxisPlane.XZ;
            this.storeVector = null;
            this.storeMagnitude = null;
        }

        public enum AxisPlane
        {
            XZ,
            XY,
            YZ
        }
    }
}

