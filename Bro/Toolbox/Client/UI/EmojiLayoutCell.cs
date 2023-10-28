#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public class EmojiLayoutCell : RadialLayoutCell
    {
        [SerializeField] private Transform _iconTransorm;
        [SerializeField] private float _iconOffset;
        
        public override void SetRotation(float fAngle)
        {
            base.SetRotation(fAngle);
            
            var vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
            _iconTransorm.localPosition = vPos * _iconOffset;
        }
    }
}
#endif