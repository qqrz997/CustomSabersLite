using System;
using System.IO;
using CustomSabersLite.Utilities.Common;
using static CustomSabersLite.Utilities.Common.FileUtils;

namespace CustomSabersLite.Models;

internal record SaberFileInfo(
    FileInfo FileInfo,
    string Hash,
    DateTime DateAdded,
    CustomSaberType Type)
{
    private string? relativePath;
    public string RelativePath => relativePath ??= TrimPath(FileInfo.FullName, PluginDirs.CustomSabers.FullName);
}
