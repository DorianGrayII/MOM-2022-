namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Debug), HutongGames.PlayMaker.Tooltip("Draws a line from a Start point in a direction. Specify the start point as Game Objects or Vector3 world positions. If both are specified, position is used as a local offset from the Object's position.")]
    public class DrawDebugRay : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Draw ray from a GameObject.")]
        public FsmGameObject fromObject;
        [HutongGames.PlayMaker.Tooltip("Draw ray from a world position, or local offset from GameObject if provided.")]
        public FsmVector3 fromPosition;
        [HutongGames.PlayMaker.Tooltip("Direction vector of ray.")]
        public FsmVector3 direction;
        [HutongGames.PlayMaker.Tooltip("The color of the ray.")]
        public FsmColor color;

        public override void OnUpdate()
        {
            Debug.DrawRay(ActionHelpers.GetPosition(this.fromObject, this.fromPosition), this.direction.get_Value(), this.color.get_Value());
        }

        public override void Reset()
        {
            FsmGameObject obj1 = new FsmGameObject();
            obj1.UseVariable = true;
            this.fromObject = obj1;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.fromPosition = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.direction = vector2;
            this.color = (FsmColor) Color.white;
        }
    }
}

