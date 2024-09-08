namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [NoActionTargets, ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Gets a world direction Vector from 2 Input Axis. Typically used for a third person controller with Relative To set to the camera.")]
    public class GetAxisVector : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The name of the horizontal input axis. See Unity Input Manager.")]
        public FsmString horizontalAxis;
        [HutongGames.PlayMaker.Tooltip("The name of the vertical input axis. See Unity Input Manager.")]
        public FsmString verticalAxis;
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
            float num = (this.verticalAxis.IsNone || string.IsNullOrEmpty(this.verticalAxis.Value)) ? 0f : Input.GetAxis(this.verticalAxis.Value);
            Vector3 vector3 = (Vector3) (((((this.horizontalAxis.IsNone || string.IsNullOrEmpty(this.horizontalAxis.Value)) ? 0f : Input.GetAxis(this.horizontalAxis.Value)) * right) + (num * normalized)) * this.multiplier.Value);
            this.storeVector.set_Value(vector3);
            if (!this.storeMagnitude.IsNone)
            {
                this.storeMagnitude.Value = vector3.magnitude;
            }
        }

        public override void Reset()
        {
            this.horizontalAxis = "Horizontal";
            this.verticalAxis = "Vertical";
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

