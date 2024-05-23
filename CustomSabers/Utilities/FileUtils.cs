using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.Utilities
{
    internal class FileUtils
    {
        public static List<string> GetFilePaths(string path, IEnumerable<string> fileExtensions, SearchOption searchOption = SearchOption.AllDirectories, bool returnShortPath = false)
        {
            List<string> filePaths = new List<string>();

            foreach (string extension in fileExtensions)
            {
                string[] files = Directory.GetFiles(path, "*"+extension, searchOption);
                    
                if (!returnShortPath)
                {
                    filePaths.AddRange(files.ToList());
                }
                else
                {
                    filePaths.AddRange(GetSubPaths(files, path));
                }
            }

            return filePaths;
        }

        private static List<string> GetSubPaths(IEnumerable<string> fullPaths, string trimPath)
        {
            List<string> subPaths = new List<string>();

            foreach (string file in fullPaths)
            {
                string filePath = file.Replace(trimPath, "");
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
}
