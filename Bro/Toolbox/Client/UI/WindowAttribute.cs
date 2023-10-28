#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )

using System;

namespace Bro.Toolbox.Client.UI
{
	public class WindowAttribute : Attribute
	{
		public Window.WindowItemType ItemType { get; }
		
		public WindowAttribute (Window.WindowItemType itemType = Window.WindowItemType.Window)
		{
			ItemType = itemType;
        }
	}
}
#endif