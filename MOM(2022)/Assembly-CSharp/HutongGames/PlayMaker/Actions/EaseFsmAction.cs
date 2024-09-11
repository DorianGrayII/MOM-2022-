using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Ease base action - don't use!")]
    public abstract class EaseFsmAction : FsmStateAction
    {
        protected delegate float EasingFunction(float start, float end, float value);

        public enum EaseType
        {
            easeInQuad = 0,
            easeOutQuad = 1,
            easeInOutQuad = 2,
            easeInCubic = 3,
            easeOutCubic = 4,
            easeInOutCubic = 5,
            easeInQuart = 6,
            easeOutQuart = 7,
            easeInOutQuart = 8,
            easeInQuint = 9,
            easeOutQuint = 10,
            easeInOutQuint = 11,
            easeInSine = 12,
            easeOutSine = 13,
            easeInOutSine = 14,
            easeInExpo = 15,
            easeOutExpo = 16,
            easeInOutExpo = 17,
            easeInCirc = 18,
            easeOutCirc = 19,
            easeInOutCirc = 20,
            linear = 21,
            spring = 22,
            bounce = 23,
            easeInBack = 24,
            easeOutBack = 25,
            easeInOutBack = 26,
            elastic = 27,
            punch = 28
        }

        [RequiredField]
        public FsmFloat time;

        public FsmFloat speed;

        public FsmFloat delay;

        public EaseType easeType = EaseType.linear;

        public FsmBool reverse;

        [Tooltip("Optionally send an Event when the animation finishes.")]
        public FsmEvent finishEvent;

        [Tooltip("Ignore TimeScale. Useful if the game is paused.")]
        public bool realTime;

        protected EasingFunction ease;

        protected float runningTime;

        protected float lastTime;

        protected float startTime;

        protected float deltaTime;

        protected float delayTime;

        protected float percentage;

        protected float[] fromFloats = new float[0];

        protected float[] toFloats = new float[0];

        protected float[] resultFloats = new float[0];

        protected bool finishAction;

        protected bool start;

        protected bool finished;

        protected bool isRunning;

        public override void Reset()
        {
            this.easeType = EaseType.linear;
            this.time = new FsmFloat
            {
                Value = 1f
            };
            this.delay = new FsmFloat
            {
                UseVariable = true
            };
            this.speed = new FsmFloat
            {
                UseVariable = true
            };
            this.reverse = new FsmBool
            {
                Value = false
            };
            this.realTime = false;
            this.finishEvent = null;
            this.ease = null;
            this.runningTime = 0f;
            this.lastTime = 0f;
            this.percentage = 0f;
            this.fromFloats = new float[0];
            this.toFloats = new float[0];
            this.resultFloats = new float[0];
            this.finishAction = false;
            this.start = false;
            this.finished = false;
            this.isRunning = false;
        }

        public override void OnEnter()
        {
            this.finished = false;
            this.isRunning = false;
            this.SetEasingFunction();
            this.runningTime = 0f;
            this.percentage = (this.reverse.IsNone ? 0f : (this.reverse.Value ? 1f : 0f));
            this.finishAction = false;
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
            this.delayTime = (this.delay.IsNone ? 0f : (this.delayTime = this.delay.Value));
            this.start = true;
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            if (this.start && !this.isRunning)
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
            if (!this.isRunning || this.finished)
            {
                return;
            }
            if (this.reverse.IsNone || !this.reverse.Value)
            {
                this.UpdatePercentage();
                if (this.percentage < 1f)
                {
                    for (int i = 0; i < this.fromFloats.Length; i++)
                    {
                        this.resultFloats[i] = this.ease(this.fromFloats[i], this.toFloats[i], this.percentage);
                    }
                }
                else
                {
                    this.finishAction = true;
                    this.finished = true;
                    this.isRunning = false;
                }
                return;
            }
            this.UpdatePercentage();
            if (this.percentage > 0f)
            {
                for (int j = 0; j < this.fromFloats.Length; j++)
                {
                    this.resultFloats[j] = this.ease(this.fromFloats[j], this.toFloats[j], this.percentage);
                }
            }
            else
            {
                this.finishAction = true;
                this.finished = true;
                this.isRunning = false;
            }
        }

        protected void UpdatePercentage()
        {
            if (this.realTime)
            {
                this.deltaTime = FsmTime.RealtimeSinceStartup - this.startTime - this.lastTime;
                this.lastTime = FsmTime.RealtimeSinceStartup - this.startTime;
                if (!this.speed.IsNone)
                {
                    this.runningTime += this.deltaTime * this.speed.Value;
                }
                else
                {
                    this.runningTime += this.deltaTime;
                }
            }
            else if (!this.speed.IsNone)
            {
                this.runningTime += Time.deltaTime * this.speed.Value;
            }
            else
            {
                this.runningTime += Time.deltaTime;
            }
            if (!this.reverse.IsNone && this.reverse.Value)
            {
                this.percentage = 1f - this.runningTime / this.time.Value;
            }
            else
            {
                this.percentage = this.runningTime / this.time.Value;
            }
        }

        protected void SetEasingFunction()
        {
            switch (this.easeType)
            {
            case EaseType.easeInQuad:
                this.ease = easeInQuad;
                break;
            case EaseType.easeOutQuad:
                this.ease = easeOutQuad;
                break;
            case EaseType.easeInOutQuad:
                this.ease = easeInOutQuad;
                break;
            case EaseType.easeInCubic:
                this.ease = easeInCubic;
                break;
            case EaseType.easeOutCubic:
                this.ease = easeOutCubic;
                break;
            case EaseType.easeInOutCubic:
                this.ease = easeInOutCubic;
                break;
            case EaseType.easeInQuart:
                this.ease = easeInQuart;
                break;
            case EaseType.easeOutQuart:
                this.ease = easeOutQuart;
                break;
            case EaseType.easeInOutQuart:
                this.ease = easeInOutQuart;
                break;
            case EaseType.easeInQuint:
                this.ease = easeInQuint;
                break;
            case EaseType.easeOutQuint:
                this.ease = easeOutQuint;
                break;
            case EaseType.easeInOutQuint:
                this.ease = easeInOutQuint;
                break;
            case EaseType.easeInSine:
                this.ease = easeInSine;
                break;
            case EaseType.easeOutSine:
                this.ease = easeOutSine;
                break;
            case EaseType.easeInOutSine:
                this.ease = easeInOutSine;
                break;
            case EaseType.easeInExpo:
                this.ease = easeInExpo;
                break;
            case EaseType.easeOutExpo:
                this.ease = easeOutExpo;
                break;
            case EaseType.easeInOutExpo:
                this.ease = easeInOutExpo;
                break;
            case EaseType.easeInCirc:
                this.ease = easeInCirc;
                break;
            case EaseType.easeOutCirc:
                this.ease = easeOutCirc;
                break;
            case EaseType.easeInOutCirc:
                this.ease = easeInOutCirc;
                break;
            case EaseType.linear:
                this.ease = linear;
                break;
            case EaseType.spring:
                this.ease = spring;
                break;
            case EaseType.bounce:
                this.ease = bounce;
                break;
            case EaseType.easeInBack:
                this.ease = easeInBack;
                break;
            case EaseType.easeOutBack:
                this.ease = easeOutBack;
                break;
            case EaseType.easeInOutBack:
                this.ease = easeInOutBack;
                break;
            case EaseType.elastic:
                this.ease = elastic;
                break;
            case EaseType.punch:
                this.ease = elastic;
                break;
            }
        }

        protected float linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        protected float clerp(float start, float end, float value)
        {
            float num = 0f;
            float num2 = 360f;
            float num3 = Mathf.Abs((num2 - num) / 2f);
      //      float num4 = 0f;
            float num5 = 0f;
            if (end - start < 0f - num3)
            {
                num5 = (num2 - start + end) * value;
                return start + num5;
            }
            if (end - start > num3)
            {
                num5 = (0f - (num2 - end + start)) * value;
                return start + num5;
            }
            return start + (end - start) * value;
        }

        protected float spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * (float)Math.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
            return start + (end - start) * value;
        }

        protected float easeInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        protected float easeOutQuad(float start, float end, float value)
        {
            end -= start;
            return (0f - end) * value * (value - 2f) + start;
        }

        protected float easeInOutQuad(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return end / 2f * value * value + start;
            }
            value -= 1f;
            return (0f - end) / 2f * (value * (value - 2f) - 1f) + start;
        }

        protected float easeInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        protected float easeOutCubic(float start, float end, float value)
        {
            value -= 1f;
            end -= start;
            return end * (value * value * value + 1f) + start;
        }

        protected float easeInOutCubic(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return end / 2f * value * value * value + start;
            }
            value -= 2f;
            return end / 2f * (value * value * value + 2f) + start;
        }

        protected float easeInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        protected float easeOutQuart(float start, float end, float value)
        {
            value -= 1f;
            end -= start;
            return (0f - end) * (value * value * value * value - 1f) + start;
        }

        protected float easeInOutQuart(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return end / 2f * value * value * value * value + start;
            }
            value -= 2f;
            return (0f - end) / 2f * (value * value * value * value - 2f) + start;
        }

        protected float easeInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        protected float easeOutQuint(float start, float end, float value)
        {
            value -= 1f;
            end -= start;
            return end * (value * value * value * value * value + 1f) + start;
        }

        protected float easeInOutQuint(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return end / 2f * value * value * value * value * value + start;
            }
            value -= 2f;
            return end / 2f * (value * value * value * value * value + 2f) + start;
        }

        protected float easeInSine(float start, float end, float value)
        {
            end -= start;
            return (0f - end) * Mathf.Cos(value / 1f * ((float)Math.PI / 2f)) + end + start;
        }

        protected float easeOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value / 1f * ((float)Math.PI / 2f)) + start;
        }

        protected float easeInOutSine(float start, float end, float value)
        {
            end -= start;
            return (0f - end) / 2f * (Mathf.Cos((float)Math.PI * value / 1f) - 1f) + start;
        }

        protected float easeInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
        }

        protected float easeOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (0f - Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
        }

        protected float easeInOutExpo(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
            }
            value -= 1f;
            return end / 2f * (0f - Mathf.Pow(2f, -10f * value) + 2f) + start;
        }

        protected float easeInCirc(float start, float end, float value)
        {
            end -= start;
            return (0f - end) * (Mathf.Sqrt(1f - value * value) - 1f) + start;
        }

        protected float easeOutCirc(float start, float end, float value)
        {
            value -= 1f;
            end -= start;
            return end * Mathf.Sqrt(1f - value * value) + start;
        }

        protected float easeInOutCirc(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return (0f - end) / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
            }
            value -= 2f;
            return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
        }

        protected float bounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < 0.36363637f)
            {
                return end * (7.5625f * value * value) + start;
            }
            if (value < 0.72727275f)
            {
                value -= 0.54545456f;
                return end * (7.5625f * value * value + 0.75f) + start;
            }
            if ((double)value < 0.9090909090909091)
            {
                value -= 0.8181818f;
                return end * (7.5625f * value * value + 0.9375f) + start;
            }
            value -= 21f / 22f;
            return end * (7.5625f * value * value + 63f / 64f) + start;
        }

        protected float easeInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1f;
            float num = 1.70158f;
            return end * value * value * ((num + 1f) * value - num) + start;
        }

        protected float easeOutBack(float start, float end, float value)
        {
            float num = 1.70158f;
            end -= start;
            value = value / 1f - 1f;
            return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
        }

        protected float easeInOutBack(float start, float end, float value)
        {
            float num = 1.70158f;
            end -= start;
            value /= 0.5f;
            if (value < 1f)
            {
                num *= 1.525f;
                return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
            }
            value -= 2f;
            num *= 1.525f;
            return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
        }

        protected float punch(float amplitude, float value)
        {
            float num = 9f;
            if (value == 0f)
            {
                return 0f;
            }
            if (value == 1f)
            {
                return 0f;
            }
            float num2 = 0.3f;
            num = num2 / ((float)Math.PI * 2f) * Mathf.Asin(0f);
            return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num) * ((float)Math.PI * 2f) / num2);
        }

        protected float elastic(float start, float end, float value)
        {
            end -= start;
            float num = 1f;
            float num2 = num * 0.3f;
            float num3 = 0f;
            float num4 = 0f;
            if (value == 0f)
            {
                return start;
            }
            if ((value /= num) == 1f)
            {
                return start + end;
            }
            if (num4 == 0f || num4 < Mathf.Abs(end))
            {
                num4 = end;
                num3 = num2 / 4f;
            }
            else
            {
                num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(end / num4);
            }
            return num4 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num3) * ((float)Math.PI * 2f) / num2) + end + start;
        }
    }
}
