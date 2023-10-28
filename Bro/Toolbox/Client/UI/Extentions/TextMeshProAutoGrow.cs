#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace Bro.Toolbox.Client.UI
{
    public class TextMeshProAutoGrow : MonoBehaviour
    {
        [SerializeField] private bool _growWidth;
        [SerializeField] private bool _growHeight;
        
        private TextMeshProUGUI _label;
        private RectTransform _transform;
        private float _lastWidth;
        private float _lastHeight;
        
        private void Awake()
        {
            _label = GetComponent<TextMeshProUGUI>();
            _transform = GetComponent<RectTransform>();
            Configurate(true);
        }

        private void LateUpdate()
        {
            Configurate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Configurate(bool force =false)
        {
            if (_label == null || _transform == null)
            {
                _label = GetComponent<TextMeshProUGUI>();
                _transform = GetComponent<RectTransform>();
            }

            if (force)
            {
                _label.ForceMeshUpdate();
            }

            var bounds = _label.textBounds.size;

            if (_growHeight && _growHeight && ( ! _lastWidth.IsApproximatelyEqual( bounds.x ) || ! _lastHeight.IsApproximatelyEqual( bounds.y )) )
            {
                _transform.sizeDelta = new Vector2( bounds.x, bounds.y );
                _lastWidth = bounds.x;
                _lastHeight = bounds.y;
            }
            else  if (_growWidth && ! _lastWidth.IsApproximatelyEqual( bounds.x ))
            {
                _transform.sizeDelta = new Vector2( bounds.x, _transform.sizeDelta.y );
                _lastWidth = bounds.x;
            }
            else if (_growHeight && ! _lastHeight.IsApproximatelyEqual( bounds.y ))
            {
                _transform.sizeDelta = new Vector2( _transform.sizeDelta.x, bounds.y );
                _lastHeight = bounds.y;
            }
        }
    }
    
}

#endif