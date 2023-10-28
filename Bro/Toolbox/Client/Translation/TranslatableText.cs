#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bro.Client;
using Bro.Toolbox.Client.UI;
using TMPro;

namespace Bro.Toolbox.Client
{
    public class TranslatableText : MonoBehaviour
    {
        [TextArea(3, 10)] [SerializeField] private string _translatingKey = string.Empty;
        [SerializeField] private string[] _replaceValues;

        private EventObserver<LocalizationUpdateEvent> _localizationEventObserver;
        private LanguageType _labelLanguage;
        private LocalizationModule _localizationModule;
        private Window _parentWindow;
        private TextMeshProUGUI _textMeshProElement;
        private Text _textElement;

        public string GetText()
        {
            if (_textMeshProElement != null)
            {
                return _textMeshProElement.text;
            }
            
            if (_textElement != null)
            {
                return _textElement.text;
            }

            return string.Empty;
        }

        public void SetKey(string key)
        {
            TranslatingKey = key;
        }

        public void SetText(string text)
        {
            _translatingKey = string.Empty;

            if (_textMeshProElement != null)
            {
                _textMeshProElement.text = text;
            }
            
            if (_textElement != null)
            {
                _textElement.text = text;
            }
        }


        public string TranslatingKey
        {
            set
            {
                _translatingKey = value;
                Translate();
            }
        }

        public void Setup(string key, List<string> replaceValues)
        {
            _translatingKey = key;
            _replaceValues = replaceValues.ToArray();
            Translate();
        }

        public void Setup(string key, params string[] replaceValues)
        {
            _translatingKey = key;
            _replaceValues = replaceValues;
            Translate();
        }

        public void SetupParentWindow(Window window)
        {
            _parentWindow = window;
        }

        private void FindLocalizationComponent() 
        {
            if (_localizationModule != null)
            {
                return;
            }
            
            _parentWindow = _parentWindow ?? FindParentWindow();
            if (_parentWindow == null)
            {
                Bro.Log.Error("translate text : can't find window in parent object, key = " + _translatingKey);
                gameObject.name += "_error";
                return;
            }

            var context = _parentWindow.Context;
            if (context == null)
            {
                Bro.Log.Error("translate text : can't find context in window object, key = " + _translatingKey);
                return;
            }

            _localizationModule = context.Application.GlobalContext.GetModule<LocalizationModule>();
        } 

        private Window FindParentWindow()
        {
            Transform parent = transform;
            Window window = null;
            do
            {
                parent = parent.parent;
                if (parent == null)
                {
                    break;
                }

                window = parent.GetComponent<Window>();
            } while (window == null);

            return window;
        }

        private void Awake()
        {
            _textMeshProElement = GetComponent<TextMeshProUGUI>();
            if (_textMeshProElement == null)
            {
                _textElement = GetComponent<Text>();
            }
        }

        private void Start()
        {
            Translate();
        }

        private void OnEnable()
        {
            _localizationEventObserver = new EventObserver<LocalizationUpdateEvent>();
            _localizationEventObserver.Subscribe(OnLocalizationEvent);

            if (_localizationModule != null && _labelLanguage != _localizationModule.CurrentLanguage)
            {
                Translate();
            }
        }

        private void OnDisable()
        {
            if (_localizationEventObserver != null)
            {
                _localizationEventObserver.Dispose();
                _localizationEventObserver = null;
            }
        }

        private void OnLocalizationEvent(LocalizationUpdateEvent e)
        {
            Translate();
        }

        private void Translate()
        {
            FindLocalizationComponent();
            if (_localizationModule != null && !string.IsNullOrEmpty(_translatingKey))
            {
                _labelLanguage = _localizationModule.CurrentLanguage;
                
                var text = _replaceValues == null ? _localizationModule.Translate(_translatingKey):
                        _localizationModule.Translate(_translatingKey, _replaceValues);

                if (_textMeshProElement != null)
                {
                    _textMeshProElement.text = text;
                }
                else if (_textElement != null)
                {
                    _textElement.text = text;
                }
            }
        }
    }
}
#endif