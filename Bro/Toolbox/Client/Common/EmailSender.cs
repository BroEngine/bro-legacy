#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class EmailSender : StaticSingleton<EmailSender>
    {
        private static AndroidJavaClass _androidNativeClass;

        public EmailSender()
        {
#if UNITY_ANDROID && ! UNITY_EDITOR
            InitAndroidNative();
#endif
        }

#if UNITY_IOS
        [DllImport ( "__Internal" )]
        private static extern void _NATIVE_Email ( string email, string title, string msg, string logPath );
#endif

        private static void InitAndroidNative()
        {
#if UNITY_ANDROID && ! UNITY_EDITOR
            AndroidJNI.AttachCurrentThread();
            _androidNativeClass = new AndroidJavaClass( "com.bro.EmailInterface" );
#endif
        }

        private static AndroidJavaClass AndroidNativeClass
        {
            get
            {
                if (_androidNativeClass == null)
                {
                    InitAndroidNative();
                }
                return _androidNativeClass;
            }
        }

        public void Send(string email, string title, string text, string path)
        {
            Bro.Log.Info("Send email = " + email + "; title = " + title + "; text = " + text + "; path = " + path);

#if UNITY_ANDROID && !UNITY_EDITOR
            {
			    AndroidNativeClass.CallStatic ( "_NATIVE_Email", email, title, text, path );
            }
#elif UNITY_IOS && !UNITY_EDITOR
            {
			    _NATIVE_Email( email, title, text, path ); 
            }
#endif
        }
    }
}