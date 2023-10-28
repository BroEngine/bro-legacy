#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Bro.Toolbox.Client
{
    [InitializeOnLoad]
    public class ClearEmptyFolders
    {

        [MenuItem( "Tools/Clear/Empty Folders", false )]
        static void CleanEmptyFoldersEditor()
        {
            Clean();
        }

        private static void Clean()
        {
            foreach ( var dir in Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories) )
            {
                var path = dir;
                DirectoryInfo info = new DirectoryInfo( path );

                if ( info.Name.StartsWith( "." ) )
                    continue;

                RemoveSingleFile( path );

                while ( Directory.GetFileSystemEntries( path ).Length == 0 )
                {
                    var asset = "Assets/" + path.Substring( Application.dataPath.Length + 1 );

                    new DirectoryInfo( path );

                    Debug.Log( "Removing empty directory: " + asset );

                    if ( !AssetDatabase.DeleteAsset( asset ) )
                    {
                        Bro.Log.Error( "Error deleting empty directory asset " + asset );
                    }

                    path = Path.GetDirectoryName( path );
                    RemoveSingleFile( path );
                }
            }
        }

        private static void RemoveSingleFile( string path )
        {
            if ( Directory.GetFileSystemEntries( path ).Length == 1 )
            {
                string filePath = Directory.GetFileSystemEntries( path ) [0];
                string fileName = Path.GetFileName( filePath );

                if ( fileName == ".DS_Store" )
                {				
                    Debug.Log( "Removing single .DS_Store file at " + filePath );
                    File.Delete( filePath );
                } 
            }
        }

    }
}
#endif