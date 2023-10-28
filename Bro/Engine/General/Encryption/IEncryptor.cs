namespace Bro.Encryption
{
    public interface IEncryptor
    {
        string Key { get; }
        string Encrypt(string input);
        string Decrypt(string input);
        byte[] Encrypt(byte[] input);
        byte[] Decrypt(byte[] input);
    }
}