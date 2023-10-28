using UnityEngine;

namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public class Vector2Operation : Operation<Vector2>
            {
                public Vector2Operation(Setter<Vector2> setter, Vector2 startValue, Vector2 endValue, float duration, EasingFunctions.Ease ease = EasingFunctions.Ease.Linear)
                    : base(setter, Vector2.Lerp, startValue, endValue, duration, ease)
                {
                }
            }
        }
    }
}