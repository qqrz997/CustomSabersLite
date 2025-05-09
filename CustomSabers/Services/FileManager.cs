using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Services;

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
    public async Task<SaberFileInfo[]> GetSaberFilesAsync(CancellationToken token, IProgress<int> progress) => 
        await Task.Run(() => GetDistinctSaberFiles(token, progress), token);
    
    private SaberFileInfo[] GetDistinctSaberFiles(CancellationToken token, IProgress<int> progress)
    {
        progress.Report(0);
        
        var fileInfos = directoryManager.CustomSabers.EnumerateSaberFiles(SearchOption.AllDirectories).ToList();
        int i = 0;
        int lastPercent = 0;
        var saberFileBag = new ConcurrentBag<SaberFileInfo>();
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount / 2 - 1,
            CancellationToken = token
        };
        
        Parallel.ForEach(fileInfos, parallelOptions, file =>
        {
            if (TryCreateSaberFile(file, out var saberFileInfo)) saberFileBag.Add(saberFileInfo);
            
            int newPercent = (i + 1) * 100 / fileInfos.Count;
            if (newPercent != lastPercent)
            {
                progress.Report(newPercent);
                lastPercent = newPercent;
            }
            i++;
        });

        return saberFileBag.Distinct(new SaberFileInfoHashComparer()).ToArray();
    }

    private bool TryCreateSaberFile(FileInfo file, [NotNullWhen(true)] out SaberFileInfo? saberFileInfo)
    {
        try
        {
            saberFileInfo = new(file, SaberHashing.MD5Checksum(file, "x2"), timeService.GetUtcTime());
            return true;
        }
        catch
        {
            saberFileInfo = null;
            return false;
        }
    }

    private class SaberFileInfoHashComparer : IEqualityComparer<SaberFileInfo>
    {
        public bool Equals(SaberFileInfo? a, SaberFileInfo? b) => a != null && b != null && a.Hash == b.Hash;
        public int GetHashCode(SaberFileInfo obj) => obj.Hash.GetHashCode();
    }
}