#if UNITY_EDITOR
using UnityEditor;

namespace Bro.Toolbox.Client
{
	[InitializeOnLoad]
	public class AndroidKeystore
    {
		static AndroidKeystore ()
        {
            Fill();
        }

		private static void Fill ()
		{
            var androidKeyStoreName =  AndroidKeystoreSettings.Instance.AndroidKeyStoreName;
            var androidKeyStorePassword = AndroidKeystoreSettings.Instance.AndroidKeyStorePassword;
            var androidKeyAlies = AndroidKeystoreSettings.Instance.AndroidKeyAlias;
            var androidKeyAliesPassword = AndroidKeystoreSettings.Instance.AndroidKeyAliasPassword;
          
            PlayerSettings.Android.keystoreName = androidKeyStoreName;
            PlayerSettings.Android.keystorePass = androidKeyStorePassword;
            PlayerSettings.Android.keyaliasName = androidKeyAlies;
            PlayerSettings.Android.keyaliasPass = androidKeyAliesPassword;
		}
	}
} 
#endif