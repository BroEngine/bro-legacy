using UnityEngine;

namespace Bro.Toolbox.Client
{
    class CameraFollowerSmoothOffset : CameraFollowerBase
    {
        private Vector3 _catchUpPosition;
        private Vector3 _currentOffset;
        private Vector3 _targetSmooth;
        private Transform _previousTarget;

        private float _xClampFactor = 0;
        private float _yClampFactor = 0;

        private const float ClampOffset = 5f;
        private const float ClampMin = 0.2f;
        private const float ClampMax = 1f;

        public CameraFollowerSmoothOffset(Transform obj) : base(obj)
        {

        }

        public override void CatchUp(Transform target, float speed)
        {
            if(_previousTarget == null)
            {
                _targetSmooth = target.position;
                _previousTarget = target;
            }

            if (_previousTarget != target)
            {
                _targetSmooth = _previousTarget.position;
                _previousTarget = target;
            }
            _targetSmooth = Vector3.Lerp(_targetSmooth,target.position, Time.deltaTime * speed * 2);
            _currentOffset = Vector3.Lerp(_currentOffset, _directedOffset, Time.deltaTime * speed);
            _catchUpPosition = _targetSmooth + _currentOffset;
        }

        public override void Clamp(Vector2 minPosition, Vector2 maxPosition, float clampSpeed)
        {
            
            var xClamp = _catchUpPosition.x;

            if (_catchUpPosition.x < minPosition.x)
            {
                if (_xClampFactor < 1)
                {
                    var deltaPos = Mathf.Clamp( (ClampOffset - (minPosition.x - _catchUpPosition.x)) / ClampOffset, ClampMin, ClampMax);
                    _xClampFactor += Time.deltaTime * clampSpeed / deltaPos;
                }
                
                xClamp = Mathf.Lerp(_catchUpPosition.x, minPosition.x, _xClampFactor);
            }
            else if (_catchUpPosition.x > maxPosition.x)
            {
                if (_xClampFactor < 1)
                {
                    var deltaPos = Mathf.Clamp((ClampOffset - (_catchUpPosition.x - maxPosition.x)) / ClampOffset, ClampMin, ClampMax);
                    _xClampFactor += Time.deltaTime * clampSpeed / deltaPos;
                }
                
                xClamp = Mathf.Lerp(_catchUpPosition.x, maxPosition.x, _xClampFactor);
            }
            else
            {
                _xClampFactor = 0;
            }

            var yClamp = _catchUpPosition.z;

            if (_catchUpPosition.z < minPosition.y)
            {
                if (_yClampFactor < 1)
                {
                    var deltaPos = Mathf.Clamp((ClampOffset - (minPosition.y - _catchUpPosition.z)) / ClampOffset, ClampMin, ClampMax);
                    _yClampFactor += Time.deltaTime * clampSpeed / deltaPos;
                }
                
                yClamp = Mathf.Lerp(_catchUpPosition.z, minPosition.y, _yClampFactor);
            }
            else if (_catchUpPosition.z > maxPosition.y)
            {
                if (_yClampFactor < 1)
                {
                    var deltaPos = Mathf.Clamp((ClampOffset - (_catchUpPosition.z - maxPosition.y)) / ClampOffset, ClampMin, ClampMax);
                    _yClampFactor += Time.deltaTime * clampSpeed / deltaPos;
                }
                
                yClamp = Mathf.Lerp(_catchUpPosition.z, maxPosition.y, _yClampFactor);
            }
            else
            {
                _yClampFactor = 0;
            }

            _catchUpPosition = new Vector3(xClamp, _catchUpPosition.y, yClamp);

        }

        public override void OverTake(Transform target)
        {
            _currentOffset = _directedOffset;
            _catchUpPosition = target.position + _currentOffset;

            _xClampFactor = 1;
            _yClampFactor = 1;
        }

        public override void Follow(Transform target)
        {
            _target.position = new Vector3(_catchUpPosition.x, 0, _catchUpPosition.z);
        }
    }
}