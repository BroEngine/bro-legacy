using System;
using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using UnityEngine.SceneManagement;

namespace Bro.Sketch.Client
{
    public class ContextSceneSwitcher : IContextSwitcher
    {
        private readonly ClientApplication _application;
        private readonly Dictionary<Type, string> _contextScenes;
        public ContextSceneSwitcher(ClientApplication application, Dictionary<Type,string> contextScenes)
        {
            _application = application;
            _contextScenes = contextScenes;
        }

        public void Switch(IClientContext a, IClientContext b, Action onComplete = null)
        {
            if (a != null)
            {
                a?.Unload(() =>
                {
                    _application.GlobalContext.Scheduler.StartCoroutine(LoadSceneAsync(b, () =>
                    {
                        b.Load(_application);
                        onComplete?.Invoke();
                    }));
                });
            }
            else
            {
                _application.GlobalContext.Scheduler.StartCoroutine(LoadSceneAsync(b, () =>
                {
                    b.Load(_application);
                    onComplete?.Invoke();
                }));
            }
        }

        private IEnumerator LoadSceneAsync(IClientContext context, Action callback)
        {
            if (!_contextScenes.ContainsKey(context.GetType()))
            {
                Bro.Log.Error("context scene switcher :: no scene provided for context = " + callback.GetType());
                callback?.Invoke();
            }
            else
            {
                var activeSceneName = SceneManager.GetActiveScene().name;
                var sceneName = _contextScenes[context.GetType()];
                if (activeSceneName == sceneName && activeSceneName != "scene_level") // todo 
                {
                    callback?.Invoke();
                }
                else
                {
                    var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                    while (!asyncLoad.isDone)
                    {
                        yield return null;
                    }
                    callback?.Invoke();   
                }
            }
        }
        
    }
}