using CustomSabersLite.Utilities;
using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Configuration;
using CustomSabersLite.Components.Interfaces;

namespace CustomSabersLite.Components
{
    internal class CSLSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
    {
        private CustomTrailHandler trailHandler;
        private ColorManager colorManager;
        private ISaberSet saberSet;
        private EventManagerManager eventManagerManager;
        private CSLConfig config;
        private LevelSaberManager levelSaberManager;

        [Inject]
        public void Construct(CustomTrailHandler trailHandler, ColorManager colorManager, ISaberSet saberSet, EventManagerManager eventManagerManager, CSLConfig config, LevelSaberManager levelSaberManager)
        {
            this.trailHandler = trailHandler;
            this.colorManager = colorManager;
            this.saberSet = saberSet;
            this.eventManagerManager = eventManagerManager;
            this.config = config;
            this.levelSaberManager = levelSaberManager;
        }

        private Color? color;
        private bool defaultInit = true;

        public CSLSaber customSaberInstance;

        public CSLSaberTrail customTrailInstance;

        public Color Color
        {
            get => color.GetValueOrDefault();
            set => SetColor(value);
        }

        public bool PreInit(Transform parent, Saber saber)
        {
            CSLSaberInit(parent, saber);
            return defaultInit;
        }

        private async void CSLSaberInit(Transform parent, Saber saber)
        {
            await levelSaberManager.SaberSetupTask;

            SaberType saberType = saber.saberType;
            customSaberInstance = saberSet.CustomSaberForSaberType(saberType);

            if (customSaberInstance is null)
            {
                Logger.Error("Something went wrong when getting the custom saber instance");
                return;
            }

            transform.SetParent(parent, false);

            customSaberInstance.Setup(saber.transform);
            eventManagerManager.InitializeEventManager(customSaberInstance.EventManager, saberType);

            Color saberColor = config.EnableCustomColorScheme
                ? saberType == SaberType.SaberA ? config.LeftSaberColor : config.RightSaberColor
                : colorManager.ColorForSaberType(saberType); // don't judge me
            SaberTrail defaultTrail = this._saberTrail;

            defaultInit = trailHandler.CreateTrail(defaultTrail, saberColor, customSaberInstance.gameObject); // returns false if a custom trail is created

            if (trailHandler.Trail != null)
            {
                customTrailInstance = trailHandler.Trail;
            }

            SetColor(saberColor);
        }

        public void SetColor(Color color)
        {
            this.color = color;

            // set the color of the saber here!!!!
            customSaberInstance?.SetColor(color);
            customTrailInstance?.SetColor(color);
        }
    }
}
