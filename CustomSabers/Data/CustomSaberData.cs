using UnityEngine;

namespace CustomSabersLite.Data;

/// <summary>
/// Class that declares the neccessary information to manage a custom saber instance
/// </summary>
internal class CustomSaberData(string relativePath, GameObject saberPrefab, SaberDescriptor descriptor, CustomSaberType customSaberType)
{
    public string FilePath { get; } = relativePath;

    public SaberPrefab Left { get; } = new(saberPrefab, SaberType.SaberA);

    public SaberPrefab Right { get; } = new(saberPrefab, SaberType.SaberB);

    public SaberDescriptor Descriptor { get; } = descriptor;

    public CustomSaberType Type { get; } = customSaberType;

    public bool MissingShaders; // not yet implemented

    private readonly GameObject parentPrefab = saberPrefab;

    public static CustomSaberData Default =>
        new(null, null, new SaberDescriptor { SaberName = "Default", AuthorName = "Beat Games" }, CustomSaberType.Default);

    public void Destroy()
    {
        Object.Destroy(Descriptor);
        Object.Destroy(parentPrefab);
    }
}
