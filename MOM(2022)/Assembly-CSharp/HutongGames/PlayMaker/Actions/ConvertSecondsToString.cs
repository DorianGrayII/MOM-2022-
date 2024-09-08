namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Convert), HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=1711.0"), Tooltip("Converts Seconds to a String value representing the time.")]
    public class ConvertSecondsToString : FsmStateAction
    {
        [RequiredField, UIHint(UIHint.Variable), Tooltip("The seconds variable to convert.")]
        public FsmFloat secondsVariable;
        [RequiredField, UIHint(UIHint.Variable), Tooltip("A string variable to store the time value.")]
        public FsmString stringVariable;
        [RequiredField, Tooltip("Format. 0 for days, 1 is for hours, 2 for minutes, 3 for seconds and 4 for milliseconds. 5 for total days, 6 for total hours, 7 for total minutes, 8 for total seconds, 9 for total milliseconds, 10 for two digits milliseconds. so {2:D2} would just show the seconds of the current time, NOT the grand total number of seconds, the grand total of seconds would be {8:F0}")]
        public FsmString format;
        [Tooltip("Repeat every frame. Useful if the seconds variable is changing.")]
        public bool everyFrame;

        private void DoConvertSecondsToString()
        {
            TimeSpan span = TimeSpan.FromSeconds((double) this.secondsVariable.Value);
            string str = span.Milliseconds.ToString("D3").PadLeft(2, '0').Substring(0, 2);
            object[] args = new object[11];
            args[0] = span.Days;
            args[1] = span.Hours;
            args[2] = span.Minutes;
            args[3] = span.Seconds;
            args[4] = span.Milliseconds;
            args[5] = span.TotalDays;
            args[6] = span.TotalHours;
            args[7] = span.TotalMinutes;
            args[8] = span.TotalSeconds;
            args[9] = span.TotalMilliseconds;
            args[10] = str;
            this.stringVariable.Value = string.Format(this.format.Value, args);
        }

        public override void OnEnter()
        {
            this.DoConvertSecondsToString();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoConvertSecondsToString();
        }

        public override void Reset()
        {
            this.secondsVariable = null;
            this.stringVariable = null;
            this.everyFrame = false;
            this.format = "{1:D2}h:{2:D2}m:{3:D2}s:{10}ms";
        }
    }
}

