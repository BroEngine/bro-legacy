using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bro.Toolbox.Client
{
    public class InputControllerBase : IDisposable
    {
        private readonly List<RaycastResult> _results = new List<RaycastResult>();
        public virtual void ProcessInput()
        {
            
        }

        protected bool IsUiPressed(Vector2 pressPosition)
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {    
                position = pressPosition
            };
            
            _results.Clear();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, _results);
            
            return _results.Count > 0;
        }

        public virtual void Dispose()
        {
        }
    }
}