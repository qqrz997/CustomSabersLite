﻿using CustomSaber.Configuration;
using CustomSaber.Data;
using IPA.Utilities;
using System;
using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class CustomTrailHandler
    {
        public CustomSaberTrail TrailInstance { get; private set; }

        private readonly CustomTrail customTrail;

        public CustomTrailHandler(GameObject customSaber, CustomTrail customTrail)
        {
            this.customTrail = customTrail;
            TrailInstance = customSaber.gameObject.AddComponent<CustomSaberTrail>();
        }

        private SaberTrailRenderer defaultTrailRendererPrefab;

        private MeshRenderer defaultMeshRenderer;

        private int defaultSamplingFrequency;

        private int defaultGranularity;

        private SaberTrailRenderer defaultSaberTrailRenderer;

        private TrailElementCollection defaultTrailElementCollection;

        public void CreateTrail(SaberTrail defaultTrail, Color saberTrailColor)
        {
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
                Plugin.Log.Error(ex);
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
                case CustomSaberColorType.CustomColor:
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
            ReflectionUtil.SetField<SaberTrail, SaberTrailRenderer>(TrailInstance, "_trailRendererPrefab", defaultTrailRendererPrefab);
            ReflectionUtil.SetField<SaberTrail, int>(TrailInstance, "_samplingFrequency", defaultSamplingFrequency);
            ReflectionUtil.SetField<SaberTrail, int>(TrailInstance, "_granularity", defaultGranularity);
            ReflectionUtil.SetField<SaberTrail, Color>(TrailInstance, "_color", saberTrailColor);
            ReflectionUtil.SetField<SaberTrail, IBladeMovementData>(TrailInstance, "_movementData", TrailInstance.CustomTrailMovementData);
            ReflectionUtil.SetField<SaberTrail, SaberTrailRenderer>(TrailInstance, "_trailRenderer", defaultSaberTrailRenderer);
            ReflectionUtil.SetField<SaberTrail, TrailElementCollection>(TrailInstance, "_trailElementCollection", defaultTrailElementCollection);
            if (CustomSaberConfig.Instance.OverrideTrailDuration)
            {
                CustomSaberUtils.SetTrailDuration(TrailInstance); // Has to be done at the end
            }
            if (CustomSaberConfig.Instance.DisableWhiteTrail)
            {
                CustomSaberUtils.SetWhiteTrailDuration(TrailInstance);
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
