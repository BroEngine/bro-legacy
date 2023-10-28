#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    [CustomEditor(typeof(ObjectsAlongSpline))]
    public class ObjectsAlongSplineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
 
            ObjectsAlongSpline component = (ObjectsAlongSpline)target;
 
            if (GUILayout.Button("Update Objects"))
            {
                component.CreateObjects();
            }
        }
    }
}
#endif