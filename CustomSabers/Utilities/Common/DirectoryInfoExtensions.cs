using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.Utilities.Common;

internal static class DirectoryInfoExtensions
{
    private static string[] SaberFileTypes => ["saber", "whacker"];

    /// <summary>
    /// Enumerates supported saber file types in a directory
    /// </summary>
    /// <param name="directory">The directory to search</param>
    /// <param name="searchOption">If set to AllDirectories, then subfolders will be searched</param>
    /// <returns>A sequence containing info of each saber file found in the directory</returns>
    public static IEnumerable<FileInfo> EnumerateSaberFiles(this DirectoryInfo directory, SearchOption searchOption) =>
        SaberFileTypes.SelectMany(type => directory.EnumerateFiles($"*.{type}", searchOption));

    /// <summary>
    /// Filters a sequence of DirectoryInfos, removing duplicates based on the full path
    /// </summary>
    /// <param name="directoryInfos">The sequence of directories to filter</param>
    /// <returns>A sequence of distinct DirectoryInfos</returns>
    public static IEnumerable<DirectoryInfo> DistinctByPath(this IEnumerable<DirectoryInfo> directoryInfos) =>
        directoryInfos.GroupBy(directoryInfo => directoryInfo.FullName).Select(group => group.First());
}
