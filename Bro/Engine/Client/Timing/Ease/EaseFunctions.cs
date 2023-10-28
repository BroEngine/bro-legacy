using System;

namespace Bro.Client
{
    public class EasingFunctions
    {
        public enum Ease
        {
            EaseInQuad = 0,
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
        }

        private const float NATURAL_LOG_OF_2 = 0.693147181f;

        //
        // Easing functions
        //

        public static float Linear(float start, float end, float value)
        {
            return Lerp(start, end, value);
        }

        public static float Spring(float start, float end, float value)
        {
            value = Clamp01(value);
            value = (float) ((Math.Sin(value * Math.PI * (0.2f + 2.5f * value * value * value)) *
                              Math.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value))));
            return start + (end - start) * value;
        }

        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1)
                return end * 0.5f * value * value + start;
            value--;
            return -end * 0.5f * (value * (value - 2) - 1) + start;
        }

        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1)
                return end * 0.5f * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value + 2) + start;
        }

        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1)
                return end * 0.5f * value * value * value * value + start;
            value -= 2;
            return -end * 0.5f * (value * value * value * value - 2) + start;
        }

        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1)
                return end * 0.5f * value * value * value * value * value + start;
            value -= 2;
            return end * 0.5f * (value * value * value * value * value + 2) + start;
        }

        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return (float) (-end * Math.Cos(value * (Math.PI * 0.5f)) + end + start);
        }

        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return (float) (end * Math.Sin(value * (Math.PI * 0.5f)) + start);
        }

        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return (float) (-end * 0.5f * (Math.Cos(Math.PI * value) - 1) + start);
        }

        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return (float) (end * Math.Pow(2, 10 * (value - 1)) + start);
        }

        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return (float) (end * (-Math.Pow(2, -10 * value) + 1) + start);
        }

        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1)
                return (float) (end * 0.5f * Math.Pow(2, 10 * (value - 1)) + start);
            value--;
            return (float) (end * 0.5f * (-Math.Pow(2, -10 * value) + 2) + start);
        }

        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return (float) (-end * (Math.Sqrt(1 - value * value) - 1) + start);
        }

        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return (float) (end * Math.Sqrt(1 - value * value) + start);
        }

        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1)
                return (float) (-end * 0.5f * (Math.Sqrt(1 - value * value) - 1) + start);
            value -= 2;
            return (float) (end * 0.5f * (Math.Sqrt(1 - value * value) + 1) + start);
        }

        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }

        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }

        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value < d * 0.5f)
                return EaseInBounce(0, end, value * 2) * 0.5f + start;
            else
                return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }

        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        public static float EaseOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        public static float EaseInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
            }

            value -= 2;
            s *= (1.525f);
            return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        public static float EaseInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (value == 0)
                return start;

            if ((value /= d) == 1)
                return start + end;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = (float) (p / (2 * Math.PI) * Math.Asin(end / a));
            }

            return (float) (-(a * Math.Pow(2, 10 * (value -= 1)) * Math.Sin((value * d - s) * (2 * Math.PI) / p)) +
                            start);
        }

        public static float EaseOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (value == 0)
                return start;

            if ((value /= d) == 1)
                return start + end;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p * 0.25f;
            }
            else
            {
                s = (float) (p / (2 * Math.PI) * Math.Asin(end / a));
            }

            return (float) (a * Math.Pow(2, -10 * value) * Math.Sin((value * d - s) * (2 * Math.PI) / p) + end + start);
        }

        public static float EaseInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (value == 0)
                return start;

            if ((value /= d * 0.5f) == 2)
                return start + end;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = (float) (p / (2 * Math.PI) * Math.Asin(end / a));
            }

            if (value < 1)
                return (float) (-0.5f * (a * Math.Pow(2, 10 * (value -= 1)) *
                                         Math.Sin((value * d - s) * (2 * Math.PI) / p)) + start);
            return (float) (a * Math.Pow(2, -10 * (value -= 1)) * Math.Sin((value * d - s) * (2 * Math.PI) / p) * 0.5f +
                            end + start);
        }

        //
        // These are derived functions that the motor can use to get the speed at a specific time.
        //
        // The easing functions all work with a normalized time (0 to 1) and the returned value here
        // reflects that. Values returned here should be divided by the actual time.
        //
        // TODO: These functions have not had the testing they deserve. If there is odd behavior around
        //       dash speeds then this would be the first place I'd look.

        public static float LinearD(float start, float end, float value)
        {
            return end - start;
        }

        public static float EaseInQuadD(float start, float end, float value)
        {
            return 2f * (end - start) * value;
        }

        public static float EaseOutQuadD(float start, float end, float value)
        {
            end -= start;
            return -end * value - end * (value - 2);
        }

        public static float EaseInOutQuadD(float start, float end, float value)
        {
            value /= .5f;
            end -= start;

            if (value < 1)
            {
                return end * value;
            }

            value--;

            return end * (1 - value);
        }

        public static float EaseInCubicD(float start, float end, float value)
        {
            return 3f * (end - start) * value * value;
        }

        public static float EaseOutCubicD(float start, float end, float value)
        {
            value--;
            end -= start;
            return 3f * end * value * value;
        }

        public static float EaseInOutCubicD(float start, float end, float value)
        {
            value /= .5f;
            end -= start;

            if (value < 1)
            {
                return (3f / 2f) * end * value * value;
            }

            value -= 2;

            return (3f / 2f) * end * value * value;
        }

        public static float EaseInQuartD(float start, float end, float value)
        {
            return 4f * (end - start) * value * value * value;
        }

        public static float EaseOutQuartD(float start, float end, float value)
        {
            value--;
            end -= start;
            return -4f * end * value * value * value;
        }

        public static float EaseInOutQuartD(float start, float end, float value)
        {
            value /= .5f;
            end -= start;

            if (value < 1)
            {
                return 2f * end * value * value * value;
            }

            value -= 2;

            return -2f * end * value * value * value;
        }

        public static float EaseInQuintD(float start, float end, float value)
        {
            return 5f * (end - start) * value * value * value * value;
        }

        public static float EaseOutQuintD(float start, float end, float value)
        {
            value--;
            end -= start;
            return 5f * end * value * value * value * value;
        }

        public static float EaseInOutQuintD(float start, float end, float value)
        {
            value /= .5f;
            end -= start;

            if (value < 1)
            {
                return (5f / 2f) * end * value * value * value * value;
            }

            value -= 2;

            return (5f / 2f) * end * value * value * value * value;
        }

        public static float EaseInSineD(float start, float end, float value)
        {
            return (float) ((end - start) * 0.5f * Math.PI * Math.Sin(0.5f * Math.PI * value));
        }

        public static float EaseOutSineD(float start, float end, float value)
        {
            end -= start;
            return (float) ((Math.PI * 0.5f) * end * Math.Cos(value * (Math.PI * 0.5f)));
        }

        public static float EaseInOutSineD(float start, float end, float value)
        {
            end -= start;
            return (float) (end * 0.5f * Math.PI * Math.Cos(Math.PI * value));
        }

        public static float EaseInExpoD(float start, float end, float value)
        {
            return (float) (10f * NATURAL_LOG_OF_2 * (end - start) * Math.Pow(2f, 10f * (value - 1)));
        }

        public static float EaseOutExpoD(float start, float end, float value)
        {
            end -= start;
            return (float) (5f * NATURAL_LOG_OF_2 * end * Math.Pow(2f, 1f - 10f * value));
        }

        public static float EaseInOutExpoD(float start, float end, float value)
        {
            value /= .5f;
            end -= start;

            if (value < 1)
            {
                return (float) (5f * NATURAL_LOG_OF_2 * end * Math.Pow(2f, 10f * (value - 1)));
            }

            value--;

            return (float) ((5f * NATURAL_LOG_OF_2 * end) / (Math.Pow(2f, 10f * value)));
        }

        public static float EaseInCircD(float start, float end, float value)
        {
            return (float) (((end - start) * value) / Math.Sqrt(1f - value * value));
        }

        public static float EaseOutCircD(float start, float end, float value)
        {
            value--;
            end -= start;
            return (float) ((-end * value) / Math.Sqrt(1f - value * value));
        }

        public static float EaseInOutCircD(float start, float end, float value)
        {
            value /= .5f;
            end -= start;

            if (value < 1)
            {
                return (float) ((end * value) / (2f * Math.Sqrt(1f - value * value)));
            }

            value -= 2;

            return (float) ((-end * value) / (2f * Math.Sqrt(1f - value * value)));
        }

        public static float EaseInBounceD(float start, float end, float value)
        {
            end -= start;
            float d = 1f;

            return EaseOutBounceD(0, end, d - value);
        }

        public static float EaseOutBounceD(float start, float end, float value)
        {
            value /= 1f;
            end -= start;

            if (value < (1 / 2.75f))
            {
                return 2f * end * 7.5625f * value;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return 2f * end * 7.5625f * value;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return 2f * end * 7.5625f * value;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return 2f * end * 7.5625f * value;
            }
        }

        public static float EaseInOutBounceD(float start, float end, float value)
        {
            end -= start;
            float d = 1f;

            if (value < d * 0.5f)
            {
                return EaseInBounceD(0, end, value * 2) * 0.5f;
            }
            else
            {
                return EaseOutBounceD(0, end, value * 2 - d) * 0.5f;
            }
        }

        public static float EaseInBackD(float start, float end, float value)
        {
            float s = 1.70158f;

            return 3f * (s + 1f) * (end - start) * value * value - 2f * s * (end - start) * value;
        }

        public static float EaseOutBackD(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value) - 1;

            return end * ((s + 1f) * value * value + 2f * value * ((s + 1f) * value + s));
        }

        public static float EaseInOutBackD(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;

            if ((value) < 1)
            {
                s *= (1.525f);
                return 0.5f * end * (s + 1) * value * value + end * value * ((s + 1f) * value - s);
            }

            value -= 2;
            s *= (1.525f);
            return 0.5f * end * ((s + 1) * value * value + 2f * value * ((s + 1f) * value + s));
        }

        public static float EaseInElasticD(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = (float) (p / (2 * Math.PI) * Math.Asin(end / a));
            }

            double c = 2 * Math.PI;

            // From an online derivative calculator, kinda hoping it is right.
            return (float) (((-a) * d * c * Math.Cos((c * (d * (value - 1f) - s)) / p)) / p -
                            5f * NATURAL_LOG_OF_2 * a * Math.Sin((c * (d * (value - 1f) - s)) / p) *
                            Math.Pow(2f, 10f * (value - 1f) + 1f));
        }

        public static float EaseOutElasticD(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p * 0.25f;
            }
            else
            {
                s = (float) (p / (2 * Math.PI) * Math.Asin(end / a));
            }

            return (float) ((a * Math.PI * d * Math.Pow(2f, 1f - 10f * value) *
                             Math.Cos((2f * Math.PI * (d * value - s)) / p)) / p - 5f * NATURAL_LOG_OF_2 * a *
                            Math.Pow(2f, 1f - 10f * value) * Math.Sin((2f * Math.PI * (d * value - s)) / p));
        }

        public static float EaseInOutElasticD(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s;
            float a = 0;

            if (a == 0f || a < Math.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = (float) (p / (2 * Math.PI) * Math.Asin(end / a));
            }

            if (value < 1)
            {
                value -= 1;

                return (float) (-5f * NATURAL_LOG_OF_2 * a * Math.Pow(2f, 10f * value) *
                                Math.Sin(2 * Math.PI * (d * value - 2f) / p) -
                                a * Math.PI * d * Math.Pow(2f, 10f * value) *
                                Math.Cos(2 * Math.PI * (d * value - s) / p) / p);
            }

            value -= 1;

            return (float) (a * Math.PI * d * Math.Cos(2f * Math.PI * (d * value - s) / p) /
                            (p * Math.Pow(2f, 10f * value)) -
                            5f * NATURAL_LOG_OF_2 * a * Math.Sin(2f * Math.PI * (d * value - s) / p) /
                            (Math.Pow(2f, 10f * value)));
        }

        public static float SpringD(float start, float end, float value)
        {
            value = Clamp01(value);
            end -= start;

            // Damn... Thanks http://www.derivative-calculator.net/
            return (float) (end * (6f * (1f - value) / 5f + 1f) * (-2.2f * Math.Pow(1f - value, 1.2f) *
                                                                   Math.Sin(Math.PI * value *
                                                                            (2.5f * value * value * value + 0.2f)) +
                                                                   Math.Pow(1f - value, 2.2f) *
                                                                   (Math.PI * (2.5f * value * value * value + 0.2f) +
                                                                    7.5f * Math.PI * value * value * value) *
                                                                   Math.Cos(Math.PI * value *
                                                                            (2.5f * value * value * value + 0.2f)) +
                                                                   1f) -
                            6f * end * (Math.Pow(1 - value, 2.2f) *
                                        Math.Sin(Math.PI * value * (2.5f * value * value * value + 0.2f)) + value
                                        / 5f));
        }

        public delegate float Function(float s, float e, float v);

        /// <summary>
        /// Returns the function associated to the easingFunction enum. This value returned should be cached as it allocates memory
        /// to return.
        /// </summary>
        /// <param name="easingFunction">The enum associated with the easing function.</param>
        /// <returns>The easing function</returns>
        public static Function GetEasingFunction(Ease easingFunction)
        {
            if (easingFunction == Ease.EaseInQuad)
            {
                return EaseInQuad;
            }

            if (easingFunction == Ease.EaseOutQuad)
            {
                return EaseOutQuad;
            }

            if (easingFunction == Ease.EaseInOutQuad)
            {
                return EaseInOutQuad;
            }

            if (easingFunction == Ease.EaseInCubic)
            {
                return EaseInCubic;
            }

            if (easingFunction == Ease.EaseOutCubic)
            {
                return EaseOutCubic;
            }

            if (easingFunction == Ease.EaseInOutCubic)
            {
                return EaseInOutCubic;
            }

            if (easingFunction == Ease.EaseInQuart)
            {
                return EaseInQuart;
            }

            if (easingFunction == Ease.EaseOutQuart)
            {
                return EaseOutQuart;
            }

            if (easingFunction == Ease.EaseInOutQuart)
            {
                return EaseInOutQuart;
            }

            if (easingFunction == Ease.EaseInQuint)
            {
                return EaseInQuint;
            }

            if (easingFunction == Ease.EaseOutQuint)
            {
                return EaseOutQuint;
            }

            if (easingFunction == Ease.EaseInOutQuint)
            {
                return EaseInOutQuint;
            }

            if (easingFunction == Ease.EaseInSine)
            {
                return EaseInSine;
            }

            if (easingFunction == Ease.EaseOutSine)
            {
                return EaseOutSine;
            }

            if (easingFunction == Ease.EaseInOutSine)
            {
                return EaseInOutSine;
            }

            if (easingFunction == Ease.EaseInExpo)
            {
                return EaseInExpo;
            }

            if (easingFunction == Ease.EaseOutExpo)
            {
                return EaseOutExpo;
            }

            if (easingFunction == Ease.EaseInOutExpo)
            {
                return EaseInOutExpo;
            }

            if (easingFunction == Ease.EaseInCirc)
            {
                return EaseInCirc;
            }

            if (easingFunction == Ease.EaseOutCirc)
            {
                return EaseOutCirc;
            }

            if (easingFunction == Ease.EaseInOutCirc)
            {
                return EaseInOutCirc;
            }

            if (easingFunction == Ease.Linear)
            {
                return Linear;
            }

            if (easingFunction == Ease.Spring)
            {
                return Spring;
            }

            if (easingFunction == Ease.EaseInBounce)
            {
                return EaseInBounce;
            }

            if (easingFunction == Ease.EaseOutBounce)
            {
                return EaseOutBounce;
            }

            if (easingFunction == Ease.EaseInOutBounce)
            {
                return EaseInOutBounce;
            }

            if (easingFunction == Ease.EaseInBack)
            {
                return EaseInBack;
            }

            if (easingFunction == Ease.EaseOutBack)
            {
                return EaseOutBack;
            }

            if (easingFunction == Ease.EaseInOutBack)
            {
                return EaseInOutBack;
            }

            if (easingFunction == Ease.EaseInElastic)
            {
                return EaseInElastic;
            }

            if (easingFunction == Ease.EaseOutElastic)
            {
                return EaseOutElastic;
            }

            if (easingFunction == Ease.EaseInOutElastic)
            {
                return EaseInOutElastic;
            }

            return null;
        }

        /// <summary>
        /// Gets the derivative function of the appropriate easing function. If you use an easing function for position then this
        /// function can get you the speed at a given time (normalized).
        /// </summary>
        /// <param name="easingFunction"></param>
        /// <returns>The derivative function</returns>
        public static Function GetEasingFunctionDerivative(Ease easingFunction)
        {
            if (easingFunction == Ease.EaseInQuad)
            {
                return EaseInQuadD;
            }

            if (easingFunction == Ease.EaseOutQuad)
            {
                return EaseOutQuadD;
            }

            if (easingFunction == Ease.EaseInOutQuad)
            {
                return EaseInOutQuadD;
            }

            if (easingFunction == Ease.EaseInCubic)
            {
                return EaseInCubicD;
            }

            if (easingFunction == Ease.EaseOutCubic)
            {
                return EaseOutCubicD;
            }

            if (easingFunction == Ease.EaseInOutCubic)
            {
                return EaseInOutCubicD;
            }

            if (easingFunction == Ease.EaseInQuart)
            {
                return EaseInQuartD;
            }

            if (easingFunction == Ease.EaseOutQuart)
            {
                return EaseOutQuartD;
            }

            if (easingFunction == Ease.EaseInOutQuart)
            {
                return EaseInOutQuartD;
            }

            if (easingFunction == Ease.EaseInQuint)
            {
                return EaseInQuintD;
            }

            if (easingFunction == Ease.EaseOutQuint)
            {
                return EaseOutQuintD;
            }

            if (easingFunction == Ease.EaseInOutQuint)
            {
                return EaseInOutQuintD;
            }

            if (easingFunction == Ease.EaseInSine)
            {
                return EaseInSineD;
            }

            if (easingFunction == Ease.EaseOutSine)
            {
                return EaseOutSineD;
            }

            if (easingFunction == Ease.EaseInOutSine)
            {
                return EaseInOutSineD;
            }

            if (easingFunction == Ease.EaseInExpo)
            {
                return EaseInExpoD;
            }

            if (easingFunction == Ease.EaseOutExpo)
            {
                return EaseOutExpoD;
            }

            if (easingFunction == Ease.EaseInOutExpo)
            {
                return EaseInOutExpoD;
            }

            if (easingFunction == Ease.EaseInCirc)
            {
                return EaseInCircD;
            }

            if (easingFunction == Ease.EaseOutCirc)
            {
                return EaseOutCircD;
            }

            if (easingFunction == Ease.EaseInOutCirc)
            {
                return EaseInOutCircD;
            }

            if (easingFunction == Ease.Linear)
            {
                return LinearD;
            }

            if (easingFunction == Ease.Spring)
            {
                return SpringD;
            }

            if (easingFunction == Ease.EaseInBounce)
            {
                return EaseInBounceD;
            }

            if (easingFunction == Ease.EaseOutBounce)
            {
                return EaseOutBounceD;
            }

            if (easingFunction == Ease.EaseInOutBounce)
            {
                return EaseInOutBounceD;
            }

            if (easingFunction == Ease.EaseInBack)
            {
                return EaseInBackD;
            }

            if (easingFunction == Ease.EaseOutBack)
            {
                return EaseOutBackD;
            }

            if (easingFunction == Ease.EaseInOutBack)
            {
                return EaseInOutBackD;
            }

            if (easingFunction == Ease.EaseInElastic)
            {
                return EaseInElasticD;
            }

            if (easingFunction == Ease.EaseOutElastic)
            {
                return EaseOutElasticD;
            }

            if (easingFunction == Ease.EaseInOutElastic)
            {
                return EaseInOutElasticD;
            }

            return null;
        }

        private static float Clamp01(float value)
        {
            if ((double) value < 0.0)
                return 0.0f;
            return (double) value > 1.0 ? 1f : value;
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }
    }
}