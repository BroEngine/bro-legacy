using System;
using System.IO;

namespace Bro.Sketch
{
    public static class PersistentStorage
    {
        private static string GetPersistentPath(string relativePath)
        {
            #if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
            return $"{UnityEngine.Application.persistentDataPath}/{relativePath}";
            #else
            throw new NotSupportedException("cannot use it on not unity side");
            #endif
        }

        public static bool IsFileExists(string relativePath)
        {
            var persistentPath = GetPersistentPath(relativePath);
            return File.Exists(persistentPath);
        }

        public static void SaveText(string relativePath, string data)
        {
            var fullPath = GetPersistentPath(relativePath);
            File.WriteAllText(fullPath, data);
        }

        public static string LoadText(string relativePath)
        {
            var fullPath = GetPersistentPath(relativePath);
            return File.Exists(fullPath) ? File.ReadAllText(fullPath) : string.Empty;
        }
    }
}