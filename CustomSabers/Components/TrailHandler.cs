using CustomSabersLite.Configuration;
using CustomSaber;
using IPA.Utilities;
using System;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities
{
    internal class CustomTrailHandler
    {
        private readonly CSLConfig config;

        public CustomTrailHandler(CSLConfig config)
        {
            this.config = config;
        }

        private CSLSaberTrail TrailInstance;

        private SaberTrailRenderer defaultTrailRendererPrefab;
        private MeshRenderer defaultMeshRenderer;
        private int defaultSamplingFrequency;
        private int defaultGranularity;
        private SaberTrailRenderer defaultSaberTrailRenderer;
        private TrailElementCollection defaultTrailElementCollection;

        public void CreateTrail(SaberTrail defaultTrail, Color saberTrailColor, GameObject customSaber, CustomTrail customTrail)
        {

            Logger.Debug($"Initializing custom trail to {defaultTrail.name}");
            TrailInstance = customSaber.gameObject.AddComponent<CSLSaberTrail>();

            try
            {
                defaultTrailRendererPrefab = ReflectionUtil.GetField<SaberTrailRenderer, SaberTrail>(defaultTrail, "_trailRendererPrefab");
                defaultSaberTrailRenderer = ReflectionUtil.GetField<SaberTrailRenderer, SaberTrail>(defaultTrail, "_trailRenderer");
                defaultMeshRenderer = ReflectionUtil.GetField<MeshRenderer, SaberTrailRenderer>(defaultSaberTrailRenderer, "_meshRenderer");
                defaultSamplingFrequency = ReflectionUtil.GetField<int, SaberTrail>(defaultTrail, "_samplingFrequency");
                defaultGranularity = ReflectionUtil.GetField<int, SaberTrail>(defaultTrail, "_granularity");
                defaultTrailElementCollection = ReflectionUtil.GetField<TrailElementCollection, SaberTrail>(defaultTrail, "_trailElementCollection");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw;
            }

            TrailInstance.Setup(customTrail.PointEnd, customTrail.PointStart);
            // We will setup the trail values here

            Color materialColor;
            MeshRenderer newMeshRenderer = defaultMeshRenderer;
            float trailIntensity = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.playerSpecificSettings.saberTrailIntensity;

            // a later version should do this in a more elegant way if i can figure out a way to
            // Swap material
            newMeshRenderer.material = customTrail.TrailMaterial;

            switch (customTrail.colorType)
            {
                case CustomSaber.ColorType.CustomColor:
                    materialColor = customTrail.TrailColor;
                    break;

                default:
                    materialColor = saberTrailColor;
                    materialColor.a = trailIntensity;
                    break;
            }

            newMeshRenderer.material.color = materialColor;
            
            // Adjusting the trail's meshrenderer before adding it to our trail
            ReflectionUtil.SetField(defaultSaberTrailRenderer, "_meshRenderer", newMeshRenderer);

            // Variables are null so set them
            TrailInstance.TrailRendererPrefab = defaultTrailRendererPrefab;
            TrailInstance.SamplingFrequency = defaultSamplingFrequency;
            TrailInstance.Granularity = defaultGranularity;
            TrailInstance.Color = saberTrailColor;
            TrailInstance.MovementData = TrailInstance.CustomTrailMovementData;
            TrailInstance.TrailRenderer = defaultSaberTrailRenderer;
            TrailInstance.TrailElementCollection = defaultTrailElementCollection;
            TrailInstance.colorType = customTrail.colorType;
            if (config.OverrideTrailDuration)
            {
                // CSLUtils.Instance.SetTrailDuration(TrailInstance); // Has to be done at the end
                float trailDuration = 0.4f;
                if (config.OverrideTrailDuration)
                {
                    trailDuration = config.TrailDuration / 100f * trailDuration;
                }

                if (trailDuration == 0)
                {
                    CSLUtils.HideTrail(TrailInstance);
                }
                else
                {
                    ReflectionUtil.SetField<SaberTrail, float>(TrailInstance, "_trailDuration", trailDuration);
                }
            }
            if (config.DisableWhiteTrail)
            {
                ReflectionUtil.SetField<SaberTrail, float>(TrailInstance, "_whiteSectionMaxDuration", 0f);
            }
        }

        private void SetClampTexture(Material mat)
        {
            if(mat.TryGetMainTexture(out Texture texture))
            {
                texture.wrapMode = TextureWrapMode.Clamp;
            }
        }
    }
}
