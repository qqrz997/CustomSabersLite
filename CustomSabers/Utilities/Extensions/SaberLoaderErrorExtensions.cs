using SabersCore.Models;

namespace CustomSabersLite.Utilities.Extensions;

internal static class SaberLoaderErrorExtensions
{
    public static string GetErrorMessage(this SaberLoaderError error) => error switch
    {
        SaberLoaderError.None => string.Empty,
        SaberLoaderError.InvalidFileType => "<color=#F77>Error - File is not of a valid type</color>",
        SaberLoaderError.FileNotFound => "<color=#F77>Error - Couldn't find a file</color>",
        SaberLoaderError.LegacyWhacker => "<color=#F77>Not loaded - Legacy whacker, incompatible with PC</color>",
        SaberLoaderError.NullBundle => "<color=#F77>Error - Problem encountered when loading asset</color>",
        SaberLoaderError.NullAsset => "<color=#F77>Error - Problem encountered when loading saber model</color>",
        _ => "<color=#F77>Error - Unknown error encountered during loading</color>"
    };
}