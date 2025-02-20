﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Common;

internal static class FileUtils
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
        
    public static IEnumerable<string> GetFilePaths(string directory, string[] extensions, SearchOption searchOption, bool returnShortPath)
    {
        var files = GetFiles(directory, extensions, searchOption);
        return returnShortPath ? TrimPaths(files, directory) : files;
    }

    private static IEnumerable<string> GetFiles(string directory, string[] extensions, SearchOption searchOption) =>
        extensions
        .SelectMany(extension => Directory.EnumerateFiles(directory, $"*{extension}", searchOption));

    private static IEnumerable<string> TrimPaths(IEnumerable<string> fullPaths, string trimPath) =>
        fullPaths
        .Where(path => path != trimPath)
        .Select(path => path.Replace(trimPath, string.Empty))
        .Select(path => path.Substring(1, path.Length - 1));

    public static string TrimPath(string fullPath, string trimPath)
    {
        if (fullPath == trimPath) return string.Empty;
        string trimmed = fullPath.Replace(trimPath, string.Empty);
        string path = trimmed.Substring(1, trimmed.Length - 1);
        return path;
    }
}
