using UnityEngine;

namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public class Vector3Operation : Operation<Vector3>
            {
                public Vector3Operation(Setter<Vector3> setter, Vector3 startValue, Vector3 endValue, float duration, EasingFunctions.Ease ease = EasingFunctions.Ease.Linear)
                    : base(setter, UnityEngine.Vector3.Lerp, startValue, endValue, duration, ease)
                {
                    
                }
            }
        }
    }
}