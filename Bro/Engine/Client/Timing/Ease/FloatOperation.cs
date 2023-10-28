namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public class FloatOperation : Operation<float>
            {
                public FloatOperation(Setter<float> setter, float startValue, float endValue,  float duration, EasingFunctions.Ease ease = EasingFunctions.Ease.Linear)
                    : base(setter, BroMath.Lerp, startValue, endValue, duration, ease)
                {
                    
                }
            }
        }
    }
}