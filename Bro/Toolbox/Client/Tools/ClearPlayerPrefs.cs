#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Bro.Toolbox.Client
{
	public class ClearPlayerPrefs : EditorWindow
	{
		[MenuItem ("Tools/Clear/Player Prefs")]
		static void ClearPrefs ()
		{
			PlayerPrefs.DeleteAll ();
			PlayerPrefs.Save ();
		}
	}
}
#endif