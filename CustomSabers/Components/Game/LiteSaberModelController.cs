using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Configuration;
using CustomSabersLite.Components.Managers;

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

        private Color? color;

        public LiteSaber customSaberInstance;

        public LiteSaberTrail[] customTrailInstances;

        public Color Color { get => color.GetValueOrDefault(); set => SetColor(value); }

        public bool PreInit(Transform parent, Saber saber)
        {
            CustomSaberInit(parent, saber);
            return true;
        }

        private async void CustomSaberInit(Transform parent, Saber saber)
        {
            await levelSaberManager.SaberSetupTask;
            customSaberInstance = saberSet.CustomSaberForSaberType(saber.saberType);

            if (customSaberInstance is null)
            {
                Logger.Error("Something went wrong when getting the custom saber instance");
                return;
            }

            transform.SetParent(parent, false);

            customSaberInstance.Setup(parent, saberSet.Data.Type);
            eventManagerManager.InitializeEventManager(customSaberInstance.EventManager, saber.saberType);

            Color saberColor = config.EnableCustomColorScheme
                ? CustomSchemeColorForSaberType(saber.saberType)
                : colorManager.ColorForSaberType(saber.saberType);

            customTrailInstances = trailManager.CreateTrail(saber, _saberTrail, saberColor, customSaberInstance);

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
