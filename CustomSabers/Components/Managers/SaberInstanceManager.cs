using CustomSabersLite.Data;
using System.Collections.Generic;

namespace CustomSabersLite.Components.Managers
{
    internal class SaberInstanceManager
    {
        private readonly Dictionary<string, CustomSaberData> saberInstances = new Dictionary<string, CustomSaberData>();

        public void AddSaber(CustomSaberData saberData)
        {
            if (saberData.FilePath is null)
            {
                return;
            }

            if (!HasSaber(saberData.FilePath))
            {
                saberInstances.Add(saberData.FilePath, saberData);
            }
        }

        public bool HasSaber(string saberPath) => 
            saberInstances.ContainsKey(saberPath);

        public bool TryGetSaber(string saberPath, out CustomSaberData saber) => 
            saberInstances.TryGetValue(saberPath, out saber);
    }
}
