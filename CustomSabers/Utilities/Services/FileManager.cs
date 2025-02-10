using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Utilities.Services;

internal class FileManager
{
    private readonly ITimeService timeService;
    private readonly string[] saberFileTypes = ["saber", "whacker"];

    // Key is file hash
    // private readonly Dictionary<string, SaberFileInfo> saberFileInfos = [];

    public FileManager(ITimeService timeService)
    {
        this.timeService = timeService;
    }

    public async Task<SaberFileInfo[]> GetSaberFilesAsync() => await Task.Run(GetSaberFiles);
   
    private SaberFileInfo[] GetSaberFiles() => saberFileTypes
        .SelectMany(t => PluginDirs.CustomSabers.GetFiles($"*.{t}", SearchOption.AllDirectories))
        .Select(FileInfoToSaberFileInfo)
        .ToArray();

    private SaberFileInfo FileInfoToSaberFileInfo(FileInfo info) => 
        new(info,
            GetFileHash(info),
            timeService.GetUtcTime());
    
    private static string GetFileHash(FileInfo info) => Hashing.MD5Checksum(info.FullName, "x2");
}