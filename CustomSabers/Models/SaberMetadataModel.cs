using System;

namespace CustomSabersLite.Models;

internal record SaberMetadataModel(
    string RelativePath,
    string Hash,
    CustomSaberType SaberType,
    SaberLoaderError LoaderError,
    string SaberName,
    string AuthorName,
    DateTime DateAdded);