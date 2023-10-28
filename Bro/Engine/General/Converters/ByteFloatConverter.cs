namespace Bro
{
    public class ByteFloatConverter
    {
        public readonly float MinValue;
        public readonly float MaxValue;

        private readonly float _diffValue;
        private const float ByteRange = (float) (byte.MaxValue - byte.MinValue);
        private readonly string _debugData;

        public ByteFloatConverter(float minValue, float maxValue, string debugData = null)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            _debugData = debugData;
            _diffValue = maxValue - MinValue;
        }

        public float ToFloat(byte value)
        {
            return MinValue + _diffValue * ((float) (value - byte.MinValue) / (ByteRange));
        }

        public byte ToByte(float value)
        {
            if (value < MinValue)
            {
                Log.Error(string.Format("value = {2} should be between {0} and {1}; {3}", MinValue, MaxValue, value,
                    _debugData));
                return byte.MinValue;
            }

            if (value > MaxValue)
            {
                Log.Error(string.Format("value = {2} should be between {0} and {1}; {3}", MinValue, MaxValue, value,
                    _debugData));
                return byte.MaxValue;
            }

            byte result = byte.MinValue;
            result += (byte) (ByteRange * ((value - MinValue) / _diffValue));

            return result;
        }
    }
}