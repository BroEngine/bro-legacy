#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII)

using UnityEngine;
using System.Collections;
using System;
using Bro.Client;
using Bro.Sketch.Client;

namespace Bro.Toolbox.Client.UI
{
    public abstract class Window : BaseWindow
    {
        private IClientContext _context;
        private IDisposable _showHideEaseDisposable;
        private IDisposable _hideSchedule;
        
        // ReSharper disable once InconsistentNaming
        protected Animator _animator;
        
        private readonly int _windowShowAnimation = Animator.StringToHash("Show");
        private readonly int _windowHideAnimation = Animator.StringToHash("Hide");

        private const float CompletedStateTimeout = .3f;

        public WindowItemType ItemType { get; set; }
        public IClientContext Context => _context;
        public UIModule UIModule => _context.GetModule<UIModule>();

        private enum State
        {
            Closed,
            Closing,
            Shown,
            Showing
        }

        /// <summary>
        /// Specific window cant to be closed externally
        /// </summary>
        public virtual bool IsCloseAvailable { get { return true; } }

        public enum WindowItemType
        {
            Window,
            Popup,
            Widget
        }

        [SerializeField] private bool _coroutineAnimation = false;
        [SerializeField] private bool _customAnimation = false;
        [SerializeField] protected float _hideAnimationDuration = 0.3f;

        private GameObject _windowObject;
        private float _completedStateTimer;

        private State _currentState;

        public IWindowArgs CurrentArgument { get; private set; }
        public bool ReturnedFromStack { get; private set; }
        public int WindowPriority;
        public virtual string HideSoundId => "hide";
        public virtual string ShowSoundId => "show";
        
        public void ShowWindow(IClientContext context, IWindowArgs args = null, bool fromStack = false)
        {
            _context = context;
            
            Show();
            ReturnedFromStack = fromStack;
            CurrentArgument = args;
            OnShow(args);
        }

        public void HideWindow()
        {
            OnHide();
            Hide();
        }

        protected virtual void OnShowAnimated()
        {
        }

        protected virtual void OnHideAnimated()
        {
            DirectlyHide();
        }

        public void DirectlyHide()
        {
            gameObject.SetActive(false);
            _currentState = State.Closed;
        }

        protected virtual void OnDestroy()
        {
            if (gameObject.activeSelf)
            {
                OnHide();
            }
        }

        private void Show()
        {
            SetupComponents();

            _currentState = State.Closed;

            gameObject.SetActive(true);
            _windowObject.SetActive(true);

            if (_coroutineAnimation)
            {
                _showHideEaseDisposable?.Dispose();
                _showHideEaseDisposable = _context.Scheduler.ScheduleEase(ShowEase(OnShowAnimated));
            }
            else if (_customAnimation)
            {
                AnimateShow();
            }
            else
            {
                OnShowAnimated();
            }

            _currentState = State.Showing;
            _completedStateTimer = CompletedStateTimeout;
        }

        private void Hide()
        {
            if (_currentState == State.Showing || _currentState == State.Shown)
            {
                _currentState = State.Closed;
                
                if (_coroutineAnimation)
                {
                    _showHideEaseDisposable?.Dispose();
                    if (gameObject.activeInHierarchy)
                    {
                        _showHideEaseDisposable = _context.Scheduler.ScheduleEase(HideEase(OnHideAnimated));
                    }
                }
                else if (_customAnimation)
                {
                    AnimateHide();
                }
                else
                {
                    OnHideAnimated();
                }
            }
            else
            {
                _currentState = State.Closing;
                _completedStateTimer = CompletedStateTimeout;
            }
        }

        private void AnimateShow()
        {
            if (_animator != null)
            {
                _hideSchedule?.Dispose();
                _animator.Play(_windowShowAnimation);
            }
            else
            {
                Bro.Log.Error("window :: cannot find animator for custom animation");
            }
        }

        private void AnimateHide()
        {
            _animator = GetComponent<Animator>();
            if (_animator != null)
            {
                _animator.Play(_windowHideAnimation);
                _hideSchedule = Context.Scheduler.Schedule(OnHideAnimated, _hideAnimationDuration);
            }
        }
        
        protected virtual Timing.Ease.Base ShowEase(Action onShowAnimated)
        {
            return AnimateWindowAction(1000, 0, onShowAnimated);
        }

        protected virtual Timing.Ease.Base HideEase(Action onHideAnimated)
        {
            return AnimateWindowAction(0, -1000, onHideAnimated);
        }

        private void SetupComponents()
        {
            _animator = GetComponent<Animator>();
            _windowObject = gameObject;
        }

        protected virtual void FixedUpdate()
        {
            _completedStateTimer -= Time.fixedDeltaTime;

            if ((_currentState == State.Closing) && _completedStateTimer < 0)
            {
                _currentState = State.Closed;
            }

            if ((_currentState == State.Showing) && _completedStateTimer < 0)
            {
                _currentState = State.Shown;
            }
        }

        public bool IsPermanent;
        
        public bool IsShowing()
        {
            return gameObject.activeInHierarchy && (_currentState != State.Closed && _currentState != State.Closing);
        }

        #region utils

        protected void DestroyCoroutine(Coroutine c)
        {
            if (c != null)
            {
                StopCoroutine(c);
            }
            c = null;
        }

        #endregion

        private Timing.Ease.Base AnimateWindowAction(float from, float to, Action onEndAnimation)
        {
            var duration = 0.24f;
            var windowPosition = _windowObject.transform.position;
            var startV3 = new Vector3(windowPosition.x, from, windowPosition.z);
            var endV3 = new Vector3(windowPosition.x, to, windowPosition.z);
            var moveEase = _windowObject.transform.CreateMovePosition(startV3, endV3, duration, EasingFunctions.Ease.Linear);
            var onEndEase = new Timing.Ease.Instant(() => onEndAnimation?.Invoke());
            return new Timing.Ease.Sequence(moveEase, onEndEase);
        }
    }
}

#endif