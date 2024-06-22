using UnityEngine;

namespace CustomSabersLite.Data;

/// <summary>
/// Class that declares the neccessary information to manage a custom saber instance
/// </summary>
internal class CustomSaberData(string relativePath, GameObject saberPrefab, SaberDescriptor descriptor, CustomSaberType customSaberType, CustomTrailData[] trails)
{
    public string FilePath { get; } = relativePath;

    public GameObject LeftSaberPrefab { get; } = saberPrefab.transform.Find("LeftSaber").gameObject;

    public GameObject RightSaberPrefab { get; } = saberPrefab.transform.Find("RightSaber").gameObject;

    public SaberDescriptor Descriptor { get; } = descriptor;

    public CustomSaberType Type { get; } = customSaberType;

    public CustomTrailData[] Trails { get; } = trails;

    public bool MissingShaders; // not yet implemented

    private readonly GameObject parentPrefab = saberPrefab;

    public static CustomSaberData Default =>
        new(null, null, new SaberDescriptor { SaberName = "Default", AuthorName = "Beat Games" }, CustomSaberType.Default, null);

    public void Destroy()
    {
        Object.Destroy(Descriptor);
        Object.Destroy(parentPrefab);
    }
}
