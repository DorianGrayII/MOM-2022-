namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Rect), HutongGames.PlayMaker.Tooltip("Tests if a point is inside a rectangle.")]
    public class RectContains : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Rectangle to test.")]
        public FsmRect rectangle;
        [HutongGames.PlayMaker.Tooltip("Point to test.")]
        public FsmVector3 point;
        [HutongGames.PlayMaker.Tooltip("Specify/override X value.")]
        public FsmFloat x;
        [HutongGames.PlayMaker.Tooltip("Specify/override Y value.")]
        public FsmFloat y;
        [HutongGames.PlayMaker.Tooltip("Event to send if the Point is inside the Rectangle.")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send if the Point is outside the Rectangle.")]
        public FsmEvent falseEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in a variable.")]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoRectContains()
        {
            if (!this.rectangle.IsNone)
            {
                Vector3 point = this.point.get_Value();
                if (!this.x.IsNone)
                {
                    point.x = this.x.Value;
                }
                if (!this.y.IsNone)
                {
                    point.y = this.y.Value;
                }
                bool flag = this.rectangle.get_Value().Contains(point);
                this.storeResult.Value = flag;
                base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
            }
        }

        public override void OnEnter()
        {
            this.DoRectContains();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoRectContains();
        }

        public override void Reset()
        {
            FsmRect rect1 = new FsmRect();
            rect1.UseVariable = true;
            this.rectangle = rect1;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.point = vector1;
            FsmFloat num1 = new FsmFloat();
            num1.UseVariable = true;
            this.x = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.y = num2;
            this.storeResult = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.everyFrame = false;
        }
    }
}

