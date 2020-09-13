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
    }
}