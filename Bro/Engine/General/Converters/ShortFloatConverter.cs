namespace Bro
{
    public class ShortFloatConverter
    {
        public readonly float MinValue;
        public readonly float MaxValue;

        private readonly float _diffValue;
        private const float ShortRange = (float) (short.MaxValue - short.MinValue);

        public ShortFloatConverter(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            _diffValue = maxValue - MinValue;
        }

        public float ToFloat(short value)
        {
            return MinValue + _diffValue * ((float) (value - short.MinValue) / (ShortRange));
        }

        public bool IsValid(float value)
        {
            return value >= MinValue && value <= MaxValue;
        }

        public short ToShort(float value)
        {
            if (value < MinValue)
            {
                Log.Error(string.Format("value = {2} should be between {0} and {1}", MinValue, MaxValue, value));
                return short.MinValue;
            }

            if (value > MaxValue)
            {
                Log.Error(string.Format("value = {2} should be between {0} and {1}", MinValue, MaxValue, value));
                return short.MaxValue;
            }

            short result = short.MinValue;
            result += (short) (ShortRange * ((value - MinValue) / _diffValue));

            return result;
        }
    }
}