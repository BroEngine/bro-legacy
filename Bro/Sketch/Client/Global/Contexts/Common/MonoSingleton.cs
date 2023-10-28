using UnityEngine;

namespace Bro.Sketch.Client
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }
                }

                return _instance;
            }

        }

        void OnDestroy()
        {
            _instance = null;
        }
    }

}