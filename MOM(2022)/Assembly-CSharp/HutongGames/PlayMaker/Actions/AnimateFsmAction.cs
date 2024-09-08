using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public abstract class AnimateFsmAction : FsmStateAction
    {
        public enum Calculation
        {
            None = 0,
            SetValue = 1,
            AddToValue = 2,
            SubtractFromValue = 3,
            SubtractValueFromCurve = 4,
            MultiplyValue = 5,
            DivideValue = 6,
            DivideCurveByValue = 7
        }

        [Tooltip("Define animation time,\u00a0scaling the curve to fit.")]
        public FsmFloat time;

        [Tooltip("If you define speed, your animation will speed up or slow down.")]
        public FsmFloat speed;

        [Tooltip("Delayed animation start.")]
        public FsmFloat delay;

        [Tooltip("Animation curve start from any time. If IgnoreCurveOffset is true the animation starts right after the state become entered.")]
        public FsmBool ignoreCurveOffset;

        [Tooltip("Optionally send an Event when the animation finishes.")]
        public FsmEvent finishEvent;

        [Tooltip("Ignore TimeScale. Useful if the game is paused.")]
        public bool realTime;

        private float startTime;

        private float currentTime;

        private float[] endTimes;

        private float lastTime;

        private float deltaTime;

        private float delayTime;

        private float[] keyOffsets;

        protected AnimationCurve[] curves;

        protected Calculation[] calculations;

        protected float[] resultFloats;

        protected float[] fromFloats;

        protected float[] toFloats;

        protected bool finishAction;

        protected bool isRunning;

        protected bool looping;

        private bool start;

        private float largestEndTime;

        public override void Reset()
        {
            this.finishEvent = null;
            this.realTime = false;
            this.time = new FsmFloat
            {
                UseVariable = true
            };
            this.speed = new FsmFloat
            {
                UseVariable = true
            };
            this.delay = new FsmFloat
            {
                UseVariable = true
            };
            this.ignoreCurveOffset = new FsmBool
            {
                Value = true
            };
            this.resultFloats = new float[0];
            this.fromFloats = new float[0];
            this.toFloats = new float[0];
            this.endTimes = new float[0];
            this.keyOffsets = new float[0];
            this.curves = new AnimationCurve[0];
            this.finishAction = false;
            this.start = false;
        }

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
            this.deltaTime = 0f;
            this.currentTime = 0f;
            this.isRunning = false;
            this.finishAction = false;
            this.looping = false;
            this.delayTime = (this.delay.IsNone ? 0f : (this.delayTime = this.delay.Value));
            this.start = true;
        }

        protected void Init()
        {
            this.endTimes = new float[this.curves.Length];
            this.keyOffsets = new float[this.curves.Length];
            this.largestEndTime = 0f;
            for (int i = 0; i < this.curves.Length; i++)
            {
                if (this.curves[i] != null && this.curves[i].keys.Length != 0)
                {
                    this.keyOffsets[i] = ((this.curves[i].keys.Length == 0) ? 0f : (this.time.IsNone ? this.curves[i].keys[0].time : (this.time.Value / this.curves[i].keys[this.curves[i].length - 1].time * this.curves[i].keys[0].time)));
                    this.currentTime = (this.ignoreCurveOffset.IsNone ? 0f : (this.ignoreCurveOffset.Value ? this.keyOffsets[i] : 0f));
                    if (!this.time.IsNone)
                    {
                        this.endTimes[i] = this.time.Value;
                    }
                    else
                    {
                        this.endTimes[i] = this.curves[i].keys[this.curves[i].length - 1].time;
                    }
                    if (this.largestEndTime < this.endTimes[i])
                    {
                        this.largestEndTime = this.endTimes[i];
                    }
                    if (!this.looping)
                    {
                        this.looping = ActionHelpers.IsLoopingWrapMode(this.curves[i].postWrapMode);
                    }
                }
                else
                {
                    this.endTimes[i] = -1f;
                }
            }
            for (int j = 0; j < this.curves.Length; j++)
            {
                if (this.largestEndTime > 0f && this.endTimes[j] == -1f)
                {
                    this.endTimes[j] = this.largestEndTime;
                }
                else if (this.largestEndTime == 0f && this.endTimes[j] == -1f)
                {
                    if (this.time.IsNone)
                    {
                        this.endTimes[j] = 1f;
                    }
                    else
                    {
                        this.endTimes[j] = this.time.Value;
                    }
                }
            }
            this.UpdateAnimation();
        }

        public override void OnUpdate()
        {
            this.CheckStart();
            if (this.isRunning)
            {
                this.UpdateTime();
                this.UpdateAnimation();
                this.CheckFinished();
            }
        }

        private void CheckStart()
        {
            if (this.isRunning || !this.start)
            {
                return;
            }
            if (this.delayTime >= 0f)
            {
                if (this.realTime)
                {
                    this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
                    this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
                    this.delayTime -= this.deltaTime;
                }
                else
                {
                    this.delayTime -= Time.deltaTime;
                }
            }
            else
            {
                this.isRunning = true;
                this.start = false;
            }
        }

        private void UpdateTime()
        {
            if (this.realTime)
            {
                this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
                this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
                if (!this.speed.IsNone)
                {
                    this.currentTime += this.deltaTime * this.speed.Value;
                }
                else
                {
                    this.currentTime += this.deltaTime;
                }
            }
            else if (!this.speed.IsNone)
            {
                this.currentTime += Time.deltaTime * this.speed.Value;
            }
            else
            {
                this.currentTime += Time.deltaTime;
            }
        }

        public void UpdateAnimation()
        {
            for (int i = 0; i < this.curves.Length; i++)
            {
                if (this.curves[i] != null && this.curves[i].keys.Length != 0)
                {
                    if (this.calculations[i] != 0)
                    {
                        switch (this.calculations[i])
                        {
                        case Calculation.SetValue:
                            if (!this.time.IsNone)
                            {
                                this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
                            }
                            else
                            {
                                this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime);
                            }
                            break;
                        case Calculation.AddToValue:
                            if (!this.time.IsNone)
                            {
                                this.resultFloats[i] = this.fromFloats[i] + this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
                            }
                            else
                            {
                                this.resultFloats[i] = this.fromFloats[i] + this.curves[i].Evaluate(this.currentTime);
                            }
                            break;
                        case Calculation.SubtractFromValue:
                            if (!this.time.IsNone)
                            {
                                this.resultFloats[i] = this.fromFloats[i] - this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time);
                            }
                            else
                            {
                                this.resultFloats[i] = this.fromFloats[i] - this.curves[i].Evaluate(this.currentTime);
                            }
                            break;
                        case Calculation.SubtractValueFromCurve:
                            if (!this.time.IsNone)
                            {
                                this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) - this.fromFloats[i];
                            }
                            else
                            {
                                this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) - this.fromFloats[i];
                            }
                            break;
                        case Calculation.MultiplyValue:
                            if (!this.time.IsNone)
                            {
                                this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) * this.fromFloats[i];
                            }
                            else
                            {
                                this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) * this.fromFloats[i];
                            }
                            break;
                        case Calculation.DivideValue:
                            if (!this.time.IsNone)
                            {
                                this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) != 0f) ? (this.fromFloats[i] / this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time)) : float.MaxValue);
                            }
                            else
                            {
                                this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime) != 0f) ? (this.fromFloats[i] / this.curves[i].Evaluate(this.currentTime)) : float.MaxValue);
                            }
                            break;
                        case Calculation.DivideCurveByValue:
                            if (!this.time.IsNone)
                            {
                                this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) / this.fromFloats[i]) : float.MaxValue);
                            }
                            else
                            {
                                this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime) / this.fromFloats[i]) : float.MaxValue);
                            }
                            break;
                        }
                    }
                    else
                    {
                        this.resultFloats[i] = this.fromFloats[i];
                    }
                }
                else
                {
                    this.resultFloats[i] = this.fromFloats[i];
                }
            }
        }

        private void CheckFinished()
        {
            if (!this.isRunning || this.looping)
            {
                return;
            }
            this.finishAction = true;
            for (int i = 0; i < this.endTimes.Length; i++)
            {
                if (this.currentTime < this.endTimes[i])
                {
                    this.finishAction = false;
                }
            }
            this.isRunning = !this.finishAction;
        }
    }
}
