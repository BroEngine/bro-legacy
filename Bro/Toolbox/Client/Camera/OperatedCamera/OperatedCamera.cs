using Bro.Sketch;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;
using Rect = UnityEngine.Rect;

namespace Bro.Toolbox.Client
{
    public class OperatedCamera : MonoBehaviour
    {   
        [SerializeField] private Camera _camera;
        [SerializeField] public Rect SoftViewBounds = new Rect(0f, 0f, 40f, 40f);
        [SerializeField] public Rect HardViewBounds = new Rect(0f, 0f, 40f, 40f);

        private ICameraProjection _cameraProjection;
        
        public float MinZoom = 2f;
        public float MaxZoom = 8f;
        public float ZoomSpeed = 2f;
        
        [Range(0f,1f)] public float SoftCorrectionFactor = 0.5f;
        [Range(0.1f,3f)] public float InertiaFactorDelta = 0.2f;
        [SerializeField] private float _inertiaDuration = 1f;

        public bool IsMoving => _isDragging || _isScrolling || ForcedMove;
        
        private Vector3 _inertiaVector;
        
        private bool _isScrolling = false;
        private bool _isDragging = false;

        private float _timeSinceDragEnd;

        [HideInInspector] public bool IsSoftCorrectionEnable = true;
        [HideInInspector] public bool IsHardCorrectionEnable = true;
        [HideInInspector] public bool ForcedMove = false;
        
        public Rect CameraBounds => _cameraProjection.CameraBounds;

        private void Awake()
        {
            if (_camera.orthographic)
            {
                _cameraProjection = new OrthographicProjection(_camera);
            }
            else
            {
                _cameraProjection = new PerspectiveProjection(_camera);       
            }
        }

        public void SetViewBounds(Rect softBounds, Rect hardBounds)
        {
            SoftViewBounds = softBounds;
            HardViewBounds = hardBounds;
        }

        private void SoftCorrectPosition()
        {
            var cameraBounds = CameraBounds;
            
            if (IsSoftCorrectionEnable)
            {
                if (!SoftViewBounds.Contains(cameraBounds))
                {
                    var deltaPos = CalculateCorrectionVector(SoftViewBounds, cameraBounds);
                    deltaPos *= SoftCorrectionFactor;
                    InstantMove(deltaPos);
                }
            }
        }

        private void HardCorrectPosition()
        {
            var cameraBounds = CameraBounds;

            if (IsHardCorrectionEnable)
            {
                if (!HardViewBounds.Contains(cameraBounds))
                {
                    var deltaPos = CalculateCorrectionVector(HardViewBounds, cameraBounds);
                    InstantMove(deltaPos);
                }
            }
        }

        public void OnEndDrag()
        {
            _isScrolling = true;
            _isDragging = false;
            _timeSinceDragEnd = 0f;
        }

        private void UpdateInertiaValue(Vector3 deltaPos)
        {
            _inertiaVector = deltaPos * InertiaFactorDelta;
        }
        
        private Vector3 CalculateCorrectionVector(Rect targetBounds, Rect cameraBounds)
        {
            var result = new Vector3();
            
            if (cameraBounds.xMin < targetBounds.xMin)
            {
                result.x = targetBounds.xMin - cameraBounds.xMin;
            }
            else if (cameraBounds.xMax > targetBounds.xMax)
            {
                result.x = targetBounds.xMax - cameraBounds.xMax;
            }

            if (cameraBounds.yMin < targetBounds.yMin)
            {
                result.y = targetBounds.yMin - cameraBounds.yMin;
            }
            else if (cameraBounds.yMax > targetBounds.yMax)
            {
                result.y = targetBounds.yMax - cameraBounds.yMax;
            }
            return result;
        }

        public void Move(Vector3 deltaPos)
        {
            _isDragging = true;
            InstantMove(deltaPos);
            UpdateInertiaValue(deltaPos);
            HardCorrectPosition();
        }

        private void InstantMove(Vector3 deltaPos)
        {
           AddDeltaPosition(deltaPos);
        }

        public void Zoom(float deltaZoom)
        {
            var currentZoom = _camera.orthographicSize;
            var targetZoom = currentZoom + deltaZoom;
            targetZoom = Mathf.Clamp(targetZoom, MinZoom, MaxZoom);
            _camera.orthographicSize = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * ZoomSpeed);
            HardCorrectPosition();
        }
        
        private void LateUpdate()
        {
            
            #if UNITY_EDITOR
            _cameraProjection.OnChangeViewPort();
            #endif

            SimulateInertia(Time.deltaTime);
            SoftCorrectPosition();
            HardCorrectPosition();
        }

        private void SimulateInertia(float dt)
        {
            if (!_isDragging)
            {
                if (_isScrolling && _inertiaVector.sqrMagnitude > float.Epsilon)
                {
                    _timeSinceDragEnd += dt;
                    if (_timeSinceDragEnd >= _inertiaDuration)
                    {
                        _isScrolling = false;
                        _inertiaVector = Vector3.zero;
                    }
                    else
                    {
                        var t = _timeSinceDragEnd / _inertiaDuration;
                        var speedX = EaseQuartOut(_inertiaVector.x, 0f, t);
                        var speedY = EaseQuartOut(_inertiaVector.y, 0f, t);
                        AddDeltaPosition(new Vector3(speedX, speedY, 0f));
                    }
                }
            }
        }

        private void AddDeltaPosition(Vector3 v3)
        {
            transform.localPosition += v3;
        }

        private static float EaseQuartOut(float from, float to, float t)
        {
            return Mathf.Lerp(from, to, 1 - --t * t * t * t);
        }

#if UNITY_EDITOR
        [ContextMenu("Validate")]
        void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlaying && !UnityEditor.EditorApplication.isCompiling)
            {
                if (_camera == null)
                {
                    _camera = gameObject.GetComponent<Camera>();
                }
                if (_camera == null)
                {
                    UnityEngine.Debug.LogError("Hey developer, add camera script!");
                }
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 ToVector3(Vector2 v)
            {
                return new Vector3(v.x, v.y, 0f);
            }

            void DrawRect(Rect rect, Color color)
            {
                Handles.color = color;
                
                Handles.DrawLine(ToVector3(rect.min), new Vector3(rect.min.x, rect.max.y));
                Handles.DrawLine(ToVector3(rect.max), new Vector3(rect.min.x, rect.max.y));
                Handles.DrawLine(ToVector3(rect.min), new Vector3(rect.max.x, rect.min.y));
                Handles.DrawLine(ToVector3(rect.max), new Vector3(rect.max.x, rect.min.y));
            }

            DrawRect(SoftViewBounds, Color.yellow);
            DrawRect(HardViewBounds, Color.red);

            if (Application.isPlaying)
            {
                DrawRect(CameraBounds, Color.green);
            }
        }
#endif
    }
}