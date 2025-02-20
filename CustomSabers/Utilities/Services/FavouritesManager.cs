using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using Newtonsoft.Json;
using Zenject;

namespace CustomSabersLite.Utilities.Services;

internal class FavouritesManager : IInitializable
{
    private readonly HashSet<string> favouriteSaberHashes = [];
    private readonly FileInfo favouritesFile;
    
    public FavouritesManager(DirectoryManager directoryManager)
    {
        favouritesFile = new(Path.Combine(directoryManager.UserData.FullName, "favourites.json"));
    }

    private CancellationTokenSource updateFavouritesTokenSource = new();
    
    public void Initialize()
    {
        if (favouritesFile.Exists)
        {
            using var stream = favouritesFile.OpenRead();
            stream.DeserializeStream<string[]>()?.ForEach(hash => favouriteSaberHashes.Add(hash));
        }
    }

    public void AddFavourite(SaberFileInfo saberFile)
    {
        favouriteSaberHashes.Add(saberFile.Hash);
        InitiateUpdate();
    }

    public void RemoveFavourite(SaberFileInfo saberFile)
    {
        favouriteSaberHashes.Remove(saberFile.Hash);
        InitiateUpdate();
    }

    public bool IsFavourite(SaberFileInfo saberFile) => favouriteSaberHashes.Contains(saberFile.Hash);
    
    private void InitiateUpdate()
    {
        updateFavouritesTokenSource.Cancel();
        updateFavouritesTokenSource.Dispose();
        updateFavouritesTokenSource = new();

        try
        {
            Task.Run(() => WriteUpdateTo(favouritesFile, updateFavouritesTokenSource.Token));
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Logger.Error($"Problem encountered while updating favourites file:\n{e}");
        }
    }

    private async Task WriteUpdateTo(FileInfo file, CancellationToken token)
    {
        file.Delete();
        await using var streamWriter = file.CreateText();
        using var jsonWriter = new JsonTextWriter(streamWriter);
        JsonSerializer.CreateDefault().Serialize(jsonWriter, favouriteSaberHashes);
        await jsonWriter.FlushAsync(token);
    }
}