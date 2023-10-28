namespace Bro.Sketch.Network
{
    public static class Request
    {
        public static class OperationCode
        {
            #region Common[0-9]
            public const byte Authorization = 0;
            public const byte EcsWorld = 5;
            #endregion

            #region Special[10-19]

            public const byte TimeSync = 10;
            public const byte InventorySync = 11;

            #endregion

            #region LobbyServer[20-39]

            public static class Lobby
            {
                // public const byte JoinRoom = 20;
                // public const byte DirectJoinRoom = 21;
                // public const byte JoinGroup = 24;
                // public const byte LeaveGroup = 25;
                // public const byte GroupRoomTransition = 26;
            }

            #endregion

            #region BattleServer[40-59]

          
            #endregion

            #region Infrastructure[60-69]

            public static class Infrastructure 
            {
                public const byte ProfileLoad = 60;                /* private */          
                public const byte ProfileSave = 61;                /* private */

                public const byte RegistrySet = 62;                /* private */
                public const byte RegistryGet = 63;                /* private */
                
                public const byte DocumentTemporarySet = 64;        /* private */
                public const byte DocumentTemporaryGet = 65;        /* private */
                public const byte DocumentCumulativeGet = 67;       /* private */
                public const byte DocumentCumulativeSet = 68;       /* private */
                public const byte DocumentCumulativeDelete = 69;    /* private */    
            }

            #endregion            
            
            #region Social[70-75]

            public static class Social
            {
                public const byte FriendsGet = 70;             /* public and private */
                public const byte FriendsSet = 71;             /* public and private */
                public const byte FriendsDelete = 72;          /* public and private */
                public const byte FriendsIdentity = 73;        /* public and private */
                public const byte UserStatusGet = 74;          /* private */
                public const byte UserStatusSet = 75;          /* private */

                public const byte GroupGet = 76;               /* private */
                public const byte GroupSet = 77;               /* private */
                public const byte GroupLeave = 78;             /* private */
                public const byte GroupDebug = 79;             /* public */

                public const byte Invite = 80;                 /* private */
                
                public const byte ConversationMessage = 81;            /* public and private */
                public const byte ConversationHistory = 82;                    /* public and private */
                public const byte ConversationRegistration = 83;       /* private */
                public const byte ConversationAction = 84;             /* public and private */
                public const byte ConversationGet = 85;                /* public and private */

                public const byte MatchmakingPull = 86;
                public const byte MatchmakingLeave = 87;
                public const byte MatchmakingPoll = 88;
            }

            #endregion
            
            #region ReserverForUser[99-255]
            
            #endregion
        }
    }

    public static class Event
    {
        public static class OperationCode
        {
            #region Common[0-9]
            public const byte EcsWorld = 5;

            #endregion

            #region Special[10-15]

            public const byte KickOut = 10;
            public const byte TimeSync = 11;
            public const byte InventorySync = 12;

            #endregion
            
            #region Social[16-19]

            public static class Social
            {
                public const byte UserStatus = 16;             /* public and private */
                public const byte FriendsStatus = 17;          /* public and private */
                public const byte GroupStatus = 18;            /* private */
                public const byte GroupDebug = 19;             /* public */
                public const byte Invite = 20;                 /* private */

                public const byte Messages = 21;               /* public and private */
                public const byte Conversations = 22;          /* public and private */
                public const byte MatchmakingLeave = 23;       /* */
            }

            #endregion

            #region LobbyServer[20-39]
            #endregion

            #region BattleServer[40-59]
            #endregion

            #region ReserverForUser[99-255]
            
            #endregion
        }
    }
}