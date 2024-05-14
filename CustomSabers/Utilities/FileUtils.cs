using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.Utilities
{
    internal class FileUtils
    {
        public static IEnumerable<string> GetFilePaths(string path, IEnumerable<string> fileExtensions, SearchOption searchOption = SearchOption.AllDirectories, bool returnShortPath = false)
        {
            IList<string> filePaths = new List<string>();

            foreach (string extension in fileExtensions)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => extension.Contains(Path.GetExtension(s).TrimEnd('.').ToLowerInvariant()));

                if (returnShortPath)
                {
                    foreach (string file in files)
                    {
                        string filePath = file.Replace(path, "");
                        if (filePath.Length > 0 && filePath.StartsWith(@"\"))
                        {
                            filePath = filePath.Substring(1, filePath.Length - 1);
                        }

                        if (!string.IsNullOrWhiteSpace(filePath) && !filePaths.Contains(filePath))
                        {
                            filePaths.Add(filePath);
                        }
                    }
                }
                else
                {
                    filePaths = filePaths.Union(files).ToList();
                }
            }

            return filePaths.Distinct();
        }
    }
}
