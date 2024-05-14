using CustomSabersLite.Data;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.AssetBundles
{
    internal interface ICustomSaberLoader
    {
        Task<CustomSaberData> LoadCustomSaberAsync(string saberFileName);
    }
}
