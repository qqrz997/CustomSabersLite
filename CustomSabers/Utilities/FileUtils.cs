using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.Utilities;

internal class FileUtils
{
    public static string[] GetFilePaths(string directory, string[] extensions, SearchOption searchOption, bool returnShortPath) =>
        returnShortPath ? TrimPaths(GetFiles(directory, extensions, searchOption), directory)
        : GetFiles(directory, extensions, searchOption);

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
