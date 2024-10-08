using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Color)]
    [Tooltip("Interpolate through an array of Colors over a specified amount of Time.")]
    public class ColorInterpolate : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Array of colors to interpolate through.")]
        public FsmColor[] colors;

        [RequiredField]
        [Tooltip("Interpolation time.")]
        public FsmFloat time;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the interpolated color in a Color variable.")]
        public FsmColor storeColor;

        [Tooltip("Event to send when the interpolation finishes.")]
        public FsmEvent finishEvent;

        [Tooltip("Ignore TimeScale")]
        public bool realTime;

        private float startTime;

        private float currentTime;

        public override void Reset()
        {
            this.colors = new FsmColor[3];
            this.time = 1f;
            this.storeColor = null;
            this.finishEvent = null;
            this.realTime = false;
        }

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0f;
            if (this.colors.Length < 2)
            {
                if (this.colors.Length == 1)
                {
                    this.storeColor.Value = this.colors[0].Value;
                }
                base.Finish();
            }
            else
            {
                this.storeColor.Value = this.colors[0].Value;
            }
        }

        public override void OnUpdate()
        {
            if (this.realTime)
            {
                this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
            }
            else
            {
                this.currentTime += Time.deltaTime;
            }
            if (this.currentTime > this.time.Value)
            {
                base.Finish();
                this.storeColor.Value = this.colors[this.colors.Length - 1].Value;
                if (this.finishEvent != null)
                {
                    base.Fsm.Event(this.finishEvent);
                }
                return;
            }
            float num = (float)(this.colors.Length - 1) * this.currentTime / this.time.Value;
            Color value;
            if (num.Equals(0f))
            {
                value = this.colors[0].Value;
            }
            else if (num.Equals(this.colors.Length - 1))
            {
                value = this.colors[this.colors.Length - 1].Value;
            }
            else
            {
                Color value2 = this.colors[Mathf.FloorToInt(num)].Value;
                Color value3 = this.colors[Mathf.CeilToInt(num)].Value;
                num -= Mathf.Floor(num);
                value = Color.Lerp(value2, value3, num);
            }
            this.storeColor.Value = value;
        }

        public override string ErrorCheck()
        {
            if (this.colors.Length >= 2)
            {
                return null;
            }
            return "Define at least 2 colors to make a gradient.";
        }
    }
}
