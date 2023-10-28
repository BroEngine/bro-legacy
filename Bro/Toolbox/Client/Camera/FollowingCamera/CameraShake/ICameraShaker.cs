using UnityEngine;

namespace Bro.Toolbox.Client
{
    internal interface ICameraShaker
    {
        void AddShake(CameraShakeData data, float distance, Vector2 direction, bool isSingleShake);
        void ProcessShakes();
    }
}