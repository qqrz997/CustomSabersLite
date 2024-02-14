using CustomSaber.Configuration;
using CustomSaber.Data;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static PlayerSaveData;

namespace CustomSaber.Utilities
{
    internal class CustomSaberTrailHandler
    {
        public CustomSaberTrail TrailInstance { get; set; }

        private readonly CustomTrail _customTrail;

        public CustomSaberTrailHandler(GameObject customSaber, CustomTrail customTrail)
        {
            _customTrail = customTrail;
            TrailInstance = customSaber.gameObject.AddComponent<CustomSaberTrail>();
        }

        private SaberTrailRenderer defaultTrailRendererPrefab;

        private MeshRenderer defaultMeshRenderer;

        private int defaultSamplingFrequency;

        private int defaultGranularity;

        private SaberTrailRenderer defaultSaberTrailRenderer;

        private TrailElementCollection defaultTrailElementCollection;

        private IBladeMovementData defaultBladeMovementData;

        public void CreateTrail(SaberTrail defaultTrail, Color saberColour)
        {
            try
            {
                defaultTrailRendererPrefab = ReflectionUtil.GetField<SaberTrailRenderer, SaberTrail>(defaultTrail, "_trailRendererPrefab");
                defaultSaberTrailRenderer = ReflectionUtil.GetField<SaberTrailRenderer, SaberTrail>(defaultTrail, "_trailRenderer");
                defaultMeshRenderer = ReflectionUtil.GetField<MeshRenderer, SaberTrailRenderer>(defaultSaberTrailRenderer, "_meshRenderer");
                defaultSamplingFrequency = ReflectionUtil.GetField<int, SaberTrail>(defaultTrail, "_samplingFrequency");
                defaultGranularity = ReflectionUtil.GetField<int, SaberTrail>(defaultTrail, "_granularity");
                defaultTrailElementCollection = ReflectionUtil.GetField<TrailElementCollection, SaberTrail>(defaultTrail, "_trailElementCollection");
                defaultBladeMovementData = ReflectionUtil.GetField<IBladeMovementData, SaberTrail>(defaultTrail, "_movementData");
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex);
                throw;
            }

            TrailInstance.Setup();
            //We will setup the trail values here

            float trailIntensity = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.playerSpecificSettings.saberTrailIntensity;
            Color trailColour = new Color { r = saberColour.r, g = saberColour.g, b = saberColour.b, a = trailIntensity };

            //a later version should do this in a more elegant way if i can figure out a way to
            //Swap material
            MeshRenderer newMeshRenderer = defaultMeshRenderer;
            newMeshRenderer.material = _customTrail.TrailMaterial;

            //Adjusting the trail's meshrenderer before adding it to our trail
            ReflectionUtil.SetField(defaultSaberTrailRenderer, "_meshRenderer", newMeshRenderer);

            //Variables are null so set them
            ReflectionUtil.SetField<SaberTrail, SaberTrailRenderer>(TrailInstance, "_trailRendererPrefab", defaultTrailRendererPrefab);
            ReflectionUtil.SetField<SaberTrail, int>(TrailInstance, "_samplingFrequency", defaultSamplingFrequency);
            ReflectionUtil.SetField<SaberTrail, int>(TrailInstance, "_granularity", defaultGranularity);
            ReflectionUtil.SetField<SaberTrail, Color>(TrailInstance, "_color", trailColour);
            ReflectionUtil.SetField<SaberTrail, IBladeMovementData>(TrailInstance, "_movementData", defaultBladeMovementData);
            ReflectionUtil.SetField<SaberTrail, SaberTrailRenderer>(TrailInstance, "_trailRenderer", defaultSaberTrailRenderer);
            ReflectionUtil.SetField<SaberTrail, TrailElementCollection>(TrailInstance, "_trailElementCollection", defaultTrailElementCollection);
            if (CustomSaberConfig.Instance.OverrideTrailDuration)
            {
                CustomSaberUtils.SetTrailDuration(TrailInstance); //Has to be done at the end
            }
            if (CustomSaberConfig.Instance.DisableWhiteTrail)
            {
                CustomSaberUtils.SetWhiteTrailDuration(TrailInstance);
            }
        }
    }
}
