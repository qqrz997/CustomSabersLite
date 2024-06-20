using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.Utilities;

internal class FileUtils
{
    public static List<string> GetFilePaths(string path, IEnumerable<string> fileExtensions, SearchOption searchOption = SearchOption.AllDirectories, bool returnShortPath = false)
    {
        List<string> filePaths = [];

        foreach (var extension in fileExtensions)
        {
            var files = Directory.GetFiles(path, "*"+extension, searchOption);
            filePaths.AddRange(returnShortPath ? GetSubPaths(files, path) : files);
        }

        return filePaths;
    }

    private static List<string> GetSubPaths(IEnumerable<string> fullPaths, string trimPath)
    {
        List<string> subPaths = [];

        foreach (var file in fullPaths)
        {
            var filePath = file.Replace(trimPath, "");
            if (filePath.Length > 0 && filePath.StartsWith(@"\"))
            {
                filePath = filePath.Substring(1, filePath.Length - 1);
            }

            if (!string.IsNullOrWhiteSpace(filePath) && !fullPaths.Contains(filePath))
            {
                subPaths.Add(filePath);
            }
        }

        return subPaths;
    }
}
