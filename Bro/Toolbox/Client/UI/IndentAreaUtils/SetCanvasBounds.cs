using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client.UI
{
    public class SetCanvasBounds : MonoBehaviour
    {
        public enum Apply
        {
            MoveLeft,
            MoveRight
        }

        [SerializeField] private Apply _applyType;
        private RectTransform _panel;
        private const float _maxCameraAspect = 1.86f;
        private const float _maxOffsetFactor = 0.02f;
        
        private CanvasScaler _canvasScaler; 
        private Vector3 _originLocalPosition;
        private float _scaleRatio;
 
        private void Awake()
        {
            _panel = GetComponent<RectTransform>();
            var scalers = GetComponentsInParent<CanvasScaler>();
            if (scalers.Length > 0)
            {
                _canvasScaler = scalers[0];
            }

            if (_canvasScaler == null)
            {
                Bro.Log.Error("Joystick cant find CanvasScaler on scene");
            }
            
            CalculateConstants();
            
            if (_panel == null)
            {
                _panel = GetComponent<RectTransform>();
            }

            _originLocalPosition = _panel.anchoredPosition;
            ApplyUnSafeWidthArea( GetUnSafeWidth() ); // // Screen.safeArea;
        }

        #if UNITY_EDITOR
        private void Update()
        {
            CalculateConstants();
            ApplyUnSafeWidthArea( GetUnSafeWidth() ); // // Screen.safeArea;
        }
        #endif

        private float GetUnSafeWidth()
        {
            var saveWidth = Screen.safeArea.width;
            return Screen.width - saveWidth;
        }

        private void ApplyUnSafeWidthArea(float width)
        {
            var widthOffset = Screen.width * ( width / 2 / Screen.width  ) / _scaleRatio;
            var maxOffset = Screen.width * _maxOffsetFactor / _scaleRatio;
            var offset = Mathf.Min(widthOffset, maxOffset);
            
            switch (_applyType)
            {
                case Apply.MoveLeft:
                    _panel.anchoredPosition = new Vector3(  _originLocalPosition.x - offset, _originLocalPosition.y, _originLocalPosition.z );
                    break;
                case Apply.MoveRight:
                    _panel.anchoredPosition = new Vector3(  _originLocalPosition.x + offset, _originLocalPosition.y, _originLocalPosition.z );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void CalculateConstants()
        {
            _scaleRatio = _canvasScaler.transform.localScale.x;
        }
    }
}