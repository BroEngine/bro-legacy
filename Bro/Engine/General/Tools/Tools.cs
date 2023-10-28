using System.Text;

namespace Bro
{
    public static class Tools
    {
        public static string MD5(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; ++i)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static bool? CompareBaseEquality(object a, object b)
        {
            if (ReferenceEquals(null, a) && ReferenceEquals(null, b))
            {
                return true;
            }
            
            if (ReferenceEquals(null, a) || ReferenceEquals(null, b))
            {
                return false;
            }

            return null;
        }

        public static bool? CompareBaseInEquality(object a, object b)
        {
            if (ReferenceEquals(null, a) && ReferenceEquals(null, b))
            {
                return false;
            }
            
            if (ReferenceEquals(null, a) || ReferenceEquals(null, b))
            {
                return true;
            }

            return null;
        }
    }
}