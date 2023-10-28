#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public static class ParticleSystemExtensions
    {
        public static void SetStartSpeed(this ParticleSystem p, float min, float max)
        {
            var main = p.main;
            var speed = main.startSpeed;
            speed.constantMax = max;
            speed.constantMin = min;
            main.startSpeed = speed;
        }

        public static void SetStartLifetime(this ParticleSystem p, float min, float max)
        {
            var main = p.main;
            var startLifetime = main.startLifetime;
            startLifetime.constantMax = max;
            startLifetime.constantMin = min;
            main.startLifetime = startLifetime;
        }

        public static void SetShapeScale(this ParticleSystem p, Vector3 value)
        {
            var shape = p.shape;
            shape.scale = value;
        }

        public static void SetShapePosition(this ParticleSystem p, Vector3 value)
        {
            var shape = p.shape;
            shape.position = value;
        }
        
        public static void SetShapeAngle(this ParticleSystem p, float value)
        {
            var shape = p.shape;
            shape.angle = value;
        }

        public static void SetEmissionRateOverTime(this ParticleSystem p, float value)
        {
            var em = p.emission;
            var rate = em.rateOverTime;
            rate.constant = value;
            em.rateOverTime = rate;
        }
        
        public static void SetMainColor(this ParticleSystem p, Color value)
        {
            var main = p.main;
            main.startColor = value;
        }
    }
}
#endif