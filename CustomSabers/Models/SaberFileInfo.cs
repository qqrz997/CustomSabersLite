using System.IO;

namespace CustomSabersLite.Models;

internal class SaberFileInfo(string relativePath, string hash, CustomSaberType saberType)
{
    public string RelativePath { get; } = relativePath;

    public string Hash { get; } = hash;

    public CustomSaberType Type { get; } = saberType;

    public string FileName => Path.GetFileName(RelativePath);

    public static SaberFileInfo DefaultSabers { get; } = new(string.Empty, string.Empty, CustomSaberType.Default);
}
