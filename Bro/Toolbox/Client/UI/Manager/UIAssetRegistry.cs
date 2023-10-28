using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT

namespace Bro.Toolbox.Client.UI
{
    [Settings("ui_asset_registry", "Resources/UI")]
    public class UIAssetRegistry : SystemSettings<UIAssetRegistry>
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("Settings/Registries/UI Assets")]
        public static void Edit()
        {
            Instance = null;
            UnityEditor.Selection.activeObject = Instance;
            DirtyEditor();
        }
        #endif
        
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private List<Window> _windows = new List<Window>();
        
        // ReSharper disable once CollectionNeverUpdated.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private SerializableDictionary<string,MonoBehaviour> _elements = new SerializableDictionary<string,MonoBehaviour>();

        public Window GetWindow<T>()
        {
            foreach (var window in _windows)
            {
                if (window.GetType() == typeof(T))
                {
                    return window;
                }
            }
            Bro.Log.Error("no window of type = " + typeof(T) + " in ui asset registry, add it yo the ui_asset_registry file");
            return null;
        }
        
        public MonoBehaviour GetElement<T>() where T : MonoBehaviour
        {
            foreach (var element in _elements)
            {
                if (element.Value.GetType() == typeof(T))
                {
                    return element.Value;
                }
            }
            Bro.Log.Error("no element of type = " + typeof(T) + " in ui asset registry, add it yo the ui_asset_registry file");
            return null;
        } 
        
        public MonoBehaviour GetElement(string key)
        {
            foreach (var element in _elements)
            {
                if (element.Key == key)
                {
                    return element.Value;
                }
            }
            Bro.Log.Error("no element with key = " + key + " in ui asset registry, add it yo the ui_asset_registry file");
            return null;
        }
    }
}

#endif