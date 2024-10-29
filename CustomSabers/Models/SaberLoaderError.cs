namespace CustomSabersLite.Models;

public enum SaberLoaderError
{
    None = 0,
    Unknown,
    InvalidFileType,
    FileNotFound,
    LegacyWhacker,
    NullBundle,
    NullAsset,
}
