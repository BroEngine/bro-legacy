using System.Collections.Generic;

namespace Bro.Sketch
{
    public static class GameConfig
    {
        public static class Time
        {
            public const long SyncPeriodTimestamp = 2000L;
        }

        public static class Inventory
        {
            public const long SendSyncPeriodTimestamp = 100L;
            public const long CheckSyncPeriodTimestamp = 1000L;
        }
        
        public static class GameSpeed
        {
            public static List<GameSpeedMargin> GameSpeedMargins = new List<GameSpeedMargin>()
            {
                new GameSpeedMargin()
                {
                    MinFps = 0,
                    MaxFps = 10,
                    MaxFpsSpeed = 3.34f,
                    MinFpsSpeed = 3.34f
                },
                new GameSpeedMargin()
                {
                    MinFps = 10,
                    MaxFps = 20,
                    MaxFpsSpeed = 1.67f,
                    MinFpsSpeed = 3.34f
                },
                new GameSpeedMargin()
                {
                    MinFps = 20,
                    MaxFps = 30,
                    MaxFpsSpeed = 1.11f,
                    MinFpsSpeed = 1.67f
                },
                new GameSpeedMargin()
                {
                    MinFps = 30,
                    MaxFps = 40,
                    MaxFpsSpeed = 1.02f,
                    MinFpsSpeed = 1.11f
                },
                new GameSpeedMargin()
                {
                    MinFps = 40,
                    MaxFps = 50,
                    MaxFpsSpeed = 1.01f,
                    MinFpsSpeed = 1.02f
                },
                new GameSpeedMargin()
                {
                    MinFps = 50,
                    MaxFps = 60,
                    MaxFpsSpeed = 1.00f,
                    MinFpsSpeed = 1.01f
                }
            };

            public static float GameSpeedChangeSpeed = 1.0f;
            public static float DeltaFps = 1.0f;

            public class GameSpeedMargin
            {
                public float MinFps;
                public float MaxFps;
                public float MaxFpsSpeed;
                public float MinFpsSpeed;
            }
        }
    }
}