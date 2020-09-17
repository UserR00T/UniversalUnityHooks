using System;
using System.IO;

namespace UniversalUnityHooks.Core.Utility
{
    /// <summary>
    /// A class that will mostly consist of static utility methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Finds '?_Data' in the supplied directory. It will loop over all directories and check which one ends with _Data.
        /// </summary>
        /// <param name="directory">The directory to search in. You may supply <c>Directory.GetCurrentDirectory()</c> here.</param>
        /// <returns>null if not found, otherwise a <see cref="System.IO.FileInfo"/> instance.</returns>
        /// <remarks>
        /// Note: If the Data folder was found, but Managed or Assembly-CSharp does not exist, it will still return a <see cref="System.IO.FileInfo"/> instance. The variable <see cref="System.IO.FileInfo.Exists"/> will be set to <c>false</c> at that point.
        /// </remarks>
        public static FileInfo FindAssemblyCSharp(string directory)
        {
            foreach (var currentDirectory in Directory.GetDirectories(directory))
            {
                if (!currentDirectory.EndsWith("_Data", StringComparison.CurrentCulture))
                {
                    continue;
                }
                return new FileInfo(Path.Combine(currentDirectory, "Managed", "Assembly-CSharp.dll"));
            }
            return null;
        }

        /// <summary>
        /// Flattens directories by grabbing all files inside the directory and range adding them to the file list.
        /// </summary>
        /// <param name="files">The list of files and directories. Files will be ignored, It will only flatten directories.</param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static List<FileInfo> FlattenDirectory(List<FileInfo> files, string searchPattern)
        {
            var newFiles = new List<FileInfo>(files);
            // Loops over input files and appends all files inside directories to files
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var attr = File.GetAttributes(file.FullName);
                if (!attr.HasFlag(FileAttributes.Directory))
                {
                    continue;
                }
                // Adds all files from directory to list
                newFiles.AddRange(new DirectoryInfo(file.FullName).GetFiles(searchPattern));
                newFiles.RemoveAt(i);
            }
            return newFiles;
        }
    }
}