#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Bro.Toolbox.Client
{
    public class OpenPlayerPersistentData : EditorWindow
    {
        [MenuItem("Tools/Open/Persistent Data")]
        static void CreateWindow()
        {
            UnityEditorTools.OpenFileExplorer(Path.GetFullPath(Application.persistentDataPath));
        }
    }
}
#endif