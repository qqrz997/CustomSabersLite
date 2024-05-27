using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using UnityEngine;
using CustomSabersLite.Data;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;

namespace CustomSabersLite.Components.Game
{
    /// <summary>
    /// Manages an instance of a <seealso cref="LiteSaberTrail"/>
    /// </summary>
    internal class TrailManager
    {
        private readonly CSLConfig config;
        private readonly float saberTrailIntensity;

        private TrailManager(CSLConfig config, GameplayCoreSceneSetupData gameplaySetupData)
        {
            this.config = config;
            saberTrailIntensity  = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;
        }

        private SaberTrailRenderer defaultTrailRendererPrefab;
        private int defaultSamplingFrequency;
        private int defaultGranularity;
        private TrailElementCollection defaultTrailElementCollection;

        /// <summary>
        /// Sets up custom trails for a custom saber
        /// </summary>
        /// <returns>null if no trail is created</returns>
        public LiteSaberTrail[] CreateTrail(SaberTrail defaultTrail, Color saberTrailColor, GameObject saberObject, CustomSaberType customSaberType)
        {
            CustomTrailData[] customTrailData = null;

            switch (customSaberType)
            {
                case CustomSaberType.Saber:
                    customTrailData = TrailsFromSaber(saberTrailColor, saberObject); break;

                case CustomSaberType.Whacker:
                    customTrailData = TrailsFromWhacker(); break;
            }

            if (customTrailData is null)
            {
                return null;
            }

            defaultTrailRendererPrefab = defaultTrail._trailRendererPrefab;
            defaultSamplingFrequency = defaultTrail._samplingFrequency;
            defaultGranularity = defaultTrail._granularity;
            defaultTrailElementCollection = defaultTrail._trailElementCollection;

            List<LiteSaberTrail> trails = new List<LiteSaberTrail>();

            for (int i = 0; i < customTrailData.Length; i++)
            {
                trails.Add(i == 0 ? SetupTrail(saberObject, customTrailData[i]) 
                    : SetupSecondaryTrail(saberObject, customTrailData[i]));
            }

            return trails.ToArray();
        }

        private CustomTrailData[] TrailsFromWhacker()
        {
            return null;
        }

        private CustomTrailData[] TrailsFromSaber(Color saberTrailColor, GameObject saberObject)
        {
            CustomTrail[] customTrails = saberObject.GetComponentsInChildren<CustomTrail>();

            if (customTrails.Length < 1)
            {
                return null;
            }

            IList<CustomTrailData> customTrailData = new List<CustomTrailData>();

            foreach (CustomTrail customTrail in customTrails)
            {
                customTrailData.Add(new CustomTrailData(customTrail.PointEnd, customTrail.PointStart, customTrail.TrailMaterial, saberTrailColor));
            }

            return customTrailData.ToArray();
        }

        private LiteSaberTrail SetupTrail(GameObject saberObject, CustomTrailData customTrailData)
        {
            LiteSaberTrail trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();

            if (config.OverrideTrailWidth)
            {
                Vector3 trailTop = customTrailData.TrailTop.position;
                Vector3 trailBottom = customTrailData.TrailBottom.position;
                float distance = Vector3.Distance(trailTop, trailBottom);
                float width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;

                customTrailData.TrailBottom.position = Vector3.LerpUnclamped(trailTop, trailBottom, width);
            }
            return InitTrail(customTrailData, trail);
        }

        private LiteSaberTrail SetupSecondaryTrail(GameObject saberObject, CustomTrailData customTrailData)
        {
            LiteSaberTrail trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();
            return InitTrail(customTrailData, trail);
        }

        private LiteSaberTrail InitTrail(CustomTrailData customTrailData, LiteSaberTrail trail)
        {
            trail._trailRendererPrefab = defaultTrailRendererPrefab;
            trail._samplingFrequency = defaultSamplingFrequency;
            trail._granularity = defaultGranularity;
            trail._color = customTrailData.TrailColor;
            trail._trailRenderer = UnityEngine.Object.Instantiate(trail._trailRendererPrefab, Vector3.zero, Quaternion.identity);
            trail._trailRenderer._meshRenderer.material = customTrailData.TrailMaterial;
            trail._trailRenderer._meshRenderer.material.color = customTrailData.TrailColor.ColorWithAlpha(saberTrailIntensity);
            trail._trailElementCollection = defaultTrailElementCollection;

            trail.Setup(customTrailData.TrailTop, customTrailData.TrailBottom);
            trail.ConfigureTrail(config);

            return trail;
        }
    }
}
