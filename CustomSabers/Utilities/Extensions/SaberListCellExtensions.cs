using System.Diagnostics.CodeAnalysis;
using System.IO;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Extensions;

internal static class SaberListCellExtensions
{
    public static bool TryGetCellDirectory(this IListCellInfo cellInfo, [NotNullWhen(true)] out DirectoryInfo? dir)
    {
        dir = cellInfo switch
        {
            ListDirectoryCellInfo directoryCell => directoryCell.DirectoryInfo,
            ListUpDirectoryCellInfo upDirectoryCell => upDirectoryCell.DirectoryInfo,
            _ => null
        };
        return dir != null;
    }

    public static bool TryGetSaberValue(this IListCellInfo cellInfo, [NotNullWhen(true)] out SaberValue? saberValue)
    {
        saberValue = cellInfo is ListInfoCellInfo infoCell ? infoCell.Value : null;
        return saberValue != null;
    }
}