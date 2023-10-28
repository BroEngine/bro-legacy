namespace Bro.Network
{
    public class NetworkConfig
    {
        /// <summary>
        /// The response timeout in ms.
        /// </summary>
        public static readonly long ResponseTimeout = 3000;

        /// <summary>
        /// The response timeout in ms.
        /// </summary>
        public static readonly long ResponseIncreasedTimeout = 5000;
        
        /// <summary>
        /// The max size of the message in bytes.
        /// </summary>
        public static readonly int MessageMaxSize = 1024 * 256 + 2;
    }
}