using UnityEngine;

namespace CustomSabersLite.Data;

/// <summary>
/// Class that declares the neccessary information to manage a custom saber instance
/// </summary>
internal class CustomSaberData(string relativePath, GameObject saberPrefab, SaberDescriptor descriptor, CustomSaberType customSaberType)
{
    public string FilePath { get; private set; } = relativePath;
    public GameObject SaberPrefab { get; private set; } = saberPrefab;
    public SaberDescriptor Descriptor { get; private set; } = descriptor;
    public CustomSaberType Type { get; private set; } = customSaberType;

    public bool MissingShaders; // not yet implemented

    public static CustomSaberData Default =>
        new(null, null, new SaberDescriptor { SaberName = "Default", AuthorName = "Beat Games" }, CustomSaberType.Default);

    public void Destroy()
    {
        Object.Destroy(Descriptor);
        Object.Destroy(SaberPrefab);
    }
}
