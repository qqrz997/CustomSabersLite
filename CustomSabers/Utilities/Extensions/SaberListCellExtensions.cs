using System.Diagnostics.CodeAnalysis;
using System.IO;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Extensions;

internal static class SaberListCellExtensions
{
    public static bool TryGetCellDirectory(this ISaberListCell cell, [NotNullWhen(true)] out DirectoryInfo? dir)
    {
        if (cell is SaberListDirectoryCell directoryCell)
        {
            return (dir = directoryCell.DirectoryInfo) != null;
        }

        if (cell is SaberListUpDirectoryCell upDirectoryCell)
        {
            return (dir = upDirectoryCell.DirectoryInfo) != null;
        }

        dir = null;
        return false;
    }

    public static bool TryGetSaberValue(this ISaberListCell cell, [NotNullWhen(true)] out SaberValue? saberValue)
    {
        saberValue = null;
        if (cell is SaberListInfoCell infoCell)
        {
            saberValue = infoCell.Value;
        }
        return saberValue != null;
    }
}