using System;
using System.Collections;
using System.Collections.Generic;
using Bro.Client;
using Bro.Toolbox.Client.UI;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class CheatModule : IClientContextModule
    {
        private IDisposable _onUpdateHandler;
        private IClientContext _context;
        private CheatWindow _cheatWindow;
        
        IList<CustomHandlerDispatcher.HandlerInfo> IClientContextModule.Handlers => null;
        
        private readonly ICheatSettings _settings;
        private readonly Dictionary<Enum, CheatPopup> _popups = new Dictionary<Enum, CheatPopup>();

        public IClientContext Context => _context;
        public ICheatSettings Settings => _settings;
        
        public CheatModule(ICheatSettings settings)
        {
            _settings = settings;
        }

        public void Initialize(IClientContext context)
        {
            _context = context;
            Init();   
        }

        public IEnumerator Load()
        {
            _onUpdateHandler = _context.Scheduler.ScheduleUpdate(OnUpdate);
            return null;
        }

        public IEnumerator Unload()
        {
            _onUpdateHandler.Dispose();
            _onUpdateHandler = null;
            
            UnityEngine.Object.Destroy(_cheatWindow);
            _cheatWindow = null;
            return null;
        }

        private void OnUpdate(float delta)
        {
            var isEditor = Input.GetKeyUp(KeyCode.F1);
            var isDevice = Input.touchCount > 3;

            if (isEditor || isDevice)
            {
                Show();
            }
        }

        private void Init()
        {
            var ui = _context.GetModule<UIModule>();
            var canvas = ui.CanvasTransform;
            _cheatWindow = Create<CheatWindow>(_settings.WindowPrefab, canvas);
            _cheatWindow.Setup(this);
            _cheatWindow.gameObject.SetActive(false);
        }

        private T Create<T>(string path, Transform transform)
        {
            var prefab = Resources.Load<GameObject>(path);
            Bro.Log.Assert(prefab != null, $"cheat component prefab {path} is null");
            var instance = UnityEngine.Object.Instantiate(prefab, transform);
            return instance.GetComponent<T>(); 
        } 

        public void AddButton(string name, Action callback, bool autoHide = true)
        {
            var button = Create<CheatButton>(_settings.ButtonPrefab, _cheatWindow.ScrollTransform);
            button.Setup(this);
            button.SetTitle(name);
            button.SetCallback(callback);
            
            if (autoHide)
            {
                button.SetCallback(Hide);
            }
        }
        
        public void AddPopup(string name, Enum type)
        {
            var popup = Create<CheatPopup>(_settings.PopupPrefabs[type], _cheatWindow.PopupTransform);
            popup.Setup(this);
            popup.gameObject.SetActive(false);
            _popups[type] = popup;
            AddButton(name, () =>
            {
                _popups[type].gameObject.SetActive(true);
                _popups[type].OnShow();
            }, false);
        }

        private void Show()
        {
            _cheatWindow.gameObject.SetActive(true);
            HidePopup();
        }

        public void Hide()
        {
            _cheatWindow.gameObject.SetActive(false);
            HidePopup();
        }

        public void HidePopup()
        {
            foreach (var popup in _popups)
            {
                popup.Value.gameObject.SetActive(false);
            }
        }
    }
}