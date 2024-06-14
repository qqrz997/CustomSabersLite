using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components.Game
{
    internal class DefaultSaberSetter : IInitializable
    {
        private readonly CSLConfig config;
        private readonly SaberManager saberManager;
        private readonly GameplayCoreSceneSetupData gameplayCoreData;
        private readonly ICoroutineStarter coroutineStarter;

        private DefaultSaberSetter(CSLConfig config, SaberManager saberManager, GameplayCoreSceneSetupData gameplayCoreData, ICoroutineStarter coroutineStarter)
        {
            this.config = config;
            this.saberManager = saberManager;
            this.gameplayCoreData = gameplayCoreData;
            this.coroutineStarter = coroutineStarter;
        }

        public void Initialize() => coroutineStarter.StartCoroutine(WaitForSaberModelController());

        private IEnumerator WaitForSaberModelController()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<SaberModelController>().Any());
            SetupSabers();
        }

        private void SetupSabers()
        {
            SetupSaber(saberManager.leftSaber);
            SetupSaber(saberManager.rightSaber);
        }
    
        private void SetupSaber(Saber saber)
        {
            SaberModelController saberModelController = saber.GetComponentInChildren<SaberModelController>();
            SaberTrail trail = saberModelController?.gameObject.GetComponent<SaberTrail>() ?? saber.GetComponentInChildren<SaberTrail>();
            if (!saberModelController || !trail)
            {
                return;
            }

            if (config.EnableCustomColorScheme)
            {
                Color color = saber.saberType == SaberType.SaberA ? config.LeftSaberColor : config.RightSaberColor;

                SetCustomSchemeColor(color, saberModelController);
                trail._color = color.ColorWithAlpha(gameplayCoreData.playerSpecificSettings.saberTrailIntensity);
            }

            trail.ConfigureTrail(config);
        }

        private static void SetCustomSchemeColor(Color color, SaberModelController saberModelController)
        {
            foreach (SetSaberGlowColor setSaberGlowColor in saberModelController._setSaberGlowColors)
            {
                MaterialPropertyBlock materialPropertyBlock = setSaberGlowColor._materialPropertyBlock ?? new MaterialPropertyBlock();

                foreach (SetSaberGlowColor.PropertyTintColorPair propertyTintColorPair in setSaberGlowColor._propertyTintColorPairs)
                {
                    materialPropertyBlock.SetColor(propertyTintColorPair.property, color * propertyTintColorPair.tintColor);
                }

                setSaberGlowColor._meshRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
}
