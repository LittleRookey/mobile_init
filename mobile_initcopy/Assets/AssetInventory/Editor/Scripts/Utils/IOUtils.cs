using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssetInventory
{
    public static class IOUtils
    {
        // Regex version
        public static IEnumerable<string> GetFiles(string path, string searchPatternExpression = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                .Where(file =>
                    reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path, IEnumerable<string> searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel()
                .SelectMany(searchPattern =>
                    Directory.EnumerateFiles(path, searchPattern, searchOption));
        }

        public static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public static bool IsSameDirectory(string path1, string path2)
        {
            DirectoryInfo di1 = new DirectoryInfo(path1);
            DirectoryInfo di2 = new DirectoryInfo(path2);

            return string.Equals(di1.FullName, di2.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public static void CopyDirectory(string sourceDir, string destDir, bool includeSubDirs = true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDir, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (includeSubDirs)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string tempPath = Path.Combine(destDir, subDir.Name);
                    CopyDirectory(subDir.FullName, tempPath, includeSubDirs);
                }
            }
        }

        public static async Task<long> GetFolderSize(string folder)
        {
            if (!Directory.Exists(folder)) return 0;
            DirectoryInfo dirInfo = new DirectoryInfo(folder);
            try
            {
                return await Task.Run(() => dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns a combined path with unified slashes
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static string PathCombine(string path1, string path2)
        {
            return Path.GetFullPath(Path.Combine(path1, path2));
        }

        /// <summary>
        /// Returns a combined path with unified slashes
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="path3"></param>
        /// <returns></returns>
        public static string PathCombine(string path1, string path2, string path3)
        {
            return Path.GetFullPath(Path.Combine(path1, path2, path3));
        }
        
        /// <summary>
        /// Returns a combined path with unified slashes
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <param name="path3"></param>
        /// <param name="path4"></param>
        /// <returns></returns>
        public static string PathCombine(string path1, string path2, string path3, string path4)
        {
            return Path.GetFullPath(Path.Combine(path1, path2, path3, path4));
        }
    }
}