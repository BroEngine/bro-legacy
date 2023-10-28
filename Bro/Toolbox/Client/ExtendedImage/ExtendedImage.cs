using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bro.Toolbox.Client
{
#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII)

    public class ExtendedImage : UnityEngine.UI.Image
    {
        protected override void Start()
        {
            base.Start();

            // возможно временно, возможно нет. Пусть пока так будет.
            useSpriteMesh = true;

#if UNITY_EDITOR
            if (sprite == null || SpriteAtlasValidatorSettings.Instance.IsException(sprite.name))
            {
                return;
            }

            var atlases = new List<string>();
            foreach (var atlas in SpriteAtlasValidation.Atlases)
            {
                if (atlas.GetSprite(sprite.name) != null)
                {
                    atlases.Add(atlas.name);
                }
            }

            if (atlases.Count <= 0)
            {
                Debug.LogError($"validator :: sprite \"{sprite.name}\" don't contains in any atlas, object = " + gameObject.name);
                return;
            }
            
            var sceneName = SceneManager.GetActiveScene().name;
            var atlasName = sceneName.Replace("scene_", "");

            foreach (var atlas in atlases)
            {
                if (!string.Equals(atlas, SpriteAtlasValidation.GlobalAtlasName, StringComparison.CurrentCultureIgnoreCase) &&
                    !string.Equals(atlas, atlasName, StringComparison.CurrentCultureIgnoreCase))
                {
                    //Debug.LogError($"validator :: sprite \"{sprite.name}\" in texture \"{atlas}\" don't match current context");
                    return;
                }
            }
#endif
        }
    }
#endif
}