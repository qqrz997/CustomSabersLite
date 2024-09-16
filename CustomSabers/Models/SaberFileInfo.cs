using System.IO;

namespace CustomSabersLite.Models;

internal class SaberFileInfo(string? relativePath, CustomSaberType saberType)
{
    public string? RelativePath { get; } = string.IsNullOrWhiteSpace(relativePath) ? null : relativePath;

    public CustomSaberType Type { get; } = saberType;

    public string FileName => Path.GetFileName(RelativePath);
}
