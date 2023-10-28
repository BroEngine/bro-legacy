namespace Bro
{
    public static class BroMath
    {
        public const float Deg2Rad = 0.01745329f;
        
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }
        
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }
        
        public static float Lerp(float startValue, float endValue, float interpolationValue)
        {
            interpolationValue = Clamp(interpolationValue, 0f, 1f);
            return startValue + (endValue - startValue) * interpolationValue;
        }

        public static int Limit(int value, int max) => value > max ? max : value;

        /// <summary>
        /// Calculate value percentage between 2 points
        /// </summary>
        public static float PercentsBetween(float cur, float min, float max) => (cur - min) / (max - min);
    }
}