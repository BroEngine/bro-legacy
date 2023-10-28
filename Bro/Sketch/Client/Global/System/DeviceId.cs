// ReSharper disable MemberCanBePrivate.Global



namespace Bro.Client
{
    public static class DeviceId
    {
        const string GlyphRange = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789";
        public static string GenerateUid()
        {
            var uid = string.Empty;
            for (var i = 0; i < 32; ++i)
            {
                uid += GlyphRange[Bro.Random.Instance.Range(0, GlyphRange.Length)];
            }

            return uid;
        }
        
#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )
      
        const string StorageKey = "_uniq_device_id_";

        private static string _deviceRnd;

        public static string Value
        {
            get
            {
                var deviceId = UnityEngine.SystemInfo.deviceUniqueIdentifier;
                
                if (IsInvalidUid(deviceId))
                {
                    if (!UnityEngine.PlayerPrefs.HasKey(StorageKey))
                    {
                        deviceId = GenerateUid();
                        UnityEngine.PlayerPrefs.SetString(StorageKey, deviceId);
                        UnityEngine.PlayerPrefs.Save();
                    }
                    else
                    {
                        deviceId = UnityEngine.PlayerPrefs.GetString(StorageKey);
                    }
                }

                return deviceId;
            }
        }

    

        private static bool IsInvalidUid(string uid)
        {
            return uid == "00000000000000000000000000000000" ||
                   uid == "cd9e459ea708a948d5c2f5a6ca8838cf" ||
                   uid == "21371d265b5711b289344b479f583909" ||
                   uid == "a739e25e1b02fac2c9d8f5d10fbc8856" ||
                   uid == "5284047f4ffb4e04824a2fd1d1f0cd62" ||
                   string.IsNullOrEmpty(uid);
        }
        
#elif CONSOLE_CLIENT
        public static string Value { get; set; }

#endif
    }
}
