namespace CustomSabersLite.Models;

internal class SaberListCellText
{
    public string Text { get; }

    public string Subtext { get; }

    public SaberListCellText(CustomSaberMetadata meta) => (Text, Subtext) = meta.LoaderError switch
    {
        SaberLoaderError.None => (meta.Descriptor.SaberName.FullName, meta.Descriptor.AuthorName.FullName),
        SaberLoaderError.InvalidFileType => ($"<color=#F77>Error - File is not of a valid type</color>", meta.FileInfo.FileName),
        SaberLoaderError.FileNotFound => ($"<color=#F77>Error - Couldn't find file</color>", meta.FileInfo.FileName),
        SaberLoaderError.LegacyWhacker => ($"<color=#F77>Not loaded - Legacy whacker, incompatible with PC</color>", meta.FileInfo.FileName),
        SaberLoaderError.NullBundle => ($"<color=#F77>Error - Problem encountered when loading asset</color>", meta.FileInfo.FileName),
        SaberLoaderError.NullAsset => ($"<color=#F77>Error - Problem encountered when loading saber model</color>", meta.FileInfo.FileName),
        _ => ($"<color=#F77>Error - Unknown error encountered during loading</color>", meta.FileInfo.FileName)
    };
}