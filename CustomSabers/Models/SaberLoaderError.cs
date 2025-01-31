namespace CustomSabersLite.Models;

// TODO: make strongly typed errors

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