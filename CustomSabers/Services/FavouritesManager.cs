using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using Newtonsoft.Json;
using SabersLib.Models;
using Zenject;

namespace CustomSabersLite.Services;

internal class FavouritesManager : IInitializable
{
    private readonly HashSet<string> favouriteSaberHashes = [];
    private readonly FileInfo favouritesFile;
    
    public FavouritesManager(DirectoryManager directoryManager)
    {
        favouritesFile = new(Path.Combine(directoryManager.UserData.FullName, "favourites.json"));
    }

    private CancellationTokenSource updateFavouritesTokenSource = new();

    public void Initialize() => ReadFavourites();

    public void AddFavourite(SaberFileInfo saberFile)
    {
        favouriteSaberHashes.Add(saberFile.Hash);
        SaveFavourites();
    }

    public void RemoveFavourite(SaberFileInfo saberFile)
    {
        favouriteSaberHashes.Remove(saberFile.Hash);
        SaveFavourites();
    }

    public bool IsFavourite(SaberFileInfo saberFile) => 
        favouriteSaberHashes.Contains(saberFile.Hash);

    private void ReadFavourites()
    {
        if (!favouritesFile.Exists) return;
        using var favouritesStream = favouritesFile.OpenRead();
        var savedFavourites = favouritesStream.DeserializeStream<string[]>();
        if (savedFavourites is null) return;
        favouriteSaberHashes.Clear();
        foreach (var hash in savedFavourites)
        {
            favouriteSaberHashes.Add(hash);
        }
    }
    
    private void SaveFavourites()
    {
        updateFavouritesTokenSource.CancelThenDispose();
        updateFavouritesTokenSource = new();

        try
        {
            Task.Run(() => SaveFavouritesAsync(updateFavouritesTokenSource.Token));
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Logger.Error($"Problem encountered while updating favourites file:\n{e}");
        }
        return;
        
        async Task SaveFavouritesAsync(CancellationToken token)
        {
            favouritesFile.Delete();
            await using var streamWriter = favouritesFile.CreateText();
            using var jsonWriter = new JsonTextWriter(streamWriter);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, favouriteSaberHashes);
            await jsonWriter.FlushAsync(token);
        }
    }
}