using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components
{
    internal class CSLSaberSet : IInitializable
    {
        private CSLConfig config;
        private CSLAssetLoader assetLoader;

        [Inject]
        public void Construct(CSLConfig config, CSLAssetLoader assetLoader)
        {
            this.config = config;
            this.assetLoader = assetLoader;
        }

        public CSLSaber LeftSaber { get; private set; }

        public CSLSaber RightSaber { get; private set; }

        public void Initialize()
        {
            string selectedSaber = config.CurrentlySelectedSaber;
            if (selectedSaber == "Default" || selectedSaber == null)
            {
                assetLoader.SelectedSaber = new CustomSaberData("DefaultSabers");
            }
            else
            {
                if (selectedSaber != assetLoader.SelectedSaber?.FileName)
                {
                    // The saber was changed so load the new one
                    assetLoader.SelectedSaber?.Destroy();
                    assetLoader.SelectedSaber = assetLoader.LoadSaberWithRepair(selectedSaber);
                }
            }

            CustomSaberData customSaberData = assetLoader.SelectedSaber;

            // Create custom saber
            if (customSaberData == null)
            {
                Logger.Error("Current CustomSaberData is null");
                return;
            }

            if (customSaberData.FileName != "DefaultSabers")
            {
                GameObject sabersObject = GameObject.Instantiate(customSaberData.SabersObject);

                GameObject leftSaberObject = sabersObject.transform.Find("LeftSaber").gameObject;
                GameObject rightSaberObject = sabersObject.transform.Find("RightSaber").gameObject;

                LeftSaber = leftSaberObject.AddComponent<CSLSaber>();
                RightSaber = rightSaberObject.AddComponent<CSLSaber>();
            }
        }

        public CSLSaber CustomSaberForSaberType(SaberType saberType)
        {
            switch (saberType)
            {
                case SaberType.SaberA:
                    return LeftSaber;

                case SaberType.SaberB:
                    return RightSaber;
            }
            return null;
        }
    }
}
