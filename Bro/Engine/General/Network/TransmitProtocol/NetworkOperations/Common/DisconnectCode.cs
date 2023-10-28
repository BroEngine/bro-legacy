namespace Bro.Network
{
    public class DisconnectCode
    {
        public const byte Undefined = 0;

        // 1 - 10 - native level
        public const byte ConnectionFailed = 1;
        public const byte Timeout = 2;
        public const byte HostUnreachable = 3;
        public const byte NetworkUnreachable = 4;
        public const byte RemoteConnectionClose = 5;
        public const byte DisconnectPeerCalled = 6;
        public const byte ConnectionRejected = 7;
        public const byte InvalidProtocol = 8;
        public const byte InvalidHost = 9;
        
        // 10 - 20 - system level
        public const byte System = 10;
        public const byte SystemTerminal = 11;
        public const byte SystemTimeout = 12;
        public const byte SystemNoServers = 13;
        public const byte SystemActorIsNull = 14;

        // 20 - 30 - low level context
        public const byte ContextTerminate = 21;
        
        
        // 30 -40 client reason
        
        public const byte ClientForceClose = 30;
        public const byte ClientLeaveServer = 32;
        public const byte ClientLeaveAfterPing = 33;
        
        // 100+ gameplay level

        public const byte BattleFinished = 101;
        public const byte InactiveTimeout = 102;
        public const byte SleepTimeout = 103;
        public const byte DuplicateUser = 104;
    }
}