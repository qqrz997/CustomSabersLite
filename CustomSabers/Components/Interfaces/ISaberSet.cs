using System.Threading.Tasks;

namespace CustomSabersLite.Components.Interfaces
{
    internal interface ISaberSet
    {
        CSLSaber CustomSaberForSaberType(SaberType saberType);

        Task SetSabers(string filePath);

        Task InstantiateSabers(string saberPath);
    }
}
