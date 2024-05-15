using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using System.Collections.Generic;
using Zenject;

namespace CustomSabersLite.Managers
{
    internal class SaberInstanceManager : IInitializable
    {
        private readonly CSLConfig config;

        public SaberInstanceManager(CSLConfig config)
        {
            this.config = config;
        }

        private readonly Dictionary<string, CustomSaberData> saberInstances = new Dictionary<string, CustomSaberData>();

        public void Initialize() => 
            AddSaber(new CustomSaberData().ForDefaultSabers());

        public void AddSaber(CustomSaberData saberData)
        {
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
