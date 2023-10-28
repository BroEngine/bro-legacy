#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
#define CLIENT
#elif BRO_SERVER
#define SERVER
#else
#define DUMMY
#endif

namespace Bro.Sketch
{
    public static class AssemblyInfo
    {
#if CLIENT

        private static int _version;
        
        public static void Initialize()
        {
            var version = UnityEngine.Application.version;
            var data = version.Split('.');
            _version = data.Length != 0 ? int.Parse( data[data.Length - 1] ) : 0;
        }

        public static int Version
        {
            get { return _version; }
        }
        
        public static string ApplicationType
        {
            get { return "client"; }
        }
#endif
    
#if SERVER

        private static string _applicationType = string.Empty;
        
        public static void Initialize(Server.Options options)
        {
            _applicationType = options.ServerType;
        }

        public static int Version
        {
            get 
            {
                var versionText = System.IO.File.ReadAllText("version.txt").Trim();
                var version = int.Parse(versionText);
                return version;
            }
        }
        
        public static string ApplicationType
        {
            get { return _applicationType; }
        }
#endif
        
#if DUMMY
        public static int Version { get; set; }
        
        public static string ApplicationType
        {
            get { return string.Empty; }
        }
#endif
    }
}