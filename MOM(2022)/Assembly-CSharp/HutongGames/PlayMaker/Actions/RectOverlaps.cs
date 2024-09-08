namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Rect), HutongGames.PlayMaker.Tooltip("Tests if 2 Rects overlap.")]
    public class RectOverlaps : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("First Rectangle.")]
        public FsmRect rect1;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Second Rectangle.")]
        public FsmRect rect2;
        [HutongGames.PlayMaker.Tooltip("Event to send if the Rects overlap.")]
        public FsmEvent trueEvent;
        [HutongGames.PlayMaker.Tooltip("Event to send if the Rects do not overlap.")]
        public FsmEvent falseEvent;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result in a variable.")]
        public FsmBool storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private void DoRectOverlap()
        {
            if (!this.rect1.IsNone && !this.rect2.IsNone)
            {
                bool flag = Intersect(this.rect1.get_Value(), this.rect2.get_Value());
                this.storeResult.Value = flag;
                base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
            }
        }

        public static void FlipNegative(ref Rect r)
        {
            float num;
            if (r.width < 0f)
            {
                r.width = num = r.width * -1f;
                r.x -= num;
            }
            if (r.height < 0f)
            {
                r.height = num = r.height * -1f;
                r.y -= num;
            }
        }

        public static bool Intersect(Rect a, Rect b)
        {
            FlipNegative(ref a);
            FlipNegative(ref b);
            bool flag2 = a.yMin < b.yMax;
            bool flag3 = a.yMax > b.yMin;
            return ((((a.xMin < b.xMax) & (a.xMax > b.xMin)) & flag2) & flag3);
        }

        public override void OnEnter()
        {
            this.DoRectOverlap();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoRectOverlap();
        }

        public override void Reset()
        {
            FsmRect rect1 = new FsmRect();
            rect1.UseVariable = true;
            this.rect1 = rect1;
            FsmRect rect2 = new FsmRect();
            rect2.UseVariable = true;
            this.rect2 = rect2;
            this.storeResult = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.everyFrame = false;
        }
    }
}

