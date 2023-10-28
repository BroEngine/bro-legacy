using System;

namespace Bro.Sketch
{
    public struct Color : IEquatable<Color>
    {
        public class Palette
        {
            public Color[] Colors = new Color[]
            {
                new Color(43, 51, 191),
                new Color(109, 204, 242),
                new Color(191, 216, 4),
                new Color(242, 182, 4),
                new Color(242, 135, 4),
                new Color(255, 0, 0),
                new Color(0, 255, 0),
            };
        }
        
        public float r;
        public float g;
        public float b;
        public float a;

        /// <summary>
        /// All values should be in range from 0 to 1
        /// </summary>
        public Color(float r, float g, float b, float a = 1f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Color(int r, int g, int b, int a = 255)
        {
            this.r = r / 255f;
            this.g = g / 255f;
            this.b = b / 255f;
            this.a = a / 255f;
        }
        
        public override string ToString()
        {
            return string.Format("({0:F3}, {1:F3}, {2:F3}, {3:F3})", r, g, b, a);
        }

      
        
        


#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
        public Color(UnityEngine.Color color) : this(color.r, color.g, color.b, color.a)
        {
        }

        public static explicit operator UnityEngine.Color(Color color)
        {
            return new UnityEngine.Color(color.r, color.g, color.b, color.a);
        }
#endif
        public bool Equals(Color other)
        {
            return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b) && a.Equals(other.a);
        }

        public override bool Equals(object obj)
        {
            return obj is Color other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = r.GetHashCode();
                hashCode = (hashCode * 397) ^ g.GetHashCode();
                hashCode = (hashCode * 397) ^ b.GetHashCode();
                hashCode = (hashCode * 397) ^ a.GetHashCode();
                return hashCode;
            }
        }
    }
}