using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Rect)]
    [Tooltip("Tests if 2 Rects overlap.")]
    public class RectOverlaps : FsmStateAction
    {
        [RequiredField]
        [Tooltip("First Rectangle.")]
        public FsmRect rect1;

        [RequiredField]
        [Tooltip("Second Rectangle.")]
        public FsmRect rect2;

        [Tooltip("Event to send if the Rects overlap.")]
        public FsmEvent trueEvent;

        [Tooltip("Event to send if the Rects do not overlap.")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a variable.")]
        public FsmBool storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.rect1 = new FsmRect
            {
                UseVariable = true
            };
            this.rect2 = new FsmRect
            {
                UseVariable = true
            };
            this.storeResult = null;
            this.trueEvent = null;
            this.falseEvent = null;
            this.everyFrame = false;
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

        private void DoRectOverlap()
        {
            if (!this.rect1.IsNone && !this.rect2.IsNone)
            {
                bool flag = RectOverlaps.Intersect(this.rect1.Value, this.rect2.Value);
                this.storeResult.Value = flag;
                base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
            }
        }

        public static bool Intersect(Rect a, Rect b)
        {
            RectOverlaps.FlipNegative(ref a);
            RectOverlaps.FlipNegative(ref b);
            bool num = a.xMin < b.xMax;
            bool flag = a.xMax > b.xMin;
            bool flag2 = a.yMin < b.yMax;
            bool flag3 = a.yMax > b.yMin;
            return num && flag && flag2 && flag3;
        }

        public static void FlipNegative(ref Rect r)
        {
            if (r.width < 0f)
            {
                r.x -= (r.width *= -1f);
            }
            if (r.height < 0f)
            {
                r.y -= (r.height *= -1f);
            }
        }
    }
}
