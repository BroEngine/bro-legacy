using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    internal abstract class CameraShakerBase : ICameraShaker
    {
        protected readonly Transform _shakeTransform = null;
        protected readonly List<CameraShakeBase> _shakes = new List<CameraShakeBase>();

        private Vector3 _prevPositionOffset;
        private Vector3 _prevRotationOffset;

        public CameraShakerBase(Transform shakeTransform)
        {
            _shakeTransform = shakeTransform;
        }

        public virtual void AddShake(CameraShakeData data, float distance, Vector2 direction, bool isSingleShake)
        {

        }

        public virtual void ProcessShakes()
        {
            if (_shakeTransform == null || _shakes.Count == 0)
                return;

            var positionOffset = Vector3.zero;
            var rotationOffset = Vector3.zero;

            for (int i = _shakes.Count - 1; i != -1; i--)
            {
                var shake = _shakes[i];
               
                shake.UpdateOffset();

                if (shake.Target == CameraShakeData.Property.Position)
                {
                    SetShakeOffset(shake, ref positionOffset);
                }
                else
                {
                    SetShakeOffset(shake, ref rotationOffset);
                }
                
                if (!shake.IsAlive)
                {
                    _shakes.RemoveAt(i);
                }
            }

            _shakeTransform.position += positionOffset - _prevPositionOffset;
            _shakeTransform.eulerAngles += rotationOffset - _prevRotationOffset;

            _prevPositionOffset = positionOffset;
            _prevRotationOffset = rotationOffset;
        }

        private void SetShakeOffset(CameraShakeBase shake, ref Vector3 offset)
        {
            var maxAxisOffset = Mathf.Max(shake.Offset.x, shake.Offset.y, shake.Offset.z);
            var maxPositionAxisOffset = Mathf.Max(offset.x, offset.y, offset.z);
            if (maxPositionAxisOffset < maxAxisOffset)
            {
                offset = shake.Offset;
            }
        }
    }
}