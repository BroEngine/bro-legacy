#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public class RadialLayoutCell : MonoBehaviour
    {
        [SerializeField] private Transform _rotatableObject;

        public virtual void SetRotation(float angle)
        {
            if (_rotatableObject == null) return;

            _rotatableObject.eulerAngles = new Vector3(
                _rotatableObject.eulerAngles.x, 
                _rotatableObject.eulerAngles.y,
                180 + angle);
        }
    }
}
#endif