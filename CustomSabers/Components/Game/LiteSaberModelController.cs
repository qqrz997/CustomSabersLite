using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Configuration;
using CustomSabersLite.Components.Managers;
using System.Threading.Tasks;

namespace CustomSabersLite.Components.Game
{
    internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
    {
        private TrailManager trailManager;
        private ColorManager colorManager;
        private LiteSaberSet saberSet;
        private EventManagerManager eventManagerManager;
        private CSLConfig config;
        private LevelSaberManager levelSaberManager;

        [Inject]
        public void Construct(TrailManager trailManager, ColorManager colorManager, LiteSaberSet saberSet, EventManagerManager eventManagerManager, CSLConfig config, LevelSaberManager levelSaberManager)
        {
            this.trailManager = trailManager;
            this.colorManager = colorManager;
            this.saberSet = saberSet;
            this.eventManagerManager = eventManagerManager;
            this.config = config;
            this.levelSaberManager = levelSaberManager;
        }

        private bool defaultInit = true;
        private Color? color;

        public LiteSaber customSaberInstance;

        public LiteSaberTrail[] customTrailInstances;

        public Color Color { get => color.GetValueOrDefault(); set => SetColor(value); }

        public bool PreInit(Transform parent, Saber saber)
        {
            CustomSaberInit(parent, saber);
            _saberTrail.enabled = defaultInit; // It's safe to enable the default trail because SiraUtil is using our SaberModelController
            return defaultInit;
        }

        private async void CustomSaberInit(Transform parent, Saber saber)
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

            customSaberInstance.Setup(saber.transform, saberSet.Type);
            eventManagerManager.InitializeEventManager(customSaberInstance.EventManager, saberType);

            Color saberColor = config.EnableCustomColorScheme
                ? CustomSchemeColorForSaberType(saberType)
                : colorManager.ColorForSaberType(saberType);

            customTrailInstances = trailManager.CreateTrail(_saberTrail, saberColor, customSaberInstance.gameObject, customSaberInstance.Type);

            if (customTrailInstances != null)
            {
                defaultInit = false;
            }
            else
            {
                Logger.Warn("No custom trails. Defaulting to existing saber trails.");
            }

            SetColor(saberColor);
        }

        private Color CustomSchemeColorForSaberType(SaberType saberType) =>
            saberType == SaberType.SaberA ? config.LeftSaberColor : config.RightSaberColor;

        private void SetColor(Color color)
        {
            this.color = color;
            customSaberInstance?.SetColor(color);
            if (customTrailInstances != null)
            {
                foreach (LiteSaberTrail customTrail in customTrailInstances)
                {
                    customTrail?.SetColor(color);
                }
            }
        }
    }
}
