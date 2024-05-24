using CustomSabersLite.Data;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.AssetBundles
{
    internal interface IWhackerLoader
    {
        Task<CustomSaberData> LoadWhackerAsync(string relativePath);
    }
}
