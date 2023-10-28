namespace Bro
{
    public class AngleNormalizer
    {
        private readonly float _minValue;
        private readonly float _maxValue;

        public AngleNormalizer(float minValue)
        {
            _minValue = minValue;
            _maxValue = _minValue + 360f;
        }

        public float GetMinDelta(float fromAngle, float toAngle)
        {
            fromAngle = Normalize(fromAngle);
            toAngle = Normalize(toAngle);
            float min1 = toAngle - fromAngle;

            if (fromAngle < toAngle)
            {
                fromAngle += 360f;
            }
            else
            {
                toAngle += 360f;
            }

            float min2 = toAngle - fromAngle;
            return System.Math.Abs(min1) < System.Math.Abs(min2) ? min1 : min2;
        }

        public float Normalize(float angle)
        {
            if (angle >= _minValue && angle < _maxValue)
            {
                return angle;
            }

            if (angle < _minValue)
            {
                angle += 360f * (float) ((int) (_minValue - angle) / 360 + 1);
                return angle;
            }

            angle -= 360f * (float) ((int) (angle - _maxValue) / 360 + 1);
            return angle;
        }
    }
}