using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Bro.Encryption
{
    public class AES : IEncryptor
    {
        private readonly object _lock = new object();
        private readonly byte[] _key;

        private static readonly byte[] iv = new byte[16] {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};

        public AES(string key) // 32 chars, ex = t5994471abb01112afcc18159f6cc74b4
        {
            _key = Encoding.ASCII.GetBytes(key);
            Key = key;
        }

        public string Key { get; private set; }

        public string Encrypt(string input)
        {
            return EncryptString(input, _key, iv);
        }

        public string Decrypt(string input)
        {
            return DecryptString(input, _key, iv);
        }

        public byte[] Encrypt(byte[] input)
        {
            return EncryptBytes(input, _key, iv);
        }

        public byte[] Decrypt(byte[] input)
        {
            return DecryptBytes(input, _key, iv);
        }

        private static byte[] EncryptBytes(byte[] plainBytes, byte[] key, byte[] iv)
        {
            Aes encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;

            encryptor.Key = key;
            encryptor.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            ICryptoTransform aesEncryptor = encryptor.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return cipherBytes;
        }

        private static string EncryptString(string plainText, byte[] key, byte[] iv)
        {
            var encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;

            encryptor.Key = key;
            encryptor.IV = iv;
            var memoryStream = new MemoryStream();
            var aesEncryptor = encryptor.CreateEncryptor();
            var cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();
            var cipherBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
            return cipherText;
        }

        private string DecryptString(string cipherText, byte[] key, byte[] iv)
        {
            var encryptor = Aes.Create();
            encryptor.Mode = CipherMode.CBC;

            encryptor.Key = key;
            encryptor.IV = iv;
            var memoryStream = new MemoryStream();
            var aesDecryptor = encryptor.CreateDecryptor();
            var cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);
            string plainText;
            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);
                cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                cryptoStream.FlushFinalBlock();
                var plainBytes = memoryStream.ToArray();
                plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
            }
            finally
            {
                memoryStream.Close();
                cryptoStream.Close();
            }

            return plainText;
        }

        private byte[] DecryptBytes(byte[] cipherBytes, byte[] key, byte[] iv)
        {
            lock (_lock)
            {
                byte[] plainBytes;
                var encryptor = Aes.Create();
                encryptor.Mode = CipherMode.CBC;
                encryptor.Key = key;
                encryptor.IV = iv;
                var memoryStream = new MemoryStream();
                var aesDecryptor = encryptor.CreateDecryptor();
                var cryptoStream = new CryptoStream(memoryStream, aesDecryptor, CryptoStreamMode.Write);

                try
                {
                    cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    plainBytes = memoryStream.ToArray();
                }
                catch (Exception e)
                {
                    Bro.Log.Info("error :: aes cipher bytes length = " + cipherBytes.Length);
                    Bro.Log.Error(e);
                    return new byte[0];
                }
                finally
                {
                    memoryStream.Close();
                    cryptoStream.Close();
                }

                return plainBytes;
            }
        }
    }
}