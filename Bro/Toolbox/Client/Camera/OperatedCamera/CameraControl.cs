using Bro.Client;
using Bro.Toolbox.Client.Gesture;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private OperatedCamera _operatedCamera;
        [SerializeField] private bool _freezeX = false;
        [SerializeField] private bool _freezeY = false;
        [SerializeField] [Range(0.25f, 1.5f)] private float _axisXAccelerationFactor = 1f;
        [SerializeField] [Range(0.25f, 1.5f)] private float _axisYAccelerationFactor = 1f;
        
        private EventObserver<GestureEvent> _gestureEventObserver;

        public OperatedCamera OperatedCamera => _operatedCamera;

        protected virtual void Awake()
        {
            _gestureEventObserver = new EventObserver<GestureEvent>(OnGestureEvent);
        }

        private void OnGestureEvent(GestureEvent gestureEvent)
        {
            var gesture = gestureEvent.Gesture;

            switch (gesture.Type)
            {
                case Gesture.Gesture.GestureType.Move:
                    var moveGesture = (MoveGesture) gesture;
                    MoveGesture(moveGesture);
                    break;
                case Gesture.Gesture.GestureType.Scale:
                    var scaleGesture = (ScaleGesture) gesture;
                    ScaleGesture(scaleGesture);
                    break;
                case Gesture.Gesture.GestureType.Touch:
                    var touchGesture = (TouchGesture) gesture;
                    TouchGesture(touchGesture);
                    break;
            }
        }

        protected virtual void TouchGesture(TouchGesture gesture)
        {
        }
        
        protected virtual void MoveGesture(MoveGesture gesture)
        {
            var moveDelta = gesture.MoveDelta();
            moveDelta.x = _freezeX ? 0f : moveDelta.x * _axisXAccelerationFactor;
            moveDelta.y = _freezeY ? 0f : moveDelta.y * _axisYAccelerationFactor;

            var isEnded = gesture.Phase == Gesture.Gesture.GesturePhase.PressEnded;
            _operatedCamera.IsSoftCorrectionEnable = isEnded;
                    
            if (!isEnded)
            {
                _operatedCamera.Move(-moveDelta);
            }
            else
            {
                _operatedCamera.OnEndDrag();
            }
        }
        
        protected virtual void ScaleGesture(ScaleGesture gesture)
        {
            _operatedCamera.Zoom(-gesture.Scale);
        }
        
        private void OnDestroy()
        {
            _gestureEventObserver.Dispose();
            _gestureEventObserver = null;
        }
        
#if UNITY_EDITOR
        [ContextMenu("Validate")]
        void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlaying && !UnityEditor.EditorApplication.isCompiling)
            {
                if (_operatedCamera == null)
                {
                    _operatedCamera = GetComponent<OperatedCamera>();
                }
            }
        }
#endif
    }
}