namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.Color), HutongGames.PlayMaker.Tooltip("Interpolate through an array of Colors over a specified amount of Time.")]
    public class ColorInterpolate : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("Array of colors to interpolate through.")]
        public FsmColor[] colors;
        [RequiredField, HutongGames.PlayMaker.Tooltip("Interpolation time.")]
        public FsmFloat time;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the interpolated color in a Color variable.")]
        public FsmColor storeColor;
        [HutongGames.PlayMaker.Tooltip("Event to send when the interpolation finishes.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale")]
        public bool realTime;
        private float startTime;
        private float currentTime;

        public override string ErrorCheck()
        {
            return ((this.colors.Length < 2) ? "Define at least 2 colors to make a gradient." : null);
        }

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0f;
            if (this.colors.Length >= 2)
            {
                this.storeColor.set_Value(this.colors[0].get_Value());
            }
            else
            {
                if (this.colors.Length == 1)
                {
                    this.storeColor.set_Value(this.colors[0].get_Value());
                }
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.currentTime = !this.realTime ? (this.currentTime + Time.deltaTime) : (FsmTime.RealtimeSinceStartup - this.startTime);
            if (this.currentTime > this.time.Value)
            {
                base.Finish();
                this.storeColor.set_Value(this.colors[this.colors.Length - 1].get_Value());
                if (this.finishEvent != null)
                {
                    base.Fsm.Event(this.finishEvent);
                }
            }
            else
            {
                Color color;
                float f = ((this.colors.Length - 1) * this.currentTime) / this.time.Value;
                if (f.Equals((float) 0f))
                {
                    color = this.colors[0].get_Value();
                }
                else if (f.Equals((float) (this.colors.Length - 1)))
                {
                    color = this.colors[this.colors.Length - 1].get_Value();
                }
                else
                {
                    Color a = this.colors[Mathf.FloorToInt(f)].get_Value();
                    color = Color.Lerp(a, this.colors[Mathf.CeilToInt(f)].get_Value(), f - Mathf.Floor(f));
                }
                this.storeColor.set_Value(color);
            }
        }

        public override void Reset()
        {
            this.colors = new FsmColor[3];
            this.time = 1f;
            this.storeColor = null;
            this.finishEvent = null;
            this.realTime = false;
        }
    }
}

