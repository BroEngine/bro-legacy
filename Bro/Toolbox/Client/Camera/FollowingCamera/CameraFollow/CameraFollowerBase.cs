using UnityEngine;
using System;
using Bro.Sketch;

namespace Bro.Toolbox.Client
{
    abstract class CameraFollowerBase : ICameraFollower
    {
        public event Action OnDeparted;
        public event Action OnArrived;
        
        protected readonly Transform _target;
        
        protected Vector2 _normalizedOffset;
        protected float _offset;
        protected Vector3 _directedOffset;

        protected Vector3 _targetPosition;

        public CameraFollowerBase(Transform obj)
        {
            _target = obj;
        }

        public virtual void OverTake(Transform target)
        {
            _target.position = new Vector3(target.position.x, 0, target.position.z);
        }

        public virtual void CatchUp(Transform target, float catchUpTime)
        {
            _targetPosition = target.position + _directedOffset;
        }

        public virtual void StartCatchingUp()
        {

        }

        public virtual void Clamp(Vector2 minPosition, Vector2 maxPosition, float clampSpeed)
        {
            
        }

        public virtual void UpdateOffset(Vector2 normalizedOffset, float offset)
        {
            if (!_normalizedOffset.IsEqual(normalizedOffset) || !_offset.IsApproximatelyEqual(offset))
            {
                _normalizedOffset = normalizedOffset;
                _offset = offset;
                _directedOffset = new Vector3(_normalizedOffset.x * offset, 0, _normalizedOffset.y * offset);

                StartCatchingUp();
            }
        }

        public virtual void Follow(Transform target)
        {
            _target.position = new Vector3(target.position.x, 0, target.position.z);
        }

        public void CallOnDeparted()
        {
            if (OnDeparted != null)
            {
                OnDeparted();
                OnDeparted = null;
            }
        }

        public void CallOnArrived()
        {
            if (OnArrived != null)
            {
                OnArrived();
                OnArrived = null;
            }
        }
    }
}