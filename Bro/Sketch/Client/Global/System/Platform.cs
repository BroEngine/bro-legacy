namespace Bro.Client
{
    public static class Platform
    {
            

#if UNITY_ANDROID
        public static PlatformType Type = PlatformType.Android; 
        public const bool UNITY_ANDROID = true;
#else
        public const bool UNITY_ANDROID = false;
#endif
       
            
#if UNITY_IOS
        public static PlatformType Type = PlatformType.iOS; 
        public const bool UNITY_IOS = true;
#else
        public const bool UNITY_IOS = false;
#endif            
            

#if UNITY_STANDALONE
        public static PlatformType Type = PlatformType.Standalone;
        public const bool UNITY_STANDALONE = true;
#else
        public static bool UNITY_STANDALONE = false; 
#endif
     
            
#if UNITY_WEBGL
        public static PlatformType Type = PlatformType.WebGL
        public const bool UNITY_WEBGL = true;
#else
        public const bool UNITY_WEBGL = false;
#endif
            
            
#if UNITY_EDITOR
        public const bool UNITY_EDITOR = true;
#else
        public const bool UNITY_EDITOR = false; 
#endif
            
#if ! UNITY_ANDROID && ! UNITY_IOS && !UNITY_STANDALONE && !UNITY_WEBGL
        public static PlatformType Type = PlatformType.Android;
#endif

    }
}