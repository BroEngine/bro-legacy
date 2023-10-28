using System.Collections;
using System.Collections.Generic;
using Bro.Client;

namespace Bro.Toolbox.Client
{
    public class LocalizationModule : IClientContextModule
    {
        private Dictionary<string, string> _config;
        private IClientContext _context;
        
        private bool IsInitialized => _config != null && _config.Count > 0;

        public IList<CustomHandlerDispatcher.HandlerInfo> Handlers { get; }
        public LanguageType CurrentLanguage;
        
        public void Initialize(IClientContext context)
        {
            _context = context;
        }

        public IEnumerator Load()
        {
            return null;
        }

        public IEnumerator Unload()
        {
            return null;
        }
        
        public string Translate(string key)
        {
            if (!IsInitialized)
            {
                new LocalizationChangeEvent(CurrentLanguage).Launch();
            }
            
            if (_config.TryGetValue(key, out var value))
            {
                return value;
            }

            Log.Error($"localization component :: no localization for key {key}");
            return key;

        }

        public string Translate(string key, string[] arguments)
        {
            if (!IsInitialized)
            {
                new LocalizationChangeEvent(CurrentLanguage).Launch();
            }
            
            if (_config.TryGetValue(key, out var value))
            {

                if (arguments != null)
                {
                    for (var i = 0; i < arguments.Length; ++ i)
                    {
                        value = value.Replace("{" + i + "}", arguments[i]);
                    }
                }

                return value;
            }
            
            Log.Error($"localization component replacement :: no localization for key {key}");
            return key;
        }
        

        public string Translate(string key, Dictionary<string,string> replacement)
        {
            if (!IsInitialized)
            {
                new LocalizationChangeEvent(CurrentLanguage).Launch();
            }
            
            if (_config.TryGetValue(key, out var value))
            {

                if (replacement != null)
                {
                    foreach (var pair in replacement)
                    {
                        value = value.Replace(pair.Key, pair.Value);
                    }
                }

                return value;
            }
            
            Log.Error($"localization component replacement :: no localization for key {key}");
            return key;
        }

        public void ChangeLanguage(LanguageType newLang)
        {
            if (CurrentLanguage == newLang)
            {
                return;
            }

            CurrentLanguage = newLang;
            new LocalizationChangeEvent(CurrentLanguage).Launch();
        }
        
        public void SetupConfigHandler(Dictionary<string, string> config)
        {
            _config = config;
        }
    }
}