using UnityEngine;

namespace Bro.Toolbox.Client
{
    class CameraShakerRandom : CameraShakerBase
    {
        public CameraShakerRandom(Transform shakeTransform) : base(shakeTransform)
        {

        }

        public override void AddShake(CameraShakeData data, float distance, Vector2 direction, bool isSingleShake)
        {
            _shakes.Add(new CameraShakeRandom(data, distance, direction, isSingleShake));
        }

    }
}