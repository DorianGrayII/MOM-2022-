namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Debug), HutongGames.PlayMaker.Tooltip("Draw Gizmos in the Scene View.")]
    public class DebugDrawShape : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Draw the Gizmo at a GameObject's position.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The type of Gizmo to draw:\nSphere, Cube, WireSphere, or WireCube.")]
        public ShapeType shape;
        [HutongGames.PlayMaker.Tooltip("The color to use.")]
        public FsmColor color;
        [HutongGames.PlayMaker.Tooltip("Use this for sphere gizmos")]
        public FsmFloat radius;
        [HutongGames.PlayMaker.Tooltip("Use this for cube gizmos")]
        public FsmVector3 size;

        public override void OnDrawActionGizmos()
        {
            Transform transform = base.Fsm.GetOwnerDefaultTarget(this.gameObject).transform;
            if (transform != null)
            {
                Gizmos.color = this.color.get_Value();
                switch (this.shape)
                {
                    case ShapeType.Sphere:
                        Gizmos.DrawSphere(transform.position, this.radius.Value);
                        return;

                    case ShapeType.Cube:
                        Gizmos.DrawCube(transform.position, this.size.get_Value());
                        return;

                    case ShapeType.WireSphere:
                        Gizmos.DrawWireSphere(transform.position, this.radius.Value);
                        return;

                    case ShapeType.WireCube:
                        Gizmos.DrawWireCube(transform.position, this.size.get_Value());
                        return;
                }
            }
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.shape = ShapeType.Sphere;
            this.color = (FsmColor) Color.grey;
            this.radius = 1f;
            this.size = (FsmVector3) new Vector3(1f, 1f, 1f);
        }

        public enum ShapeType
        {
            Sphere,
            Cube,
            WireSphere,
            WireCube
        }
    }
}

