using System;
using System.Collections.Generic;

namespace Bro
{
    public static class TypeExtension
    {
        public static bool IsDigitsOnly(this string str)
        {
            foreach (var c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }

        public static List<Type> GetHierarchy(this Type type, bool exceptObjectType = true)
        {
            var result = new List<Type>();
            while (type != null)
            {
                if (type == typeof(Object))
                {
                    if (!exceptObjectType)
                    {
                        result.Add(type);
                    }
                }
                else
                {
                    result.Add(type);
                }

                type = type.BaseType;
            }

            return result;
        }

        public static string ToRomanString(this int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException(nameof(number),"insert value between 1 and 3999");
            if (number < 1) return string.Empty;            
            if (number >= 1000) return "M" + ToRomanString(number - 1000);
            if (number >= 900) return "CM" + ToRomanString(number - 900); 
            if (number >= 500) return "D" + ToRomanString(number - 500);
            if (number >= 400) return "CD" + ToRomanString(number - 400);
            if (number >= 100) return "C" + ToRomanString(number - 100);            
            if (number >= 90) return "XC" + ToRomanString(number - 90);
            if (number >= 50) return "L" + ToRomanString(number - 50);
            if (number >= 40) return "XL" + ToRomanString(number - 40);
            if (number >= 10) return "X" + ToRomanString(number - 10);
            if (number >= 9) return "IX" + ToRomanString(number - 9);
            if (number >= 5) return "V" + ToRomanString(number - 5);
            if (number >= 4) return "IV" + ToRomanString(number - 4);
            if (number >= 1) return "I" + ToRomanString(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        public static string ToPercentsString(this float val) => val == 0 ? "0" : $"{val * 100}%";
    }
}