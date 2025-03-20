using System.Diagnostics.CodeAnalysis;
using System.IO;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Extensions;

internal static class SaberListCellExtensions
{
    public static bool TryGetCellDirectory(this IListCellInfo cellInfo, [NotNullWhen(true)] out DirectoryInfo? dir)
    {
        if (cellInfo is ListDirectoryCellInfo directoryCell)
        {
            return (dir = directoryCell.DirectoryInfo) != null;
        }

        if (cellInfo is ListUpDirectoryCellInfo upDirectoryCell)
        {
            return (dir = upDirectoryCell.DirectoryInfo) != null;
        }

        dir = null;
        return false;
    }

    public static bool TryGetSaberValue(this IListCellInfo cellInfo, [NotNullWhen(true)] out SaberValue? saberValue)
    {
        saberValue = null;
        if (cellInfo is ListInfoCellInfo infoCell)
        {
            saberValue = infoCell.Value;
        }
        return saberValue != null;
    }
}