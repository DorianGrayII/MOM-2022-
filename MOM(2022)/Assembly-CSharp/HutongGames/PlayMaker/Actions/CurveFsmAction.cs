// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// HutongGames.PlayMaker.Actions.CurveFsmAction
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

[global::HutongGames.PlayMaker.Tooltip("Animate base action - DON'T USE IT!")]
public abstract class CurveFsmAction : FsmStateAction
{
    public enum Calculation
    {
        None = 0,
        AddToValue = 1,
        SubtractFromValue = 2,
        SubtractValueFromCurve = 3,
        MultiplyValue = 4,
        DivideValue = 5,
        DivideCurveByValue = 6
    }

    [global::HutongGames.PlayMaker.Tooltip("Define animation time, scaling the curve to fit.")]
    public FsmFloat time;

    [global::HutongGames.PlayMaker.Tooltip("If you define speed, your animation will speed up or slow down.")]
    public FsmFloat speed;

    [global::HutongGames.PlayMaker.Tooltip("Delayed animation start.")]
    public FsmFloat delay;

    [global::HutongGames.PlayMaker.Tooltip("Animation curve start from any time. If IgnoreCurveOffset is true the animation starts right after the state become entered.")]
    public FsmBool ignoreCurveOffset;

    [global::HutongGames.PlayMaker.Tooltip("Optionally send an Event when the animation finishes.")]
    public FsmEvent finishEvent;

    [global::HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
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

    private float[] distances;

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
        this.distances = new float[0];
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
        this.distances = new float[this.fromFloats.Length];
        for (int k = 0; k < this.fromFloats.Length; k++)
        {
            this.distances[k] = this.toFloats[k] - this.fromFloats[k];
        }
    }

    public override void OnUpdate()
    {
        if (!this.isRunning && this.start)
        {
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
                this.startTime = FsmTime.RealtimeSinceStartup;
                this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
            }
        }
        if (!this.isRunning || this.finishAction)
        {
            return;
        }
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
        for (int i = 0; i < this.curves.Length; i++)
        {
            if (this.curves[i] != null && this.curves[i].keys.Length != 0)
            {
                if (this.calculations[i] != 0)
                {
                    switch (this.calculations[i])
                    {
                    case Calculation.AddToValue:
                        if (!this.time.IsNone)
                        {
                            this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.time.Value) + this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time));
                        }
                        else
                        {
                            this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.endTimes[i]) + this.curves[i].Evaluate(this.currentTime));
                        }
                        break;
                    case Calculation.SubtractFromValue:
                        if (!this.time.IsNone)
                        {
                            this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.time.Value) - this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time));
                        }
                        else
                        {
                            this.resultFloats[i] = this.fromFloats[i] + (this.distances[i] * (this.currentTime / this.endTimes[i]) - this.curves[i].Evaluate(this.currentTime));
                        }
                        break;
                    case Calculation.SubtractValueFromCurve:
                        if (!this.time.IsNone)
                        {
                            this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) - this.distances[i] * (this.currentTime / this.time.Value) + this.fromFloats[i];
                        }
                        else
                        {
                            this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) - this.distances[i] * (this.currentTime / this.endTimes[i]) + this.fromFloats[i];
                        }
                        break;
                    case Calculation.MultiplyValue:
                        if (!this.time.IsNone)
                        {
                            this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) * this.distances[i] * (this.currentTime / this.time.Value) + this.fromFloats[i];
                        }
                        else
                        {
                            this.resultFloats[i] = this.curves[i].Evaluate(this.currentTime) * this.distances[i] * (this.currentTime / this.endTimes[i]) + this.fromFloats[i];
                        }
                        break;
                    case Calculation.DivideValue:
                        if (!this.time.IsNone)
                        {
                            this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) != 0f) ? (this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value) / this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time)) : float.MaxValue);
                        }
                        else
                        {
                            this.resultFloats[i] = ((this.curves[i].Evaluate(this.currentTime) != 0f) ? (this.fromFloats[i] + this.distances[i] * (this.currentTime / this.endTimes[i]) / this.curves[i].Evaluate(this.currentTime)) : float.MaxValue);
                        }
                        break;
                    case Calculation.DivideCurveByValue:
                        if (!this.time.IsNone)
                        {
                            this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime / this.time.Value * this.curves[i].keys[this.curves[i].length - 1].time) / (this.distances[i] * (this.currentTime / this.time.Value)) + this.fromFloats[i]) : float.MaxValue);
                        }
                        else
                        {
                            this.resultFloats[i] = ((this.fromFloats[i] != 0f) ? (this.curves[i].Evaluate(this.currentTime) / (this.distances[i] * (this.currentTime / this.endTimes[i])) + this.fromFloats[i]) : float.MaxValue);
                        }
                        break;
                    }
                }
                else if (!this.time.IsNone)
                {
                    this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value);
                }
                else
                {
                    this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.endTimes[i]);
                }
            }
            else if (!this.time.IsNone)
            {
                this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.time.Value);
            }
            else if (this.largestEndTime == 0f)
            {
                this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / 1f);
            }
            else
            {
                this.resultFloats[i] = this.fromFloats[i] + this.distances[i] * (this.currentTime / this.largestEndTime);
            }
        }
        if (!this.isRunning)
        {
            return;
        }
        this.finishAction = true;
        for (int j = 0; j < this.endTimes.Length; j++)
        {
            if (this.currentTime < this.endTimes[j])
            {
                this.finishAction = false;
            }
        }
        this.isRunning = !this.finishAction;
    }
}
