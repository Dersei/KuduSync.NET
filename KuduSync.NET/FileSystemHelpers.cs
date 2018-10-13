﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace KuduSync.NET
{
    internal static class FileSystemHelpers
    {
        public static string GetDestinationPath(string sourceRootPath, string destinationRootPath, FileSystemInfoBase info)
        {
            string sourcePath = info.FullName;
            sourcePath = sourcePath.Substring(sourceRootPath.Length)
                                   .Trim(Path.DirectorySeparatorChar);

            return Path.Combine(destinationRootPath, sourcePath);
        }

        public static IDictionary<string, FileInfoBase> GetFiles(DirectoryInfoBase info)
        {
            return info?.GetFilesWithRetry().ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsEmpty(this DirectoryInfoBase info)
        {
            if (info == null)
            {
                return true;
            }

            FileSystemInfoBase[] fileSystemInfos = OperationManager.Attempt(() => info.GetFileSystemInfos());
            return fileSystemInfos.Length == 0;
        }

        public static IDictionary<string, DirectoryInfoBase> GetDirectories(DirectoryInfoBase info)
        {
            return info?.GetDirectories().ToDictionary(d => d.Name, StringComparer.OrdinalIgnoreCase);
        }

        // Call DirectoryInfoBase.GetFiles under a retry loop to make the system
        // more resilient when some files are temporarily in use
        public static FileInfoBase[] GetFilesWithRetry(this DirectoryInfoBase info)
        {
            return OperationManager.Attempt(info.GetFiles);
        }

        public static string GetRelativePath(string rootPath, string path)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return path.Substring(rootPath.Length).TrimStart('\\');
        }

        public static bool IsSubDirectory(string path1, string path2)
        {
            if (path1 == null || path2 == null) return false;
            path1 = Path.GetFullPath(path1);
            path2 = Path.GetFullPath(path2);
            return path2.StartsWith(path1, StringComparison.OrdinalIgnoreCase);

        }
    }
}
