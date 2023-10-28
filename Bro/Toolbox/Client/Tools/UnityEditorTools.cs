#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class UnityEditorTools
    {
        public static string ReadText(string path)
        {
            return File.ReadAllText(Application.dataPath + "/" + path);
        }

        public static void WriteText(string path, string data)
        {
            File.WriteAllText(Application.dataPath + "/" + path, data);
            UnityEditor.AssetDatabase.Refresh();
        }

        public static void OpenFileExplorer(string path)
        {
#if UNITY_EDITOR_OSX
            OpenFinder(path);
#else
            Process.Start("explorer.exe", path);
#endif
        }

        private static void OpenFinder(string path)
        {
            bool openInsidesOfFolder = false;

            // try mac
            string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

            if (System.IO.Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }

            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }

            string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

            try
            {
                System.Diagnostics.Process.Start("open", arguments);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogError(exception);
            }
        }
    }
}
#endif