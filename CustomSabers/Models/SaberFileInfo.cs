using CustomSabersLite.Utilities;
using System;
using System.IO;

namespace CustomSabersLite.Models;

internal class SaberFileInfo(string? fullPath, string? hash, DateTime dateAdded, CustomSaberType saberType)
{
    private string? relativePath = null;

    public FileInfo? FileInfo { get; } = string.IsNullOrEmpty(fullPath) ? null : new FileInfo(fullPath);

    public string? RelativePath => 
        FileInfo is null || !FileInfo.Exists ? null 
        : relativePath ??= FileUtils.TrimPath(FileInfo.FullName, PluginDirs.CustomSabers.FullName);

    public string? Hash { get; } = hash;

    public CustomSaberType Type { get; } = saberType;

    public DateTime DateAdded { get; } = dateAdded;

    public static SaberFileInfo DefaultSabers { get; } = new(null, string.Empty, DateTime.MinValue, CustomSaberType.Default);
}
