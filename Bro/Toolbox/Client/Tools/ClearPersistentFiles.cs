#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Bro.Toolbox.Client
{
    [InitializeOnLoad]
    public class ClearPersistentFiles
    {
        [MenuItem( "Tools/Clear/Persistent Files", false )]
        static void ClearPersistentFilesEditor()
        {
            Clean();
        }

        private static void Clean()
        {
            foreach ( var file in Directory.GetFiles(Application.persistentDataPath, "*.*", SearchOption.AllDirectories) )
            {
                FileInfo info = new FileInfo(file);
                Bro.Log.Info("deleted " + info.FullName);
                File.Delete( info.FullName );
            }
        }
    }
}

#endif