namespace Bro.Network
{
    public class ErrorCode
    {
        public const byte NoError = 0;
        public const byte UnknownError = 1;
        public const byte NoHandlerForOperationCode = 2;
        public const byte MissingParam = 3;
        public const byte InvalidParam = 4;
        public const byte DatabaseError = 5;
        public const byte ServerError = 6;

        public const byte MaxReservedCode = 16;
    }
}