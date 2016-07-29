using System;
using System.IO;

namespace PoGoNecroBotTools
{
    internal static class FileUtils
    {
        #region Methods

        public static void CopyTo(this DirectoryInfo directoryInfo, DirectoryInfo target)
        {
            if (string.Equals(directoryInfo.FullName, target.FullName, StringComparison.CurrentCultureIgnoreCase)) return;

            // Check if the target directory exists, if not, create it.
            if (!Directory.Exists(target.FullName)) target.Create();

            // Copy each file into it's new directory.
            foreach (var fileInfo in directoryInfo.EnumerateFiles())
            {
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var subDirectoryInfo in directoryInfo.EnumerateDirectories())
            {
                var targetSubDirectoryInfo = new DirectoryInfo(Path.Combine(target.FullName, subDirectoryInfo.Name));
                CopyTo(subDirectoryInfo, targetSubDirectoryInfo);
            }
        }

        #endregion
    }
}