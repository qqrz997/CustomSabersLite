using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.Utilities;

internal class FileUtils
{
    public static string[] GetFilePaths(string directory, string[] extensions, SearchOption searchOption, bool returnShortPath)
    {
        var files = GetFiles(directory, extensions, searchOption);
        return returnShortPath ? TrimPaths(files, directory) : files;
    }

    private static string[] GetFiles(string directory, string[] extensions, SearchOption searchOption) =>
        extensions
        .Select(extension => Directory.EnumerateFiles(directory, $"*{extension}", searchOption))
        .SelectMany(files => files)
        .ToArray();

    private static string[] TrimPaths(IEnumerable<string> fullPaths, string trimPath) =>
        fullPaths
        .Where(path => path != trimPath)
        .Select(path => path.Replace(trimPath, string.Empty))
        .Select(path => path.Substring(1, path.Length - 1))
        .ToArray();
}
