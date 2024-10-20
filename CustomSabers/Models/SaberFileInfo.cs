using System;
using System.IO;

namespace CustomSabersLite.Models;

internal class SaberFileInfo(string? relativePath, string hash, DateTime dateAdded, CustomSaberType saberType)
{
    public string? RelativePath { get; } = relativePath;

    public string Hash { get; } = hash;

    public CustomSaberType Type { get; } = saberType;

    public DateTime DateAdded { get; } = dateAdded;

    public string FileName => Path.GetFileName(RelativePath);

    public static SaberFileInfo DefaultSabers { get; } = new(null, string.Empty, DateTime.MinValue, CustomSaberType.Default);
}
