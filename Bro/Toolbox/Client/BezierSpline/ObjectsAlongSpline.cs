using UnityEngine;

namespace Bro.Toolbox.Client
{
    /// <summary>
    /// MonoBehaviour for object creation along Bezier spline
    /// </summary>
    public class ObjectsAlongSpline : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _count;
        [SerializeField] private bool _lookForward;
        
        private BezierSpline _spline;

#if UNITY_EDITOR
        public void CreateObjects()
        {
            if (_count <= 0 || _prefab == null)
            {
                return;
            }

            if (_spline == null)
            {
                _spline = GetComponent<BezierSpline>();
            }

            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            
            float stepSize = 1 / (_spline.Loop ? _count : (_count - 1f));
            for (int i = 0; i < _count; i++)
            {
                var newObject = Instantiate(_prefab, transform);
                var newPosition = _spline.GetPointWithLength(i * stepSize);
                newObject.transform.position = newPosition;
                if (_lookForward)
                {
                    newObject.transform.LookAt(newPosition + _spline.GetDirection(i * stepSize));
                }
            }
        }
#endif
    }
}