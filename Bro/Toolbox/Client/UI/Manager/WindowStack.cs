#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT )

using System.Collections.Generic;
using System.Linq;
using Bro.Json.Utilities;

namespace Bro.Toolbox.Client.UI.Manager
{
    public class WindowStack
    {
        List<WindowStackItem> _openedWindows = new List<WindowStackItem>();

        public bool Push(WindowStackItem stackItem)
        {
            var openNew = true;

            var prevWindow = _openedWindows.LastOrDefault();
            var curwindow = stackItem.Window;
            
            _openedWindows.Add(stackItem);
            
            if (prevWindow != null)
            {
                openNew = curwindow.WindowPriority <= prevWindow.Window.WindowPriority;
                _openedWindows.Sort((item1, item2) => item2.Window.WindowPriority.CompareTo(item1.Window.WindowPriority));
            }
            
            return openNew;
        }

        public WindowStackItem Pop()
        {
            var lastIndex = _openedWindows.Count - 1;
            var pop = _openedWindows[lastIndex];
            _openedWindows.RemoveAt(lastIndex);
            
            return pop;
        }
        
        public WindowStackItem Peek()
        {
            var lastIndex = _openedWindows.Count - 1;
            return lastIndex < 0 ? null : _openedWindows[lastIndex];
        }

        public void Remove(Window window)
        {
            var removeIndex = _openedWindows.IndexOf(item => item.Window == window);
            if (removeIndex >= 0)
            {
                _openedWindows.RemoveAt(removeIndex);
            }
        }
        
        public int Count
        {
            get { return _openedWindows.Count; }
        }

        public void Clear()
        {
            _openedWindows.Clear();
        }

        public WindowStackItem GetLastWindowOfType(Window.WindowItemType type)
        {
            for (int i = _openedWindows.Count - 1; i >= 0; i--)
            {
                if (_openedWindows[i].Window.ItemType == type)
                {
                    return _openedWindows[i];
                }
            }

            return null;
        }

        public int IndexOf(Window window) =>
            _openedWindows.IndexOf(item => item.Window == window);
    }
}

#endif