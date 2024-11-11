namespace CustomSabersLite.Models;

internal class SaberListCellText
{
    public string Text { get; }

    public string Subtext { get; }

    public SaberListCellText(CustomSaberMetadata meta)
    {
        string? saberFileName = meta.SaberFile.FileInfo?.Name ?? "?";
        (Text, Subtext) = meta.LoaderError switch
        {
            SaberLoaderError.None => (meta.Descriptor.SaberName.FullName, meta.Descriptor.AuthorName.FullName),
            SaberLoaderError.InvalidFileType => ($"<color=#F77>Error - File is not of a valid type</color>", saberFileName),
            SaberLoaderError.FileNotFound => ($"<color=#F77>Error - Couldn't find file</color>", saberFileName),
            SaberLoaderError.LegacyWhacker => ($"<color=#F77>Not loaded - Legacy whacker, incompatible with PC</color>", saberFileName),
            SaberLoaderError.NullBundle => ($"<color=#F77>Error - Problem encountered when loading asset</color>", saberFileName),
            SaberLoaderError.NullAsset => ($"<color=#F77>Error - Problem encountered when loading saber model</color>", saberFileName),
            _ => ($"<color=#F77>Error - Unknown error encountered during loading</color>", saberFileName)
        };
    }
}