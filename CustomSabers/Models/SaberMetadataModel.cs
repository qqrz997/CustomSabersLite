using Newtonsoft.Json;

namespace CustomSabersLite.Models;

public class SaberMetadataModel
{
    public string? RelativePath { get; }

    public CustomSaberType SaberType { get; }

    public SaberLoaderError LoaderError { get; }

    public string SaberName { get; }

    public string AuthorName { get; }

    public byte[]? Image { get; }

    public bool IncompatibleShaders { get; }

    public string[] IncompatibleShaderNames { get; }

    [JsonConstructor]
    public SaberMetadataModel(
        string? relativePath, CustomSaberType saberType, SaberLoaderError loaderError, string saberName, 
        string authorName, byte[]? image, bool incompatibleShaders, string[] incompatibleShaderNames)
    {
        RelativePath = relativePath;
        SaberType = saberType;
        LoaderError = loaderError;
        SaberName = saberName;
        AuthorName = authorName;
        Image = image;
        IncompatibleShaders = incompatibleShaders;
        IncompatibleShaderNames = incompatibleShaderNames;
    }
}
