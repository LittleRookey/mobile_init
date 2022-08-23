using System.IO;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.Common
{
    public static class EditorUtils
    {
        internal const string PACKAGE_BASE_PATH = "Packages/com.infinitevoid.spamabilityframework";
        
        public static string GetCurrentAssetDirectory()
        {
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                    continue;

                if (System.IO.Directory.Exists(path))
                    return path;
                if (File.Exists(path))
                    return Path.GetDirectoryName(path);
            }

            return "Assets";
        }
    }
}