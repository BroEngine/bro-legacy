#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )

using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
	public class TabletManager 
	{
		private static bool _inited = false;
		private static bool _isTablet = false;

		public static bool IsTablet ()
		{
			if ( _inited ) {
				return _isTablet;
			}

			if ( DeviceDiagonalSizeInInches () > 6.5f ) {
				_isTablet = true;
			}

			_inited = true;
			return _isTablet;
		}

		public static float DeviceDiagonalSizeInInches ()
		{
			float screenWidth = Screen.width / Screen.dpi;
			float screenHeight = Screen.height / Screen.dpi;
			float diagonalInches = Mathf.Sqrt (Mathf.Pow (screenWidth, 2) + Mathf.Pow (screenHeight, 2));
			return diagonalInches;
		}
	}
}

#endif