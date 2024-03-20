using CustomSabersLite.Utilities;
using SiraUtil.Interfaces;
using UnityEngine;
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

        public CSLSaber customSaberInstance;

        public CSLSaberTrail customTrailInstance;

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

            if (CSLUtils.CheckMultiplayer())
            {
                return true;
            }

            transform.SetParent(parent, false);

            customSaberInstance.Setup(gameSetupData.transformedBeatmapData, saber.transform);
            customSaberInstance.Controller = this;

            Color saberColor = colorManager.ColorForSaberType(saber.saberType);
            customSaberInstance.SetColor(saberColor);

            SaberTrail defaultTrail = ReflectionUtil.GetField<SaberTrail, SaberModelController>(this, "_saberTrail");

            // Returns false if a custom trail is created
            bool defaultInit = trailHandler.CreateTrail(defaultTrail, saberColor, customSaberInstance.gameObject);

            if (trailHandler.TrailInstance != null)
            {
                customTrailInstance = trailHandler.TrailInstance;
            }

            return defaultInit;
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
