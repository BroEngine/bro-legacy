
using System;
using System.Text;

namespace Bro.Encryption
{
    public class XOR : IEncryptor
    {
        private string _secretKey;

        public XOR(string key)
        {
            _secretKey = key;
            Key = key;
        }

        public string Key { get; private set; }

        private string MakeXOR(string str)
        {
            string outText = string.Empty;
            for (int i = 0; i < str.Length;)
            {
                for (int j = 0; (j < _secretKey.Length && i < str.Length); j++, i++)
                {
                    outText += (char) (str[i] ^ _secretKey[j]);
                }
            }

            return outText;
        }

        private string Base64Decode(string str)
        {
            byte[] data = Convert.FromBase64String(str);
            return Encoding.UTF8.GetString(data);
        }

        private string Base64Encode(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(data);
        }

        public string Decrypt(string str)
        {
            return MakeXOR(Base64Decode(str));
        }

        public byte[] Encrypt(byte[] input)
        {
            throw new NotImplementedException();
        }

        public byte[] Decrypt(byte[] input)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string str)
        {
            return Base64Encode(MakeXOR(Encoding.UTF8.GetString(Encoding.Default.GetBytes(str))));
        }
    }
}