namespace CustomSabersLite.Models;

internal record SaberListCellText(string Text, string Subtext)
{
    public static SaberListCellText Create(CustomSaberMetadata meta) => meta.LoaderError switch
    {
        SaberLoaderError.None => new(meta.Descriptor.SaberName.FullName, meta.Descriptor.AuthorName.FullName),
        SaberLoaderError.InvalidFileType => new($"<color=#F77>Error - File is not of a valid type</color>", meta.FileInfo.FileName),
        SaberLoaderError.FileNotFound => new($"<color=#F77>Error - Couldn't find file</color>", meta.FileInfo.FileName),
        SaberLoaderError.LegacyWhacker => new($"<color=#F77>Not loaded - Legacy whacker, incompatible with PC</color>", meta.FileInfo.FileName),
        SaberLoaderError.NullBundle => new($"<color=#F77>Error - Problem encountered when loading asset</color>", meta.FileInfo.FileName),
        SaberLoaderError.NullAsset => new($"<color=#F77>Error - Problem encountered when loading saber model</color>", meta.FileInfo.FileName),
        _ => new($"<color=#F77>Error - Unknown error encountered during loading</color>", meta.FileInfo.FileName)
    };
}
