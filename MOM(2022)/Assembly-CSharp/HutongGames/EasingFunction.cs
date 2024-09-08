namespace HutongGames
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class EasingFunction
    {
        private const float NATURAL_LOG_OF_2 = 0.6931472f;
        public static UnityEngine.AnimationCurve AnimationCurve;

        public static float CustomCurve(float start, float end, float value)
        {
            return ((AnimationCurve != null) ? Mathf.Lerp(start, end, AnimationCurve.Evaluate(value)) : Mathf.Lerp(start, end, value));
        }

        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1f;
            float num = 1.70158f;
            return ((((end * value) * value) * (((num + 1f) * value) - num)) + start);
        }

        public static float EaseInBackD(float start, float end, float value)
        {
            float num = 1.70158f;
            return (((((3f * (num + 1f)) * (end - start)) * value) * value) - (((2f * num) * (end - start)) * value));
        }

        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float num = 1f;
            return ((end - EaseOutBounce(0f, end, num - value)) + start);
        }

        public static float EaseInBounceD(float start, float end, float value)
        {
            end -= start;
            float num = 1f;
            return EaseOutBounceD(0f, end, num - value);
        }

        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return ((-end * (Mathf.Sqrt(1f - (value * value)) - 1f)) + start);
        }

        public static float EaseInCircD(float start, float end, float value)
        {
            return (((end - start) * value) / Mathf.Sqrt(1f - (value * value)));
        }

        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return ((((end * value) * value) * value) + start);
        }

        public static float EaseInCubicD(float start, float end, float value)
        {
            return (((3f * (end - start)) * value) * value);
        }

        public static float EaseInElastic(float start, float end, float value)
        {
            float num3;
            end -= start;
            float num = 1f;
            float num2 = num * 0.3f;
            float num4 = 0f;
            if (value == 0f)
            {
                return start;
            }
            float single1 = value / num;
            if ((value = single1) == 1f)
            {
                return (start + end);
            }
            if ((num4 != 0f) && (num4 >= Mathf.Abs(end)))
            {
                num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
            }
            else
            {
                num4 = end;
                num3 = num2 / 4f;
            }
            float single2 = value - 1f;
            return (-((num4 * Mathf.Pow(2f, 10f * (value = single2))) * Mathf.Sin((((value * num) - num3) * 6.283185f) / num2)) + start);
        }

        public static float EaseInElasticD(float start, float end, float value)
        {
            float num3;
            end -= start;
            float num = 1f;
            float num2 = num * 0.3f;
            float num4 = 0f;
            if ((num4 != 0f) && (num4 >= Mathf.Abs(end)))
            {
                num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
            }
            else
            {
                num4 = end;
                num3 = num2 / 4f;
            }
            float num5 = 6.283185f;
            return (((((-num4 * num) * num5) * Mathf.Cos((num5 * ((num * (value - 1f)) - num3)) / num2)) / num2) - (((3.465736f * num4) * Mathf.Sin((num5 * ((num * (value - 1f)) - num3)) / num2)) * Mathf.Pow(2f, (10f * (value - 1f)) + 1f)));
        }

        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return ((end * Mathf.Pow(2f, 10f * (value - 1f))) + start);
        }

        public static float EaseInExpoD(float start, float end, float value)
        {
            return ((6.931472f * (end - start)) * Mathf.Pow(2f, 10f * (value - 1f)));
        }

        public static float EaseInOutBack(float start, float end, float value)
        {
            float num = 1.70158f;
            end -= start;
            value /= 0.5f;
            if (value < 1f)
            {
                num *= 1.525f;
                return (((end * 0.5f) * ((value * value) * (((num + 1f) * value) - num))) + start);
            }
            value -= 2f;
            num *= 1.525f;
            return (((end * 0.5f) * (((value * value) * (((num + 1f) * value) + num)) + 2f)) + start);
        }

        public static float EaseInOutBackD(float start, float end, float value)
        {
            float num = 1.70158f;
            end -= start;
            value /= 0.5f;
            if (value < 1f)
            {
                num *= 1.525f;
                return (((((0.5f * end) * (num + 1f)) * value) * value) + ((end * value) * (((num + 1f) * value) - num)));
            }
            value -= 2f;
            num *= 1.525f;
            return ((0.5f * end) * ((((num + 1f) * value) * value) + ((2f * value) * (((num + 1f) * value) + num))));
        }

        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float num = 1f;
            return ((value >= (num * 0.5f)) ? (((EaseOutBounce(0f, end, (value * 2f) - num) * 0.5f) + (end * 0.5f)) + start) : ((EaseInBounce(0f, end, value * 2f) * 0.5f) + start));
        }

        public static float EaseInOutBounceD(float start, float end, float value)
        {
            end -= start;
            float num = 1f;
            return ((value >= (num * 0.5f)) ? (EaseOutBounceD(0f, end, (value * 2f) - num) * 0.5f) : (EaseInBounceD(0f, end, value * 2f) * 0.5f));
        }

        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return (((-end * 0.5f) * (Mathf.Sqrt(1f - (value * value)) - 1f)) + start);
            }
            value -= 2f;
            return (((end * 0.5f) * (Mathf.Sqrt(1f - (value * value)) + 1f)) + start);
        }

        public static float EaseInOutCircD(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return ((end * value) / (2f * Mathf.Sqrt(1f - (value * value))));
            }
            value -= 2f;
            return ((-end * value) / (2f * Mathf.Sqrt(1f - (value * value))));
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return (((((end * 0.5f) * value) * value) * value) + start);
            }
            value -= 2f;
            return (((end * 0.5f) * (((value * value) * value) + 2f)) + start);
        }

        public static float EaseInOutCubicD(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value >= 1f)
            {
                value -= 2f;
            }
            return (((1.5f * end) * value) * value);
        }

        public static float EaseInOutElastic(float start, float end, float value)
        {
            float num3;
            end -= start;
            float num = 1f;
            float num2 = num * 0.3f;
            float num4 = 0f;
            if (value == 0f)
            {
                return start;
            }
            float single1 = value / (num * 0.5f);
            if ((value = single1) == 2f)
            {
                return (start + end);
            }
            if ((num4 != 0f) && (num4 >= Mathf.Abs(end)))
            {
                num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
            }
            else
            {
                num4 = end;
                num3 = num2 / 4f;
            }
            if (value < 1f)
            {
                float single2 = value - 1f;
                return ((-0.5f * ((num4 * Mathf.Pow(2f, 10f * (value = single2))) * Mathf.Sin((((value * num) - num3) * 6.283185f) / num2))) + start);
            }
            float single3 = value - 1f;
            return (((((num4 * Mathf.Pow(2f, -10f * (value = single3))) * Mathf.Sin((((value * num) - num3) * 6.283185f) / num2)) * 0.5f) + end) + start);
        }

        public static float EaseInOutElasticD(float start, float end, float value)
        {
            float num3;
            end -= start;
            float num = 1f;
            float num2 = num * 0.3f;
            float num4 = 0f;
            if ((num4 != 0f) && (num4 >= Mathf.Abs(end)))
            {
                num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
            }
            else
            {
                num4 = end;
                num3 = num2 / 4f;
            }
            if (value < 1f)
            {
                value--;
                return ((((-3.465736f * num4) * Mathf.Pow(2f, 10f * value)) * Mathf.Sin((6.283185f * ((num * value) - 2f)) / num2)) - (((((num4 * 3.141593f) * num) * Mathf.Pow(2f, 10f * value)) * Mathf.Cos((6.283185f * ((num * value) - num3)) / num2)) / num2));
            }
            value--;
            return (((((num4 * 3.141593f) * num) * Mathf.Cos((6.283185f * ((num * value) - num3)) / num2)) / (num2 * Mathf.Pow(2f, 10f * value))) - (((3.465736f * num4) * Mathf.Sin((6.283185f * ((num * value) - num3)) / num2)) / Mathf.Pow(2f, 10f * value)));
        }

        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return (((end * 0.5f) * Mathf.Pow(2f, 10f * (value - 1f))) + start);
            }
            value--;
            return (((end * 0.5f) * (-Mathf.Pow(2f, -10f * value) + 2f)) + start);
        }

        public static float EaseInOutExpoD(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return ((3.465736f * end) * Mathf.Pow(2f, 10f * (value - 1f)));
            }
            value--;
            return ((3.465736f * end) / Mathf.Pow(2f, 10f * value));
        }

        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return ((((end * 0.5f) * value) * value) + start);
            }
            value--;
            return (((-end * 0.5f) * ((value * (value - 2f)) - 1f)) + start);
        }

        public static float EaseInOutQuadD(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return (end * value);
            }
            value--;
            return (end * (1f - value));
        }

        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return ((((((end * 0.5f) * value) * value) * value) * value) + start);
            }
            value -= 2f;
            return (((-end * 0.5f) * ((((value * value) * value) * value) - 2f)) + start);
        }

        public static float EaseInOutQuartD(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return ((((2f * end) * value) * value) * value);
            }
            value -= 2f;
            return ((((-2f * end) * value) * value) * value);
        }

        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value < 1f)
            {
                return (((((((end * 0.5f) * value) * value) * value) * value) * value) + start);
            }
            value -= 2f;
            return (((end * 0.5f) * (((((value * value) * value) * value) * value) + 2f)) + start);
        }

        public static float EaseInOutQuintD(float start, float end, float value)
        {
            value /= 0.5f;
            end -= start;
            if (value >= 1f)
            {
                value -= 2f;
            }
            return (((((2.5f * end) * value) * value) * value) * value);
        }

        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return (((-end * 0.5f) * (Mathf.Cos(3.141593f * value) - 1f)) + start);
        }

        public static float EaseInOutSineD(float start, float end, float value)
        {
            end -= start;
            return (((end * 0.5f) * 3.141593f) * Mathf.Cos(3.141593f * value));
        }

        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return (((end * value) * value) + start);
        }

        public static float EaseInQuadD(float start, float end, float value)
        {
            return ((2f * (end - start)) * value);
        }

        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return (((((end * value) * value) * value) * value) + start);
        }

        public static float EaseInQuartD(float start, float end, float value)
        {
            return ((((4f * (end - start)) * value) * value) * value);
        }

        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return ((((((end * value) * value) * value) * value) * value) + start);
        }

        public static float EaseInQuintD(float start, float end, float value)
        {
            return (((((5f * (end - start)) * value) * value) * value) * value);
        }

        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return (((-end * Mathf.Cos(value * 1.570796f)) + end) + start);
        }

        public static float EaseInSineD(float start, float end, float value)
        {
            return ((((end - start) * 0.5f) * 3.141593f) * Mathf.Sin(1.570796f * value));
        }

        public static float EaseOutBack(float start, float end, float value)
        {
            float num = 1.70158f;
            end -= start;
            value--;
            return ((end * (((value * value) * (((num + 1f) * value) + num)) + 1f)) + start);
        }

        public static float EaseOutBackD(float start, float end, float value)
        {
            float num = 1.70158f;
            end -= start;
            value--;
            return (end * ((((num + 1f) * value) * value) + ((2f * value) * (((num + 1f) * value) + num))));
        }

        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < 0.3636364f)
            {
                return ((end * ((7.5625f * value) * value)) + start);
            }
            if (value < 0.7272727f)
            {
                value -= 0.5454546f;
                return ((end * (((7.5625f * value) * value) + 0.75f)) + start);
            }
            if (value < 0.90909090909090906)
            {
                value -= 0.8181818f;
                return ((end * (((7.5625f * value) * value) + 0.9375f)) + start);
            }
            value -= 0.9545454f;
            return ((end * (((7.5625f * value) * value) + 0.984375f)) + start);
        }

        public static float EaseOutBounceD(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value >= 0.3636364f)
            {
                if (value < 0.7272727f)
                {
                    value -= 0.5454546f;
                    return (((2f * end) * 7.5625f) * value);
                }
                if (value < 0.90909090909090906)
                {
                    value -= 0.8181818f;
                    return (((2f * end) * 7.5625f) * value);
                }
                value -= 0.9545454f;
            }
            return (((2f * end) * 7.5625f) * value);
        }

        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return ((end * Mathf.Sqrt(1f - (value * value))) + start);
        }

        public static float EaseOutCircD(float start, float end, float value)
        {
            value--;
            end -= start;
            return ((-end * value) / Mathf.Sqrt(1f - (value * value)));
        }

        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return ((end * (((value * value) * value) + 1f)) + start);
        }

        public static float EaseOutCubicD(float start, float end, float value)
        {
            value--;
            end -= start;
            return (((3f * end) * value) * value);
        }

        public static float EaseOutElastic(float start, float end, float value)
        {
            float num3;
            end -= start;
            float num = 1f;
            float num2 = num * 0.3f;
            float num4 = 0f;
            if (value == 0f)
            {
                return start;
            }
            float single1 = value / num;
            if ((value = single1) == 1f)
            {
                return (start + end);
            }
            if ((num4 != 0f) && (num4 >= Mathf.Abs(end)))
            {
                num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
            }
            else
            {
                num4 = end;
                num3 = num2 * 0.25f;
            }
            return ((((num4 * Mathf.Pow(2f, -10f * value)) * Mathf.Sin((((value * num) - num3) * 6.283185f) / num2)) + end) + start);
        }

        public static float EaseOutElasticD(float start, float end, float value)
        {
            float num3;
            end -= start;
            float num = 1f;
            float num2 = num * 0.3f;
            float num4 = 0f;
            if ((num4 != 0f) && (num4 >= Mathf.Abs(end)))
            {
                num3 = (num2 / 6.283185f) * Mathf.Asin(end / num4);
            }
            else
            {
                num4 = end;
                num3 = num2 * 0.25f;
            }
            return ((((((num4 * 3.141593f) * num) * Mathf.Pow(2f, 1f - (10f * value))) * Mathf.Cos((6.283185f * ((num * value) - num3)) / num2)) / num2) - (((3.465736f * num4) * Mathf.Pow(2f, 1f - (10f * value))) * Mathf.Sin((6.283185f * ((num * value) - num3)) / num2)));
        }

        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return ((end * (-Mathf.Pow(2f, -10f * value) + 1f)) + start);
        }

        public static float EaseOutExpoD(float start, float end, float value)
        {
            end -= start;
            return ((3.465736f * end) * Mathf.Pow(2f, 1f - (10f * value)));
        }

        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return (((-end * value) * (value - 2f)) + start);
        }

        public static float EaseOutQuadD(float start, float end, float value)
        {
            end -= start;
            return ((-end * value) - (end * (value - 2f)));
        }

        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return ((-end * ((((value * value) * value) * value) - 1f)) + start);
        }

        public static float EaseOutQuartD(float start, float end, float value)
        {
            value--;
            end -= start;
            return ((((-4f * end) * value) * value) * value);
        }

        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return ((end * (((((value * value) * value) * value) * value) + 1f)) + start);
        }

        public static float EaseOutQuintD(float start, float end, float value)
        {
            value--;
            end -= start;
            return (((((5f * end) * value) * value) * value) * value);
        }

        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return ((end * Mathf.Sin(value * 1.570796f)) + start);
        }

        public static float EaseOutSineD(float start, float end, float value)
        {
            end -= start;
            return ((1.570796f * end) * Mathf.Cos(value * 1.570796f));
        }

        public static Function GetEasingFunction(Ease easingFunction)
        {
            return ((easingFunction != Ease.CustomCurve) ? ((easingFunction != Ease.EaseInQuad) ? ((easingFunction != Ease.EaseOutQuad) ? ((easingFunction != Ease.EaseInOutQuad) ? ((easingFunction != Ease.EaseInCubic) ? ((easingFunction != Ease.EaseOutCubic) ? ((easingFunction != Ease.EaseInOutCubic) ? ((easingFunction != Ease.EaseInQuart) ? ((easingFunction != Ease.EaseOutQuart) ? ((easingFunction != Ease.EaseInOutQuart) ? ((easingFunction != Ease.EaseInQuint) ? ((easingFunction != Ease.EaseOutQuint) ? ((easingFunction != Ease.EaseInOutQuint) ? ((easingFunction != Ease.EaseInSine) ? ((easingFunction != Ease.EaseOutSine) ? ((easingFunction != Ease.EaseInOutSine) ? ((easingFunction != Ease.EaseInExpo) ? ((easingFunction != Ease.EaseOutExpo) ? ((easingFunction != Ease.EaseInOutExpo) ? ((easingFunction != Ease.EaseInCirc) ? ((easingFunction != Ease.EaseOutCirc) ? ((easingFunction != Ease.EaseInOutCirc) ? ((easingFunction != Ease.Linear) ? ((easingFunction != Ease.Spring) ? ((easingFunction != Ease.EaseInBounce) ? ((easingFunction != Ease.EaseOutBounce) ? ((easingFunction != Ease.EaseInOutBounce) ? ((easingFunction != Ease.EaseInBack) ? ((easingFunction != Ease.EaseOutBack) ? ((easingFunction != Ease.EaseInOutBack) ? ((easingFunction != Ease.EaseInElastic) ? ((easingFunction != Ease.EaseOutElastic) ? ((easingFunction != Ease.EaseInOutElastic) ? null : new Function(EasingFunction.EaseInOutElastic)) : new Function(EasingFunction.EaseOutElastic)) : new Function(EasingFunction.EaseInElastic)) : new Function(EasingFunction.EaseInOutBack)) : new Function(EasingFunction.EaseOutBack)) : new Function(EasingFunction.EaseInBack)) : new Function(EasingFunction.EaseInOutBounce)) : new Function(EasingFunction.EaseOutBounce)) : new Function(EasingFunction.EaseInBounce)) : new Function(EasingFunction.Spring)) : new Function(EasingFunction.Linear)) : new Function(EasingFunction.EaseInOutCirc)) : new Function(EasingFunction.EaseOutCirc)) : new Function(EasingFunction.EaseInCirc)) : new Function(EasingFunction.EaseInOutExpo)) : new Function(EasingFunction.EaseOutExpo)) : new Function(EasingFunction.EaseInExpo)) : new Function(EasingFunction.EaseInOutSine)) : new Function(EasingFunction.EaseOutSine)) : new Function(EasingFunction.EaseInSine)) : new Function(EasingFunction.EaseInOutQuint)) : new Function(EasingFunction.EaseOutQuint)) : new Function(EasingFunction.EaseInQuint)) : new Function(EasingFunction.EaseInOutQuart)) : new Function(EasingFunction.EaseOutQuart)) : new Function(EasingFunction.EaseInQuart)) : new Function(EasingFunction.EaseInOutCubic)) : new Function(EasingFunction.EaseOutCubic)) : new Function(EasingFunction.EaseInCubic)) : new Function(EasingFunction.EaseInOutQuad)) : new Function(EasingFunction.EaseOutQuad)) : new Function(EasingFunction.EaseInQuad)) : new Function(EasingFunction.CustomCurve));
        }

        public static Function GetEasingFunctionDerivative(Ease easingFunction)
        {
            return ((easingFunction != Ease.EaseInQuad) ? ((easingFunction != Ease.EaseOutQuad) ? ((easingFunction != Ease.EaseInOutQuad) ? ((easingFunction != Ease.EaseInCubic) ? ((easingFunction != Ease.EaseOutCubic) ? ((easingFunction != Ease.EaseInOutCubic) ? ((easingFunction != Ease.EaseInQuart) ? ((easingFunction != Ease.EaseOutQuart) ? ((easingFunction != Ease.EaseInOutQuart) ? ((easingFunction != Ease.EaseInQuint) ? ((easingFunction != Ease.EaseOutQuint) ? ((easingFunction != Ease.EaseInOutQuint) ? ((easingFunction != Ease.EaseInSine) ? ((easingFunction != Ease.EaseOutSine) ? ((easingFunction != Ease.EaseInOutSine) ? ((easingFunction != Ease.EaseInExpo) ? ((easingFunction != Ease.EaseOutExpo) ? ((easingFunction != Ease.EaseInOutExpo) ? ((easingFunction != Ease.EaseInCirc) ? ((easingFunction != Ease.EaseOutCirc) ? ((easingFunction != Ease.EaseInOutCirc) ? ((easingFunction != Ease.Linear) ? ((easingFunction != Ease.Spring) ? ((easingFunction != Ease.EaseInBounce) ? ((easingFunction != Ease.EaseOutBounce) ? ((easingFunction != Ease.EaseInOutBounce) ? ((easingFunction != Ease.EaseInBack) ? ((easingFunction != Ease.EaseOutBack) ? ((easingFunction != Ease.EaseInOutBack) ? ((easingFunction != Ease.EaseInElastic) ? ((easingFunction != Ease.EaseOutElastic) ? ((easingFunction != Ease.EaseInOutElastic) ? null : new Function(EasingFunction.EaseInOutElasticD)) : new Function(EasingFunction.EaseOutElasticD)) : new Function(EasingFunction.EaseInElasticD)) : new Function(EasingFunction.EaseInOutBackD)) : new Function(EasingFunction.EaseOutBackD)) : new Function(EasingFunction.EaseInBackD)) : new Function(EasingFunction.EaseInOutBounceD)) : new Function(EasingFunction.EaseOutBounceD)) : new Function(EasingFunction.EaseInBounceD)) : new Function(EasingFunction.SpringD)) : new Function(EasingFunction.LinearD)) : new Function(EasingFunction.EaseInOutCircD)) : new Function(EasingFunction.EaseOutCircD)) : new Function(EasingFunction.EaseInCircD)) : new Function(EasingFunction.EaseInOutExpoD)) : new Function(EasingFunction.EaseOutExpoD)) : new Function(EasingFunction.EaseInExpoD)) : new Function(EasingFunction.EaseInOutSineD)) : new Function(EasingFunction.EaseOutSineD)) : new Function(EasingFunction.EaseInSineD)) : new Function(EasingFunction.EaseInOutQuintD)) : new Function(EasingFunction.EaseOutQuintD)) : new Function(EasingFunction.EaseInQuintD)) : new Function(EasingFunction.EaseInOutQuartD)) : new Function(EasingFunction.EaseOutQuartD)) : new Function(EasingFunction.EaseInQuartD)) : new Function(EasingFunction.EaseInOutCubicD)) : new Function(EasingFunction.EaseOutCubicD)) : new Function(EasingFunction.EaseInCubicD)) : new Function(EasingFunction.EaseInOutQuadD)) : new Function(EasingFunction.EaseOutQuadD)) : new Function(EasingFunction.EaseInQuadD));
        }

        public static float Linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        public static float LinearD(float start, float end, float value)
        {
            return (end - start);
        }

        public static float Spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = ((Mathf.Sin((value * 3.141593f) * (0.2f + (((2.5f * value) * value) * value))) * Mathf.Pow(1f - value, 2.2f)) + value) * (1f + (1.2f * (1f - value)));
            return (start + ((end - start) * value));
        }

        public static float SpringD(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            end -= start;
            return (((end * (((6f * (1f - value)) / 5f) + 1f)) * ((((-2.2f * Mathf.Pow(1f - value, 1.2f)) * Mathf.Sin((3.141593f * value) * ((((2.5f * value) * value) * value) + 0.2f))) + ((Mathf.Pow(1f - value, 2.2f) * ((3.141593f * ((((2.5f * value) * value) * value) + 0.2f)) + (((23.56194f * value) * value) * value))) * Mathf.Cos((3.141593f * value) * ((((2.5f * value) * value) * value) + 0.2f)))) + 1f)) - ((6f * end) * ((Mathf.Pow(1f - value, 2.2f) * Mathf.Sin((3.141593f * value) * ((((2.5f * value) * value) * value) + 0.2f))) + (value / 5f))));
        }

        public enum Ease
        {
            EaseInQuad,
            EaseOutQuad,
            EaseInOutQuad,
            EaseInCubic,
            EaseOutCubic,
            EaseInOutCubic,
            EaseInQuart,
            EaseOutQuart,
            EaseInOutQuart,
            EaseInQuint,
            EaseOutQuint,
            EaseInOutQuint,
            EaseInSine,
            EaseOutSine,
            EaseInOutSine,
            EaseInExpo,
            EaseOutExpo,
            EaseInOutExpo,
            EaseInCirc,
            EaseOutCirc,
            EaseInOutCirc,
            Linear,
            Spring,
            EaseInBounce,
            EaseOutBounce,
            EaseInOutBounce,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
            EaseInElastic,
            EaseOutElastic,
            EaseInOutElastic,
            CustomCurve
        }

        public delegate float Function(float s, float e, float v);
    }
}

