using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Utilities.Common;
using Zenject;

namespace CustomSabersLite.Menu.Views;

internal class SaberListFolderManager : IInitializable
{
    private readonly DirectoryManager directoryManager;
    private readonly List<DirectoryInfo> customSabersSubDirs = [];
    
    public SaberListFolderManager(DirectoryManager directoryManager)
    {
        this.directoryManager = directoryManager;
        CurrentDirectory = directoryManager.CustomSabers;
    }

    public DirectoryInfo CurrentDirectory { get; set; }
    public DirectoryInfo ParentDirectory => CurrentDirectory.Parent ?? directoryManager.CustomSabers;
    public bool InTopDirectory => CurrentDirectory.FullName == directoryManager.CustomSabers.FullName;

    public IEnumerable<DirectoryInfo> CurrentDirectorySubDirectories => GetSubDirectoriesOf(CurrentDirectory);


    public void Initialize() => Refresh();
    public void Refresh()
    {
        customSabersSubDirs.Clear();
        customSabersSubDirs.AddRange(directoryManager.CustomSabers
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Where(d => d.EnumerateSaberFiles(SearchOption.AllDirectories).Any())
            .ToArray());
    }
    
    public IEnumerable<DirectoryInfo> GetSubDirectoriesOf(DirectoryInfo directory) =>
        customSabersSubDirs.Where(dir => dir.Parent?.FullName == directory.FullName);
}