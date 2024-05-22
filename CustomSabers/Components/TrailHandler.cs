using CustomSabersLite.Configuration;
using CustomSaber;
using UnityEngine;
using CustomSabersLite.Data;

namespace CustomSabersLite.Utilities
{
    internal class CustomTrailHandler
    {
        private readonly CSLConfig config;
        private readonly TrailUtils trailUtils;
        private readonly GameplayCoreSceneSetupData gameplaySetupData;

        public CustomTrailHandler(CSLConfig config, TrailUtils trailUtils, GameplayCoreSceneSetupData gameplaySetupData)
        {
            this.config = config;
            this.trailUtils = trailUtils;
            this.gameplaySetupData = gameplaySetupData;
        }

        public CSLSaberTrail Trail { get; private set; }

        private SaberTrailRenderer defaultTrailRendererPrefab;
        private MeshRenderer defaultMeshRenderer;
        private int defaultSamplingFrequency;
        private int defaultGranularity;
        private SaberTrailRenderer defaultSaberTrailRenderer;
        private TrailElementCollection defaultTrailElementCollection;

        public bool CreateTrail(SaberTrail defaultTrail, Color saberTrailColor, GameObject customSaber)
        {
            switch (config.TrailType)
            {
                case TrailType.Custom:
                    return CreateCustomTrail(defaultTrail, saberTrailColor, customSaber);

                case TrailType.Vanilla:
                    SetupDefaultTrail(defaultTrail);
                    return true;

                default: return true;
            }
        }

        private bool CreateCustomTrail(SaberTrail defaultTrail, Color saberTrailColor, GameObject customSaber)
        {
            CustomTrail customTrail = customSaber.GetComponent<CustomTrail>();

            if (customTrail is null)
            {
                Logger.Warn("No custom trails. Defaulting to existing saber trails.");
                SetupDefaultTrail(defaultTrail);
                return true;
            }

            Logger.Debug($"Initializing custom trail to {defaultTrail.name}");
            Trail = customSaber.gameObject.AddComponent<CSLSaberTrail>();

            defaultTrailRendererPrefab = defaultTrail._trailRendererPrefab;
            defaultSaberTrailRenderer = defaultTrail._trailRenderer;
            defaultMeshRenderer = defaultSaberTrailRenderer._meshRenderer;
            defaultSamplingFrequency = defaultTrail._samplingFrequency;
            defaultGranularity = defaultTrail._granularity;
            defaultTrailElementCollection = defaultTrail._trailElementCollection;

            // We will setup the trail values here
            if (config.OverrideTrailWidth)
            {
                Vector3 trailTop = customTrail.PointEnd.position;
                Vector3 trailBottom = customTrail.PointStart.position;
                float distance = Vector3.Distance(trailTop, trailBottom);
                float width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;

                customTrail.PointStart.position = Vector3.LerpUnclamped(trailTop, trailBottom, width);
            }
            Trail.Setup(customTrail.PointEnd, customTrail.PointStart);

            Color materialColor;
            MeshRenderer newMeshRenderer = defaultMeshRenderer;
            float trailIntensity = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;

            // Swap material
            newMeshRenderer.material = customTrail.TrailMaterial;

            switch (customTrail.colorType)
            {
                case CustomSaber.ColorType.CustomColor:
                    materialColor = customTrail.TrailColor;
                    break;

                default:
                    materialColor = saberTrailColor.ColorWithAlpha(trailIntensity);
                    break;
            }

            newMeshRenderer.material.color = materialColor;

            // Adjusting the trail's meshrenderer before adding it to our trail
            defaultSaberTrailRenderer._meshRenderer = newMeshRenderer;

            // Variables are null so set them
            Trail._trailRendererPrefab = defaultTrailRendererPrefab;
            Trail._samplingFrequency = defaultSamplingFrequency;
            Trail._granularity = defaultGranularity;
            Trail._color = saberTrailColor;
            Trail._movementData = Trail.CustomTrailMovementData;
            Trail._trailRenderer = defaultSaberTrailRenderer;
            Trail._trailElementCollection = defaultTrailElementCollection;
            trailUtils.SetTrailDuration(Trail);
            trailUtils.SetWhiteTrailDuration(Trail);

            return false;
        }

        private void SetupDefaultTrail(SaberTrail defaultTrail)
        {
            trailUtils.SetTrailDuration(defaultTrail, isDefaultSaber: true);
            trailUtils.SetWhiteTrailDuration(defaultTrail);
        }
    }
}
