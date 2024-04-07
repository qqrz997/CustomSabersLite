using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.Extensions;
using IPA.Config.Data;
using IPA.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities
{
    internal class DefaultSaberSetter : MonoBehaviour
    {
        private CSLConfig config;
        private SaberManager saberManager;
        private TrailUtils trailUtils;
        private GameplayCoreSceneSetupData gameplayCoreData;

        [Inject]
        public void Construct(CSLConfig config, SaberManager saberManager, TrailUtils trailUtils, GameplayCoreSceneSetupData gameplayCoreData)
        {
            this.config = config;
            this.saberManager = saberManager;
            this.trailUtils = trailUtils;
            this.gameplayCoreData = gameplayCoreData;
        }

        private void Start()
        {
            StartCoroutine(WaitForSaberModelController());
        }

        private IEnumerator WaitForSaberModelController()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<SaberModelController>().Any());
            SetupSabers();
        }

        private void SetupSabers()
        {
            SetupSaber(saberManager.leftSaber, config.LeftSaberColor);
            SetupSaber(saberManager.rightSaber, config.RightSaberColor);
        }
    
        private void SetupSaber(Saber saber, Color color)
        {
            SaberModelController saberModelController = saber.GetComponentInChildren<SaberModelController>();
            if (saberModelController != null && config.EnableCustomColorScheme)
            {
                SetSaberGlowColor[] setSaberGlowColors = saberModelController.GetField<SetSaberGlowColor[], SaberModelController>("_setSaberGlowColors");
                foreach (SetSaberGlowColor setSaberGlowColor in setSaberGlowColors)
                {
                    MeshRenderer meshRenderer = setSaberGlowColor.GetField<MeshRenderer, SetSaberGlowColor>("_meshRenderer");
                    MaterialPropertyBlock materialPropertyBlock = setSaberGlowColor.GetField<MaterialPropertyBlock, SetSaberGlowColor>("_materialPropertyBlock") ?? new MaterialPropertyBlock();

                    SetSaberGlowColor.PropertyTintColorPair[] propertyTintColorPairs = setSaberGlowColor.GetField<SetSaberGlowColor.PropertyTintColorPair[], SetSaberGlowColor>("_propertyTintColorPairs");
                    foreach (SetSaberGlowColor.PropertyTintColorPair propertyTintColorPair in propertyTintColorPairs)
                    {
                        materialPropertyBlock.SetColor(propertyTintColorPair.property, color * propertyTintColorPair.tintColor);
                    }

                    meshRenderer.SetPropertyBlock(materialPropertyBlock);
                }
            }

            SaberTrail trail = saberModelController?.gameObject.GetComponent<SaberTrail>() ?? saber.GetComponentInChildren<SaberTrail>();
            if (trail != null)
            {
                if (config.EnableCustomColorScheme)
                {
                    ReflectionUtil.SetField(trail, "_color", color.ColorWithAlpha(gameplayCoreData.playerSpecificSettings.saberTrailIntensity));
                }

                trailUtils.SetTrailDuration(trail, true, config.TrailType == TrailType.None ? 0f : 0.4f);
                trailUtils.SetWhiteTrailDuration(trail);
            }
        }
    }
}
