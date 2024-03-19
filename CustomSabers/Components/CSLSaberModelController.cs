using CustomSabersLite.Utilities;
using SiraUtil.Interfaces;
using UnityEngine;
using CustomSabersLite.Configuration;
using Zenject;
using IPA.Utilities;

namespace CustomSabersLite.Components
{
    internal class CSLSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
    {
        private CustomTrailHandler trailHandler;
        private ColorManager colorManager;
        private GameplayCoreSceneSetupData gameSetupData;
        [InjectOptional] private CSLSaberSet saberSet;

        [Inject]
        public void Construct(CustomTrailHandler trailHandler, ColorManager colorManager, GameplayCoreSceneSetupData gameSetupData)
        {
            this.trailHandler = trailHandler;
            this.colorManager = colorManager;
            this.gameSetupData = gameSetupData;
        }

        private Color? color;

        private CSLSaber customSaberInstance;

        private CSLSaberTrail customTrailInstance;

        public Color Color
        {
            get => color.GetValueOrDefault();
            set => SetColor(value);
        }

        public bool PreInit(Transform parent, Saber saber)
        {
            // Do you want the original Init to run?
            return CSLSaberInit(parent, saber);
        }

        private bool CSLSaberInit(Transform parent, Saber saber)
        {
            customSaberInstance = saberSet.CustomSaberForSaberType(saber.saberType);

            if (customSaberInstance == null)
            {
                Logger.Error("Something went wrong when getting the custom saber instance");
                return true;
            }

            customSaberInstance.Setup(gameSetupData.transformedBeatmapData);

            customSaberInstance.transform.SetParent(saber.transform);
            customSaberInstance.transform.position = saber.transform.position;
            customSaberInstance.transform.rotation = saber.transform.rotation;

            Color saberColor = colorManager.ColorForSaberType(saber.saberType);
            customSaberInstance.SetColor(saberColor);

            SaberTrail defaultTrail = ReflectionUtil.GetField<SaberTrail, SaberModelController>(this, "_saberTrail");

            transform.SetParent(parent, false);

            return trailHandler.CreateTrail(defaultTrail, saberColor, customSaberInstance.gameObject);
        }

        public void SetColor(Color color)
        {
            this.color = color;

            // set the color of the saber here!!!!
            if (customSaberInstance != null)
            {
                customSaberInstance.SetColor(color);
            }

            if (customTrailInstance != null)
            {
                customTrailInstance.SetColor(color);
            }
        }
    }
}
