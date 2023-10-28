using Bro.Client;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client
{
    public static class EaseExtensions
    {
        public static Timing.Ease.Base CreateFade(this CanvasGroup target, float startAlpha, float endAlpha, float duration)
        {
            target.alpha = startAlpha;
            var ease = new Timing.Ease.FloatOperation(value => target.alpha = value, startAlpha, endAlpha, duration);
            return ease;
        }

        public static Timing.Ease.Base CreateScale(this RectTransform target, UnityEngine.Vector3 startScale, UnityEngine.Vector3 endScale, float duration)
        {
            target.localScale = startScale;
            var ease = new Timing.Ease.Vector3Operation(value => target.localScale = value, startScale, endScale, duration);
            return ease;
        }

        public static Timing.Ease.Base CreateMove(this RectTransform target, UnityEngine.Vector2 start, UnityEngine.Vector2 end, float duration)
        {
            target.anchoredPosition = start;
            var ease = new Timing.Ease.Vector2Operation((a) => { target.position = a; }, start, end, duration);
            return ease;
        }

        public static Timing.Ease.Base CreateMoveX(this RectTransform target, float startPositionX, float endPositionX, float duration)
        {
            target.position = new UnityEngine.Vector3(startPositionX, target.position.y);
            var ease = new Timing.Ease.FloatOperation(value => target.position = new UnityEngine.Vector3(value, target.position.y),
                startPositionX, endPositionX, duration);

            return ease;
        }

        public static Timing.Ease.Base CreateMoveY(this RectTransform target, float startPositionY, float endPositionY, float duration)
        {
            target.position = new UnityEngine.Vector3(target.position.x, startPositionY);
            var ease = new Timing.Ease.FloatOperation(value => target.position = new UnityEngine.Vector3(target.position.x, value),
                startPositionY, endPositionY, duration);

            return ease;
        }


        public static Timing.Ease.Base CreateFillAmount(this Image target, float startValue, float endValue, float duration)
        {
            target.fillAmount = startValue;
            var ease = new Timing.Ease.FloatOperation(value => target.fillAmount = value, startValue, endValue, duration);
            return ease;
        }

        public static Timing.Ease.Base CreateFade(this Image target, float startValue, float endValue, float duration)
        {
            target.color = new UnityEngine.Color(target.color.r, target.color.g, target.color.b, startValue);
            var ease = new Timing.Ease.FloatOperation(value => target.color =
                    new UnityEngine.Color(target.color.r, target.color.g, target.color.b, value), startValue, endValue, duration);
            return ease;
        }

        public static Timing.Ease.Base CreateMoveAnchoredPosition(this RectTransform target, UnityEngine.Vector2 startPosition, UnityEngine.Vector2 endPosition, float duration, EasingFunctions.Ease easingFunctions)
        {
            target.anchoredPosition = (UnityEngine.Vector2) startPosition;
            var ease = new Timing.Ease.Vector2Operation(value => target.anchoredPosition = new UnityEngine.Vector2(value.x, value.y), startPosition, endPosition, duration, easingFunctions);
            return ease;
        }

        public static Timing.Ease.Base CreateMovePosition(this Transform target, UnityEngine.Vector3 startPosition, UnityEngine.Vector3 endPosition, float duration, EasingFunctions.Ease easingFunctions)
        {
            target.position = startPosition;
            var ease = new Timing.Ease.Vector3Operation(value => target.position = value,
                startPosition, endPosition, duration, easingFunctions);
            return ease;
        }

        public static Timing.Ease.Base CreateMovePositionY(this Transform target, float endPositionY, float duration, EasingFunctions.Ease easingFunctions)
        {
            var startPosition = target.position;
            var endPosition = new UnityEngine.Vector3(target.position.x, endPositionY, target.position.z);
            var ease = new Timing.Ease.Vector3Operation(value => target.position = value,
                startPosition, endPosition, duration, easingFunctions);
            return ease;
        }

        public static Timing.Ease.Base CreateScale(this RectTransform target, UnityEngine.Vector3 endScale, float duration, EasingFunctions.Ease easingFunctions)
        {
            var startScale = target.localScale;
            var ease = new Timing.Ease.Vector3Operation(value => target.localScale = value,
                startScale, endScale, duration, easingFunctions);
            return ease;
        }

        public static Timing.Ease.Base CreateFade(this SpriteRenderer target, float startValue, float endValue, float duration)
        {
            var start = new Timing.Ease.Instant(() => target.color = new UnityEngine.Color(target.color.r, target.color.g, target.color.b, startValue));
            var ease = new Timing.Ease.FloatOperation(value => target.color =
                    new UnityEngine.Color(target.color.r, target.color.g, target.color.b, value),
                startValue, endValue, duration);
            var sequence = new Timing.Ease.Sequence(start, ease);
            return sequence;
        }

        public static Timing.Ease.Base CreateLerp(this float target, float startPositionX, float endPositionX, float duration)
        {
            target = startPositionX;
            var ease = new Timing.Ease.FloatOperation(value => target = value,
                startPositionX, endPositionX, duration);

            return ease;
        }
    }
}