using System.Collections.Generic;

namespace Bro.StateMachine
{
    public class StateMachine<T> where T : class, IState
    {
        private readonly List<T> _states = new List<T>();
        private T _activeState;

        public List<T> States
        {
            get { return _states; }
        }

        public T ActiveState
        {
            get { return _activeState; }
            set
            {
                if (_activeState == value)
                {
                    return;
                }

                if (_activeState != null)
                {
                    _activeState.OnFinish();
                }

                _activeState = value;
                if (_activeState != null)
                {
                    _activeState.OnStart();
                }
            }
        }

        protected void Add(T state)
        {
            _states.Add(state);
        }

        public void Update()
        {
            if (_activeState != null)
            {
                _activeState.Update();
            }
        }
    }
}