using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Settings("SpriteAtlasValidatorSettings", "Resources/Settings")]
    public class SpriteAtlasValidatorSettings : SystemSettings<SpriteAtlasValidatorSettings>
    {
#if UNITY_EDITOR
        [SerializeField] private List<string> _atlasExceptionSprites;


        public bool IsException(string spriteName)
        {

            if (_atlasExceptionSprites.Contains(spriteName))
            {
                return true;
            }

            return false;

            
        }
        
        [MenuItem("Settings/Sprite Atlas Validator Settings")]
        public static void Edit()
        {
            Instance = null;
            Selection.activeObject = Instance;
            DirtyEditor();
        }
#endif
        
    }
}