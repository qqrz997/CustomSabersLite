using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Interfaces
{
    internal interface ISaberSet
    {
        /// <summary>
        /// Gets the instance of the selected saber by type
        /// </summary>
        CSLSaber CustomSaberForSaberType(SaberType saberType);

        /// <summary>
        /// Replaces the current saber set with another one 
        /// </summary>
        /// <param name="filePath">Relative path to the saber file in Custom Sabers</param>
        Task SetSabers(string filePath);

        /// <summary>
        /// Replaces the current saber set with another one by object reference
        /// </summary>
        /// <param name="sabersObject"></param>
        void SetSabers(GameObject sabersObject);
    }
}
