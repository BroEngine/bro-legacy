using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform _trackingTransform;
        public bool IsInverted;
        private bool _isInverted;

        private ICameraFollower _follower;
        private CameraFollowerSmoothOffset _followerSmoothOffset;
        private CameraFollowData _currentFollowData = new CameraFollowData(0, 1, new Vector2(0, 0));

        [Header("Follow Data")]
        [SerializeField] float _clampSpeed = 0.5f;
        
        [SerializeField] public Vector2 MinPosition = new Vector2(-10, -20);
        [SerializeField] public Vector2 MaxPosition = new Vector2(10, 20);
        [SerializeField] public float CameraSpeedFactor = 1f;
        
        [SerializeField] private readonly CameraFollowData _defaultFollowData = new CameraFollowData(1, 1, new Vector2(0, 1));

        [SerializeField] private Vector3 _inversionRotation = new Vector3(0, 180, 0);

        private void FollowTrackingObject()
        {
            _currentFollowData = _defaultFollowData;
            ToFollowSmoothOffset();
            
            _follower.OverTake(_trackingTransform.transform);
        }
        
        public void SetTrackingObject(Transform trackingObject)
        {
            if (trackingObject == _trackingTransform)
            {
                return;
            }

            if (trackingObject != null)
            {
                _trackingTransform = trackingObject;

                FollowTrackingObject();
                
                _follower.StartCatchingUp();
                _follower.OverTake(_trackingTransform);
            }
            else
            {
                _trackingTransform = null;
            }

            SetInversion();
        }

        private void LateUpdate()
        {
            if (_trackingTransform == null || _follower == null || _currentFollowData == null)
            {
                return;
            }
            
            if (_follower == _followerSmoothOffset && !_currentFollowData.Offset.IsZero())
            {
                _follower.UpdateOffset(_currentFollowData.NormalizedOffset, _currentFollowData.Offset);
            }
            
            _follower.CatchUp(_trackingTransform.transform, _currentFollowData.Speed / CameraSpeedFactor);
            _follower.Clamp(MinPosition, MaxPosition, _clampSpeed);
            _follower.Follow(_trackingTransform.transform);
        }

        private void ToFollowSmoothOffset()
        {
            if (_followerSmoothOffset == null)
            {
                _followerSmoothOffset = new CameraFollowerSmoothOffset(transform);
            }

            _follower = _followerSmoothOffset;
        }

        private void SetInversion()
        {
            if (_isInverted == IsInverted)
            {
                return;
            }
            
            _isInverted = IsInverted;
            transform.Rotate(_inversionRotation);
        }
    }
}
