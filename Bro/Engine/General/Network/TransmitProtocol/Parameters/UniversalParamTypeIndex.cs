namespace Bro.Network.TransmitProtocol
{
    public static class UniversalParamTypeIndex
    {
        // [00-31]
        public const byte UnknownObject = 0;
        public const byte Bool = 1;
        public const byte Byte = 2;
        public const byte ByteArray = 3;
        public const byte Short = 4;
        public const byte ShortArray = 5;
        public const byte Int = 6;
        public const byte IntArray = 7;
        public const byte Float = 8;
        public const byte FloatArray = 9;
        public const byte Double = 10;
        public const byte Long = 11;

        public const byte String = 12;
        public const byte StringArray = 13;
        public const byte DataTable = 14;
        public const byte LanguageType = 15;
        public const byte StringList = 16;
    }
}