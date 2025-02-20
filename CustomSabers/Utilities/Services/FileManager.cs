using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Utilities.Services;

internal class FileManager
{
    private readonly ITimeService timeService;
    private readonly DirectoryManager directoryManager;

    public FileManager(ITimeService timeService, DirectoryManager directoryManager)
    {
        this.timeService = timeService;
        this.directoryManager = directoryManager;
    }

    /// <summary>
    /// Search the sabers directory for all saber files, getting their file info and computing their checksum. This will
    /// ignore duplicate saber files with the same hash.
    /// </summary>
    /// <returns>An array containing each saber file info</returns>
    public async Task<SaberFileInfo[]> GetSaberFilesAsync() => await Task.Run(GetDistinctSaberFiles);
   
    private SaberFileInfo[] GetDistinctSaberFiles() => 
        directoryManager.CustomSabers.EnumerateSaberFiles(SearchOption.AllDirectories)
            .Select(TryCreateSaberFile)
            .OfType<SaberFileInfo>()
            .Distinct(new SaberFileInfoHashComparer())
            .ToArray();

    private SaberFileInfo? TryCreateSaberFile(FileInfo file)
    {
        try
        {
            return new(file, Hashing.MD5Checksum(file, "x2"), timeService.GetUtcTime());
        }
        catch
        {
            return null;
        }
    }

    private class SaberFileInfoHashComparer : IEqualityComparer<SaberFileInfo>
    {
        public bool Equals(SaberFileInfo? a, SaberFileInfo? b) => a != null && b != null && a.Hash == b.Hash;
        public int GetHashCode(SaberFileInfo obj) => obj.Hash.GetHashCode();
    }
}