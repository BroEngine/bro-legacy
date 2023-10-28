using System;
using System.Linq;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    /// <summary>
    /// MonoBehaviour for Bezier spline creation
    /// </summary>
    public class BezierSpline : MonoBehaviour
    {
        [SerializeField] private Vector3[] _points;
        [SerializeField] private BezierControlPointMode[] _modes;
        [SerializeField] private float[] _length;
        [SerializeField] private bool _loop;

        public bool Loop
        {
            get => _loop;
            set
            {
                _loop = value;
                if (value)
                {
                    _modes[_modes.Length - 1] = _modes[0];
                    SetControlPoint(0, _points[0]);
                }
            }
        }
        public int CurveCount => (_points.Length - 1) / 3;
        public int ControlPointCount => _points.Length;

        public Vector3 GetControlPoint(int index)
        {
            return _points[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - _points[index];
                if (_loop)
                {
                    if (index == 0)
                    {
                        _points[1] += delta;
                        _points[_points.Length - 2] += delta;
                        _points[_points.Length - 1] = point;
                    }
                    else if (index == _points.Length - 1)
                    {
                        _points[0] = point;
                        _points[1] += delta;
                        _points[index - 1] += delta;
                    }
                    else
                    {
                        _points[index - 1] += delta;
                        _points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        _points[index - 1] += delta;
                    }
                    if (index + 1 < _points.Length)
                    {
                        _points[index + 1] += delta;
                    }
                }
            }
            
            _points[index] = point;
            EnforceMode(index);
        }

        public Vector3 GetPoint(float t)
        {
            int i;
            
            if (t >= 1f)
            {
                t = 1f;
                i = _points.Length - 4;
            }
            else {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            
            return transform.TransformPoint(Bezier.GetPoint(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t));
        }
        
        public Vector3 GetPointWithLength(float t)
        {
            int pointIndex = 0;
            
            if (t >= 1)
            {
                t = 1f;
                pointIndex = _points.Length - 4;
            }
            else
            {
                var tLength = Mathf.Lerp(0, _length.Sum(), t);
                var sum = 0f;
                var splineIndex = 0;
                for (int i = 0; i < _length.Length; i++)
                {
                    sum += _length[i];
                    if (sum > tLength)
                    {
                        splineIndex = i;
                        break;
                    }
                }

                if (splineIndex > 0)
                {
                    tLength -= sum - _length[splineIndex];
                }

                pointIndex = splineIndex * 3;
                t = Mathf.InverseLerp(0, _length[splineIndex], tLength);
            }
            
            return transform.TransformPoint(Bezier.GetPoint(_points[pointIndex], _points[pointIndex + 1], _points[pointIndex + 2], _points[pointIndex + 3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            
            if (t >= 1f)
            {
                t = 1f;
                i = _points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int)t;
                t -= i;
                i *= 3;
            }
            
            return transform.TransformPoint(Bezier.GetFirstDerivative(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t)) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public void Reset()
        {
            _points = new Vector3[]
            {
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(4f, 0f, 0f)
            };
            
            _modes = new BezierControlPointMode[]
            {
                BezierControlPointMode.Free,
                BezierControlPointMode.Free
            };
            
            _length = new float[] {1};
        }
        
        public void AddCurve()
        {
            Vector3 point = _points[_points.Length - 1];
            Array.Resize(ref _points, _points.Length + 3);
            point.x += 1f;
            _points[_points.Length - 3] = point;
            point.x += 1f;
            _points[_points.Length - 2] = point;
            point.x += 1f;
            _points[_points.Length - 1] = point;
            
            Array.Resize(ref _modes, _modes.Length + 1);
            _modes[_modes.Length - 1] = _modes[_modes.Length - 2];
            EnforceMode(_points.Length - 4);
            
            Array.Resize(ref _length, _length.Length + 1);
            _length[_length.Length - 1] = 1;
            
            if (_loop)
            {
                _points[_points.Length - 1] = _points[0];
                _modes[_modes.Length - 1] = _modes[0];
                EnforceMode(0);
            }
        }
        
        public void RemoveCurve()
        {
            if (_points.Length <= 4)
            {
                return;
            }
            
            Array.Resize(ref _points, _points.Length - 3);
            Array.Resize(ref _modes, _modes.Length - 1);
            Array.Resize(ref _length, _length.Length - 1);
            
            if (_loop)
            {
                _points[_points.Length - 1] = _points[0];
                _modes[_modes.Length - 1] = _modes[0];
                EnforceMode(0);
            }
        }
        
        public BezierControlPointMode GetControlPointMode(int index)
        {
            return _modes[(index + 1) / 3];
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            int modeIndex = (index + 1) / 3;
            _modes[modeIndex] = mode;
            
            if (_loop)
            {
                if (modeIndex == 0)
                {
                    _modes[_modes.Length - 1] = mode;
                }
                else if (modeIndex == _modes.Length - 1)
                {
                    _modes[0] = mode;
                }
            }
            EnforceMode(index);
        }
        
        private void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            
            BezierControlPointMode mode = _modes[modeIndex];
            if (mode == BezierControlPointMode.Free || !_loop && (modeIndex == 0 || modeIndex == _modes.Length - 1))
            {
                return;
            }
            
            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                {
                    fixedIndex = _points.Length - 2;
                }
                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= _points.Length)
                {
                    enforcedIndex = 1;
                }
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= _points.Length)
                {
                    fixedIndex = 1;
                }
                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                {
                    enforcedIndex = _points.Length - 2;
                }
            }
            
            Vector3 middle = _points[middleIndex];
            Vector3 enforcedTangent = middle - _points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, _points[enforcedIndex]);
            }
            _points[enforcedIndex] = middle + enforcedTangent;
        }

        public float GetCurveLength(int index)
        {
            return _length[Mathf.Max(0, index - 1) / 3];
        }
        
        public void SetCurveLength(int index, float length)
        {
            int lengthIndex = Mathf.Max(0, index - 1) / 3;
            
            _length[lengthIndex] = length;
        }
    }
}