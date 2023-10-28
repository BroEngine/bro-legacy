using System;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    [Serializable]
    class CameraFollowData
    {
        public float Offset;
        public float Speed;
        public Vector2 NormalizedOffset;

        public CameraFollowData(float offset, float speed, Vector2 normalizedOffset)
        {
            Offset = offset;
            Speed = speed;
            NormalizedOffset = normalizedOffset;
        }
    }
}