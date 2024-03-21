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
        private CSLSaberSet saberSet;
        private EventManagerManager eventManagerManager;

        [Inject]
        public void Construct(CustomTrailHandler trailHandler, ColorManager colorManager, GameplayCoreSceneSetupData gameSetupData, CSLSaberSet saberSet, EventManagerManager eventManagerManager)
        {
            this.trailHandler = trailHandler;
            this.colorManager = colorManager;
            this.gameSetupData = gameSetupData;
            this.saberSet = saberSet;
            this.eventManagerManager = eventManagerManager;
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

            transform.SetParent(parent, false);

            customSaberInstance.Setup(saber.transform);

            eventManagerManager.InitializeEventManager(customSaberInstance.EventManager);

            Color saberColor = colorManager.ColorForSaberType(saber.saberType);
            
            SaberTrail defaultTrail = ReflectionUtil.GetField<SaberTrail, SaberModelController>(this, "_saberTrail");

            // Returns false if a custom trail is created
            bool defaultInit = trailHandler.CreateTrail(defaultTrail, saberColor, customSaberInstance.gameObject);

            if (trailHandler.TrailInstance != null)
            {
                customTrailInstance = trailHandler.TrailInstance;
            }

            SetColor(saberColor);

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
