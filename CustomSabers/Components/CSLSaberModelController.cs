using CustomSabersLite.Utilities;
using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using IPA.Utilities;
using CustomSabersLite.Configuration;

namespace CustomSabersLite.Components
{
    internal class CSLSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
    {
        private CustomTrailHandler trailHandler;
        private ColorManager colorManager;
        private CSLSaberSet saberSet;
        private EventManagerManager eventManagerManager;
        private CSLConfig config;

        [Inject]
        public void Construct(CustomTrailHandler trailHandler, ColorManager colorManager, CSLSaberSet saberSet, EventManagerManager eventManagerManager, CSLConfig config)
        {
            this.trailHandler = trailHandler;
            this.colorManager = colorManager;
            this.saberSet = saberSet;
            this.eventManagerManager = eventManagerManager;
            this.config = config;
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
            if (config.CurrentlySelectedSaber == "Default")
            {
                Logger.Error("Somehow, the default saber is selected, but we don't know why");
                return true;
            }

            // Do you want the original Init to run?
            return CSLSaberInit(parent, saber);
        }

        private bool CSLSaberInit(Transform parent, Saber saber)
        {
            SaberType saberType = saber.saberType;

            customSaberInstance = saberSet.CustomSaberForSaberType(saberType);

            if (customSaberInstance == null)
            {
                Logger.Error("Something went wrong when getting the custom saber instance");
                return true;
            }

            transform.SetParent(parent, false);

            customSaberInstance.Setup(saber.transform);

            eventManagerManager.InitializeEventManager(customSaberInstance.EventManager, saberType);

            Color saberColor;

            if (config.EnableCustomColorScheme)
            {
                if (saberType == SaberType.SaberA)
                {
                    saberColor = config.LeftSaberColor;
                }
                else
                {
                    saberColor = config.RightSaberColor;
                }
            }
            else
            {
                saberColor = colorManager.ColorForSaberType(saberType);
            }

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
