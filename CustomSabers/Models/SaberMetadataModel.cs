using System;

namespace CustomSabersLite.Models;

/// <summary>
/// Serializable model for a custom saber's metadata
/// </summary>
internal record SaberMetadataModel(
    string RelativePath,
    string Hash,
    DateTime DateAdded,
    string SaberName,
    string AuthorName,
    bool Trails,
    SaberLoaderError LoaderError);
