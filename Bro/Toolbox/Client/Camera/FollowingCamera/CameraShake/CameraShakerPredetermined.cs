using UnityEngine;

namespace Bro.Toolbox.Client
{
    class CameraShakerPredetermined : CameraShakerBase
    {
        public CameraShakerPredetermined(Transform shakeTransform) : base(shakeTransform)
        {

        }

        public override void AddShake(CameraShakeData data, float distance, Vector2 direction, bool isSingleShake)
        {
            _shakes.Add(new CameraShakePredetermined(data, distance, direction, isSingleShake));
        }
    }
}