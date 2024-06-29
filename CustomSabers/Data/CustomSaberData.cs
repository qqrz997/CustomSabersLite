using System;
using System.IO;
using UnityEngine;

namespace CustomSabersLite.Data;

/// <summary>
/// Class that declares the neccessary information to manage a custom saber instance
/// </summary>
internal class CustomSaberData(string relativePath, AssetBundle assetBundle, GameObject saberPrefab, SaberDescriptor descriptor, CustomSaberType customSaberType) : IDisposable
{
    public string FilePath { get; } = relativePath;

    private SaberPrefab Left { get; } = new(saberPrefab, SaberType.SaberA);

    private SaberPrefab Right { get; } = new(saberPrefab, SaberType.SaberB);

    public SaberDescriptor Descriptor { get; } = descriptor;

    public CustomSaberType Type { get; } = customSaberType;

    public bool MissingShaders; // not yet implemented

    private readonly AssetBundle assetBundle = assetBundle;
    private readonly GameObject parentPrefab = saberPrefab;

    public static CustomSaberData Default =>
        new(null, null, null, new SaberDescriptor { SaberName = "Default", AuthorName = "Beat Games" }, CustomSaberType.Default);

    public GameObject GetPrefab(SaberType type) =>
        type == SaberType.SaberA ? Left.Prefab : Right.Prefab;

    public void Dispose()
    {
        try
        {
            if (assetBundle) assetBundle.Unload(true);
            GameObject.Destroy(Descriptor);
            GameObject.Destroy(parentPrefab);
        }
        catch (Exception ex)
        {
            Logger.Error($"Couldn't dispose data for saber asset {Path.GetFileName(FilePath)}\n{ex}");
        }
    }
}
