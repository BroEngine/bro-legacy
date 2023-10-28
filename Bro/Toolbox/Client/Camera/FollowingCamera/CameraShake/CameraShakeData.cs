using System.Collections.Generic;
using UnityEngine;
using System;

namespace Bro.Toolbox.Client
{
    [Serializable]
    class CameraShakeData
    {
        public static float OutTangentValue = 720.0f;
        
        [Serializable]
        public class ShakeAnimation
        {
            public AnimationCurve ShakeX = new AnimationCurve(new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * OutTangentValue), new Keyframe(0.2f, 1.0f), new Keyframe(1.0f, 0.0f));
            public AnimationCurve ShakeY = new AnimationCurve(new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * OutTangentValue), new Keyframe(0.2f, 1.0f), new Keyframe(1.0f, 0.0f));
            public AnimationCurve ShakeZ = new AnimationCurve(new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * OutTangentValue), new Keyframe(0.2f, 1.0f), new Keyframe(1.0f, 0.0f));
        }

        public enum Property
        {
            Position,
            Rotation
        }

        [Header("Shake Type")]
        public Property TargetProperty = Property.Position;

        [Header("Procedural Generation")]
        public float Amplitude = 2.0f;
        public float Frequency = 5.0f;
        public float Duration = 1.0f;
        public float MaxDistanceSquared = 100.0f;

        [Header("Predetermined Values")]
        public List<ShakeAnimation> ShakeAnimations;

        [Header("Intensity Over Time")]
        public AnimationCurve BlendOverLifetimeSingle = new AnimationCurve(new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * OutTangentValue), new Keyframe(0.2f, 1.0f), new Keyframe(1.0f, 0.0f));
        public AnimationCurve BlendOverLifetimeAuto = new AnimationCurve(new Keyframe(0.0f, 0.0f, Mathf.Deg2Rad * 0.0f, Mathf.Deg2Rad * OutTangentValue), new Keyframe(0.2f, 1.0f), new Keyframe(1.0f, 0.0f));
    }
}