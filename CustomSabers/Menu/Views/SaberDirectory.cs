using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Menu.Views;

internal class SaberListFolderManager
{
    private readonly DirectoryManager directoryManager;
    private readonly DirectoryInfo[] customSabersSubDirs;
    
    public SaberListFolderManager(DirectoryManager directoryManager)
    {
        this.directoryManager = directoryManager;
        
        customSabersSubDirs = directoryManager.CustomSabers
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Where(d => d.EnumerateSaberFiles(SearchOption.AllDirectories).Any())
            .ToArray();

        CurrentDirectory = directoryManager.CustomSabers;
        // CustomSabers
        // -- saber.file
        // -- Folder
        // ---- Folder2
        // ------- saber.file
    }

    public DirectoryInfo CurrentDirectory { get; set; }
    public DirectoryInfo ParentDirectory => CurrentDirectory.Parent ?? directoryManager.CustomSabers;
    public bool InTopDirectory => CurrentDirectory.FullName == directoryManager.CustomSabers.FullName;

    public IEnumerable<DirectoryInfo> CurrentDirectorySubDirectories => GetSubDirectoriesOf(CurrentDirectory);
    
    public IEnumerable<DirectoryInfo> GetSubDirectoriesOf(DirectoryInfo directory) =>
        customSabersSubDirs.Where(dir => dir.Parent?.FullName == directory.FullName);
    
    public void GoBack()
    {
        if (CurrentDirectory == directoryManager.CustomSabers || CurrentDirectory.Parent is null)
        {
            return;
        }
        
        CurrentDirectory = CurrentDirectory.Parent;
    }
}

internal class SaberDirectory
{
    public SaberDirectory(DirectoryInfo directoryInfo, SaberFileInfo[] topLevelSabers)
    {
        Info = directoryInfo;
        TopLevelSabers = topLevelSabers;
    }
    
    public DirectoryInfo Info { get; }
    public SaberFileInfo[] TopLevelSabers { get; }
}