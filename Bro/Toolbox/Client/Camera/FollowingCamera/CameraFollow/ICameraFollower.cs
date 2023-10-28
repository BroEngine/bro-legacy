using UnityEngine;
using System;

namespace Bro.Toolbox.Client
{
    interface ICameraFollower
    {
        event Action OnDeparted;
        event Action OnArrived;

        void CallOnDeparted();
        void CallOnArrived();

        void StartCatchingUp();
        void CatchUp(Transform target, float catchUpTime);
        void Clamp(Vector2 minPosition, Vector2 maxPosition, float clampSpeed);
        void OverTake(Transform target);
        void UpdateOffset(Vector2 normalizedOffset, float offset);
        void Follow(Transform target);
    }
}