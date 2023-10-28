#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using Bro.Client;
using Bro.Sketch.Client;
using Bro.Toolbox.Client.UI.Manager;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Bro.Toolbox.Client.UI
{
    // ReSharper disable once InconsistentNaming
    public class UIModule : IClientContextModule
    {
        private int _counterShow;
        private Transform _canvasTransform;
        private IClientContext _clientContext;
        private readonly WindowStack _windowStack = new WindowStack();
        private readonly Dictionary<Type, Window> _windows = new Dictionary<Type, Window>();
        private readonly Dictionary<Type, Action> _pendingWindowsShow = new Dictionary<Type, Action>();
        private readonly List<Type> _autoUpWindows = new List<Type>();
        
        private Window _curActiveWindow;
        
        public int StackCount => _windowStack.Count;
        public Transform CanvasTransform => _canvasTransform;
        
        public bool LastInStack(Window window) => _windowStack.IndexOf(window) == StackCount - 1;
        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;
        
        void IClientContextModule.Initialize(IClientContext clientContext)
        {
            _clientContext = clientContext;
        }

        private void CreateUiEventSystem()
        {
            if (GameObject.Find("ui_event") == null)
            {
                var objectEvent = new GameObject("ui_event");
                objectEvent.AddComponent<EventSystem>();
                objectEvent.AddComponent<StandaloneInputModule>();
            }
        }

        public GameObject CreateCanvas(string canvasName, int sortingOrder)
        {
            var objectCanvas = new GameObject(canvasName);
            var canvas = objectCanvas.AddComponent<Canvas>();
            var scaler = objectCanvas.AddComponent<CanvasScaler>();
            
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.referenceResolution = new Vector2(1920, 1080);

            return objectCanvas;
        }

        IEnumerator IClientContextModule.Load()
        {
            CreateUiEventSystem();
            var objectCanvasDynamic = CreateCanvas("ui_canvas", 3);
            objectCanvasDynamic.AddComponent<GraphicRaycaster>();
            _canvasTransform = objectCanvasDynamic.transform;
            return null;
        }
        
        IEnumerator IClientContextModule.Unload()
        {
            return null;
        }
        
        /// <summary>
        /// Returns true when window is spawned from prefab 
        /// </summary>
        public bool IsWindowSpawned<T>() where T : Window
        {
            return _windows.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Returns true when widnow is currently showing
        /// </summary>
        public bool IsWindowOpened<T>() where T : Window
        {
            return IsWindowSpawned<T>() && GetWindow<T>().IsShowing();
        }  
        
        public bool IsWindowOpened(Type type) 
        {
            return _windows.ContainsKey(type) && GetWindow(type).IsShowing();
        }

        public T GetWindow<T>() where T : Window
        {
            var type = typeof(T);
            if (_windows.ContainsKey(type))
            {
                return _windows[type] as T;
            }

            Bro.Log.Info("GetWindow Error window not spawned: " + type);
            return null;
        }    
        
        public Window GetWindow(Type type)
        {
            if (_windows.ContainsKey(type))
            {
                return _windows[type];
            }

            Bro.Log.Info("GetWindow Error window not spawned: " + type);
            return null;
        }

        public void Preload<T>(IWindowArgs args = null) where T : Window
        {
            //Bro.Log.Info("Preload: " + typeof(T));

            if (_windows.ContainsKey(typeof(T)))
            {
                return;
            }

            var openingWindow = CacheWindow<T>(_canvasTransform);
            if (openingWindow != null)
            {
                openingWindow.ShowWindow(_clientContext, args);
                openingWindow.HideWindow();
            }
        }

        public void Up<T>(bool auto) where T : Window
        {
            var canvasTransform = _canvasTransform;
            var itemToShow = CacheWindow<T>(canvasTransform);
                 
            if (itemToShow == null)
            {
                Log.Error($"prefab for ui with type {typeof(T).Name} not found");
                return;
            }

            ++ _counterShow;
            itemToShow.gameObject.transform.SetSiblingIndex(_counterShow);

            if (auto)
            {
                _autoUpWindows.Remove(typeof(T));
                _autoUpWindows.Add(typeof(T));
            }
        }

        private void ReUp()
        {
            for (var i = _autoUpWindows.Count - 1; i >= 0; --i)
            {
                if (!IsWindowOpened(_autoUpWindows[i]))
                {
                    _autoUpWindows.RemoveAt(i);
                }
            }

            foreach (var type in _autoUpWindows)
            {
                ++ _counterShow;
                _windows[type].gameObject.transform.SetSiblingIndex(_counterShow);
            }
        }

        public T Show<T>(IWindowArgs args = null, Transform customCanvasTransform = null) where T : Window
        {
            var canvasTransform = _canvasTransform;
            if (customCanvasTransform != null)
            {
                canvasTransform = customCanvasTransform;
            }

            var itemToShow = CacheWindow<T>(canvasTransform);
                 
            if (itemToShow == null)
            {
                Log.Error($"prefab for ui with type {typeof(T).Name} not found");
                return null;
            }

            ++ _counterShow;
            itemToShow.gameObject.transform.SetSiblingIndex(_counterShow);
            
            _pendingWindowsShow.Remove(typeof(T));

            // Log.Info($"ui component :: show window {typeof(T).Name}");

            if (!string.IsNullOrEmpty(itemToShow.ShowSoundId))
            {
                new PlayAudioEvent(itemToShow.ShowSoundId, SoundType.Sound).Launch();
            }
            
            switch (itemToShow.ItemType)
            {
                case Window.WindowItemType.Window:
                    ShowWindow(itemToShow, args);
                    break;
                case Window.WindowItemType.Popup:
                    ShowPopup(itemToShow, args);
                    break;
                case Window.WindowItemType.Widget:
                    itemToShow.ShowWindow(_clientContext, args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ReUp();

            return itemToShow;
        }

        private void ShowWindow(Window window, IWindowArgs args)
        {
            var prevWindow = _windowStack.Peek();

            if (_windowStack.Push(new WindowStackItem(window, args)))
            {
                _curActiveWindow?.HideWindow();

                prevWindow?.Window.HideWindow();

                _curActiveWindow = window;

                _curActiveWindow.ShowWindow(_clientContext, args);

                new WindowEvent(WindowEvent.ActionType.Showed, window).Launch();
            }
            else
            {
                window.DirectlyHide();
            }
        }

        private void ShowPopup(Window popup, IWindowArgs args = null)
        {
            var prevWindow = _windowStack.Peek();

            if (_windowStack.Push(new WindowStackItem(popup, args)))
            {
                if (prevWindow != null && prevWindow.Window.ItemType == Window.WindowItemType.Popup && prevWindow.Window.WindowPriority > popup.WindowPriority)
                {
                    prevWindow.Window.HideWindow();
                }

                popup.gameObject.transform.SetSiblingIndex(short.MaxValue);
                popup.ShowWindow(_clientContext, args);
                new WindowEvent(WindowEvent.ActionType.Showed, popup).Launch();
            }
            else
            {
                popup.DirectlyHide();
            }
        }

        public void Hide()
        {
            if (_windowStack.Count == 0)
            {
                return;
            }

            var windowToClose = _windowStack.Pop().Window;
            windowToClose.HideWindow();

            if (!string.IsNullOrEmpty(windowToClose.HideSoundId))
            {
                new PlayAudioEvent(windowToClose.HideSoundId, SoundType.Sound).Launch();
            }

            if (_curActiveWindow == windowToClose)
            {
                _curActiveWindow = null;
            }

            // Log.Info($"ui component :: hide {windowToClose.GetType().Name}");

            if (_windowStack.Count > 0)
            {
                var windowToOpen = _windowStack.Peek();

                windowToOpen.Window.ShowWindow(_clientContext, windowToOpen.Args, true);

                if (windowToOpen.Window.ItemType != Window.WindowItemType.Popup)
                {
                    _curActiveWindow = windowToOpen.Window;
                }
                else if (_curActiveWindow == null)
                {
                    var lastWindowInStack = _windowStack.GetLastWindowOfType(Window.WindowItemType.Window);

                    if (lastWindowInStack != null)
                    {
                        _curActiveWindow = lastWindowInStack.Window;
                        _curActiveWindow.ShowWindow(_clientContext, lastWindowInStack.Args, true);
                    }
                }
            }
            
            new WindowEvent(WindowEvent.ActionType.Hided, windowToClose).Launch();
        }

        public void HideAllWindow()
        {
            ///
            for (int i = 0; i < _windows.Count; ++i)
            {
                var window = _windows.Values.ElementAt(i);
                if (window.IsShowing() && ! window.IsPermanent)
                {
                    window.HideWindow();
                }
            }

            ResetData();
        }

        public void DestroyAllWindows()
        {
            for (int i = 0; i < _windows.Count; ++i)
            {
                var window = _windows.Values.ElementAt(i);
                if (window != null && window.gameObject != null)
                {
                    Object.Destroy(window.gameObject);
                }
            }

            _windows.Clear();
            ResetData();
        }

        void ResetData()
        {
            _windowStack.Clear();
            _curActiveWindow = null;
        }

        public void DestroyWindow<T>() where T : Window
        {
            for (int i = 0; i < _windows.Count; ++i)
            {
                var window = _windows.Values.ElementAt(i);
                if (window != null && window.gameObject != null && typeof(T) == window.GetType())
                {
                    Object.Destroy(window.gameObject);
                    _windows.Remove(typeof(T));
                    break;
                }
            }
        }

        public void DirectlyHide<T>() where T : Window
        {
            var type = typeof(T);
            if (!_windows.ContainsKey(type))
            {
                Bro.Log.Info("(not an error) Cant close window, case it not spawned: " + type.ToString());
            }
            else
            {
                var closingWindow = _windows[type] as T;

                _windowStack.Remove(closingWindow);
                closingWindow.HideWindow();

                new WindowEvent(WindowEvent.ActionType.Hided, closingWindow).Launch();
            }
        }

        public void ChangeArgs(IWindowArgs args)
        {
            var window = _windowStack.Peek();
            if (window == null)
            {
                return;
            }

            window.Args = args;
        }

        /// <summary>
        /// Add window type to show pending
        /// </summary>
        public void AddWindowPendingShow<T>(IWindowArgs args = null) where T : Window
        {
            if (_windowStack.Peek().Window is T) return;

            Log.Info($"ui component :: add pending window {typeof(T).Name}");

            _pendingWindowsShow[typeof(T)] = () => Show<T>(args);
        }

        public void ShowPendingWindows()
        {
            if (_pendingWindowsShow.Count == 0) return;

            var windowsToShow = new Queue<Type>(_pendingWindowsShow.Keys);

            while (windowsToShow.Count > 0)
            {
                _pendingWindowsShow[windowsToShow.Dequeue()]?.Invoke();
            }

            _pendingWindowsShow.Clear();
        }

        /// <summary>
        /// Instantiate window from resources
        /// </summary>
        /// <returns></returns>
        private T InstantiateWindow<T>(Window baseWindow, Transform canvasTransform) where T : Window
        {
            var newWinInstance = Object.Instantiate(baseWindow, canvasTransform);
            var newWinComponent = newWinInstance.GetComponent<T>();
            return newWinComponent;
        }

        /// <summary>
        /// Instantiate window to cache or find cached
        /// </summary>
        /// <returns>New instanced window of cached</returns>
        private T CacheWindow<T>(Transform canvasTransform) where T : Window
        {
            var type = typeof(T);
            T result = null;

            if (!_windows.ContainsKey(type))
            {
                var windowAttribute = GetAttribute<T>();
                var baseWindow = UIAssetRegistry.Instance.GetWindow<T>();
                if (baseWindow != null)
                {
                    result = InstantiateWindow<T>(baseWindow, canvasTransform);
                    result.ItemType = windowAttribute.ItemType;
                    _windows.Add(type, result);
                }
            }
            else
            {
                result = _windows[type] as T;
            }

            return result;
        }

        /// <summary>
        /// Getting window prefab name from attribute
        /// </summary>
        private WindowAttribute GetAttribute<T>() where T : Window
        {
            if (Attribute.IsDefined(typeof(T), typeof(WindowAttribute)))
            {
                return Attribute.GetCustomAttribute(typeof(T), typeof(WindowAttribute)) as WindowAttribute;
            }

            Log.Error($"ui component :: window {typeof(T)} has no window attribute");
            return null;
        }
    }
}
#endif