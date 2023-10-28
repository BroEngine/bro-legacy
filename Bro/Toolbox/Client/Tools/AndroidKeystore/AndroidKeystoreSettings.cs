#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Bro.Toolbox.Client
{

	[InitializeOnLoad]
	[SettingsAttribute("AndroidKeystoreSettings", "Resources/Settings")]
	public class AndroidKeystoreSettings : SystemSettings<AndroidKeystoreSettings>
	{
	    [SerializeField] private string _androidKeyStoreName;
		[SerializeField] private string _androidKeyStorePassword;
		[SerializeField] private string _androidKeyAlias;
		[SerializeField] private string _androidKeyAliasPassword;

	    public string AndroidKeyStoreName => _androidKeyStoreName;
	    public string AndroidKeyStorePassword => _androidKeyStorePassword;
	    public string AndroidKeyAlias => _androidKeyAlias;
	    public string AndroidKeyAliasPassword => _androidKeyAliasPassword;
	    
		[MenuItem("Settings/Android Keys Settings")]
		public static void Edit()
		{
			Instance = null;
			Selection.activeObject = Instance;
			DirtyEditor();
		}

	}
}

#endif